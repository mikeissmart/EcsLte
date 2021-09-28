using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public partial class EcsContext :
        IGetEntity, IEntityLife, IGetComponent, IComponentLife, IFilterBy, IGroupWith
    {
        private EcsContextData _data;

        internal EcsContext(string name)
        {
            _data = ObjectCache.Pop<EcsContextData>();
            _data.Initialize();

            DefaultCommand = CommandQueue("Default");

            Name = name;
        }

        #region EcsContext

        public string Name { get; set; }
        public bool IsDestroyed { get; private set; }
        public EntityCommandQueue DefaultCommand { get; set; }

        public Entity AddUniqueComponent<TUniqueComponent>(TUniqueComponent uniqueComponent)
            where TUniqueComponent : IUniqueComponent
        {
            if (HasUniqueComponent<TUniqueComponent>())
                throw new EntityAlreadyHasUniqueComponentException(this, typeof(TUniqueComponent));

            var entity = CreateEntity();
            AddComponent(entity, uniqueComponent);

            return entity;
        }

        public Entity ReplaceUniqueComponent<TUniqueComponent>(TUniqueComponent newUniqueComponent)
            where TUniqueComponent : IUniqueComponent
        {
            if (!HasUniqueComponent<TUniqueComponent>())
                return AddUniqueComponent(newUniqueComponent);

            var entity = GetUniqueEntity<TUniqueComponent>();
            ReplaceComponent(entity, newUniqueComponent);

            return entity;
        }

        public Entity RemoveUniqueComponent<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            if (!HasUniqueComponent<TUniqueComponent>())
                throw new EntityNotHaveUniqueComponentException(this, typeof(TUniqueComponent));

            var entity = GetUniqueEntity<TUniqueComponent>();
            RemoveComponent<TUniqueComponent>(entity);
            if (GetAllComponents(entity).Length == 0)
            {
                DestroyEntity(entity);
                return Entity.Null;
            }

            return entity;
        }

        public EntityCommandQueue CommandQueue(string name)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (_data.EntityCommandQueue)
            {
                if (!_data.EntityCommandQueue.TryGetValue(name, out var commandQueue))
                {
                    commandQueue = new EntityCommandQueue(this, name);
                    _data.EntityCommandQueue.Add(name, commandQueue);
                }

                return commandQueue;
            }
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);

            IsDestroyed = true;
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (entity.Id <= 0 || entity.Id >= _data.Entities.Length)
                return false;

            return _data.Entities[entity.Id] == entity;
        }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Entities.GetEntities();
        }

        #endregion

        #region EntityLife

        public Entity CreateEntity()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entity = _data.PrepCreateEntity();
            _data.Entities[entity.Id] = entity;

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entities = _data.PrepCreateEntities(count);
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _data.Entities[entity.Id] = entity;
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            RemoveAllComponents(entity);
            _data.Entities[entity.Id] = Entity.Null;
            lock (_data.ReuseableEntities)
            {
                _data.ReuseableEntities.Enqueue(entity);
            }
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            lock (_data.ReuseableEntities)
            {
                foreach (var entity in entities)
                {
                    RemoveAllComponents(entity);
                    _data.Entities[entity.Id] = Entity.Null;
                    _data.ReuseableEntities.Enqueue(entity);
                }
            }
        }

        internal Entity EnqueueEntityFromCommand()
        {
            return _data.PrepCreateEntity();
        }

        internal Entity[] EnqueueEntitiesFromCommand(int count)
        {
            return _data.PrepCreateEntities(count);
        }

        internal void DequeueEntityFromCommand(Entity entity)
        {
            _data.Entities[entity.Id] = entity;
        }

        #endregion

        #region GetComponent

        public bool HasUniqueComponent<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.UniqueEntities[ComponentPoolIndex<TUniqueComponent>.Index] != Entity.Null;
        }

        public TUniqueComponent GetUniqueComponent<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            var entity = GetUniqueEntity<TUniqueComponent>();
            return GetComponent<TUniqueComponent>(entity);
        }

        public Entity GetUniqueEntity<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            if (!HasUniqueComponent<TUniqueComponent>())
                throw new EntityNotHaveUniqueComponentException(this, typeof(TUniqueComponent));

            return _data.UniqueEntities[ComponentPoolIndex<TUniqueComponent>.Index];
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            return _data.ComponentPools[ComponentPoolIndex<TComponent>.Index]
                .HasComponent(entity.Id);
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            return (TComponent)_data.ComponentPools[ComponentPoolIndex<TComponent>.Index]
                .GetComponent(entity.Id);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            var archeType = _data.EntityComponentArcheTypes[entity.Id];
            if (archeType == null)
                return new IComponent[0];

            var componentIndexes = archeType.ArcheType.ComponentPoolIndexes;
            var components = new IComponent[componentIndexes.Length];
            for (var i = 0; i < components.Length; i++)
            {
                var index = componentIndexes[i];
                components[i] = _data.ComponentPools[index]
                    .GetComponent(entity.Id);
            }

            return components;
        }

        #endregion

        #region ComponentLife

        public void AddUniqueComponent<TUniqueComponent>(Entity entity, TUniqueComponent uniqueComponent)
            where TUniqueComponent : IUniqueComponent
        {
            AddComponent(entity, uniqueComponent);
        }

        public void ReplaceUniqueComponent<TUniqueComponent>(Entity entity, TUniqueComponent newUniqueComponent)
            where TUniqueComponent : IUniqueComponent
        {
            ReplaceComponent(entity, newUniqueComponent);
        }

        public void RemoveUniqueComponent<TUniqueComponent>(Entity entity)
            where TUniqueComponent : IUniqueComponent
        {
            RemoveComponent<TUniqueComponent>(entity);
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(this, entity, typeof(TComponent));

            var componentPoolIndex = ComponentPoolIndex<TComponent>.Index;
            var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
            var nextArcheType = oldArcheTypeData != null
                ? ComponentArcheType.AppendComponentPoolIndex(oldArcheTypeData.ArcheType, componentPoolIndex)
                : ComponentArcheType.AppendComponentPoolIndex(componentPoolIndex);
            if (ComponentPoolIndex<TComponent>.IsUnique)
            {
                if (_data.UniqueEntities[componentPoolIndex] != Entity.Null)
                    throw new EntityAlreadyHasUniqueComponentException(this, typeof(TComponent));
                _data.UniqueEntities[componentPoolIndex] = entity;
            }
            if (ComponentPoolIndex<TComponent>.IsPrimary)
            {
                if (_data.FilterComponentArcheTypeData(null, (IPrimaryComponent)component, null).Length != 0)
                    throw new PrimaryComponentAlreadyHasException(this, (IPrimaryComponent)component);
                nextArcheType = ComponentArcheType.SetPrimaryComponent(nextArcheType, (IPrimaryComponent)component);
            }
            else if (ComponentPoolIndex<TComponent>.IsShared)
                nextArcheType = ComponentArcheType.AppendSharedComponent(nextArcheType, (ISharedComponent)component);
            var archeTypeData = _data.CreateOrGetComponentArcheTypeData(nextArcheType);

            _data.ComponentPools[componentPoolIndex].AddComponent(entity.Id, component);
            _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;
            if (oldArcheTypeData != null)
            {
                oldArcheTypeData.RemoveEntity(entity);
                if (oldArcheTypeData.Count == 0)
                    _data.RemoveComponentArcheTypeData(oldArcheTypeData);
            }
            archeTypeData.AddEntity(entity);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponent(entity, newComponent);
            else
            {
                var componentPoolIndex = ComponentPoolIndex<TComponent>.Index;

                var oldComponent = _data.ComponentPools[componentPoolIndex].GetComponent(entity.Id);
                _data.ComponentPools[componentPoolIndex].ReplaceComponent(entity.Id, newComponent);

                var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
                var nextArcheType = oldArcheTypeData.ArcheType;
                if (ComponentPoolIndex<TComponent>.IsPrimary)
                {
                    if (_data.FilterComponentArcheTypeData(null, (IPrimaryComponent)newComponent, null).Length != 0)
                        throw new PrimaryComponentAlreadyHasException(this, (IPrimaryComponent)newComponent);
                    nextArcheType = ComponentArcheType.RemovePrimaryComponent(nextArcheType);
                    nextArcheType = ComponentArcheType.SetPrimaryComponent(nextArcheType, (IPrimaryComponent)newComponent);

                    oldArcheTypeData.RemoveEntity(entity);
                    if (oldArcheTypeData.Count == 0)
                        _data.RemoveComponentArcheTypeData(oldArcheTypeData);

                    var newArcheTypeData = _data.CreateOrGetComponentArcheTypeData(nextArcheType);
                    newArcheTypeData.AddEntity(entity);
                    _data.EntityComponentArcheTypes[entity.Id] = newArcheTypeData;
                }
                else if (ComponentPoolIndex<TComponent>.IsShared)
                {
                    nextArcheType = ComponentArcheType.RemoveSharedComponent(nextArcheType, (ISharedComponent)oldComponent);
                    nextArcheType = ComponentArcheType.AppendSharedComponent(nextArcheType, (ISharedComponent)newComponent);

                    oldArcheTypeData.RemoveEntity(entity);
                    if (oldArcheTypeData.Count == 0)
                        _data.RemoveComponentArcheTypeData(oldArcheTypeData);

                    var newArcheTypeData = _data.CreateOrGetComponentArcheTypeData(nextArcheType);
                    newArcheTypeData.AddEntity(entity);
                    _data.EntityComponentArcheTypes[entity.Id] = newArcheTypeData;
                }
                else
                    oldArcheTypeData.UpdateEntity(entity);
            }
        }

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            var componentPoolIndex = ComponentPoolIndex<TComponent>.Index;
            var oldComponent = _data.ComponentPools[componentPoolIndex].GetComponent(entity.Id);
            var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
            var nextArcheType = ComponentArcheType.RemoveComponentPoolIndex(oldArcheTypeData.ArcheType, componentPoolIndex);
            if (ComponentPoolIndex<TComponent>.IsUnique && _data.UniqueEntities[componentPoolIndex] == entity)
                _data.UniqueEntities[componentPoolIndex] = Entity.Null;
            if (ComponentPoolIndex<TComponent>.IsPrimary)
                nextArcheType = ComponentArcheType.RemovePrimaryComponent(nextArcheType);
            else if (ComponentPoolIndex<TComponent>.IsShared)
                nextArcheType = ComponentArcheType
                    .RemoveSharedComponent(nextArcheType, (ISharedComponent)oldComponent);
            var archeTypeData = _data.CreateOrGetComponentArcheTypeData(nextArcheType);

            _data.ComponentPools[componentPoolIndex].RemoveComponent(entity.Id);
            _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;

            oldArcheTypeData.RemoveEntity(entity);
            if (oldArcheTypeData.Count == 0)
                _data.RemoveComponentArcheTypeData(oldArcheTypeData);

            if (archeTypeData != null)
                archeTypeData.AddEntity(entity);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            var archeTypeData = _data.EntityComponentArcheTypes[entity.Id];
            if (archeTypeData != null)
            {
                var archeType = archeTypeData.ArcheType;
                for (int i = 0; i < archeType.ComponentPoolIndexes.Length; i++)
                    _data.ComponentPools[archeType.ComponentPoolIndexes[i]].RemoveComponent(entity.Id);

                _data.EntityComponentArcheTypes[entity.Id] = null;
                archeTypeData.RemoveEntity(entity);

                if (archeTypeData.Count == 0)
                    _data.RemoveComponentArcheTypeData(archeTypeData);
            }
        }

        #endregion

        #region FilterEntity

        public EntityFilter FilterBy(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            lock (_data.EntityFilters)
            {
                if (!_data.EntityFilters.TryGetValue(filter, out var entityFilter))
                {
                    var archeTypeDatas = _data.FilterComponentArcheTypeData(filter, null, null);
                    entityFilter = new EntityFilter(this, _data, filter, archeTypeDatas);
                    _data.EntityFilters.Add(filter, entityFilter);
                }

                return entityFilter;
            }
        }

        #endregion

        #region  WithKey

        public EntityGroup GroupWith(IPrimaryComponent primaryComponent)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (primaryComponent == null)
                throw new ArgumentNullException();

            var keyCollection = new GroupWithCollection(new[] { primaryComponent });
            lock (_data.EntityGroups)
            {
                if (!_data.EntityGroups.TryGetValue(keyCollection, out var entityKey))
                {
                    var archeTypeDatas = _data.FilterComponentArcheTypeData(null, primaryComponent, null);
                    entityKey = new EntityGroup(this, _data,
                        archeTypeDatas,
                        primaryComponent,
                        null);
                    _data.EntityGroups.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        public EntityGroup GroupWith(ISharedComponent sharedComponent)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (sharedComponent == null)
                throw new ArgumentNullException();

            return GroupWith(new[] { sharedComponent });
        }

        public EntityGroup GroupWith(params ISharedComponent[] sharedComponents)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            foreach (var sharedComponent in sharedComponents)
                if (sharedComponent == null)
                    throw new ArgumentNullException();

            var keyCollection = new GroupWithCollection(sharedComponents);
            lock (_data.EntityGroups)
            {
                if (!_data.EntityGroups.TryGetValue(keyCollection, out var entityKey))
                {
                    var archeTypeDatas = _data.FilterComponentArcheTypeData(null, null, sharedComponents);
                    entityKey = new EntityGroup(this, _data,
                        archeTypeDatas.ToArray(),
                        null,
                        sharedComponents);
                    _data.EntityGroups.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        #endregion
    }
}