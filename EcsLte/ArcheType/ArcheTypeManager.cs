using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public unsafe class ArcheTypeManager
    {
        private List<List<ArcheTypeData>> _archeTypeIndexes;
        private ChangeVersion _archeTypeDataVersion;
        private ArcheType _cachedArcheType;
        private DataPoint _rootPoint;
        private DataManager _dataManager;
        private readonly object _lockCacheObj;

        public EcsContext Context { get; private set; }

        internal List<List<ArcheTypeData>> ArcheTypeDatas => _archeTypeIndexes;

        internal ArcheTypeManager(EcsContext context)
        {
            _archeTypeIndexes = new List<List<ArcheTypeData>>();
            _cachedArcheType = ArcheType.Alloc(
                ComponentConfigs.Instance.AllComponentCount,
                ComponentConfigs.Instance.AllSharedCount);
            _rootPoint = new DataPoint();
            _dataManager = new DataManager();
            _lockCacheObj = new object();

            Context = context;
        }

        public EntityArcheType CreateArcheType()
            => new EntityArcheType(Context);

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

        public EntityArcheType[] GetArcheTypes(EntityFilter filter)
        {
            var archeTypes = new EntityArcheType[0];
            GetArcheTypes(filter, ref archeTypes, 0);

            return archeTypes;
        }

        public void GetArcheTypes(EntityFilter filter,
            ref EntityArcheType[] destArcheTypes)
            => GetArcheTypes(filter, ref destArcheTypes, 0);

        public int GetArcheTypes(EntityFilter filter,
            ref EntityArcheType[] destArcheTypes, int destStartingIndex)
        {
            Context.AssertContext();

            var archeTypeDatas = GetArcheTypeDatas(filter);
            Helper.AssertAndResizeArray(ref destArcheTypes, destStartingIndex, archeTypeDatas.Length);
            var archeTypeIndex = destStartingIndex;
            for (var i = 0; i < archeTypeDatas.Length; i++, archeTypeIndex++)
                destArcheTypes[archeTypeIndex] = new EntityArcheType(Context, archeTypeDatas[i]);

            return archeTypeIndex - destStartingIndex;
        }

        internal int GetAllEntities(ref Entity[] entities, int startingIndex)
        {
            var entityIndex = startingIndex;
            for (var i = 0; i < _archeTypeIndexes.Count; i++)
            {
                var archeTypeDatas = _archeTypeIndexes[i];
                for (var j = 1; j < archeTypeDatas.Count; j++)
                {
                    var archeTypeData = archeTypeDatas[j];
                    Helper.ResizeRefArray(ref entities, entityIndex, archeTypeData.EntityCount());
                    archeTypeData.GetAllEntities(ref entities, entityIndex);
                    entityIndex += archeTypeData.EntityCount();
                }
            }

            return entityIndex - startingIndex;
        }

        internal int GetAndClearAllEntities(ref Entity[] entities, int startingIndex)
        {
            var entityIndex = startingIndex;
            for (var i = 0; i < _archeTypeIndexes.Count; i++)
            {
                var archeTypeDatas = _archeTypeIndexes[i];
                for (var j = 1; j < archeTypeDatas.Count; j++)
                {
                    var archeTypeData = archeTypeDatas[j];
                    Helper.ResizeRefArray(ref entities, entityIndex, archeTypeData.EntityCount());
                    archeTypeData.GetAllEntities(ref entities, entityIndex);
                    entityIndex += archeTypeData.EntityCount();

                    archeTypeData.RemoveAllEntities();
                }
            }

            return entityIndex - startingIndex;
        }

        internal ArcheTypeData GetArcheTypeData(ArcheType cachedArcheType) => InternalGetArcheTypeData(cachedArcheType);

        internal ArcheTypeData GetArcheTypeData(ArcheTypeIndex archeIndex) => _archeTypeIndexes[archeIndex.ConfigLength][archeIndex.Index];

        internal ArcheTypeData GetArcheTypeData(EntityArcheType archeType)
        {
            if (archeType.ArcheTypeData != null)
                return archeType.ArcheTypeData;

            lock (_lockCacheObj)
            {
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
        }

        internal ArcheTypeData[] GetArcheTypeDatas(EntityFilter filter)
        {
            if (ChangeVersion.DidChange(_archeTypeDataVersion, filter.ArcheTypeDataVersion))
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
            _dataManager.Dispose();
            _dataManager = null;
        }

        private ArcheTypeData InternalGetArcheTypeData(ArcheType cachedArcheType)
        {
            GetArcheTypeIndexDic(cachedArcheType.ConfigsLength, out var dataList);
            var point = _rootPoint;
            for (var i = 0; i < cachedArcheType.ConfigsLength; i++)
                point = point.GetChild(cachedArcheType.Configs[i].ComponentIndex);

            ArcheTypeData archeTypeData;
            lock (point)
            {
                if (!point.ArcheTypeDatas.TryGetValue(cachedArcheType, out archeTypeData))
                {
                    archeTypeData = new ArcheTypeData(
                        ArcheType.AllocClone(cachedArcheType),
                        new ArcheTypeIndex(cachedArcheType.ConfigsLength, dataList.Count),
                        Context.SharedComponentDics,
                        _dataManager);
                    point.ArcheTypeDatas.Add(archeTypeData.ArcheType, archeTypeData);
                    dataList.Add(archeTypeData);
                    _archeTypeDataVersion = Context.Entities.GlobalVersion;
                }
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
                Children = new DataPoint[ComponentConfigs.Instance.AllComponentCount];
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
