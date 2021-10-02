using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EcsContextData
    {
        private const int _arrayInitSize = 4;

        private Dictionary<int, ComponentArcheTypeData> _componentArcheTypes;
        private List<EntityCollection> _entityCollections;
        private int _arrayCurrentSize;
        private int _nextId = 1;

        public EcsContextData()
        {
            _componentArcheTypes = new Dictionary<int, ComponentArcheTypeData>();
            _entityCollections = new List<EntityCollection>();
            _arrayCurrentSize = _arrayInitSize;

            ComponentPools = ComponentPoolIndexes.Instance.CreateComponentPools(_arrayInitSize);
            EntityGroups = new Dictionary<GroupWithCollection, EntityGroup>();
            EntityCommandQueue = new Dictionary<string, EntityCommandQueue>();
            ReuseableEntities = new Queue<Entity>();
            UniqueEntities = new Entity[ComponentPoolIndexes.Instance.Count];
            EntityFilters = new Dictionary<Filter, EntityFilter>();
            EntityComponentArcheTypes = new ComponentArcheTypeData[_arrayInitSize];
        }

        public IComponentPool[] ComponentPools { get; private set; }
        public Dictionary<GroupWithCollection, EntityGroup> EntityGroups { get; private set; }
        public Dictionary<string, EntityCommandQueue> EntityCommandQueue { get; private set; }
        public Queue<Entity> ReuseableEntities { get; private set; }
        public Entity[] UniqueEntities { get; private set; }
        public Dictionary<Filter, EntityFilter> EntityFilters { get; private set; }
        public ComponentArcheTypeData[] EntityComponentArcheTypes;
        public EntityCollection Entities { get; private set; }

        public event ComponentArcheTypeDataEvent ArcheTypeDataAdded;
        public event ComponentArcheTypeDataEvent ArcheTypeDataRemoved;

        public Entity PrepCreateEntity()
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
                {
                    ResizeEntityCollections(Entities.Length * 2);
                }
            }

            return entity;
        }

        public Entity[] PrepCreateEntities(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count must be greater than 0");

            var entities = new Entity[count];
            lock (ReuseableEntities)
            {
                lock (Entities)
                {
                    int newSize = 0;
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

        public ComponentArcheTypeData[] FilterComponentArcheTypeData(Filter? filter,
            ISharedComponent[] sharedKeys)
        {
            lock (_componentArcheTypes)
            {
                var query = _componentArcheTypes
                   .AsParallel();

                if (filter != null)
                    query = query.Where(x => filter.Value.IsFiltered(x.Value.ArcheType));

                if (sharedKeys != null)
                    query = query.Where(x =>
                        x.Value.ArcheType.SharedComponents != null &&
                        HasSameSharedComponents(x.Value.ArcheType, sharedKeys));

                return query.Select(x => x.Value).ToArray();
            }
        }

        public ComponentArcheTypeData CreateOrGetComponentArcheTypeData(ComponentArcheType archeType)
        {
            if (ComponentArcheType.IsEmpty(archeType))
                return null;

            var archeTypeHashCode = ComponentArcheType.CalculateHashCode(archeType);
            lock (_componentArcheTypes)
            {
                if (!_componentArcheTypes.TryGetValue(archeTypeHashCode, out var archeTypeData))
                {
                    archeTypeData = ObjectCache.Pop<ComponentArcheTypeData>();
                    archeTypeData.Initialize(archeType);
                    _componentArcheTypes.Add(archeTypeHashCode, archeTypeData);

                    if (ArcheTypeDataAdded != null)
                        ArcheTypeDataAdded.Invoke(archeTypeData);
                }

                return archeTypeData;
            }
        }

        public void RemoveComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
        {
            var archeTypeHashCode = ComponentArcheType.CalculateHashCode(archeTypeData.ArcheType);
            lock (_componentArcheTypes)
            {
                if (_componentArcheTypes.Remove(archeTypeHashCode) && ArcheTypeDataRemoved != null)
                    ArcheTypeDataRemoved.Invoke(archeTypeData);
            }
        }

        public EntityCollection CreateEntityCollection()
        {
            lock (_entityCollections)
            {
                var collection = ObjectCache.Pop<EntityCollection>();
                collection.Initialize(_arrayCurrentSize);

                _entityCollections.Add(collection);

                return collection;
            }
        }

        public void RemoveEntityCollection(EntityCollection collection)
        {
            lock (_entityCollections)
            {
                collection.Reset();
                _entityCollections.Remove(collection);
            }
        }

        private static bool HasSameSharedComponents(ComponentArcheType lhs, ISharedComponent[] sharedComponents)
        {
            if (lhs.SharedComponents.Length != sharedComponents.Length)
                return false;

            for (int i = 0; i < lhs.SharedComponents.Length; i++)
                if (!lhs.SharedComponents[i].Equals(sharedComponents[i]))
                    return false;

            return true;
        }
        private void ResizeEntityCollections(int newSize)
        {
            if (newSize > _arrayCurrentSize)
            {
                lock (_entityCollections)
                {
                    for (int i = 0; i < _entityCollections.Count; i++)
                        _entityCollections[i].Resize(newSize);
                }
                for (int i = 0; i < ComponentPools.Length; i++)
                    ComponentPools[i].Resize(newSize);
                Array.Resize(ref EntityComponentArcheTypes, newSize);
                _arrayCurrentSize = newSize;
            }
        }

        #region ObjectCache

        internal void Initialize()
        {
            Entities = CreateEntityCollection();
        }

        internal void Reset()
        {
            foreach (var archeTypeData in _componentArcheTypes.Values)
            {
                archeTypeData.Reset();
                ObjectCache.Push(archeTypeData);
            }
            _componentArcheTypes.Clear();
            for (int i = 0; i < _entityCollections.Count; i++)
            {
                var collection = _entityCollections[i];
                collection.Reset();
                ObjectCache.Push(collection);
            }
            _entityCollections.Clear();
            _nextId = 1;

            for (int i = 0; i < ComponentPools.Length; i++)
                ComponentPools[i].Clear();
            foreach (var entityKey in EntityGroups.Values)
                entityKey.InternalDestroy();
            EntityGroups.Clear();
            foreach (var commandQueue in EntityCommandQueue.Values)
                commandQueue.InternalDestroy();
            EntityCommandQueue.Clear();
            ReuseableEntities.Clear();
            Array.Clear(UniqueEntities, 0, UniqueEntities.Length);
            foreach (var entityFilter in EntityFilters.Values)
                entityFilter.InternalDestroy();
            EntityFilters.Clear();
            Array.Clear(EntityComponentArcheTypes, 0, EntityComponentArcheTypes.Length);
            // Entities is clear/reset with _entityCollections

            ArcheTypeDataAdded = null;
            ArcheTypeDataRemoved = null;
        }

        #endregion
    }
}