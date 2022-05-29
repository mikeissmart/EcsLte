using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal unsafe class ArcheTypeDataManager
    {
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;
        private List<IndexDictionary<ArcheType>> _archeTypeIndexes;
        private int _archeTypeDataVersion;
        private ArcheType _cachedArcheType;
        private readonly EcsContext _context;

        internal ArcheType CachedArcheType { get => _cachedArcheType; }

        internal ArcheTypeDataManager(EcsContext context)
        {
            _archeTypeDatas = new List<List<PtrWrapper>>();
            _archeTypeIndexes = new List<IndexDictionary<ArcheType>>();
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);
            _context = context;
        }

        internal void CachedArcheTypeCopyTo(ArcheType source)
        {
            _cachedArcheType.ComponentConfigLength = source.ComponentConfigLength;
            _cachedArcheType.SharedComponentDataLength = source.SharedComponentDataLength;

            MemoryHelper.Copy(
                source.ComponentConfigs,
                _cachedArcheType.ComponentConfigs,
                source.ComponentConfigLength * TypeCache<ComponentConfig>.SizeInBytes);
            if (source.SharedComponentDataLength > 0)
            {
                MemoryHelper.Copy(
                    source.SharedComponentDataIndexes,
                    _cachedArcheType.SharedComponentDataIndexes,
                    source.SharedComponentDataLength * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }
        }

        internal bool CachedArcheTypeReplaceSharedDataIndex(SharedComponentDataIndex shareDataIndex) =>
            _cachedArcheType.ReplaceSharedComponentDataIndex(shareDataIndex);

        internal ArcheTypeData* CachedArcheTypeGetNextArcheTypeData(ArcheTypeData* prevArcheTypeData, SharedComponentDataIndex shareDataIndex) =>
            _cachedArcheType.ReplaceSharedComponentDataIndex(shareDataIndex)
                ? GetCachedArcheTypeData()
                : prevArcheTypeData;

        internal ArcheTypeData* GetCachedArcheTypeData()
        {
            GetArcheTypeIndexDic(_cachedArcheType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData* archeTypeData = null;

            var wasAdded = false;
            var index = indexDic.GetOrAdd(_cachedArcheType,
                (newIndex) =>
                {
                    var archeType = ArcheType.AllocClone(_cachedArcheType);
                    archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                    {
                        ComponentConfigLength = _cachedArcheType.ComponentConfigLength,
                        Index = newIndex
                    });
                    dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                    _archeTypeDataVersion++;
                    wasAdded = true;

                    return archeType;
                });
            if (!wasAdded)
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeData(EntityArcheType entityArcheType)
        {
            if (entityArcheType.ArcheTypeData.TryGetArcheTypeIndex(_context, out var archeTypeIndex))
                return GetArcheTypeData(archeTypeIndex);

            var entityArcheTypeData = entityArcheType.ArcheTypeData;
            _cachedArcheType.ComponentConfigLength = entityArcheTypeData.ComponentConfigs.Length;
            _cachedArcheType.SharedComponentDataLength = entityArcheTypeData.SharedComponentDatas.Length;

            for (var i = 0; i < _cachedArcheType.ComponentConfigLength; i++)
                _cachedArcheType.ComponentConfigs[i] = entityArcheTypeData.ComponentConfigs[i];
            for (var i = 0; i < _cachedArcheType.SharedComponentDataLength; i++)
            {
                var component = entityArcheTypeData.SharedComponentDatas[i];
                _cachedArcheType.SharedComponentDataIndexes[i] = component.GetSharedComponentDataIndex(
                    _context.SharedIndexDics);
            }

            var archeTypeData = GetCachedArcheTypeData();
            entityArcheType.ArcheTypeData.AddArcheTypeIndex(_context, archeTypeData->ArcheTypeIndex);

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeData(ArcheTypeIndex archeTypeIndex) =>
            (ArcheTypeData*)_archeTypeDatas
                [archeTypeIndex.ComponentConfigLength]
                [archeTypeIndex.Index].Ptr;

        internal void UpdateEntityQuery(EntityQuery query)
        {
            var contextQueryData = query.QueryData.ContextQueryData[_context];
            if (contextQueryData.ArcheTypeChangeVersion != _archeTypeDataVersion)
            {
                var archeTypeDatas = new List<PtrWrapper>();
                for (var i = query.QueryData.ConfigCount; i < _archeTypeDatas.Count; i++)
                {
                    var dataList = _archeTypeDatas[i];
                    for (var j = 0; j < dataList.Count; j++)
                    {
                        var ptr = dataList[j];
                        var archeTypePData = (ArcheTypeData*)ptr.Ptr;
                        if (query.QueryData.IsFiltered(archeTypePData->ArcheType))
                            archeTypeDatas.Add(ptr);
                    }
                }

                contextQueryData.ArcheTypeDatas = archeTypeDatas.ToArray();
                contextQueryData.ArcheTypeChangeVersion = _archeTypeDataVersion;
            }
        }

        internal void InternalDestroy()
        {
            foreach (var indexDic in _archeTypeIndexes)
            {
                while (indexDic.PopKey(out var archeType))
                    archeType.Dispose();
            }
            _archeTypeIndexes = null;
            foreach (var dataList in _archeTypeDatas)
            {
                foreach (var archeTypeData in dataList)
                    ((ArcheTypeData*)archeTypeData.Ptr)->InternalDestroy();
            }
            _archeTypeDatas = null;
            _cachedArcheType.Dispose();
        }

        private void GetArcheTypeIndexDic(int configCount,
            out IndexDictionary<ArcheType> indexDic, out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<ArcheType>(ArcheTypeEqualityComparer.Comparer));
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
