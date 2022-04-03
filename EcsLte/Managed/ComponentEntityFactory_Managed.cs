using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte.Managed
{
    public class ComponentEntityFactory_Managed : IComponentEntityFactory
    {
        private static readonly int _entitiesInitCapacity = 4;

        private readonly DataCache<Entity[], Entity[]> _entities;
        private EntityData_Managed[] _entityDatas;
        private int _entitiesCount;
        private readonly Stack<Entity> _reusableEntities;
        private readonly Entity[] _uniqueComponentEntities;
        private int _nextId;

        public int Count { get => _entitiesCount; }
        public int Capacity { get => _entities.UncachedData.Length; }

        public ComponentEntityFactory_Managed()
        {
            _entities = new DataCache<Entity[], Entity[]>(UpdateCachedEntities,
                new Entity[_entitiesInitCapacity],
                null);
            _entityDatas = new EntityData_Managed[_entitiesInitCapacity];
            _entitiesCount = 0;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.UniqueComponentCount];
            _nextId = 1;
        }

        public Entity[] GetEntities()
        {
            return _entities.CachedData;
        }

        public bool HasEntity(Entity entity)
        {
            return entity.Id > 0 &&
                entity.Id < Capacity &&
                _entities.UncachedData[entity.Id] == entity;
        }

        public Entity CreateEntity(IEntityBlueprint blueprint)
        {
            CheckUnusedCapacity(1);

            var entity = AllocateEntity(out var entityData);
            if (blueprint != null)
            {
                foreach (var pair in ((EntityBlueprint_Managed)blueprint).GetComponentConfigsAndDataOrdered())
                    AddComponentPostCheck(entity, entityData, pair.Value, pair.Key);
            }
            _entities.SetDirty();

            return entity;
        }

        public Entity[] CreateEntities(int count, IEntityBlueprint blueprint)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count", "Must be greater than 0.");
            if (count == 1)
                return new Entity[] { CreateEntity(blueprint) };

            CheckUnusedCapacity(count);

            var entities = new Entity[count];
            if (blueprint != null)
            {
                var componentConfigsAndDatas = ((EntityBlueprint_Managed)blueprint).GetComponentConfigsAndDataOrdered();
                foreach (var pair in componentConfigsAndDatas)
                {
                    if (pair.Key.IsUnique)
                    {
                        var config = pair.Key;
                        var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                        if (uniqueEntity == Entity.Null)
                            uniqueEntity = AllocateEntity(out _);

                        throw new EntityAlreadyHasComponentException(uniqueEntity,
                            ComponentConfigs.Instance.AllComponentTypes[pair.Key.ComponentIndex]);
                    }
                }

                var components = new IComponent[ComponentConfigs.Instance.AllComponentCount];
                foreach (var pair in componentConfigsAndDatas)
                    components[pair.Key.ComponentIndex] = pair.Value;

                for (int i = 0; i < count; i++)
                {
                    entities[i] = AllocateEntity(out var entityData);
                    Array.Copy(components, entityData.Components, components.Length);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                    entities[i] = AllocateEntity(out _);
            }
            _entities.SetDirty();

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            // Remove components checks HasEntity
            RemoveAllComponents(entity);

            _entities.UncachedData[entity.Id] = Entity.Null;
            _reusableEntities.Push(entity);
            _entitiesCount--;
            _entities.SetDirty();
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                RemoveAllComponents(entity);

                _entities.UncachedData[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);
            }

            _entitiesCount -= entities.Count();
            _entities.SetDirty();
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var config = ComponentConfig<TComponent>.Config;

            return _entityDatas[entity.Id].Components[config.ComponentIndex] != null;
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return GetComponentPostCheck<TComponent>(entity, ComponentConfig<TComponent>.Config);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            return _entityDatas[entity.Id].Components
                .Where(x => x != null)
                .ToArray();
        }

        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponentUnique>.Config;

            return _uniqueComponentEntities[config.UniqueIndex] != Entity.Null;
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));

            var config = ComponentConfig<TComponentUnique>.Config;

            return GetComponentPostCheck<TComponentUnique>(_uniqueComponentEntities[config.UniqueIndex], config);
        }

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
            {
                AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config);
                return;
            }

            var config = ComponentConfig<TComponent>.Config;
            _entityDatas[entity.Id].Components[config.ComponentIndex] = component;
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            _entityDatas[entity.Id].Components[ComponentConfig<TComponent>.Config.ComponentIndex] = null;
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            for (int i = 0; i < entityData.Components.Length; i++)
                entityData.Components[i] = null;
        }

        public Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (HasUniqueComponent<TComponentUnique>())
                throw new EntityAlreadyHasComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = CreateEntity(null);
            AddComponentPostCheck(entity, componentUnique, config);
            _uniqueComponentEntities[config.UniqueIndex] = entity;

            return entity;
        }

        public Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(newComponentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            _entityDatas[entity.Id].Components[config.ComponentIndex] = newComponentUnique;

            return entity;
        }

        public void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            _entityDatas[entity.Id].Components[config.ComponentIndex] = null;
            _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
        }

        public void Dispose() { }

        private void CheckUnusedCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = Capacity - ((Count + 1) - _reusableEntities.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(Capacity + count, 2) + 1);
                Array.Resize(ref _entities.UncachedData, newCapacity);
                Array.Resize(ref _entityDatas, newCapacity);
            }
        }

        private Entity AllocateEntity(out EntityData_Managed entityData)
        {
            Entity entity;
            if (_reusableEntities.Count > 0)
            {
                entity = _reusableEntities.Pop();
                entity.Version++;
                entityData = _entityDatas[entity.Id];
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                entityData = new EntityData_Managed
                {
                    Components = new IComponent[ComponentConfigs.Instance.AllComponentCount]
                };
                _entityDatas[entity.Id] = entityData;
            }
            _entities.UncachedData[entity.Id] = entity;
            _entitiesCount++;

            return entity;
        }

        private void AddComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }

            _entityDatas[entity.Id].Components[config.ComponentIndex] = component;
        }

        private void AddComponentPostCheck(Entity entity, EntityData_Managed entityData, IComponent component, ComponentConfig config)
        {
            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }

            entityData.Components[config.ComponentIndex] = component;
        }

        private TComponent GetComponentPostCheck<TComponent>(Entity entity, ComponentConfig config) where TComponent : unmanaged, IComponent
        {
            return (TComponent)_entityDatas[entity.Id].Components[config.ComponentIndex];
        }

        private Entity[] UpdateCachedEntities(Entity[] uncachedData)
            => uncachedData
                .Where(x => x.Id != 0)
                .ToArray();
    }
}
