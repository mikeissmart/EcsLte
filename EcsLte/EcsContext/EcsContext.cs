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

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            if (blueprint == null)
                throw new ArgumentNullException();

            var entity = CreateEntity();

            var isChanged = false;
            var nextArcheType = new ComponentArcheType();
            var bpComponents = blueprint.GetBlueprintComponents();
            for (int i = 0; i < bpComponents.Length; i++)
            {
                var bpComponent = bpComponents[i];
                if (!_data.ComponentPools[bpComponent.Config.Index].HasComponent(entity.Id))
                {
                    PrepAddComponent(entity,
                        bpComponent.Component,
                        bpComponent.Config,
                        nextArcheType,
                        out nextArcheType);
                    isChanged = true;
                }
            }

            if (isChanged)
                AddEntityToArcheType(entity, nextArcheType);

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

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            if (blueprint == null)
                throw new ArgumentNullException();

            var entities = _data.PrepCreateEntities(count);

            var archeTypeData = _data.CreateOrGetComponentArcheTypeData(blueprint.CreateArcheType());
            var bpComponents = blueprint.GetBlueprintComponents();
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _data.Entities[entity.Id] = entity;
                _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;
                archeTypeData.AddEntity(entity);

                for (int j = 0; j < bpComponents.Length; j++)
                {
                    var bpComponent = bpComponents[j];
                    var config = bpComponent.Config;
                    if (config.IsUnique)
                        throw new EntityAlreadyHasUniqueComponentException(this,
                            bpComponent.Component.GetType());
                    _data.ComponentPools[config.Index].AddComponent(entity.Id, bpComponent.Component);
                }
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

        internal void DequeueEntityFromCommand(Entity entity, EntityBlueprint blueprint)
        {
            _data.Entities[entity.Id] = entity;
            if (blueprint != null)
            {
                var archeTypeData = _data.CreateOrGetComponentArcheTypeData(blueprint.CreateArcheType());
                archeTypeData.AddEntity(entity);
                _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;

                var bpComponents = blueprint.GetBlueprintComponents();
                for (int i = 0; i < bpComponents.Length; i++)
                {
                    var bpComponent = bpComponents[i];
                    var config = bpComponent.Config;
                    if (config.IsUnique)
                    {
                        if (_data.UniqueEntities[config.Index] != Entity.Null)
                            throw new EntityAlreadyHasUniqueComponentException(this,
                                bpComponent.Component.GetType());
                        _data.UniqueEntities[config.Index] = entity;
                    }
                    _data.ComponentPools[config.Index].AddComponent(entity.Id, bpComponent.Component);
                }
            }
        }

        #endregion

        #region GetComponent

        public bool HasUniqueComponent<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.UniqueEntities[ComponentPoolIndex<TUniqueComponent>.Config.Index] != Entity.Null;
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

            return _data.UniqueEntities[ComponentPoolIndex<TUniqueComponent>.Config.Index];
        }

        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            return _data.ComponentPools[ComponentPoolIndex<TComponent>.Config.Index]
                .HasComponent(entity.Id);
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            return (TComponent)_data.ComponentPools[ComponentPoolIndex<TComponent>.Config.Index]
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

            var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
            var config = ComponentPoolIndex<TComponent>.Config;
            PrepAddComponent(entity,
                component,
                config,
                oldArcheTypeData != null
                    ? oldArcheTypeData.ArcheType
                    : new ComponentArcheType(),
                out var nextArcheType);

            if (oldArcheTypeData != null)
                RemoveEntityFromArcheType(entity, oldArcheTypeData);
            AddEntityToArcheType(entity, nextArcheType);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponent(entity, newComponent);
            else
            {
                var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
                var config = ComponentPoolIndex<TComponent>.Config;
                if (PrepReplaceComponent(entity,
                    newComponent,
                    config,
                    oldArcheTypeData.ArcheType,
                    out var nextArcheType))
                {
                    if (config.IsShared)
                    {
                        RemoveEntityFromArcheType(entity, oldArcheTypeData);
                        AddEntityToArcheType(entity, nextArcheType);
                    }
                    else
                        oldArcheTypeData.UpdateEntity(entity);
                }
            }
        }

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(this, entity, typeof(TComponent));

            var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
            var config = ComponentPoolIndex<TComponent>.Config;
            PrepRemovComponent(entity,
                config,
                oldArcheTypeData.ArcheType,
                out var nextArcheType);

            RemoveEntityFromArcheType(entity, oldArcheTypeData);
            AddEntityToArcheType(entity, nextArcheType);
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

        private void PrepAddComponent(Entity entity, IComponent component,
            ComponentPoolConfig config, ComponentArcheType oldArcheType,
            out ComponentArcheType nextArcheType)
        {
            if (config.IsUnique)
            {
                if (_data.UniqueEntities[config.Index] != Entity.Null)
                    throw new EntityAlreadyHasUniqueComponentException(this, component.GetType());
                _data.UniqueEntities[config.Index] = entity;
            }
            _data.ComponentPools[config.Index].AddComponent(entity.Id, component);

            if (config.IsShared)
                nextArcheType = ComponentArcheType.AppendSharedComponent(
                    oldArcheType, (ISharedComponent)component, config.Index);
            else
                nextArcheType = ComponentArcheType.AppendComponentPoolIndex(
                    oldArcheType, config.Index);
        }

        private bool PrepReplaceComponent(Entity entity, IComponent component,
            ComponentPoolConfig config, ComponentArcheType oldArcheType,
            out ComponentArcheType nextArcheType)
        {
            var componentPool = _data.ComponentPools[config.Index];
            var oldComponent = componentPool.GetComponent(entity.Id);
            if (oldComponent.Equals(component))
            {
                // Didnt update
                nextArcheType = oldArcheType;
                return false;
            }

            componentPool.ReplaceComponent(entity.Id, component);

            if (config.IsShared)
            {
                nextArcheType = ComponentArcheType.RemoveSharedComponent(
                    oldArcheType, (ISharedComponent)oldComponent, config.Index);
                nextArcheType = ComponentArcheType.AppendSharedComponent(
                    nextArcheType, (ISharedComponent)component, config.Index);
            }
            else
                nextArcheType = oldArcheType;

            return true;
        }

        private void PrepRemovComponent(Entity entity,
            ComponentPoolConfig config, ComponentArcheType oldArcheType,
            out ComponentArcheType nextArcheType)
        {
            if (config.IsUnique && _data.UniqueEntities[config.Index] == entity)
                _data.UniqueEntities[config.Index] = Entity.Null;

            var componentPool = _data.ComponentPools[config.Index];
            var oldComponent = componentPool.GetComponent(entity.Id);
            componentPool.RemoveComponent(entity.Id);

            if (config.IsShared)
                nextArcheType = ComponentArcheType.RemoveSharedComponent(
                    oldArcheType, (ISharedComponent)oldComponent, config.Index);
            else
                nextArcheType = ComponentArcheType.RemoveComponentPoolIndex(
                    oldArcheType, config.Index);
        }

        private bool PrepBlueprintAddComponent(Entity entity, BlueprintComponent bpComponent)
        {
            if (_data.ComponentPools[bpComponent.Config.Index].HasComponent(entity.Id))
                return false;

            if (bpComponent.Config.IsUnique)
            {
                if (_data.UniqueEntities[bpComponent.Config.Index] != Entity.Null)
                    throw new EntityAlreadyHasUniqueComponentException(this, bpComponent.Component.GetType());
                _data.UniqueEntities[bpComponent.Config.Index] = entity;
            }
            _data.ComponentPools[bpComponent.Config.Index].AddComponent(entity.Id, bpComponent.Component);

            return true;
        }

        private void RemoveEntityFromArcheType(Entity entity, ComponentArcheTypeData archeTypeData)
        {
            archeTypeData.RemoveEntity(entity);
            _data.EntityComponentArcheTypes[entity.Id] = null;
            if (archeTypeData.Count == 0)
                _data.RemoveComponentArcheTypeData(archeTypeData);
        }

        private void AddEntityToArcheType(Entity entity, ComponentArcheType archeType)
        {
            var archeTypeData = _data.CreateOrGetComponentArcheTypeData(archeType);
            if (archeTypeData != null)
                archeTypeData.AddEntity(entity);
            _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;
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
                    var archeTypeDatas = _data.FilterComponentArcheTypeData(filter, null);
                    entityFilter = new EntityFilter(this, _data, filter, archeTypeDatas);
                    _data.EntityFilters.Add(filter, entityFilter);
                }

                return entityFilter;
            }
        }

        #endregion

        #region  WithKey

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
                    var archeTypeDatas = _data.FilterComponentArcheTypeData(null, sharedComponents);
                    entityKey = new EntityGroup(this, _data,
                        archeTypeDatas.ToArray(),
                        sharedComponents);
                    _data.EntityGroups.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
        }

        #endregion
    }
}