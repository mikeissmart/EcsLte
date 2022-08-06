using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
    public unsafe class ArcheTypeManager
    {
        private List<List<ArcheTypeData>> _archeTypeIndexes;
        private long _archeTypeDataVersion;
        private ArcheType _cachedArcheType;
        private DataPoint _rootPoint;

        public EcsContext Context { get; private set; }

        internal ArcheTypeManager(EcsContext context)
        {
            _archeTypeIndexes = new List<List<ArcheTypeData>>();
            Context = context;
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.AllComponentCount,
                ComponentConfigs.AllSharedCount);

            _rootPoint = new DataPoint();
        }

        public EntityArcheType AddComponentType<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
            => new EntityArcheType(Context)
                .AddComponentType<TComponent>();

        public EntityArcheType AddManagedComponentType<TComponent>()
            where TComponent : IManagedComponent
            => new EntityArcheType(Context)
                .AddManagedComponentType<TComponent>();

        public EntityArcheType AddSharedComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
            => new EntityArcheType(Context)
                .AddSharedComponent(component);

        internal void GetAllEntities(ref Entity[] entities, int startingIndex)
        {
            for (var i = 0; i < _archeTypeIndexes.Count; i++)
            {
                var archeTypeDatas = _archeTypeIndexes[i];
                for (var j = 1; j < archeTypeDatas.Count; j++)
                {
                    var archeTypeData = archeTypeDatas[j];
                    Helper.ResizeRefEntities(ref entities, startingIndex, archeTypeData.EntityCount);
                    archeTypeData.GetEntities(ref entities, startingIndex);
                    startingIndex += archeTypeData.EntityCount;
                }
            }
        }

        internal ArcheTypeData GetArcheTypeData(ArcheType cachedArcheType)
        {
            return InternalGetArcheTypeData(cachedArcheType);
        }

        internal ArcheTypeData GetArcheTypeData(ArcheTypeIndex archeIndex)
        {
            return _archeTypeIndexes[archeIndex.ConfigLength][archeIndex.Index];
        }

        internal ArcheTypeData GetArcheTypeData(EntityArcheType archeType)
        {
            if (archeType.ArcheTypeData != null)
                return archeType.ArcheTypeData;

            if (archeType.AllConfigs.Length > 0)
            {
                fixed (ComponentConfig* copyConfigs = &archeType.AllConfigs[0])
                {
                    MemoryHelper.Copy(
                        copyConfigs,
                        _cachedArcheType.Configs,
                        archeType.AllConfigs.Length);
                }
            }
            _cachedArcheType.ConfigsLength = archeType.AllConfigs.Length;
            if (archeType.SharedDataIndexes.Length > 0)
            {
                fixed (SharedDataIndex* copySharedIndexes = &archeType.SharedDataIndexes[0])
                {
                    MemoryHelper.Copy(
                        copySharedIndexes,
                        _cachedArcheType.SharedDataIndexes,
                        archeType.SharedDataIndexes.Length);
                }
            }
            _cachedArcheType.SharedDataIndexesLength = archeType.SharedDataIndexes.Length;

            archeType.ArcheTypeData = InternalGetArcheTypeData(_cachedArcheType);

            return archeType.ArcheTypeData;
        }

        internal ArcheTypeData[] GetArcheTypeDatas(EntityFilter filter)
        {
            if (filter.ArcheTypeDataVersion != _archeTypeDataVersion)
            {
                var filteredArcheTypeDatas = new List<ArcheTypeData>();
                for (var i = filter.ConfigCount; i < _archeTypeIndexes.Count; i++)
                {
                    var archeTypeDatas = _archeTypeIndexes[i];
                    for (var j = 1; j < archeTypeDatas.Count; j++)
                    {
                        if (filter.IsFiltered(archeTypeDatas[j].ArcheType))
                            filteredArcheTypeDatas.Add(archeTypeDatas[j]);
                    }
                }

                filter.ArcheTypeDatas = filteredArcheTypeDatas.ToArray();
                filter.ArcheTypeDataVersion = _archeTypeDataVersion;
            }

            return filter.ArcheTypeDatas;
        }

        internal void InternalDestroy()
        {
            foreach (var list in _archeTypeIndexes)
            {
                foreach (var item in list.Skip(1))
                    item.InternalDestroy();
            }
            _archeTypeIndexes = null;
            _cachedArcheType.Dispose();
            _cachedArcheType = new ArcheType();
            _rootPoint = null;
        }

        private ArcheTypeData InternalGetArcheTypeData(ArcheType cachedArcheType)
        {
            GetArcheTypeIndexDic(cachedArcheType.ConfigsLength, out var dataList);
            var point = _rootPoint;
            for (var i = 0; i < cachedArcheType.ConfigsLength; i++)
                point = point.GetChild(cachedArcheType.Configs[i].ComponentIndex);

            if (!point.ArcheTypeDatas.TryGetValue(cachedArcheType, out var archeTypeData))
            {
                archeTypeData = new ArcheTypeData(
                    ArcheType.AllocClone(cachedArcheType),
                    new ArcheTypeIndex(cachedArcheType.ConfigsLength, dataList.Count),
                    Context.SharedComponentDics);
                point.ArcheTypeDatas.Add(archeTypeData.ArcheType, archeTypeData);
                dataList.Add(archeTypeData);
                _archeTypeDataVersion = DateTime.UtcNow.Ticks;
            }

            return archeTypeData;
        }

        private void GetArcheTypeIndexDic(int configCount, out List<ArcheTypeData> indexDic)
        {
            while (_archeTypeIndexes.Count <= configCount)
                _archeTypeIndexes.Add(new List<ArcheTypeData> { null });

            indexDic = _archeTypeIndexes[configCount];
        }

        private class DataPoint
        {
            internal DataPoint[] Children { get; set; }
            internal Dictionary<ArcheType, ArcheTypeData> ArcheTypeDatas { get; set; }

            internal DataPoint()
            {
                ArcheTypeDatas = new Dictionary<ArcheType, ArcheTypeData>(ArcheTypeSharedDataIndexComparer.Comparer);
                Children = new DataPoint[ComponentConfigs.AllComponentCount];
            }

            internal DataPoint GetChild(int componentIndex)
            {
                if (Children[componentIndex] == null)
                {
                    var dataPoint = new DataPoint();
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
                if (x.SharedDataIndexesLength == y.SharedDataIndexesLength)
                {
                    for (var i = 0; i < x.SharedDataIndexesLength; i++)
                    {
                        if (x.SharedDataIndexes[i] != y.SharedDataIndexes[i])
                            return false;
                    }
                }

                return true;
            }

            public int GetHashCode(ArcheType obj)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(obj.SharedDataIndexesLength);
                for (var i = 0; i < obj.SharedDataIndexesLength; i++)
                    hashCode = hashCode.AppendHashCode(obj.SharedDataIndexes[i]);

                return hashCode.GetHashCode();
            }
        }
    }
}
