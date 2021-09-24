using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityManager
    {
        private const int _arrayInitSize = 4;

        private List<EntityCollection> _entityCollections;
        private int _arrayCurrentSize;
        private int _nextId = 1;

        public EntityManager()
        {
            _entityCollections = new List<EntityCollection>();
            _arrayCurrentSize = _arrayInitSize;

            ComponentPools = ComponentIndexes.Instance.CreateComponentPools(_arrayInitSize);
            PrimaryKeyes = ComponentIndexes.Instance.CreatePrimaryKeyes();
            SharedKeyes = ComponentIndexes.Instance.CreateSharedKeyes();
            EntityKeyes = new Dictionary<KeyCollection, EntityKey>();
            EntityCommandQueue = new Dictionary<string, EntityCommandQueue>();
            ReuseableEntities = new Queue<Entity>();
            UniqueEntities = new Entity[ComponentIndexes.Instance.Count];
            FilterComponentIndexes = new List<EntityFilter>[ComponentIndexes.Instance.Count];
            for (var i = 0; i < FilterComponentIndexes.Length; i++)
                FilterComponentIndexes[i] = new List<EntityFilter>();
            Filters = new Dictionary<Filter, EntityFilter>();
            EntityComponentIndexes = new List<int>[_arrayInitSize];
            for (var i = 0; i < EntityComponentIndexes.Length; i++)
                EntityComponentIndexes[i] = new List<int>();
            ComponentPoolEntityComponentAddedEvents = new EntityComponentChangedHandler[ComponentIndexes.Instance.Count];
            ComponentPoolEntityComponentReplacedEvents = new EntityComponentReplacedHandler[ComponentIndexes.Instance.Count];
            ComponentPoolEntityComponentRemovedEvents = new EntityComponentChangedHandler[ComponentIndexes.Instance.Count];
            EntityWillBeDestroyedEvents = new EntityEventHandler[_arrayInitSize];
            EntityComponentAddedEvents = new EntityComponentChangedHandler[_arrayInitSize];
            EntityComponentReplacedEvents = new EntityComponentReplacedHandler[_arrayInitSize];
            EntityComponentRemovedEvents = new EntityComponentChangedHandler[_arrayInitSize];
        }

        public IComponentPool[] ComponentPools { get; private set; }
        public Dictionary<int, IPrimaryKey> PrimaryKeyes { get; private set; }
        public Dictionary<int, ISharedKey> SharedKeyes { get; private set; }
        public Dictionary<KeyCollection, EntityKey> EntityKeyes { get; private set; }
        public Dictionary<string, EntityCommandQueue> EntityCommandQueue { get; private set; }
        public Queue<Entity> ReuseableEntities { get; private set; }
        public Entity[] UniqueEntities { get; private set; }
        public List<EntityFilter>[] FilterComponentIndexes { get; private set; }
        public Dictionary<Filter, EntityFilter> Filters { get; private set; }
        public EntityEventHandler AnyEntityWillBeDestroyedEvents;
        public EntityComponentChangedHandler AnyEntityComponentAddedEvents;
        public EntityComponentReplacedHandler AnyEntityComponentReplacedEvents;
        public EntityComponentChangedHandler AnyEntityComponentRemovedEvents;
        public EntityComponentChangedHandler[] ComponentPoolEntityComponentAddedEvents { get; private set; }
        public EntityComponentReplacedHandler[] ComponentPoolEntityComponentReplacedEvents { get; private set; }
        public EntityComponentChangedHandler[] ComponentPoolEntityComponentRemovedEvents { get; private set; }
        public EntityEventHandler[] EntityWillBeDestroyedEvents;
        public EntityComponentChangedHandler[] EntityComponentAddedEvents;
        public EntityComponentReplacedHandler[] EntityComponentReplacedEvents;
        public EntityComponentChangedHandler[] EntityComponentRemovedEvents;
        public EntityCollection Entities { get; private set; }
        public List<int>[] EntityComponentIndexes;

        public bool FilteredAllOf(int[] componentIndexes, Filter filter)
        {
            if (filter.AllOfIndexes == null || filter.AllOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.AllOfIndexes)
                if (!componentIndexes.Contains(index))
                    return false;

            return true;
        }

        public bool FilteredAnyOf(int[] componentIndexes, Filter filter)
        {
            if (filter.AnyOfIndexes == null || filter.AnyOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.AnyOfIndexes)
                if (componentIndexes.Contains(index))
                    return true;

            return false;
        }

        public bool FilteredNoneOf(int[] componentIndexes, Filter filter)
        {
            if (filter.NoneOfIndexes == null || filter.NoneOfIndexes.Length == 0)
                return true;

            foreach (var index in filter.NoneOfIndexes)
                if (componentIndexes.Contains(index))
                    return false;

            return true;
        }

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
                    var activeEntityCount = Entities.Length - (ReuseableEntities.Count - _nextId);
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

        public (int[], IComponent[]) SilentRemoveAllComponents(Entity entity)
        {
            var entityIndexes = EntityComponentIndexes[entity.Id];
            int[] componentIndexes;
            lock (entityIndexes)
            {
                componentIndexes = entityIndexes.ToArray();
                entityIndexes.Clear();
            }

            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < componentIndexes.Length; i++)
            {
                var componentPoolIndex = componentIndexes[i];

                if (ComponentIndexes.Instance.UniqueComponentIndexes.Any(x => x == componentPoolIndex) &&
                    UniqueEntities[componentPoolIndex] == entity)
                    UniqueEntities[componentPoolIndex] = Entity.Null;

                components[i] = ComponentPools[componentPoolIndex].GetComponent(entity.Id);
                ComponentPools[componentPoolIndex].RemoveComponent(entity.Id);
            }

            return (componentIndexes, components);
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
                Array.Resize(ref EntityWillBeDestroyedEvents, newSize);
                Array.Resize(ref EntityComponentAddedEvents, newSize);
                Array.Resize(ref EntityComponentReplacedEvents, newSize);
                Array.Resize(ref EntityComponentRemovedEvents, newSize);
                Array.Resize(ref EntityComponentIndexes, newSize);
                for (int i = _arrayCurrentSize; i < newSize; i++)
                    EntityComponentIndexes[i] = new List<int>();
                _arrayCurrentSize = newSize;
            }
        }

        #region ObjectCache

        internal void Initialize(EcsContext context)
        {
            Entities = CreateEntityCollection();

            foreach (var key in PrimaryKeyes.Values)
                key.Initialize(context, this);
            foreach (var key in SharedKeyes.Values)
                key.Initialize(context, this);
        }

        internal void Reset()
        {
            for (int i = 0; i < _entityCollections.Count; i++)
            {
                var collection = _entityCollections[i];
                collection.Reset();
                ObjectCache.Push(collection);
            }
            _entityCollections.Clear();
            _nextId = 1;

            foreach (var key in PrimaryKeyes.Values)
                key.Clear();
            foreach (var key in SharedKeyes.Values)
                key.Clear();
            EntityKeyes.Clear();
            for (int i = 0; i < ComponentPools.Length; i++)
                ComponentPools[i].Clear();
            foreach (var commandQueue in EntityCommandQueue.Values)
                commandQueue.InternalDestroy();
            EntityCommandQueue.Clear();
            ReuseableEntities.Clear();
            Array.Clear(UniqueEntities, 0, UniqueEntities.Length);
            for (var i = 0; i < FilterComponentIndexes.Length; i++)
                FilterComponentIndexes[i].Clear();
            Filters.Clear();
            AnyEntityComponentAddedEvents.Clear();
            AnyEntityComponentReplacedEvents.Clear();
            AnyEntityComponentRemovedEvents.Clear();
            Array.Clear(ComponentPoolEntityComponentAddedEvents, 0, ComponentPoolEntityComponentAddedEvents.Length);
            Array.Clear(ComponentPoolEntityComponentReplacedEvents, 0, ComponentPoolEntityComponentReplacedEvents.Length);
            Array.Clear(ComponentPoolEntityComponentRemovedEvents, 0, ComponentPoolEntityComponentRemovedEvents.Length);
            Array.Clear(EntityWillBeDestroyedEvents, 0, EntityWillBeDestroyedEvents.Length);
            Array.Clear(EntityComponentAddedEvents, 0, EntityComponentAddedEvents.Length);
            Array.Clear(EntityComponentReplacedEvents, 0, EntityComponentReplacedEvents.Length);
            Array.Clear(EntityComponentRemovedEvents, 0, EntityComponentRemovedEvents.Length);
            // Entities is clear/reset with _entityCollections
            for (var i = 0; i < EntityComponentIndexes.Length; i++)
                EntityComponentIndexes[i].Clear();
        }

        #endregion
    }
}