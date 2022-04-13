using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

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
        //TODO uncomment after blueprintBenchmark-private readonly Dictionary<int, EntityQueryData> _masterEntityQueryDatas;
        //TODO uncomment after blueprintBenchmark-private readonly List<EntityQueryData>[] _componentEntityQueryDatas;
        private int _nextId;
        //TODO uncomment after blueprintBenchmark-private readonly List<(Entity, EntityData_Managed, ComponentConfig)> _cacheAnyComponentChange;

        public int Count => _entitiesCount;
        public int Capacity => _entities.UncachedData.Length;

        public ComponentEntityFactory_Managed()
        {
            _entities = new DataCache<Entity[], Entity[]>(UpdateCachedEntities,
                new Entity[_entitiesInitCapacity],
                null);
            _entityDatas = new EntityData_Managed[_entitiesInitCapacity];
            _entitiesCount = 0;
            _reusableEntities = new Stack<Entity>();
            _uniqueComponentEntities = new Entity[ComponentConfigs.Instance.UniqueComponentCount];
            /*TODO uncomment after blueprintBenchmark-_masterEntityQueryDatas = new Dictionary<int, EntityQueryData>();
            _componentEntityQueryDatas = new List<EntityQueryData>[ComponentConfigs.Instance.AllComponentCount];
            for (int i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
                _componentEntityQueryDatas[i] = new List<EntityQueryData>();-TODO uncomment after blueprintBenchmark*/
            _nextId = 1;
            //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange = new List<(Entity, EntityData_Managed, ComponentConfig)>();
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
                //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Clear();
                foreach (var pair in ((EntityBlueprint_Managed)blueprint).GetComponentConfigsAndDataOrdered())
                {
                    if (pair.Key.IsUnique)
                    {
                        if (_uniqueComponentEntities[pair.Key.UniqueIndex] != Entity.Null)
                        {
                            throw new EntityAlreadyHasComponentException(
                                _uniqueComponentEntities[pair.Key.UniqueIndex],
                                ComponentConfigs.Instance.AllComponentTypes[pair.Key.ComponentIndex]);
                        }

                        _uniqueComponentEntities[pair.Key.UniqueIndex] = entity;
                    }

                    entityData.Components[pair.Key.ComponentIndex] = pair.Value;
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((entity, entityData, pair.Key));
                }
                /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                    OnAnyComponentAddedRemovedReplaced(entity, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
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
                //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Clear();
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
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((Entity.Null, null, pair.Key));
                }

                var components = new IComponent[ComponentConfigs.Instance.AllComponentCount];
                foreach (var pair in componentConfigsAndDatas)
                    components[pair.Key.ComponentIndex] = pair.Value;

                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity(out var entityData);
                    entities[i] = entity;
                    Array.Copy(components, entityData.Components, components.Length);

                    /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                        OnAnyComponentAddedRemovedReplaced(entity, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                    entities[i] = AllocateEntity(out _);
            }
            _entities.SetDirty();

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            RemoveAllComponentsPostCheck(entity, entityData);
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
                RemoveAllComponentsPostCheck(entity, entityData);
                _entities.UncachedData[entity.Id] = Entity.Null;
                _reusableEntities.Push(entity);

                /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                {
                    foreach (var queryData in _componentEntityQueryDatas[item.Item3.ComponentIndex])
                        queryData.RemoveEntity(entity);
                }-TODO uncomment after blueprintBenchmark*/
            }

            _entitiesCount -= entities.Count();
            _entities.SetDirty();
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            return _entityDatas[entity.Id].Components[ComponentConfig<TComponent>.Config.ComponentIndex] != null;
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
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;

            return GetComponentPostCheck<TComponentUnique>(_uniqueComponentEntities[config.UniqueIndex], config);
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

            AddComponentPostCheck(entity, _entityDatas[entity.Id], component, ComponentConfig<TComponent>.Config);
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponentPostCheck(entity, _entityDatas[entity.Id], component, ComponentConfig<TComponent>.Config);
            else
                ReplaceComponentPostCheck(entity, _entityDatas[entity.Id], component, ComponentConfig<TComponent>.Config);
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            RemoveComponentPostCheck(entity, _entityDatas[entity.Id], ComponentConfig<TComponent>.Config);
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            RemoveAllComponentsPostCheck(entity, entityData);
            /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                OnAnyComponentAddedRemovedReplaced(item.Item1, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
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

            AddComponentPostCheck(entity, _entityDatas[entity.Id], componentUnique, ComponentConfig<TComponentUnique>.Config);

            return entity;
        }

        public Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(componentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];

            ReplaceComponentPostCheck(entity, _entityDatas[entity.Id], componentUnique, config);

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

            RemoveComponentPostCheck(entity, _entityDatas[entity.Id], config);
        }

        public SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent => throw new NotImplementedException();

        public IEntityQuery EntityQueryCreate() => new EntityQuery(this);

        public void EntityQueryAddToMaster(IEntityQuery query)
        {
            /*TODO uncomment after blueprintBenchmark-var entityQuery = query as EntityQuery;
            if (entityQuery.Data.CheckConfigsZero())
                throw new EntityQueryNoWhereOfException();

            if (_masterEntityQueryDatas.TryGetValue(entityQuery.Data.GetHashCode(), out var entityQueryData))
                // Make sure query is using singular data
                entityQuery.Data = entityQueryData;
            else
            {
                // Add to master query and component queries
                _masterEntityQueryDatas.Add(entityQuery.Data.GetHashCode(), entityQuery.Data);
                foreach (var config in entityQueryData.AllConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);
                foreach (var config in entityQueryData.AnyConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);
                foreach (var config in entityQueryData.NoneConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);

                foreach (var entity in GetEntities())
                    entityQueryData.FilterEntity_Managed(entity, _entityDatas[entity.Id]);
            }-TODO uncomment after blueprintBenchmark*/
        }

        public bool EntityQueryHasEntity(IEntityQueryData queryData, Entity entity)
        {
            var entityQueryData = queryData as EntityQueryData;

            return HasEntity(entity) &&
                entityQueryData.Entities.Contains(entity);
        }

        public Entity[] EntityQueryGetEntities(EntityQueryData_ArcheType entityQueryData) => throw new NotImplementedException();

        public void ForEach<T1>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3, T4>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex],
                    (T4)entityData.Components[config4.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3, T4, T5>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex],
                    (T4)entityData.Components[config4.ComponentIndex],
                    (T5)entityData.Components[config5.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3, T4, T5, T6>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex],
                    (T4)entityData.Components[config4.ComponentIndex],
                    (T5)entityData.Components[config5.ComponentIndex],
                    (T6)entityData.Components[config6.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3, T4, T5, T6, T7>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex],
                    (T4)entityData.Components[config4.ComponentIndex],
                    (T5)entityData.Components[config5.ComponentIndex],
                    (T6)entityData.Components[config6.ComponentIndex],
                    (T7)entityData.Components[config7.ComponentIndex]);
            }
        }

        public void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;
            var config8 = ComponentConfig<T8>.Config;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    (T1)entityData.Components[config1.ComponentIndex],
                    (T2)entityData.Components[config2.ComponentIndex],
                    (T3)entityData.Components[config3.ComponentIndex],
                    (T4)entityData.Components[config4.ComponentIndex],
                    (T5)entityData.Components[config5.ComponentIndex],
                    (T6)entityData.Components[config6.ComponentIndex],
                    (T7)entityData.Components[config7.ComponentIndex],
                    (T8)entityData.Components[config8.ComponentIndex]);
            }
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

        private void AddComponentPostCheck(Entity entity, EntityData_Managed entityData, IComponent component, ComponentConfig config)
        {
            if (config.IsUnique)
            {
                if (_uniqueComponentEntities[config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }

                _uniqueComponentEntities[config.UniqueIndex] = entity;
            }

            entityData.Components[config.ComponentIndex] = component;
            //TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, config);
        }

        private void ReplaceComponentPostCheck(Entity entity, EntityData_Managed entityData, IComponent component, ComponentConfig config) => entityData.Components[config.ComponentIndex] = component;//TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, config);

        private void RemoveComponentPostCheck(Entity entity, EntityData_Managed entityData, ComponentConfig config)
        {
            if (config.IsUnique)
                _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
            entityData.Components[config.ComponentIndex] = null;
            //TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, config);
        }

        private void RemoveAllComponentsPostCheck(Entity entity, EntityData_Managed entityData)
        {
            //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Clear();
            for (var i = 0; i < entityData.Components.Length; i++)
            {
                if (entityData.Components[i] != null)
                {
                    var config = ComponentConfigs.Instance.AllComponentConfigs[i];
                    if (config.IsUnique)
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((entity, null, config));
                    entityData.Components[i] = null;
                }
            }
        }

        private TComponent GetComponentPostCheck<TComponent>(Entity entity, ComponentConfig config) where TComponent : unmanaged, IComponent => (TComponent)_entityDatas[entity.Id].Components[config.ComponentIndex];

        /*TODO uncomment after blueprintBenchmark-private void OnAnyComponentAddedRemovedReplaced(Entity entity, EntityData_Managed entityData, ComponentConfig config)
        {
            foreach (var queryData in _componentEntityQueryDatas[config.ComponentIndex])
                queryData.FilterEntity_Managed(entity, entityData);
        }-TODO uncomment after blueprintBenchmark*/

        private Entity[] UpdateCachedEntities(Entity[] uncachedData)
            => uncachedData
                .Where(x => x.Id != 0)
                .ToArray();
    }
}
