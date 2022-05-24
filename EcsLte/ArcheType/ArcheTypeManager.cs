using EcsLte.Data;
using EcsLte.Utilities;
using System.Collections.Generic;

namespace EcsLte
{
    /*internal unsafe class ArcheTypeManager
    {
        private List<IndexDictionary<ArcheType>> _archeTypeIndexes;
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        public EcsContext Context { get; private set; }
        internal int ChangeVersion { get; private set; }

        internal ArcheTypeManager(EcsContext context)
        {
            _archeTypeIndexes = new List<IndexDictionary<ArcheType>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();

            Context = context;
        }

        internal ArcheTypeData* GetArcheTypeDataFromEntityArcheType(EntityArcheType entityArcheType)
        {
            if (entityArcheType.ArcheTypeData.TryGetArcheTypeIndex(Context, out var archeTypeIndex))
                return GetArcheTypeDataFromIndex(archeTypeIndex);

            var archeType = GetArcheType(Context, entityArcheType);
            GetIndexDic(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeIndex = new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                };
                archeTypeData = ArcheTypeData.Alloc(archeType, archeTypeIndex);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                ChangeVersion++;
            }
            else
            {
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;
                archeTypeIndex = archeTypeData->ArcheTypeIndex;
                archeType.Dispose();
            }
            entityArcheType.ArcheTypeData.AddArcheTypeIndex(Context, archeTypeIndex);

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeDataFromArcheType(ref ArcheType archeType)
        {
            GetIndexDic(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                });
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                ChangeVersion++;
            }
            else
            {
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;
                archeType.Dispose();
                archeType = archeTypeData->ArcheType;
            }

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeDataFromIndex(ArcheTypeIndex archeTypeIndex) => (ArcheTypeData*)_archeTypeDatas
                [archeTypeIndex.ComponentConfigLength]
                [archeTypeIndex.Index]
                .Ptr;

        internal unsafe void UpdateEntityQuery(EntityQuery query)
        {
            var contextQueryData = query.QueryData.ContextQueryData[Context];
            if (contextQueryData.ArcheTypeChangeVersion != ChangeVersion)
            {
                var archeTypeDatas = new List<PtrWrapper>();
                for (var i = query.QueryData.ConfigCount; i < _archeTypeDatas.Count; i++)
                {
                    var dataList = _archeTypeDatas[i];
                    for (var j = 0; j < dataList.Count; j++)
                    {
                        var archeTypePtr = dataList[j];
                        if (query.QueryData.IsFiltered(((ArcheTypeData*)archeTypePtr.Ptr)->ArcheType))
                            archeTypeDatas.Add(archeTypePtr);
                    }
                }

                contextQueryData.ArcheTypeDatas = archeTypeDatas.ToArray();
                contextQueryData.ArcheTypeChangeVersion = ChangeVersion;
            }
        }

        internal void InternalDestroy()
        {
            foreach (var indexDic in _archeTypeIndexes)
            {
                foreach (var archeType in indexDic.Keys)
                    archeType.Dispose();
            }
            _archeTypeIndexes = null;
            foreach (var dataList in _archeTypeDatas)
            {
                foreach (var archeTypeData in dataList)
                {
                    ((ArcheTypeData*)archeTypeData.Ptr)->Dispose();
                    MemoryHelper.Free(archeTypeData.Ptr);
                }
            }
            _archeTypeDatas = null;
        }

        private static ArcheType GetArcheType(EcsContext context, EntityArcheType entityArcheType)
        {
            var archeTypeData = entityArcheType.ArcheTypeData;
            var archeType = new ArcheType
            {
                ComponentConfigLength = archeTypeData.ComponentConfigs.Length,
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeTypeData.ComponentConfigs.Length),
                SharedComponentDataLength = archeTypeData.SharedComponentDatas.Length,
                SharedComponentDataIndexes = archeTypeData.SharedComponentDatas.Length > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(archeTypeData.SharedComponentDatas.Length)
                    : null
            };
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
                archeType.ComponentConfigs[i] = archeTypeData.ComponentConfigs[i];
            for (var i = 0; i < archeType.SharedComponentDataLength; i++)
            {
                var component = archeTypeData.SharedComponentDatas[i];
                archeType.SharedComponentDataIndexes[i] = context.GetSharedComponentDataIndex(
                    (ISharedComponent)component.Component, component.Config);
            }

            return archeType;
        }

        private void GetIndexDic(int configCount,
            out IndexDictionary<ArcheType> indexDic,
            out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<ArcheType>());
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }*/
}
