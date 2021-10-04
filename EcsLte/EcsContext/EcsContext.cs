using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EcsContext :
        IGetEntity, IGetWatcher, IEntityLife, IGetComponent, IComponentLife
    {
        private readonly EcsContextData _data;

        internal EcsContext(string name)
        {
            _data = EcsContextData.Initialize(this);

            Name = name;
            IsDestroyed = false;
            DefaultCommand = CommandQueue("Default");
        }

        #region EcsContext

        public string Name { get; private set; }
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
                if (!_data.EntityCommandQueue.TryGetValue(name, out var data))
                {
                    data = EntityCommandQueueData.Initialize(_data, name);
                    _data.EntityCommandQueue.Add(name, data);
                }

                return new EntityCommandQueue(this, data);
            }
        }

        public EntityFilter FilterBy(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return new EntityFilter(this, _data.CreateOrGetEntityFilterData(filter));
        }

        public EntityGroup GroupWith(ISharedComponent sharedComponent)
        {
            return GroupWith(new[] { sharedComponent });
        }

        public EntityGroup GroupWith(params ISharedComponent[] sharedComponents)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            foreach (var sharedComponent in sharedComponents)
                if (sharedComponent == null)
                    throw new ArgumentNullException();

            return new EntityGroup(this, _data.CreateOrGetEntityGroupData(sharedComponents));
        }

        public EntityFilterGroup FilterByGroupWith(Filter filter, ISharedComponent sharedComponent)
        {
            return FilterByGroupWith(filter, new[] { sharedComponent });
        }

        public EntityFilterGroup FilterByGroupWith(Filter filter, params ISharedComponent[] sharedComponents)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
            foreach (var sharedComponent in sharedComponents)
                if (sharedComponent == null)
                    throw new ArgumentNullException();

            return new EntityFilterGroup(this, _data
                .CreateOrGetEntityFilterGroupData(filter, sharedComponents));
        }

        internal void InternalDestroy()
        {
            EcsContextData.Uninitialize(_data);
            IsDestroyed = true;
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Entities.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Entities.GetEntities();
        }

        #endregion

        #region GetWatcher

        public Watcher WatchAdded(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Watchers.Added(this, filter);
        }

        public Watcher WatchUpdated(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Watchers.Updated(this, filter);
        }

        public Watcher WatchRemoved(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Watchers.Removed(this, filter);
        }

        public Watcher WatchAddedOrUpdated(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Watchers.AddedOrUpdated(this, filter);
        }

        public Watcher WatchAddedOrRemoved(Filter filter)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.Watchers.AddedOrRemoved(this, filter);
        }

        #endregion

        #region EntityLife

        public Entity CreateEntity()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entity = _data.CreateEntityPrep();
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
            for (var i = 0; i < bpComponents.Length; i++)
            {
                var bpComponent = bpComponents[i];
                if (!_data.ComponentPools[bpComponent.Config.Index].HasComponent(entity.Id))
                {
                    _data.PrepAddComponent(entity,
                        bpComponent.Component,
                        bpComponent.Config,
                        nextArcheType,
                        out nextArcheType);
                    isChanged = true;
                }
            }

            if (isChanged)
                _data.AddEntityToArcheType(entity, nextArcheType);

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            var entities = _data.CreateEntitiesPrep(count);
            for (var i = 0; i < entities.Length; i++)
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

            var entities = _data.CreateEntitiesPrep(count);

            var archeTypeData = _data.CreateOrGetComponentArcheTypeData(blueprint.CreateArcheType());
            var bpComponents = blueprint.GetBlueprintComponents();
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _data.Entities[entity.Id] = entity;
                _data.EntityComponentArcheTypes[entity.Id] = archeTypeData;
                archeTypeData.AddEntity(entity);

                for (var j = 0; j < bpComponents.Length; j++)
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

        public void DestroyEntities(IEnumerable<Entity> entities)
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

        #endregion

        #region GetComponent

        public bool HasUniqueComponent<TUniqueComponent>()
            where TUniqueComponent : IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return _data.UniqueComponentEntities[ComponentPoolIndex<TUniqueComponent>.Config.Index] != Entity.Null;
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

            return _data.UniqueComponentEntities[ComponentPoolIndex<TUniqueComponent>.Config.Index];
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
            _data.PrepAddComponent(entity,
                component,
                config,
                oldArcheTypeData != null
                    ? oldArcheTypeData.ArcheType
                    : new ComponentArcheType(),
                out var nextArcheType);

            if (oldArcheTypeData != null)
                _data.RemoveEntityFromArcheType(entity, oldArcheTypeData);
            _data.AddEntityToArcheType(entity, nextArcheType);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>(entity))
            {
                AddComponent(entity, newComponent);
            }
            else
            {
                var oldArcheTypeData = _data.EntityComponentArcheTypes[entity.Id];
                var config = ComponentPoolIndex<TComponent>.Config;
                if (_data.PrepReplaceComponent(entity,
                    newComponent,
                    config,
                    oldArcheTypeData.ArcheType,
                    out var nextArcheType))
                {
                    if (config.IsShared)
                    {
                        _data.RemoveEntityFromArcheType(entity, oldArcheTypeData);
                        _data.AddEntityToArcheType(entity, nextArcheType);
                    }
                    else
                    {
                        oldArcheTypeData.UpdateEntity(entity);
                    }
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
            _data.PrepRemovComponent(entity,
                config,
                oldArcheTypeData.ArcheType,
                out var nextArcheType);

            _data.RemoveEntityFromArcheType(entity, oldArcheTypeData);
            _data.AddEntityToArcheType(entity, nextArcheType);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(this, entity);

            var archeTypeData = _data.EntityComponentArcheTypes[entity.Id];
            if (archeTypeData != null)
            {
                var archeType = archeTypeData.ArcheType;
                for (var i = 0; i < archeType.ComponentPoolIndexes.Length; i++)
                    _data.ComponentPools[archeType.ComponentPoolIndexes[i]].RemoveComponent(entity.Id);

                _data.EntityComponentArcheTypes[entity.Id] = null;
                archeTypeData.RemoveEntity(entity);

                if (archeTypeData.Count == 0)
                    _data.RemoveComponentArcheTypeData(archeTypeData);
            }
        }

        /*private bool PrepBlueprintAddComponent(Entity entity, BlueprintComponent bpComponent)
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
        }*/

        #endregion

        #region GroupWith

        #endregion
    }
}