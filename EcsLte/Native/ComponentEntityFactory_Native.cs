using EcsLte.Data.Unmanaged;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace EcsLte.Native
{
    public class ComponentEntityFactory_Native : IComponentEntityFactory
    {
        private static readonly int _initEntityLength = 4;

        private unsafe ComponentConfigOffset_Native* _configOffsets;
        private int _componentTotalSizeInBytes;
        private unsafe Entity* _entities;
        private unsafe EntityData_Native* _entityDatas;
        private int _entitiesCount;
        private int _entitiesLength;
        private Entity[] _cachedEntites;
        private bool _cachedEntitiesDirty;
        private unsafe Entity* _reusableEntities;
        private int _reusableEntitiesCount;
        private int _reusableEntitiesLength;
        private unsafe Entity* _uniqueComponentEntities;
        private EntityData_Native _tempBlueprintEntityData;
        //private readonly Dictionary<int, EntityQueryData> _masterEntityQueryDatas;
        //private readonly List<EntityQueryData>[] _componentEntityQueryDatas;
        private int _nextId;
        /// <summary>
        /// EntityData_Native*
        /// </summary>
        //TODO uncomment after blueprintBenchmark-private readonly List<(Entity, PtrWrapper, ComponentConfig)> _cacheAnyComponentChange;

        public int Count => _entitiesCount;
        public int Capacity => _entitiesLength;

        public unsafe ComponentEntityFactory_Native()
        {
            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset_Native>(ComponentConfigs.Instance.AllComponentCount);
            var offsetInBytes = ComponentConfigs.Instance.AllComponentCount;
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
            {
                var config = ComponentConfigs.Instance.AllComponentConfigs[i];
                _configOffsets[i] = new ComponentConfigOffset_Native
                {
                    Config = config,
                    OffsetInBytes = offsetInBytes,
                };
                offsetInBytes += config.UnmanagedSizeInBytes;
            }
            _componentTotalSizeInBytes = offsetInBytes;

            _entities = MemoryHelper.Alloc<Entity>(_initEntityLength);
            _entityDatas = MemoryHelper.Alloc<EntityData_Native>(_initEntityLength);
            _entitiesCount = 0;
            _entitiesLength = _initEntityLength;

            //_cachedEntites
            _cachedEntitiesDirty = true;

            _reusableEntities = MemoryHelper.Alloc<Entity>(_initEntityLength);
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = _initEntityLength;

            _uniqueComponentEntities = MemoryHelper.Alloc<Entity>(ComponentConfigs.Instance.UniqueComponentCount);
            _tempBlueprintEntityData = new EntityData_Native
            {
                AllComponents = (byte*)MemoryHelper.Alloc(_componentTotalSizeInBytes)
            };

            /*TODO uncomment after blueprintBenchmark-_masterEntityQueryDatas = new Dictionary<int, EntityQueryData>();
            _componentEntityQueryDatas = new List<EntityQueryData>[ComponentConfigs.Instance.AllComponentCount];
            for (int i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
                _componentEntityQueryDatas[i] = new List<EntityQueryData>();-TODO uncomment after blueprintBenchmark*/

            _nextId = 1;

            //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange = new List<(Entity, PtrWrapper, ComponentConfig)>();
        }

        public unsafe Entity[] GetEntities()
        {
            if (_cachedEntitiesDirty)
            {
                if (_cachedEntites == null)
                    _cachedEntites = new Entity[Count];
                else if (_cachedEntites.Length != Count)
                    Array.Resize(ref _cachedEntites, Count);

                var cachedIndex = 0;
                for (var i = 1; i <= Count; i++)
                {
                    var entity = _entities[i];
                    if (entity.IsNotNull)
                        _cachedEntites[cachedIndex++] = entity;
                }
                _cachedEntitiesDirty = false;
            }

            return _cachedEntites;
        }

        public unsafe bool HasEntity(Entity entity) => entity.Id > 0 &&
                entity.Id < Capacity &&
                _entities[entity.Id] == entity;

        public unsafe Entity CreateEntity(IEntityBlueprint blueprint)
        {
            CheckUnusedCapacity(1);

            var entity = AllocateEntity(out var entityData);
            if (blueprint != null)
            {
                //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Clear();
                foreach (var pair in ((EntityBlueprint_Native)blueprint).GetComponentConfigsAndDataOrdered())
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

                    pair.Value.CopyToBuffer(entityData->Component(_configOffsets[pair.Key.ComponentIndex].OffsetInBytes));
                    entityData->SetHasComponent(pair.Key.ComponentIndex, true);
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((entity, entityData, pair.Key));
                }
                /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                    OnAnyComponentAddedRemovedReplaced(entity, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
            }

            _cachedEntitiesDirty = true;

            return entity;
        }

        public unsafe Entity[] CreateEntities(int count, IEntityBlueprint blueprint)
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
                var componentConfigsAndDatas = ((EntityBlueprint_Native)blueprint).GetComponentConfigsAndDataOrdered();
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
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((Entity.Null, PtrWrapper.Null, pair.Key));
                }

                for (var i = 0; i < componentConfigsAndDatas.Length; i++)
                {
                    var pair = componentConfigsAndDatas[i];

                    pair.Value.CopyToBuffer(_tempBlueprintEntityData.Component(_configOffsets[pair.Key.ComponentIndex].OffsetInBytes));
                    _tempBlueprintEntityData.SetHasComponent(pair.Key.ComponentIndex, true);
                }

                for (var i = 0; i < count; i++)
                {
                    var entity = AllocateEntity(out var entityData);
                    entities[i] = entity;
                    MemoryHelper.Copy(
                        _tempBlueprintEntityData.AllComponents,
                        entityData->AllComponents,
                        _componentTotalSizeInBytes);
                    entityData->ComponentCount = componentConfigsAndDatas.Length;

                    /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                        OnAnyComponentAddedRemovedReplaced(entity, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
                }

                MemoryHelper.Clear(_tempBlueprintEntityData.AllComponents, _componentTotalSizeInBytes);
            }
            else
            {
                for (var i = 0; i < count; i++)
                    entities[i] = AllocateEntity(out _);
            }
            _cachedEntitiesDirty = true;

            return entities;
        }

        public unsafe void DestroyEntity(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            CheckReusedCapacity(1);

            var entityData = &_entityDatas[entity.Id];
            RemoveAllComponentsPostCheck(entity, entityData);
            _entities[entity.Id] = Entity.Null;
            _reusableEntities[_reusableEntitiesCount++] = entity;

            /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
            {
                foreach (var queryData in _componentEntityQueryDatas[item.Item3.ComponentIndex])
                    queryData.RemoveEntity(entity);
            }-TODO uncomment after blueprintBenchmark*/

            _entitiesCount--;
            _cachedEntitiesDirty = true;
        }

        public unsafe void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            CheckReusedCapacity(entities.Count());

            foreach (var entity in entities)
            {
                if (!HasEntity(entity))
                    throw new EntityDoesNotExistException(entity);

                var entityData = &_entityDatas[entity.Id];
                RemoveAllComponentsPostCheck(entity, entityData);
                _entities[entity.Id] = Entity.Null;
                _reusableEntities[_reusableEntitiesCount++] = entity;

                /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                {
                    foreach (var queryData in _componentEntityQueryDatas[item.Item3.ComponentIndex])
                        queryData.RemoveEntity(entity);
                }-TODO uncomment after blueprintBenchmark*/
            }

            _entitiesCount -= entities.Count();
            _cachedEntitiesDirty = true;
        }

        public unsafe bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var config = ComponentConfig<TComponent>.Config;

            return _entityDatas[entity.Id].GetHasComponent(config.ComponentIndex);
        }

        public unsafe TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            return GetComponentPostCheck<TComponent>(entity, &_entityDatas[entity.Id], _configOffsets[ComponentConfig<TComponent>.Config.ComponentIndex]);
        }

        public unsafe IComponent[] GetAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = _entityDatas[entity.Id];
            var components = new IComponent[entityData.ComponentCount];
            var componentsIndex = 0;
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
            {
                if (entityData.GetHasComponent(i))
                {
                    var configOffset = _configOffsets[i];
                    components[componentsIndex++] = (IComponent)Marshal.PtrToStructure(
                        (IntPtr)entityData.Component(configOffset.OffsetInBytes),
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
                    if (componentsIndex == components.Length)
                        break;
                }
            }

            return components;
        }

        public unsafe bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent => _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex] != Entity.Null;

        public unsafe TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];

            return GetComponentPostCheck<TComponentUnique>(entity, &_entityDatas[entity.Id], _configOffsets[config.ComponentIndex]);
        }

        public unsafe Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            return _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex];
        }

        public unsafe void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (HasComponent<TComponent>(entity))
                throw new EntityAlreadyHasComponentException(entity, typeof(TComponent));

            AddComponentPostCheck(entity, &_entityDatas[entity.Id], component, _configOffsets[ComponentConfig<TComponent>.Config.ComponentIndex]);
        }

        public unsafe void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                AddComponentPostCheck(entity, &_entityDatas[entity.Id], component, _configOffsets[ComponentConfig<TComponent>.Config.ComponentIndex]);
            else
                ReplaceComponentPostCheck(entity, &_entityDatas[entity.Id], component, _configOffsets[ComponentConfig<TComponent>.Config.ComponentIndex]);
        }

        public unsafe void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (!HasComponent<TComponent>(entity))
                throw new EntityNotHaveComponentException(entity, typeof(TComponent));

            RemoveComponentPostCheck(entity, &_entityDatas[entity.Id], _configOffsets[ComponentConfig<TComponent>.Config.ComponentIndex]);
        }

        public unsafe void RemoveAllComponents(Entity entity)
        {
            if (!HasEntity(entity))
                throw new EntityDoesNotExistException(entity);

            var entityData = &_entityDatas[entity.Id];
            RemoveAllComponentsPostCheck(entity, entityData);
            /*TODO uncomment after blueprintBenchmark-foreach (var item in _cacheAnyComponentChange)
                OnAnyComponentAddedRemovedReplaced(item.Item1, entityData, item.Item3);-TODO uncomment after blueprintBenchmark*/
        }

        public unsafe Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityAlreadyHasComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var entity = CreateEntity(null);

            AddComponentPostCheck(entity, &_entityDatas[entity.Id], componentUnique, _configOffsets[ComponentConfig<TComponentUnique>.Config.ComponentIndex]);

            return entity;
        }

        public unsafe Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
                return AddUniqueComponent(componentUnique);

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];

            ReplaceComponentPostCheck(entity, &_entityDatas[entity.Id], componentUnique, _configOffsets[config.ComponentIndex]);

            return entity;
        }

        public unsafe void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (!HasUniqueComponent<TComponentUnique>())
            {
                throw new EntityNotHaveComponentException(
                    _uniqueComponentEntities[ComponentConfig<TComponentUnique>.Config.UniqueIndex],
                    typeof(TComponentUnique));
            }

            var config = ComponentConfig<TComponentUnique>.Config;
            var entity = _uniqueComponentEntities[config.UniqueIndex];

            RemoveComponentPostCheck(entity, &_entityDatas[entity.Id], _configOffsets[config.ComponentIndex]);
        }

        public SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent => throw new NotImplementedException();

        public IEntityQuery EntityQueryCreate() => new EntityQuery(this);

        public unsafe void EntityQueryAddToMaster(IEntityQuery query)
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
                    entityQueryData.FilterEntity_Native(entity, &_entityDatas[entity.Id], _configOffsets);
            }-TODO uncomment after blueprintBenchmark*/
        }

        public unsafe bool EntityQueryHasEntity(IEntityQueryData queryData, Entity entity)
        {
            var entityQueryData = queryData as EntityQueryData;

            return HasEntity(entity) &&
                entityQueryData.Entities.Contains(entity);
        }

        public Entity[] EntityQueryGetEntities(EntityQueryData_ArcheType entityQueryData) => throw new NotImplementedException();

        public unsafe void ForEach<T1>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes));
            }
        }

        public unsafe void ForEach<T1, T2>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes));
            }
        }

        public unsafe void ForEach<T1, T2, T3>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes));
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];
            var config4 = _configOffsets[ComponentConfig<T4>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes),
                    *(T4*)entityData.Component(config4.OffsetInBytes));
            }
        }

        public unsafe void ForEach<T1, T2, T3, T4, T5>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];
            var config4 = _configOffsets[ComponentConfig<T4>.Config.ComponentIndex];
            var config5 = _configOffsets[ComponentConfig<T5>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes),
                    *(T4*)entityData.Component(config4.OffsetInBytes),
                    *(T5*)entityData.Component(config5.OffsetInBytes));
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
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];
            var config4 = _configOffsets[ComponentConfig<T4>.Config.ComponentIndex];
            var config5 = _configOffsets[ComponentConfig<T5>.Config.ComponentIndex];
            var config6 = _configOffsets[ComponentConfig<T6>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes),
                    *(T4*)entityData.Component(config4.OffsetInBytes),
                    *(T5*)entityData.Component(config5.OffsetInBytes),
                    *(T6*)entityData.Component(config6.OffsetInBytes));
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
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];
            var config4 = _configOffsets[ComponentConfig<T4>.Config.ComponentIndex];
            var config5 = _configOffsets[ComponentConfig<T5>.Config.ComponentIndex];
            var config6 = _configOffsets[ComponentConfig<T6>.Config.ComponentIndex];
            var config7 = _configOffsets[ComponentConfig<T7>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes),
                    *(T4*)entityData.Component(config4.OffsetInBytes),
                    *(T5*)entityData.Component(config5.OffsetInBytes),
                    *(T6*)entityData.Component(config6.OffsetInBytes),
                    *(T7*)entityData.Component(config7.OffsetInBytes));
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
            var entityQueryData = queryData as EntityQueryData;
            var entities = entityQueryData.Entities;

            var config1 = _configOffsets[ComponentConfig<T1>.Config.ComponentIndex];
            var config2 = _configOffsets[ComponentConfig<T2>.Config.ComponentIndex];
            var config3 = _configOffsets[ComponentConfig<T3>.Config.ComponentIndex];
            var config4 = _configOffsets[ComponentConfig<T4>.Config.ComponentIndex];
            var config5 = _configOffsets[ComponentConfig<T5>.Config.ComponentIndex];
            var config6 = _configOffsets[ComponentConfig<T6>.Config.ComponentIndex];
            var config7 = _configOffsets[ComponentConfig<T7>.Config.ComponentIndex];
            var config8 = _configOffsets[ComponentConfig<T8>.Config.ComponentIndex];

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var entityData = _entityDatas[entity.Id];

                action(entity,
                    *(T1*)entityData.Component(config1.OffsetInBytes),
                    *(T2*)entityData.Component(config2.OffsetInBytes),
                    *(T3*)entityData.Component(config3.OffsetInBytes),
                    *(T4*)entityData.Component(config4.OffsetInBytes),
                    *(T5*)entityData.Component(config5.OffsetInBytes),
                    *(T6*)entityData.Component(config6.OffsetInBytes),
                    *(T7*)entityData.Component(config7.OffsetInBytes),
                    *(T8*)entityData.Component(config8.OffsetInBytes));
            }
        }

        public unsafe void Dispose()
        {
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            _componentTotalSizeInBytes = 0;
            MemoryHelper.Free(_entities);
            _entities = null;
            for (var i = 0; i < _entitiesLength; i++)
            {
                var entityData = _entityDatas[i];
                if (entityData.AllComponents != null)
                    MemoryHelper.Free(entityData.AllComponents);
            }
            MemoryHelper.Free(_entityDatas);
            _entityDatas = null;
            _entitiesCount = 0;
            _entitiesLength = 0;
            _cachedEntites = null;
            _cachedEntitiesDirty = true;
            MemoryHelper.Free(_reusableEntities);
            _reusableEntities = null;
            _reusableEntitiesCount = 0;
            _reusableEntitiesLength = 0;
            MemoryHelper.Free(_uniqueComponentEntities);
            _uniqueComponentEntities = null;
            MemoryHelper.Free(_tempBlueprintEntityData.AllComponents);
            _tempBlueprintEntityData.AllComponents = null;
            /*TODO uncomment after blueprintBenchmark-_masterEntityQueryDatas.Clear();
            foreach (var queryData in _componentEntityQueryDatas)
                queryData.Clear();-TODO uncomment after blueprintBenchmark*/
            _nextId = 0;
        }

        private unsafe void CheckUnusedCapacity(int count)
        {
            // Account for Entity.Null
            var unusedCount = Capacity - ((Count + 1) - _reusableEntitiesCount);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(Capacity + count, 2) + 1);
                _entities = (Entity*)MemoryHelper.Realloc(
                    _entities,
                    _entitiesLength * TypeCache<Entity>.SizeInBytes,
                    newCapacity * TypeCache<Entity>.SizeInBytes);
                _entityDatas = (EntityData_Native*)MemoryHelper.Realloc(
                    _entityDatas,
                    _entitiesLength * TypeCache<EntityData_Native>.SizeInBytes,
                    newCapacity * TypeCache<EntityData_Native>.SizeInBytes);

                _entitiesLength = newCapacity;
            }
        }

        private unsafe void CheckReusedCapacity(int count)
        {
            var unusedCount = _reusableEntitiesLength - _reusableEntitiesCount;
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_reusableEntitiesLength + count, 2) + 1);
                _reusableEntities = (Entity*)MemoryHelper.Realloc(
                    _reusableEntities,
                    _reusableEntitiesLength * TypeCache<Entity>.SizeInBytes,
                    newCapacity * TypeCache<Entity>.SizeInBytes);

                _reusableEntitiesLength = newCapacity;
            }
        }

        private unsafe Entity AllocateEntity(out EntityData_Native* entityData)
        {
            Entity entity;
            if (_reusableEntitiesCount > 0)
            {
                entity = _reusableEntities[--_reusableEntitiesCount];
                entity.Version++;
                entityData = &_entityDatas[entity.Id];
            }
            else
            {
                entity = new Entity
                {
                    Id = _nextId++,
                    Version = 1
                };
                entityData = &_entityDatas[entity.Id];
                entityData->AllComponents = (byte*)MemoryHelper.Alloc(_componentTotalSizeInBytes);
            }

            _entities[entity.Id] = entity;
            _entitiesCount++;

            return entity;
        }

        private unsafe void AddComponentPostCheck<TComponent>(Entity entity, EntityData_Native* entityData, TComponent component, ComponentConfigOffset_Native configOffset) where TComponent : unmanaged, IComponent
        {
            if (configOffset.Config.IsUnique)
            {
                if (_uniqueComponentEntities[configOffset.Config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[configOffset.Config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
                }

                _uniqueComponentEntities[configOffset.Config.UniqueIndex] = entity;
            }

            *(TComponent*)entityData->Component(configOffset.OffsetInBytes) = component;
            entityData->SetHasComponent(configOffset.Config.ComponentIndex, true);
            entityData->ComponentCount++;
            //TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, configOffset.Config);
        }

        private unsafe void AddComponentPostCheck(Entity entity, EntityData_Native* entityData, void* component, ComponentConfigOffset_Native configOffset)
        {
            if (configOffset.Config.IsUnique)
            {
                if (_uniqueComponentEntities[configOffset.Config.UniqueIndex] != Entity.Null)
                {
                    throw new EntityAlreadyHasComponentException(
                        _uniqueComponentEntities[configOffset.Config.UniqueIndex],
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
                }

                _uniqueComponentEntities[configOffset.Config.UniqueIndex] = entity;
            }

            MemoryHelper.Copy(
                component,
                entityData->Component(configOffset.OffsetInBytes),
                configOffset.Config.UnmanagedSizeInBytes);
            entityData->SetHasComponent(configOffset.Config.ComponentIndex, true);
            entityData->ComponentCount++;
            //TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, configOffset.Config);
        }

        private unsafe void ReplaceComponentPostCheck<TComponent>(Entity entity, EntityData_Native* entityData, TComponent component, ComponentConfigOffset_Native configOffset) where TComponent : unmanaged, IComponent => *(TComponent*)entityData->Component(configOffset.OffsetInBytes) = component;//TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, configOffset.Config);

        private unsafe void ReplaceComponentPostCheck(Entity entity, EntityData_Native* entityData, void* component, ComponentConfigOffset_Native configOffset) => MemoryHelper.Copy(
                component,
                entityData->Component(configOffset.OffsetInBytes),
                configOffset.Config.UnmanagedSizeInBytes);//TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, configOffset.Config);

        private unsafe void RemoveComponentPostCheck(Entity entity, EntityData_Native* entityData, ComponentConfigOffset_Native configOffset)
        {
            if (configOffset.Config.IsUnique)
                _uniqueComponentEntities[configOffset.Config.UniqueIndex] = Entity.Null;
            entityData->SetHasComponent(configOffset.Config.ComponentIndex, false);
            entityData->ComponentCount--;
            //TODO uncomment after blueprintBenchmark-OnAnyComponentAddedRemovedReplaced(entity, entityData, configOffset.Config);
        }

        private unsafe void RemoveAllComponentsPostCheck(Entity entity, EntityData_Native* entityData)
        {
            //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Clear();
            for (var i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
            {
                if (entityData->GetHasComponent(i))
                {
                    var config = ComponentConfigs.Instance.AllComponentConfigs[i];
                    if (config.IsUnique)
                        _uniqueComponentEntities[config.UniqueIndex] = Entity.Null;
                    //TODO uncomment after blueprintBenchmark-_cacheAnyComponentChange.Add((entity, PtrWrapper.Null, config));
                }
            }
            MemoryHelper.Clear(entityData->AllComponents, _componentTotalSizeInBytes);
            entityData->ComponentCount = 0;
        }

        private unsafe TComponent GetComponentPostCheck<TComponent>(Entity entity, EntityData_Native* entityData, ComponentConfigOffset_Native configOffset) where TComponent : unmanaged, IComponent => *(TComponent*)entityData->Component(configOffset.OffsetInBytes);

        private unsafe byte* GetComponentPostCheck(Entity entity, EntityData_Native* entityData, ComponentConfigOffset_Native configOffset) => entityData->Component(configOffset.OffsetInBytes);

        /*TODO uncomment after blueprintBenchmark-private unsafe void OnAnyComponentAddedRemovedReplaced(Entity entity, EntityData_Native* entityData, ComponentConfig config)
        {
            foreach (var queryData in _componentEntityQueryDatas[config.ComponentIndex])
                queryData.FilterEntity_Native(entity, entityData, _configOffsets);
        }-TODO uncomment after blueprintBenchmark*/
    }
}
