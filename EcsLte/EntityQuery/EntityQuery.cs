using EcsLte.Data;
using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityQuery
    {
        internal EntityQueryData QueryData { get; set; } = new EntityQueryData();

        internal EntityQuery(EcsContext context) => QueryData.Context = context;

        public unsafe bool HasEntity(Entity entity)
        {
            CheckQueryData();

            if (QueryData.Context.EntityManager.HasEntity(entity))
            {
                var entityData = QueryData.Context.EntityManager.EntityData[entity.Id];
                foreach (var archTypeData in QueryData.ArcheTypeDatas)
                {
                    if (archTypeData.Ptr == entityData.ArcheTypeData)
                        return true;
                }
            }

            return false;
        }

        public unsafe Entity[] GetEntities()
        {
            CheckQueryData();

            var entitiesCount = 0;
            foreach (var archeTypeDataPtr in QueryData.ArcheTypeDatas)
                entitiesCount += ((ArcheTypeData*)archeTypeDataPtr.Ptr)->EntityCount;

            var entities = new Entity[entitiesCount];
            var entitiesOffset = 0;
            foreach (var archeTypeDataPtr in QueryData.ArcheTypeDatas)
            {
                var archeTypeData = (ArcheTypeData*)archeTypeDataPtr.Ptr;
                archeTypeData->CopyEntities(ref entities, entitiesOffset);
                entitiesOffset += archeTypeData->EntityCount;
            }

            return entities;
        }

        internal void CheckQueryData()
        {
            if (QueryData.Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(QueryData.Context);
            if (QueryData.EntityQueryDataIndex == null)
                QueryData = QueryData.Context.QueryManager.IndexQueryData(QueryData);
            if (QueryData.ArcheTypeChangeVersion != QueryData.Context.ArcheTypeManager.ChangeVersion)
                QueryData.Context.ArcheTypeManager.UpdateArcheTypeDatas(QueryData);
        }

        #region WhereOfs Filters

        #region WhereOfs

        public bool HasWhereOf<TComponent>()
            where TComponent : IComponent
            => HasWhereAllOf<TComponent>() ||
                HasWhereAnyOf<TComponent>() ||
                HasWhereNoneOf<TComponent>();

        #region WhereAllOf

        public bool HasWhereAllOf<TComponent>()
            where TComponent : IComponent
            => QueryData.AllComponentConfigs.Contains(ComponentConfig<TComponent>.Config);

        public EntityQuery WhereAllOf<T1>()
            where T1 : IComponent
            => new EntityQuery(this,
                QueryCategory.All,
                ComponentConfig<T1>.Config);

        public EntityQuery WhereAllOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(this,
                QueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery WhereAllOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(this,
                QueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery WhereAllOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(this,
                QueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });
        #endregion

        #region WhereAnyOf

        public bool HasWhereAnyOf<TComponent>()
            where TComponent : IComponent
            => QueryData.AnyComponentConfigs.Contains(ComponentConfig<TComponent>.Config);

        public EntityQuery WhereAnyOf<T1>()
            where T1 : IComponent
            => new EntityQuery(this,
                QueryCategory.Any,
                ComponentConfig<T1>.Config);

        public EntityQuery WhereAnyOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(this,
                QueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery WhereAnyOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(this,
                QueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery WhereAnyOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(this,
                QueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        #endregion

        #region WhereNoneOf

        public bool HasWhereNoneOf<TComponent>()
            where TComponent : IComponent
            => QueryData.NoneComponentConfigs.Contains(ComponentConfig<TComponent>.Config);

        public EntityQuery WhereNoneOf<T1>()
            where T1 : IComponent
            => new EntityQuery(this,
                QueryCategory.None,
                ComponentConfig<T1>.Config);

        public EntityQuery WhereNoneOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(this,
                QueryCategory.None,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery WhereNoneOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(this,
                QueryCategory.None,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery WhereNoneOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(this,
                QueryCategory.None,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        #endregion

        #endregion

        #region FilterBys

        public bool HasFilterBy<TSharedComponent>()
            where TSharedComponent : ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            return QueryData.FilterComponents.Any(x => x.Config == config);
        }

        public TSharedComponent GetFilterBy<TSharedComponent>()
            where TSharedComponent : ISharedComponent
        {
            if (!HasFilterBy<TSharedComponent>())
            {
                throw new EntityQueryNothavFilterByException(
                    ComponentConfigs.Instance.AllComponentTypes[ComponentConfig<TSharedComponent>.Config.ComponentIndex]);
            }

            var config = ComponentConfig<TSharedComponent>.Config;
            return (TSharedComponent)QueryData.FilterComponents.First(x => x.Config == config).Component;
        }

        public EntityQuery FilterBy<T1>(
            T1 component1)
            where T1 : ISharedComponent
            => new EntityQuery(this,
                new EntityQueryFilterComponent<T1>(component1),
                false);

        public EntityQuery FilterBy<T1, T2>(
            T1 component1,
            T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2)
                },
                false);

        public EntityQuery FilterBy<T1, T2, T3>(
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2),
                    new EntityQueryFilterComponent<T3>(component3)
                },
                false);

        public EntityQuery FilterBy<T1, T2, T3, T4>(
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            where T4 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2),
                    new EntityQueryFilterComponent<T3>(component3),
                    new EntityQueryFilterComponent<T4>(component4)
                },
                false);

        public EntityQuery FilterByReplace<T1>(
            T1 component1)
            where T1 : ISharedComponent
            => new EntityQuery(this,
                new EntityQueryFilterComponent<T1>(component1),
                true);

        public EntityQuery FilterByReplace<T1, T2>(
            T1 component1,
            T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2)
                },
                true);

        public EntityQuery FilterByReplace<T1, T2, T3>(
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2),
                    new EntityQueryFilterComponent<T3>(component3)
                },
                true);

        public EntityQuery FilterByReplace<T1, T2, T3, T4>(
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            where T4 : ISharedComponent
            => new EntityQuery(this,
                new IEntityQueryFilterComponent[]
                {
                    new EntityQueryFilterComponent<T1>(component1),
                    new EntityQueryFilterComponent<T2>(component2),
                    new EntityQueryFilterComponent<T3>(component3),
                    new EntityQueryFilterComponent<T4>(component4)
                },
                true);

        #endregion

        #region Privates

        private EntityQuery(EntityQuery query, QueryCategory category, ComponentConfig config)
            : this(query.QueryData.Context)
        {
            CheckDistinctConfig(query.QueryData.AllComponentConfigs, config);
            CheckDistinctConfig(query.QueryData.AnyComponentConfigs, config);
            CheckDistinctConfig(query.QueryData.NoneComponentConfigs, config);

            switch (category)
            {
                case QueryCategory.All:
                    QueryData.AllComponentConfigs = AddOrReplaceConfig(query.QueryData.AllComponentConfigs, config, false);
                    QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
                    QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;
                    break;
                case QueryCategory.Any:
                    QueryData.AllComponentConfigs = query.QueryData.AllComponentConfigs;
                    QueryData.AnyComponentConfigs = AddOrReplaceConfig(query.QueryData.AnyComponentConfigs, config, false);
                    QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;
                    break;
                case QueryCategory.None:
                    QueryData.AllComponentConfigs = query.QueryData.AllComponentConfigs;
                    QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
                    QueryData.NoneComponentConfigs = AddOrReplaceConfig(query.QueryData.NoneComponentConfigs, config, false);
                    break;
            }

            QueryData.FilterComponents = query.QueryData.FilterComponents;
        }

        private EntityQuery(EntityQuery query, QueryCategory category, ComponentConfig[] configs)
            : this(query.QueryData.Context)
        {
            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(query.QueryData.AllComponentConfigs, configs);
            CheckDistinctConfigs(query.QueryData.AnyComponentConfigs, configs);
            CheckDistinctConfigs(query.QueryData.NoneComponentConfigs, configs);

            switch (category)
            {
                case QueryCategory.All:
                    QueryData.AllComponentConfigs = AddOrReplaceConfigs(query.QueryData.AllComponentConfigs, configs, false);
                    QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
                    QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;
                    break;
                case QueryCategory.Any:
                    QueryData.AllComponentConfigs = query.QueryData.AllComponentConfigs;
                    QueryData.AnyComponentConfigs = AddOrReplaceConfigs(query.QueryData.AnyComponentConfigs, configs, false);
                    QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;
                    break;
                case QueryCategory.None:
                    QueryData.AllComponentConfigs = query.QueryData.AllComponentConfigs;
                    QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
                    QueryData.NoneComponentConfigs = AddOrReplaceConfigs(query.QueryData.NoneComponentConfigs, configs, false);
                    break;
            }

            QueryData.FilterComponents = query.QueryData.FilterComponents;
        }

        private EntityQuery(EntityQuery query, IEntityQueryFilterComponent filterComponent, bool replace)
            : this(query.QueryData.Context)
        {
            CheckDistinctConfig(query.QueryData.AnyComponentConfigs, filterComponent.Config);
            CheckDistinctConfig(query.QueryData.NoneComponentConfigs, filterComponent.Config);

            QueryData.AllComponentConfigs = AddOrReplaceConfig(query.QueryData.AllComponentConfigs, filterComponent.Config, true);
            QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
            QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;

            QueryData.FilterComponents = AddOrReplaceFilterComponent(query.QueryData.FilterComponents, filterComponent, replace);
        }

        private EntityQuery(EntityQuery query, IEntityQueryFilterComponent[] filterComponents, bool replace)
            : this(query.QueryData.Context)
        {
            var configs = filterComponents.Select(x => x.Config).ToArray();

            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(QueryData.AnyComponentConfigs, configs);
            CheckDistinctConfigs(QueryData.NoneComponentConfigs, configs);

            QueryData.AllComponentConfigs = AddOrReplaceConfigs(query.QueryData.AllComponentConfigs, configs, true);
            QueryData.AnyComponentConfigs = query.QueryData.AnyComponentConfigs;
            QueryData.NoneComponentConfigs = query.QueryData.NoneComponentConfigs;

            QueryData.FilterComponents = AddOrReplaceFilterComponents(query.QueryData.FilterComponents, filterComponents, replace);
        }

        private static void CheckDuplicateConfigs(ComponentConfig[] configs)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                for (var j = i + 1; j < configs.Length; j++)
                {
                    if (configs[i] == configs[j])
                    {
                        throw new EntityQueryDuplicateComponentException(
                            ComponentConfigs.Instance.AllComponentTypes[configs[i].ComponentIndex]);
                    }
                }
            }
        }

        private static void CheckDistinctConfig(ComponentConfig[] sourceConfigs, ComponentConfig config)
        {
            if (sourceConfigs.Contains(config))
            {
                throw new EntityQueryAlreadyHasWhereException(
                    ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
            }
        }

        private static void CheckDistinctConfigs(ComponentConfig[] sourceConfigs, ComponentConfig[] configs)
        {
            if (sourceConfigs.Any(x => configs.Contains(x)))
            {
                throw new EntityQueryAlreadyHasWhereException(
                    ComponentConfigs.Instance.AllComponentTypes[configs.First(x => configs.Contains(x)).ComponentIndex]);
            }
        }

        private static ComponentConfig[] AddOrReplaceConfig(ComponentConfig[] sourceConfigs, ComponentConfig config, bool replace)
        {
            ComponentConfig[] destConfigs;
            if (sourceConfigs.Contains(config))
            {
                if (!replace)
                {
                    throw new EntityQueryAlreadyHasWhereException(
                        ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                }
                else
                {
                    destConfigs = sourceConfigs;
                }
            }
            else
            {
                destConfigs = new ComponentConfig[sourceConfigs.Length + 1];
                destConfigs[sourceConfigs.Length] = config;
                Array.Copy(sourceConfigs, destConfigs, sourceConfigs.Length);
                Array.Sort(destConfigs);
            }

            return destConfigs;
        }

        private static ComponentConfig[] AddOrReplaceConfigs(ComponentConfig[] sourceConfigs, ComponentConfig[] configs, bool replace)
        {
            var destConfigs = new List<ComponentConfig>(sourceConfigs);
            foreach (var config in configs)
            {
                if (destConfigs.Contains(config))
                {
                    if (!replace)
                    {
                        throw new EntityQueryAlreadyHasWhereException(
                            ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex]);
                    }
                }
                else
                {
                    destConfigs.Add(config);
                }
            }
            destConfigs.Sort();

            return destConfigs.ToArray();
        }

        private static IEntityQueryFilterComponent[] AddOrReplaceFilterComponent(IEntityQueryFilterComponent[] sourceFilterComponents, IEntityQueryFilterComponent filterComponent, bool replace)
        {
            IEntityQueryFilterComponent[] destFilterComponents;
            var hasFilterComponent = sourceFilterComponents.Any(x => x.Config == filterComponent.Config);
            if (hasFilterComponent && !replace)
            {
                throw new EntityQueryAlreadyFilteredByException(
                    ComponentConfigs.Instance.AllComponentTypes[filterComponent.Config.ComponentIndex]);
            }
            else if (hasFilterComponent)
            {
                destFilterComponents = new IEntityQueryFilterComponent[sourceFilterComponents.Length];
                Array.Copy(sourceFilterComponents, destFilterComponents, sourceFilterComponents.Length);
                var index = sourceFilterComponents
                    .Select((x, i) => (x, i))
                    .Where(x => x.x.Config == filterComponent.Config)
                    .Select(x => x.i)
                    .First();
                destFilterComponents[index] = filterComponent;
            }
            else
            {
                destFilterComponents = new IEntityQueryFilterComponent[sourceFilterComponents.Length + 1];
                destFilterComponents[sourceFilterComponents.Length] = filterComponent;
                Array.Copy(sourceFilterComponents, destFilterComponents, sourceFilterComponents.Length);
                Array.Sort(destFilterComponents);
            }

            return destFilterComponents;
        }

        private static IEntityQueryFilterComponent[] AddOrReplaceFilterComponents(IEntityQueryFilterComponent[] sourceFilterComponents, IEntityQueryFilterComponent[] filterComponents, bool replace)
        {
            var destFilterComponents = new List<IEntityQueryFilterComponent>(sourceFilterComponents);
            foreach (var filterComponent in filterComponents)
            {
                var hasFilterComponent = sourceFilterComponents.Any(x => x.Config == filterComponent.Config);
                if (hasFilterComponent && !replace)
                {
                    throw new EntityQueryAlreadyFilteredByException(
                        ComponentConfigs.Instance.AllComponentTypes[filterComponent.Config.ComponentIndex]);
                }
                else if (hasFilterComponent)
                {
                    var index = destFilterComponents
                        .Select((x, i) => (x, i))
                        .Where(x => x.x.Config == filterComponent.Config)
                        .Select(x => x.i)
                        .First();
                    destFilterComponents[index] = filterComponent;
                }
                else
                {
                    destFilterComponents.Add(filterComponent);
                }
            }
            destFilterComponents.Sort();

            return destFilterComponents.ToArray();
        }

        private enum QueryCategory
        {
            All,
            Any,
            None
        }

        #endregion

        #endregion

        #region ForEachs

        #region ForEachs

        public unsafe void ForEach(bool runParallel, EntityQueryActions.R0W0 action) => ForEachRun(runParallel,
                new ComponentConfig[0],
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity);
                });

        #region Write 0

        public unsafe void ForEach<T1>(bool runParallel, EntityQueryActions.R1W0<T1> action)
            where T1 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2>(bool runParallel, EntityQueryActions.R2W0<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3>(bool runParallel, EntityQueryActions.R3W0<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3, T4>(bool runParallel, EntityQueryActions.R4W0<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R5W0<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R6W0<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R7W0<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R8W0<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    action(index, entity,
                        in *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes),
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));
                });

        #endregion

        #region Write 1

        public unsafe void ForEach<T1>(bool runParallel, EntityQueryActions.R0W1<T1> action)
            where T1 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2>(bool runParallel, EntityQueryActions.R1W1<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3>(bool runParallel, EntityQueryActions.R2W1<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4>(bool runParallel, EntityQueryActions.R3W1<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R4W1<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R5W1<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R6W1<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R7W1<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        in *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes),
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                    }
                });

        #endregion

        #region Write 2

        public unsafe void ForEach<T1, T2>(bool runParallel, EntityQueryActions.R0W2<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3>(bool runParallel, EntityQueryActions.R1W2<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4>(bool runParallel, EntityQueryActions.R2W2<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R3W2<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R4W2<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R5W2<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R6W2<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        in *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes),
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                    }
                });

        #endregion

        #region Write 3

        public unsafe void ForEach<T1, T2, T3>(bool runParallel, EntityQueryActions.R0W3<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4>(bool runParallel, EntityQueryActions.R1W3<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R2W3<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R3W3<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R4W3<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R5W3<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes),
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                    }
                });

        #endregion

        #region Write 4

        public unsafe void ForEach<T1, T2, T3, T4>(bool runParallel, EntityQueryActions.R0W4<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R1W4<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R2W4<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R3W4<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R4W4<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes),
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                    }
                });

        #endregion

        #region Write 5

        public unsafe void ForEach<T1, T2, T3, T4, T5>(bool runParallel, EntityQueryActions.R0W5<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R1W5<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R2W5<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R3W5<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes),
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                    }
                });

        #endregion

        #region Write 6

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(bool runParallel, EntityQueryActions.R0W6<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R1W6<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R2W6<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes),
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                    }
                });

        #endregion

        #region Write 7

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(bool runParallel, EntityQueryActions.R0W7<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);
                    var component7 = *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component7);
                    }
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R1W7<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);
                    var component7 = *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        in *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes));

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component7);
                    }
                });

        #endregion

        #region Write 8

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(bool runParallel, EntityQueryActions.R0W8<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent => ForEachRun(runParallel,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                (int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets) =>
                {
                    var component1 = *(T1*)(componentsPtr + configOffsets[0].OffsetInBytes);
                    var component2 = *(T2*)(componentsPtr + configOffsets[1].OffsetInBytes);
                    var component3 = *(T3*)(componentsPtr + configOffsets[2].OffsetInBytes);
                    var component4 = *(T4*)(componentsPtr + configOffsets[3].OffsetInBytes);
                    var component5 = *(T5*)(componentsPtr + configOffsets[4].OffsetInBytes);
                    var component6 = *(T6*)(componentsPtr + configOffsets[5].OffsetInBytes);
                    var component7 = *(T7*)(componentsPtr + configOffsets[6].OffsetInBytes);
                    var component8 = *(T8*)(componentsPtr + configOffsets[7].OffsetInBytes);

                    action(index, entity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        ref component8);

                    if (QueryData.Context.EntityManager.HasEntity(entity))
                    {
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component1);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component2);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component3);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component4);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component5);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component6);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component7);
                        QueryData.Context.EntityManager.UpdateComponentNoCheck(entity, entityData, component8);
                    }
                });

        #endregion

        #endregion

        #region Privates

        private unsafe void ForEachRun(bool runParallel, ComponentConfig[] configs, ForEachRunAction action)
        {
            var missingConfigs = configs.Where(x => !QueryData.AllComponentConfigs.Contains(x));
            if (missingConfigs.Count() > 0)
            {
                throw new EntityQueryNotHaveWhereOfAllException(
                    ComponentConfigs.Instance.AllComponentTypes[missingConfigs.First().ComponentIndex]);
            }

            CheckDuplicateConfigs(configs);

            var configOffsets = new ComponentConfigOffset[configs.Length];
            var entities = GetEntities(); // This checks queryData
            var archeTypeDatasHash = new HashSet<PtrWrapper>(QueryData.ArcheTypeDatas);
            var entityDatas = QueryData.Context.EntityManager.EntityData;

            var ptrWrapper = new PtrWrapper();
            ArcheTypeData* prevArcheTypeData = null;
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                // Check if entity is alive
                if (!QueryData.Context.EntityManager.HasEntity(entity))
                    continue;

                var entityData = entityDatas[entity.Id];
                ptrWrapper.Ptr = entityData.ArcheTypeData;
                // Check if entity changed ArcheType
                if (!archeTypeDatasHash.Contains(ptrWrapper))
                    continue;

                // Re-cache component offsets
                if (prevArcheTypeData != entityData.ArcheTypeData)
                {
                    prevArcheTypeData = entityData.ArcheTypeData;
                    for (var j = 0; j < configs.Length; j++)
                        prevArcheTypeData->GetComponentOffset(configs[j], out configOffsets[j]);
                }

                action(i, entity, entityData, entityData.ArcheTypeData->GetComponentsPtr(entityData), configOffsets);
            }
        }

        private unsafe delegate void ForEachRunAction(int index, Entity entity, EntityData entityData, byte* componentsPtr, ComponentConfigOffset[] configOffsets);

        #endregion

        #endregion
    }
}
