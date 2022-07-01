using EcsLte.Utilities;
using System.Collections.Generic;

namespace EcsLte
{
    internal unsafe class ArcheTypeDataManager
    {
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<ArcheTypeData>> _archeTypeIndexes;
        private readonly DataPoint[] _rootPoints;
        private int _archeTypeDataVersion;
        private ArcheType _cachedArcheType;
        private readonly EcsContext _context;

        internal ArcheTypeDataManager(EcsContext context)
        {
            _archeTypeIndexes = new List<List<ArcheTypeData>>();
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);
            _context = context;

            var allConfigCount = ComponentConfigs.Instance.AllComponentCount;
            _rootPoints = new DataPoint[allConfigCount];

            for (var i = 0; i < allConfigCount; i++)
                _rootPoints[i] = new DataPoint(i);
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

        internal ArcheTypeData CachedArcheTypeGetNextArcheTypeData(ArcheTypeData prevArcheTypeData, SharedComponentDataIndex shareDataIndex) =>
            _cachedArcheType.ReplaceSharedComponentDataIndex(shareDataIndex)
                ? GetCachedArcheTypeData()
                : prevArcheTypeData;

        internal ArcheTypeData GetArcheTypeData(EntityArcheType entityArcheType)
        {
            if (entityArcheType.ArcheTypeData.TryGetArcheTypeIndex(_context, out var archeTypeData))
                return archeTypeData;

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

            archeTypeData = GetCachedArcheTypeData();
            entityArcheType.ArcheTypeData.AddArcheTypeIndex(_context, archeTypeData);

            return archeTypeData;
        }

        internal ArcheTypeData GetArcheTypeData(ArcheTypeIndex archeIndex) => _archeTypeIndexes[archeIndex.ComponentConfigLength][archeIndex.Index];

        internal void UpdateEntityQuery(EntityQuery query)
        {
            var contextQueryData = query.QueryData.ContextQueryData[_context];
            if (contextQueryData.ArcheTypeChangeVersion != _archeTypeDataVersion)
            {
                var archeTypeDatas = new List<ArcheTypeData>();
                for (var i = query.QueryData.ConfigCount; i < _archeTypeIndexes.Count; i++)
                {
                    foreach (var archeTypeData in _archeTypeIndexes[i])
                    {
                        if (query.QueryData.IsFiltered(archeTypeData.ArcheType))
                            archeTypeDatas.Add(archeTypeData);
                    }
                }

                contextQueryData.ArcheTypeDatas = archeTypeDatas.ToArray();
                contextQueryData.ArcheTypeChangeVersion = _archeTypeDataVersion;
            }
        }

        internal void InternalDestroy()
        {
            foreach (var list in _archeTypeIndexes)
            {
                foreach (var item in list)
                    item.InternalDestroy();
            }
            _archeTypeIndexes = null;
            _cachedArcheType.Dispose();
        }

        private void GetArcheTypeIndexDic(int configCount, out List<ArcheTypeData> indexDic)
        {
            while (_archeTypeIndexes.Count <= configCount)
                _archeTypeIndexes.Add(new List<ArcheTypeData>());

            indexDic = _archeTypeIndexes[configCount];
        }

        internal ArcheTypeData GetCachedArcheTypeData()
        {
            GetArcheTypeIndexDic(_cachedArcheType.ComponentConfigLength, out var dataList);
            var point = _rootPoints[_cachedArcheType.ComponentConfigs[0].ComponentIndex];
            for (var i = 1; i < _cachedArcheType.ComponentConfigLength; i++)
                point = point.GetChild(_cachedArcheType.ComponentConfigs[i].ComponentIndex);

            if (!point.ArcheTypeDatas.TryGetValue(_cachedArcheType, out var archeTypeData))
            {
                archeTypeData = new ArcheTypeData(ArcheType.AllocClone(_cachedArcheType),
                    new ArcheTypeIndex(_cachedArcheType.ComponentConfigLength, dataList.Count));
                point.ArcheTypeDatas.Add(archeTypeData.ArcheType, archeTypeData);
                dataList.Add(archeTypeData);
                _archeTypeDataVersion++;
            }

            return archeTypeData;
        }

        private class DataPoint
        {
            public int ComponentIndex { get; set; }
            public DataPoint[] Children { get; set; }
            /// <summary>
            /// ArcheTypeData*
            /// </summary>
            public Dictionary<ArcheType, ArcheTypeData> ArcheTypeDatas { get; set; }

            public DataPoint(int componentIndex)
            {
                var allConfigCount = ComponentConfigs.Instance.AllComponentCount;
                ComponentIndex = componentIndex;
                ArcheTypeDatas = new Dictionary<ArcheType, ArcheTypeData>(ArcheTypeSharedDataIndexComparer.Comparer);
                Children = new DataPoint[allConfigCount];
            }

            public DataPoint GetChild(int componentIndex)
            {
                if (Children[componentIndex] == null)
                {
                    var dataPoint = new DataPoint(componentIndex);
                    Children[componentIndex] = dataPoint;

                    return dataPoint;
                }
                return Children[componentIndex];
            }
        }

        private class ArcheTypeSharedDataIndexComparer : IEqualityComparer<ArcheType>
        {
            internal static ArcheTypeSharedDataIndexComparer Comparer => new ArcheTypeSharedDataIndexComparer();

            public bool Equals(ArcheType x, ArcheType y)
            {
                if (x.SharedComponentDataLength == y.SharedComponentDataLength)
                {
                    for (var i = 0; i < x.SharedComponentDataLength; i++)
                    {
                        if (x.SharedComponentDataIndexes[i] != y.SharedComponentDataIndexes[i])
                            return false;
                    }
                }

                return true;
            }

            public int GetHashCode(ArcheType obj)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(obj.SharedComponentDataLength);
                for (var i = 0; i < obj.SharedComponentDataLength; i++)
                    hashCode = hashCode.AppendHashCode(obj.SharedComponentDataIndexes[i]);

                return hashCode.GetHashCode();
            }
        }
    }
}
