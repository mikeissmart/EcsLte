using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal interface IEntityCollection
    {
        Entity this[int index] { get; set; }
        int Length { get; }

        bool HasEntity(Entity entity);
        Entity[] GetEntities();
    }

    internal class EcsContextData
    {
        private const int _entitiesInitSize = 4;
        private readonly Dictionary<int, ComponentArcheTypeData> _componentArcheDataTypes;

        private readonly List<EntityCollection> _entityCollections;
        private readonly Dictionary<int, EntityFilterGroupData> _entityFilterGroups;
        private readonly Dictionary<Filter, EntityFilterData> _entityFilters;
        private readonly Dictionary<int, EntityGroupData> _entityGroups;
        private int _entitiesCurrentSize;
        private ComponentArcheTypeData[] _entityComponentArcheTypes;
        private int _nextId;

        public EcsContextData()
        {
            _entityCollections = new List<EntityCollection>();
            _entityFilters = new Dictionary<Filter, EntityFilterData>();
            _entityGroups = new Dictionary<int, EntityGroupData>();
            _entityFilterGroups = new Dictionary<int, EntityFilterGroupData>();
            _componentArcheDataTypes = new Dictionary<int, ComponentArcheTypeData>();
            _entityComponentArcheTypes = new ComponentArcheTypeData[_entitiesInitSize];
            _entitiesCurrentSize = _entitiesInitSize;
            _nextId = 1;

            // CurrentContext will be initialized later
            ReuseableEntities = new Queue<Entity>();
            ComponentPools = ComponentPoolIndexes.Instance.CreateComponentPools(_entitiesInitSize);
            UniqueComponentEntities = new Entity[ComponentPoolIndexes.Instance.AllComponentCount];
            // EntityComponentArcheTypes already initialized
            EntityCommandQueue = new Dictionary<string, EntityCommandQueueData>();
            // Entities will be initialized later
            // Watchers will be initialized later
        }

        internal EcsContext CurrentContext { get; private set; }
        internal Queue<Entity> ReuseableEntities { get; }
        internal IComponentPool[] ComponentPools { get; }
        internal Entity[] UniqueComponentEntities { get; }
        internal ComponentArcheTypeData[] EntityComponentArcheTypes => _entityComponentArcheTypes;
        internal Dictionary<string, EntityCommandQueueData> EntityCommandQueue { get; }
        internal IEntityCollection Entities { get; private set; }
        internal WatcherTable Watchers { get; private set; }

        internal static EcsContextData Initialize(EcsContext context)
        {
            var data = ObjectCache<EcsContextData>.Pop();

            // data._entityCollections;
            // data._entityFilters;
            // data._entityGroups;
            // data._entityFilterGroups;
            // data._componentArcheDataTypes;
            // data._entityComponentArcheTypes;
            // data._entitiesCurrentSize;
            // data._nextId;            

            data.CurrentContext = context;
            // data.ReuseableEntities;
            // data.ComponentPools;
            // data.UniqueComponentEntities;
            // data.EntityComponentArcheTypes
            // data.EntityCommandQueue;
            // data.EntityFilters;
            data.Entities = data.CreateEntityCollection();
            data.Watchers = WatcherTable.Initialize();

            return data;
        }

        internal static void Uninitialize(EcsContextData data)
        {
            // Dont clear entityCollections here.
            // They will be cleared later in code.
            data._entityCollections.Clear();
            foreach (var filterData in data._entityFilters.Values)
                EntityFilterData.Uninitialize(filterData);
            data._entityFilters.Clear();
            foreach (var entityGroup in data._entityGroups.Values)
                EntityGroupData.Uninitialize(entityGroup);
            data._entityGroups.Clear();
            foreach (var entityFilterGroup in data._entityFilterGroups.Values)
                EntityFilterGroupData.Uninitialize(entityFilterGroup);
            data._entityFilterGroups.Clear();
            foreach (var archeTypeData in data._componentArcheDataTypes.Values)
                ComponentArcheTypeData.Uninitialize(archeTypeData);
            data._componentArcheDataTypes.Clear();
            Array.Clear(data._entityComponentArcheTypes, 0, data._entitiesCurrentSize);
            // data._entitiesCurrentSize keep
            data._nextId = 1;

            data.CurrentContext = null;
            data.ReuseableEntities.Clear();
            for (var i = 0; i < data.ComponentPools.Length; i++)
                data.ComponentPools[i].Clear();
            // data.ComponentPools dont clear
            Array.Clear(data.UniqueComponentEntities, 0, ComponentPoolIndexes.Instance.AllComponentCount);
            // data.EntityComponentArcheTypes already cleared
            foreach (var commandQueueData in data.EntityCommandQueue.Values)
                EntityCommandQueueData.Uninitialize(commandQueueData);
            data.EntityCommandQueue.Clear();
            data.RemoveEntityCollection(data.Entities);
            data.Entities = null;
            WatcherTable.Uninitialize(data.Watchers);
            // data.EntitiesCurrentSize keep

            data.AnyArcheTypeDataAdded = null;

            ObjectCache<EcsContextData>.Push(data);
        }

        internal event ComponentArcheTypeDataEvent AnyArcheTypeDataAdded;

        internal EntityFilterData CreateOrGetEntityFilterData(Filter filter)
        {
            lock (_entityFilters)
            {
                if (!_entityFilters.TryGetValue(filter, out var data))
                {
                    var archeTypeDatas = FilterComponentArcheTypeData(filter, null);
                    data = EntityFilterData.Initialize(this, filter, archeTypeDatas);
                    data.NoRef += RemoveEntityFilterData;

                    _entityFilters.Add(filter, data);
                }

                return data;
            }
        }

        internal EntityGroupData CreateOrGetEntityGroupData(params ISharedComponent[] sharedComponents)
        {
            var hashCode = EntityGroupData.CalculateSharedComponentHashCode(sharedComponents);
            lock (_entityGroups)
            {
                if (!_entityGroups.TryGetValue(hashCode, out var data))
                {
                    var archeTypeDatas = FilterComponentArcheTypeData(null, sharedComponents);
                    data = EntityGroupData.Initialize(this, hashCode,
                        sharedComponents,
                        archeTypeDatas);
                    data.NoRef += RemoveEntityGroupData;

                    _entityGroups.Add(hashCode, data);
                }

                return data;
            }
        }

        internal EntityFilterGroupData CreateOrGetEntityFilterGroupData(Filter filter,
            params ISharedComponent[] sharedComponents)
        {
            var hashCode = EntityFilterGroupData.CalculateFilterSharedComponentHashCode(filter,
                sharedComponents);
            lock (_entityFilterGroups)
            {
                if (!_entityFilterGroups.TryGetValue(hashCode, out var data))
                {
                    var archeTypeDatas = FilterComponentArcheTypeData(null, sharedComponents);
                    data = EntityFilterGroupData.Initialize(this,
                        hashCode,
                        filter,
                        sharedComponents,
                        archeTypeDatas);
                    data.NoRef += RemoveEntityFilterGroupData;

                    _entityFilterGroups.Add(hashCode, data);
                }

                return data;
            }
        }

        internal Entity CreateEntityPrep()
        {
            Entity entity;
            lock (ReuseableEntities)
            {
                if (ReuseableEntities.Count > 0)
                {
                    entity = ReuseableEntities.Dequeue();
                    entity.Version++;

                    return entity;
                }
            }

            lock (Entities)
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                if (Entities.Length == _nextId)
                    ResizeEntityCollections(Entities.Length * 2);
            }

            return entity;
        }

        internal Entity[] CreateEntitiesPrep(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count must be greater than 0");

            var entities = new Entity[count];
            lock (ReuseableEntities)
            {
                lock (Entities)
                {
                    var newSize = 0;
                    var activeEntityCount = Entities.Length - (_nextId - ReuseableEntities.Count);
                    if (activeEntityCount < count)
                    {
                        newSize = count - activeEntityCount;
                        newSize = (int)Math.Pow(2, (int)Math.Log(Entities.Length + newSize, 2) + 1);
                        ResizeEntityCollections(newSize);
                    }
                    else
                    {
                        newSize = count;
                    }

                    for (var i = 0; i < count; i++)
                    {
                        Entity entity;
                        if (ReuseableEntities.Count > 0)
                        {
                            entity = ReuseableEntities.Dequeue();
                            entity.Version++;
                        }
                        else
                        {
                            entity = new Entity
                            {
                                Id = _nextId++,
                                Version = 1
                            };
                        }

                        entities[i] = entity;
                    }
                }
            }

            return entities;
        }

        internal void DequeueEntityFromCommand(Entity entity, EntityBlueprint blueprint)
        {
            Entities[entity.Id] = entity;
            if (blueprint != null)
            {
                var archeTypeData = CreateOrGetComponentArcheTypeData(blueprint.CreateArcheType());
                archeTypeData.AddEntity(entity);
                EntityComponentArcheTypes[entity.Id] = archeTypeData;

                var bpComponents = blueprint.GetBlueprintComponents();
                for (var i = 0; i < bpComponents.Length; i++)
                {
                    var bpComponent = bpComponents[i];
                    var config = bpComponent.Config;
                    if (config.IsUnique)
                    {
                        if (UniqueComponentEntities[config.PoolIndex] != Entity.Null)
                            throw new EntityAlreadyHasUniqueComponentException(CurrentContext,
                                bpComponent.Component.GetType());
                        UniqueComponentEntities[config.PoolIndex] = entity;
                    }

                    ComponentPools[config.PoolIndex].AddComponent(entity.Id, bpComponent.Component);
                }
            }
        }

        internal IEntityCollection CreateEntityCollection()
        {
            lock (_entityCollections)
            {
                var collection = ObjectCache<EntityCollection>.Pop();

                if (collection.Entities.UncachedData.Length < _entitiesCurrentSize)
                    Array.Resize(ref collection.Entities.UncachedData, _entitiesCurrentSize);

                _entityCollections.Add(collection);

                return collection;
            }
        }

        internal void RemoveEntityCollection(IEntityCollection collection)
        {
            var entityCollection = (EntityCollection)collection;
            lock (_entityCollections)
            {
                _entityCollections.Remove(entityCollection);

                Array.Clear(entityCollection.Entities.UncachedData, 0, _entitiesCurrentSize);
                entityCollection.Entities.SetDirty();

                ObjectCache<EntityCollection>.Push(entityCollection);
            }
        }

        internal void AddEntityToArcheType(Entity entity, ComponentArcheType archeType)
        {
            var archeTypeData = CreateOrGetComponentArcheTypeData(archeType);
            if (archeTypeData != null)
                archeTypeData.AddEntity(entity);
            EntityComponentArcheTypes[entity.Id] = archeTypeData;
        }

        internal void RemoveEntityFromArcheType(Entity entity, ComponentArcheTypeData archeTypeData)
        {
            archeTypeData.RemoveEntity(entity);
            EntityComponentArcheTypes[entity.Id] = null;
            if (archeTypeData.Count == 0)
                RemoveComponentArcheTypeData(archeTypeData);
        }

        internal ComponentArcheTypeData[] FilterComponentArcheTypeData(Filter? filter,
            ISharedComponent[] sharedComponents)
        {
            lock (_componentArcheDataTypes)
            {
                var query = _componentArcheDataTypes
                    .AsParallel();

                if (filter != null)
                    query = query.Where(x => filter.Value.IsFiltered(x.Value.ArcheType));

                if (sharedComponents != null)
                    query = query.Where(x =>
                        x.Value.ArcheType.SharedComponents != null &&
                        x.Value.ArcheType.SharedComponents.SequenceEqual(sharedComponents));

                return query.Select(x => x.Value).ToArray();
            }
        }

        internal ComponentArcheTypeData CreateOrGetComponentArcheTypeData(ComponentArcheType archeType)
        {
            if (archeType.PoolIndexes == null)
                throw new ArgumentNullException();
            if (ComponentArcheType.IsEmpty(archeType))
                return null;

            var archeTypeHashCode = ComponentArcheType.CalculateHashCode(archeType);
            lock (_componentArcheDataTypes)
            {
                if (!_componentArcheDataTypes.TryGetValue(archeTypeHashCode, out var data))
                {
                    data = ComponentArcheTypeData.Initialize(archeType);
                    _componentArcheDataTypes.Add(archeTypeHashCode, data);

                    if (AnyArcheTypeDataAdded != null)
                        AnyArcheTypeDataAdded.Invoke(data);
                }

                return data;
            }
        }

        internal void RemoveComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
        {
            var archeTypeHashCode = ComponentArcheType.CalculateHashCode(archeTypeData.ArcheType);
            lock (_componentArcheDataTypes)
            {
                if (_componentArcheDataTypes.Remove(archeTypeHashCode))
                    ComponentArcheTypeData.Uninitialize(archeTypeData);
            }
        }

        internal void ResizeEntityCollections(int newSize)
        {
            if (newSize > _entitiesCurrentSize)
                lock (_entityCollections)
                {
                    _entitiesCurrentSize = newSize;
                    for (var i = 0; i < _entityCollections.Count; i++)
                        Array.Resize(ref _entityCollections[i].Entities.UncachedData, _entitiesCurrentSize);
                    for (var i = 0; i < ComponentPools.Length; i++)
                        ComponentPools[i].Resize(_entitiesCurrentSize);
                    Array.Resize(ref _entityComponentArcheTypes, _entitiesCurrentSize);
                }
        }

        private void RemoveEntityFilterData(EntityFilterData data)
        {
            lock (_entityFilters)
            {
                _entityFilters.Remove(data.Filter);
                EntityFilterData.Uninitialize(data);
            }
        }

        private void RemoveEntityGroupData(EntityGroupData data)
        {
            lock (_entityGroups)
            {
                _entityGroups.Remove(data.HashCode);
                EntityGroupData.Uninitialize(data);
            }
        }

        private void RemoveEntityFilterGroupData(EntityFilterGroupData data)
        {
            lock (_entityFilters)
            {
                _entityFilterGroups.Remove(data.HashCode);
                EntityFilterGroupData.Uninitialize(data);
            }
        }

        private class EntityCollection : IEntityCollection
        {
            internal readonly DataCache<Entity[], Entity[]> Entities;

            public EntityCollection()
            {
                Entities = new DataCache<Entity[], Entity[]>(
                    new Entity[0],
                    UpdateEntitiesCache);
            }

            public Entity this[int index]
            {
                get => Entities.UncachedData[index];
                set
                {
                    Entities.UncachedData[index] = value;
                    Entities.SetDirty();
                }
            }

            public int Length => Entities.UncachedData.Length;

            public bool HasEntity(Entity entity)
            {
                if (entity.Id <= 0 || entity.Id >= Entities.UncachedData.Length)
                    return false;

                return Entities.UncachedData[entity.Id] == entity;
            }

            public Entity[] GetEntities()
            {
                return Entities.CachedData;
            }

            internal void Resize(int newSize)
            {
                if (newSize > Entities.UncachedData.Length)
                    Array.Resize(ref Entities.UncachedData, newSize);
            }

            private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
            {
                return uncachedData
                    .Where(x => x != Entity.Null)
                    .ToArray();
            }
        }
    }
}