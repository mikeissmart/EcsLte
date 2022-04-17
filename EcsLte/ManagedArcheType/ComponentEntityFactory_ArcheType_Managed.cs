using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.ManagedArcheType
{
    public class ComponentEntityFactory_ArcheType_Managed : IComponentEntityFactory
    {
        private static readonly int _entitiesInitCapacity = 4;

        private readonly DataCache<Entity[], Entity[]> _entities;
        private EntityData_ArcheType_Managed[] _entityDatas;
        private int _entitiesCount;
        private readonly Stack<Entity> _reusableEntities;
        private readonly Entity[] _uniqueComponentEntities;
        private readonly IComponent[] _uniqueComponents;
        private readonly ArcheTypeFactory_ArcheType_Managed _archeTypeFactory;
        private readonly IIndexDictionary[] _sharedComponentIndexes;
        private int _nextId;

        public int Count => _entitiesCount;
        public int Capacity => _entities.UncachedData.Length;

        public ComponentEntityFactory_ArcheType_Managed()
        {
            _entities = new DataCache<Entity[], Entity[]>(UpdateCachedEntities,
                new Entity[_entitiesInitCapacity],
                null);
            _entityDatas = new EntityData_ArcheType_Managed[_entitiesInitCapacity];
            _entitiesCount = 0;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.UniqueComponentCount];
            _uniqueComponents = new IComponent[ComponentConfigs.Instance.UniqueComponentCount];
            _archeTypeFactory = new ArcheTypeFactory_ArcheType_Managed();
            _sharedComponentIndexes = IndexDictionary.CreateSharedComponentIndexDictionaries();
            _nextId = 1;
        }

        public Entity[] GetEntities() => _entities.CachedData;

        public bool HasEntity(Entity entity) => entity.Id > 0 &&
                entity.Id < Capacity &&
                _entities.UncachedData[entity.Id] == entity;

        public Entity CreateEntity(IEntityBlueprint blueprint)
        {
            CheckUnusedCapacity(1);

            var entity = AllocateEntity(out var entityData);
            if (blueprint != null)
            {
                var archeType = ((EntityBlueprint_ArcheType_Managed)blueprint).GetArcheType(this, _sharedComponentIndexes, out var blueprintData);
                var archeTypeData = _archeTypeFactory.GetArcheTypeData(archeType);

                for (var i = 0; i < blueprintData.UniqueComponents.Length; i++)
                {
                    var config = blueprintData.UniqueConfigs[i];
                    var component = blueprintData.UniqueComponents[i];

                    if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                    {
                        throw new EntityAlreadyHasComponentException(_uniqueComponentEntities[config.UniqueIndex],
                            ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                    }

                    _uniqueComponentEntities[config.UniqueIndex] = entity;
                    archeTypeData.SetUniqueComponent(config, component, _uniqueComponents);
                }

                archeTypeData.AddEntity(_archeTypeFactory, entity, entityData);
                archeTypeData.SetEntityBlueprintData(entityData, blueprintData);
            }
            else
            {
                _archeTypeFactory.DefaultArcheTypeData.AddEntity(_archeTypeFactory, entity, entityData);
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
                var archeType = ((EntityBlueprint_ArcheType_Managed)blueprint).GetArcheType(this, _sharedComponentIndexes, out var blueprintData);
                var archeTypeData = _archeTypeFactory.GetArcheTypeData(archeType);

                if (blueprintData.UniqueComponents.Length > 0)
                {
                    var config = blueprintData.UniqueConfigs[0];
                    var uniqueEntity = _uniqueComponentEntities[config.UniqueIndex];
                    if (uniqueEntity == Entity.Null)
                        uniqueEntity = AllocateEntity(out _);

                    throw new EntityAlreadyHasComponentException(uniqueEntity,
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity(out var entityData);
                    entities[i] = entity;

                    archeTypeData.AddEntity(_archeTypeFactory, entity, entityData);
                    archeTypeData.SetEntityBlueprintData(entityData, blueprintData);
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity(out var entityData);
                    _archeTypeFactory.DefaultArcheTypeData.AddEntity(_archeTypeFactory, entity, entityData);
                    entities[i] = entity;
                }
            }
            _entities.SetDirty();

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            if (entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs != null)
            {
                foreach (var config in entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs.Where(x => x.IsUnique))
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            }
            entityData.ComponentArcheTypeData.RemoveEntity(_archeTypeFactory, entityData, _entityDatas);

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
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = _entityDatas[entity.Id];
                if (entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs != null)
                {
                    foreach (var config in entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs.Where(x => x.IsUnique))
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                }
                entityData.ComponentArcheTypeData.RemoveEntity(_archeTypeFactory, entityData, _entityDatas);

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
            var entityData = _entityDatas[entity.Id];

            return entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs != null &&
                entityData.ComponentArcheTypeData.ArcheType.ComponentConfigs.Contains(config);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return GetComponentPostCheck<TComponent>(entity, ComponentConfig<TComponent>.Config, _entityDatas[entity.Id]);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            if (entityData.ComponentArcheTypeData != null)
                return entityData.ComponentArcheTypeData.GetAllComponents(entityData, _uniqueComponents);
            return new IComponent[0];
        }

        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponentUnique>.Config;

            return _uniqueComponentEntities[config.UniqueIndex] != Entity.Null;
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;

            return (TComponentUnique)_entityDatas[_uniqueComponentEntities[config.UniqueIndex].Id]
                .ComponentArcheTypeData.GetUniqueComponent(config, _uniqueComponents);
        }

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config, _entityDatas[entity.Id]);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config, _entityDatas[entity.Id]);
            else
                ReplaceComponentPostCheck(entity, component, ComponentConfig<TComponent>.Config, _entityDatas[entity.Id]);
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            RemoveComponentPostCheck<TComponent>(entity, ComponentConfig<TComponent>.Config, _entityDatas[entity.Id]);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            var prevArcheTypeData = entityData.ComponentArcheTypeData;

            if (prevArcheTypeData.ArcheType.ComponentConfigs != null)
            {
                foreach (var config in prevArcheTypeData.ArcheType.ComponentConfigs)
                {
                    if (config.IsUnique)
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                }
            }
            if (prevArcheTypeData != _archeTypeFactory.DefaultArcheTypeData)
                _archeTypeFactory.DefaultArcheTypeData.TransferEntity(_archeTypeFactory, prevArcheTypeData, entity, entityData, _entityDatas);
        }

        public Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityAlreadyHasComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var entity = CreateEntity(null);
            AddComponentPostCheck(entity, componentUnique, ComponentConfig<TComponentUnique>.Config, _entityDatas[entity.Id]);

            return entity;
        }

        public Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(newComponentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            ReplaceComponentPostCheck(entity, newComponentUnique, config, _entityDatas[entity.Id]);

            return entity;
        }

        public void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];
            RemoveComponentPostCheck<TComponentUnique>(entity, config, _entityDatas[entity.Id]);
        }

        public SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            return new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component)
            };
        }

        public IEntityQuery EntityQueryCreate() => new EntityQuery_ArcheType(this);

        public void EntityQueryAddToMaster(IEntityQuery query)
        {
            //_archeTypeFactory.AddToMasterQuery(query as EntityQuery_ArcheType);
        }

        public unsafe bool EntityQueryHasEntity(IEntityQueryData queryData, Entity entity)
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            return HasEntity(entity) &&
                entityQueryData.HasArcheTypeData_Managed(_entityDatas[entity.Id].ComponentArcheTypeData);
        }

        public Entity[] EntityQueryGetEntities(EntityQueryData_ArcheType entityQueryData)
        {
            var totalEntities = 0;
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
                totalEntities += archeTypeData.EntityCount;

            var entities = new Entity[totalEntities];
            var startingIndex = 0;
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.CopyEntities(entities, startingIndex);
                startingIndex += archeTypeData.EntityCount;
            }

            return entities;
        }

        public unsafe void ForEach<T1>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0]);
                }
            }
        }

        public unsafe void ForEach<T1, T2>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2],
                        (T4)components[3]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4, T5>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2],
                        (T4)components[3],
                        (T5)components[4]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2],
                        (T4)components[3],
                        (T5)components[4],
                        (T6)components[5]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config,
                ComponentConfig<T7>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2],
                        (T4)components[3],
                        (T5)components[4],
                        (T6)components[5],
                        (T7)components[6]);
                }
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData_ArcheType;

            var configs = new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config,
                ComponentConfig<T7>.Config,
                ComponentConfig<T8>.Config
            };
            var mappedComponentIndexes = new int[configs.Length];
            var components = new IComponent[configs.Length];
            foreach (var archeTypeData in entityQueryData.ArcheType_Managed)
            {
                archeTypeData.GetMappedComponentIndexes(configs, ref mappedComponentIndexes);
                for (int i = 0, entityIndex = 0; i < archeTypeData.EntityCount; i++, entityIndex++)
                {
                    var entity = archeTypeData.GetEntityQueryForEachComponents(
                        entityIndex, mappedComponentIndexes, ref components);
                    action(entity,
                        (T1)components[0],
                        (T2)components[1],
                        (T3)components[2],
                        (T4)components[3],
                        (T5)components[4],
                        (T6)components[5],
                        (T7)components[6],
                        (T8)components[7]);
                }
            }
        }

        public void Dispose() { }

        private unsafe void CheckUnusedCapacity(int count)
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

        private Entity AllocateEntity(out EntityData_ArcheType_Managed entityData)
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
                entityData = new EntityData_ArcheType_Managed();
                _entityDatas[entity.Id] = entityData;
            }
            _entities.UncachedData[entity.Id] = entity;
            _entitiesCount++;

            return entity;
        }

        private void AddComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config, EntityData_ArcheType_Managed entityData) where TComponent : unmanaged, IComponent
        {
            Component_ArcheType_Managed nextArcheType;
            var prevArcheTypeData = entityData.ComponentArcheTypeData;

            if (prevArcheTypeData != null)
                nextArcheType = entityData.ComponentArcheTypeData.ArcheType;
            else
                nextArcheType = new Component_ArcheType_Managed();

            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                    throw new EntityAlreadyHasComponentException(_uniqueComponentEntities[config.UniqueIndex], typeof(TComponent));
                _uniqueComponentEntities[config.UniqueIndex] = entity;
                nextArcheType = Component_ArcheType_Managed.AppendComponent(nextArcheType, config);
            }
            else if (config.IsShared)
            {
                nextArcheType = Component_ArcheType_Managed.AppendComponent(
                    nextArcheType,
                    config,
                    _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component));
            }
            else
            {
                nextArcheType = Component_ArcheType_Managed.AppendComponent(nextArcheType, config);
            }

            var nextArcheTypeData = _archeTypeFactory.GetArcheTypeData(nextArcheType);
            nextArcheTypeData.TransferEntity(_archeTypeFactory, prevArcheTypeData, entity, entityData, _entityDatas);

            if (config.IsUnique)
                nextArcheTypeData.SetUniqueComponent(config, component, _uniqueComponents);
            else
                nextArcheTypeData.SetComponent(entityData, config, component);

        }

        private TComponent GetComponentPostCheck<TComponent>(Entity entity, ComponentConfig config, EntityData_ArcheType_Managed entityData) where TComponent : unmanaged, IComponent
        {
            if (config.IsUnique)
                return (TComponent)entityData.ComponentArcheTypeData.GetUniqueComponent(config, _uniqueComponents);
            return (TComponent)entityData.ComponentArcheTypeData.GetComponent(entityData, config);
        }

        private void ReplaceComponentPostCheck<TComponent>(Entity entity, TComponent component, ComponentConfig config, EntityData_ArcheType_Managed entityData) where TComponent : unmanaged, IComponent
        {
            var prevArcheTypeData = entityData.ComponentArcheTypeData;

            if (config.IsUnique)
            {
                prevArcheTypeData.SetUniqueComponent(config, component, _uniqueComponents);
            }
            else if (config.IsShared)
            {
                var nextSharedIndex = _sharedComponentIndexes[config.SharedIndex].GetIndexObj(component);
                if (!prevArcheTypeData.ArcheType.SharedComponentDataIndexes
                    .Any(x => x.SharedIndex == config.ComponentIndex &&
                        x.SharedDataIndex == nextSharedIndex))
                {
                    // Transfer entity since current archeTypeData does not have sharedComponent
                    var nextArcheType = Component_ArcheType_Managed.ReplaceSharedComponent(
                        prevArcheTypeData.ArcheType,
                        config,
                        nextSharedIndex);
                    var nextArcheTypeData = _archeTypeFactory.GetArcheTypeData(nextArcheType);

                    nextArcheTypeData.TransferEntity(_archeTypeFactory, prevArcheTypeData, entity, entityData, _entityDatas);
                    nextArcheTypeData.SetComponent(entityData, config, component);
                }
                else
                {
                    prevArcheTypeData.SetComponent(entityData, config, component);
                }
            }
            else
            {
                prevArcheTypeData.SetComponent(entityData, config, component);
            }
        }

        private void RemoveComponentPostCheck<TComponent>(Entity entity, ComponentConfig config, EntityData_ArcheType_Managed entityData) where TComponent : unmanaged, IComponent
        {
            var prevArcheTypeData = entityData.ComponentArcheTypeData;

            Component_ArcheType_Managed nextArcheType;
            if (prevArcheTypeData.ArcheType.ComponentConfigs.Length == 1)
            {
                // Only has one component, no need to check other configs
                _archeTypeFactory.DefaultArcheTypeData.TransferEntity(_archeTypeFactory, prevArcheTypeData, entity, entityData, _entityDatas);
                if (config.IsUnique)
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            }
            else
            {
                if (config.IsUnique)
                {
                    _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                    nextArcheType = Component_ArcheType_Managed.RemoveComponent(prevArcheTypeData.ArcheType, config);
                }
                else
                {
                    nextArcheType = config.IsShared
                        ? Component_ArcheType_Managed.RemoveSharedComponent(prevArcheTypeData.ArcheType, config)
                        : Component_ArcheType_Managed.RemoveComponent(prevArcheTypeData.ArcheType, config);
                }

                var nextArcheTypeData = _archeTypeFactory.GetArcheTypeData(nextArcheType);
                nextArcheTypeData.TransferEntity(_archeTypeFactory, prevArcheTypeData, entity, entityData, _entityDatas);
            }
        }

        private Entity[] UpdateCachedEntities(Entity[] uncachedData)
            => uncachedData
                .Where(x => x.Id != 0)
                .ToArray();
    }
}
