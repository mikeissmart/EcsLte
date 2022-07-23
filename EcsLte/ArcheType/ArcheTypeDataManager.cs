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
        private readonly object _lockObj;

        internal ArcheTypeDataManager(EcsContext context)
        {
            _archeTypeIndexes = new List<List<ArcheTypeData>>();
            _context = context;
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);
            _lockObj = new object();

            var allConfigCount = ComponentConfigs.Instance.AllComponentCount;
            _rootPoints = new DataPoint[allConfigCount];

            for (var i = 0; i < allConfigCount; i++)
                _rootPoints[i] = new DataPoint(i);
        }

        internal ArcheTypeData GetArcheTypeData(ArcheType cachedArcheType)
        {
            lock (_lockObj)
            {
                return InternalGetArcheTypeData(cachedArcheType);
            }
        }

        internal ArcheTypeData GetArcheTypeData(EntityArcheType entityArcheType)
        {
            if (entityArcheType.TryGetArcheTypeData(_context, out var archeTypeData))
                return archeTypeData;

            lock (_lockObj)
            {
                _cachedArcheType.ComponentConfigLength = entityArcheType.AllComponentConfigs.Length;
                _cachedArcheType.SharedComponentDataLength = entityArcheType.SharedComponentDatas.Length;

                fixed (ComponentConfig* configs = &entityArcheType.AllComponentConfigs[0])
                {
                    MemoryHelper.Copy(
                        configs,
                        _cachedArcheType.ComponentConfigs,
                        entityArcheType.AllComponentConfigs.Length);
                }

                for (var i = 0; i < entityArcheType.SharedComponentDatas.Length; i++)
                {
                    _cachedArcheType.SharedComponentDataIndexes[i] =
                        entityArcheType.SharedComponentDatas[i].GetSharedComponentDataIndex(_context.SharedIndexDics);
                }

                archeTypeData = InternalGetArcheTypeData(_cachedArcheType);
                entityArcheType.SetArcheTypeData(_context, archeTypeData);

                return archeTypeData;
            }
        }

        internal ArcheTypeData GetArcheTypeData(ArcheTypeIndex archeIndex)
        {
            lock (_lockObj)
            {
                return _archeTypeIndexes[archeIndex.ComponentConfigLength][archeIndex.Index];
            }
        }

        internal void UpdateEntityFilter(EntityFilter filter, EntityFilterContextData contextData)
        {
            lock (_lockObj)
            {
                if (contextData.ArcheTypeChangeVersion != _archeTypeDataVersion)
                {
                    var archeTypeDatas = new List<ArcheTypeData>();
                    for (var i = filter.ConfigCount; i < _archeTypeIndexes.Count; i++)
                    {
                        foreach (var archeTypeData in _archeTypeIndexes[i])
                        {
                            if (filter.IsFiltered(archeTypeData.ArcheType))
                            {
                                var isOk = true;
                                for (var j = 0; j < filter.FilterByComponentDatas.Length; j++)
                                {
                                    var componentData = filter.FilterByComponentDatas[j];
                                    if (!componentData.ComponentEquals(archeTypeData.GetSharedComponentPtr(componentData.Config)))
                                    {
                                        isOk = false;
                                        break;
                                    }
                                }

                                if (isOk)
                                    archeTypeDatas.Add(archeTypeData);
                            }
                        }
                    }

                    contextData.ArcheTypeDatas = archeTypeDatas.ToArray();
                    contextData.ArcheTypeChangeVersion = _archeTypeDataVersion;
                }
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
            _cachedArcheType = new ArcheType();
        }

        private ArcheTypeData InternalGetArcheTypeData(ArcheType cachedArcheType)
        {
            GetArcheTypeIndexDic(cachedArcheType.ComponentConfigLength, out var dataList);
            var point = _rootPoints[cachedArcheType.ComponentConfigs[0].ComponentIndex];
            for (var i = 1; i < cachedArcheType.ComponentConfigLength; i++)
                point = point.GetChild(cachedArcheType.ComponentConfigs[i].ComponentIndex);

            if (!point.ArcheTypeDatas.TryGetValue(cachedArcheType, out var archeTypeData))
            {
                archeTypeData = new ArcheTypeData(ArcheType.AllocClone(cachedArcheType),
                    new ArcheTypeIndex(cachedArcheType.ComponentConfigLength, dataList.Count),
                    _context.SharedIndexDics);
                point.ArcheTypeDatas.Add(archeTypeData.ArcheType, archeTypeData);
                dataList.Add(archeTypeData);
                _archeTypeDataVersion++;
            }

            return archeTypeData;
        }

        private void GetArcheTypeIndexDic(int configCount, out List<ArcheTypeData> indexDic)
        {
            while (_archeTypeIndexes.Count <= configCount)
                _archeTypeIndexes.Add(new List<ArcheTypeData>());

            indexDic = _archeTypeIndexes[configCount];
        }

        private class DataPoint
        {
            internal int ComponentIndex { get; set; }
            internal DataPoint[] Children { get; set; }
            internal Dictionary<ArcheType, ArcheTypeData> ArcheTypeDatas { get; set; }

            internal DataPoint(int componentIndex)
            {
                ComponentIndex = componentIndex;
                ArcheTypeDatas = new Dictionary<ArcheType, ArcheTypeData>(ArcheTypeSharedDataIndexComparer.Comparer);
                Children = new DataPoint[ComponentConfigs.Instance.AllComponentCount];
            }

            internal DataPoint GetChild(int componentIndex)
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
