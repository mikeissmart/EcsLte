using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsLte
{
    public class EntityQuery : IEquatable<EntityQuery>
    {
        private static readonly int _threadCount = (int)(Environment.ProcessorCount * 0.75);

        public Type[] AllComponentTypes => QueryData.AllComponentTypes;
        public Type[] AnyComponentTypes => QueryData.AnyComponentTypes;
        public Type[] NoneComponentTypes => QueryData.NoneComponentTypes;
        public ISharedComponent[] FilterComponents => QueryData.FilterComponents;
        internal EntityQueryData QueryData { get; set; }

        public EntityQuery() => QueryData = new EntityQueryData();

        public static bool operator !=(EntityQuery lhs, EntityQuery rhs) => !(lhs == rhs);

        public static bool operator ==(EntityQuery lhs, EntityQuery rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            return lhs.QueryData == rhs.QueryData;
        }

        public bool Equals(EntityQuery other) => this == other;

        public override bool Equals(object other) => other is EntityQuery obj && this == obj;

        public override int GetHashCode() => QueryData.GetHashCode();

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
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < QueryData.AllComponentConfigs.Length; i++)
            {
                if (QueryData.AllComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityQuery WhereAllOf(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            return new EntityQuery(this,
                QueryCategory.All,
                entityArcheType.ArcheTypeData.ComponentConfigs);
        }

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

        #endregion WhereAllOf

        #region WhereAnyOf

        public bool HasWhereAnyOf<TComponent>()
            where TComponent : IComponent
            => QueryData.AnyComponentConfigs.Contains(ComponentConfig<TComponent>.Config);

        public EntityQuery WhereAnyOf(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            return new EntityQuery(this,
                QueryCategory.Any,
                entityArcheType.ArcheTypeData.ComponentConfigs);
        }

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

        #endregion WhereAnyOf

        #region WhereNoneOf

        public bool HasWhereNoneOf<TComponent>()
            where TComponent : IComponent
            => QueryData.NoneComponentConfigs.Contains(ComponentConfig<TComponent>.Config);

        public EntityQuery WhereNoneOf(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            return new EntityQuery(this,
                QueryCategory.None,
                entityArcheType.ArcheTypeData.ComponentConfigs);
        }

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

        #endregion WhereNoneOf

        #endregion WhereOfs

        #region FilterBys

        public bool HasFilterBy<TSharedComponent>()
            where TSharedComponent : ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            return QueryData.FilterComponentDatas.Any(x => x.Config == config);
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
            return (TSharedComponent)QueryData.FilterComponentDatas.First(x => x.Config == config).Component;
        }

        public EntityQuery FilterBy(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            return new EntityQuery(this,
                entityArcheType.ArcheTypeData.SharedComponentDatas,
                false);
        }

        public EntityQuery FilterBy<T1>(
            T1 component1)
            where T1 : ISharedComponent
            => new EntityQuery(this,
                new ComponentData<T1>(component1),
                false);

        public EntityQuery FilterBy<T1, T2>(
            T1 component1,
            T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            => new EntityQuery(this,
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2)
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
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2),
                    new ComponentData<T3>(component3)
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
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2),
                    new ComponentData<T3>(component3),
                    new ComponentData<T4>(component4)
                },
                false);

        public EntityQuery FilterByReplace(EntityArcheType entityArcheType)
        {
            if (entityArcheType == null)
                throw new ArgumentNullException(nameof(entityArcheType));

            return new EntityQuery(this,
                entityArcheType.ArcheTypeData.SharedComponentDatas,
                true);
        }

        public EntityQuery FilterByReplace<T1>(
            T1 component1)
            where T1 : ISharedComponent
            => new EntityQuery(this,
                new ComponentData<T1>(component1),
                true);

        public EntityQuery FilterByReplace<T1, T2>(
            T1 component1,
            T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            => new EntityQuery(this,
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2)
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
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2),
                    new ComponentData<T3>(component3)
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
                new IComponentData[]
                {
                    new ComponentData<T1>(component1),
                    new ComponentData<T2>(component2),
                    new ComponentData<T3>(component3),
                    new ComponentData<T4>(component4)
                },
                true);

        #endregion FilterBys

        #region Privates

        private EntityQuery(EntityQuery query, QueryCategory category, ComponentConfig config)
        {
            CheckDistinctConfig(query.QueryData.AllComponentConfigs, config);
            CheckDistinctConfig(query.QueryData.AnyComponentConfigs, config);
            CheckDistinctConfig(query.QueryData.NoneComponentConfigs, config);

            QueryData = new EntityQueryData(
                category == QueryCategory.All
                    ? AddOrReplaceConfig(query.QueryData.AllComponentConfigs, config, false)
                    : query.QueryData.AllComponentConfigs,
                category == QueryCategory.Any
                    ? AddOrReplaceConfig(query.QueryData.AnyComponentConfigs, config, false)
                    : query.QueryData.AnyComponentConfigs,
                category == QueryCategory.None
                    ? AddOrReplaceConfig(query.QueryData.NoneComponentConfigs, config, false)
                    : query.QueryData.NoneComponentConfigs,
                query.QueryData.FilterComponentDatas);
        }

        private EntityQuery(EntityQuery query, QueryCategory category, ComponentConfig[] configs)
        {
            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(query.QueryData.AllComponentConfigs, configs);
            CheckDistinctConfigs(query.QueryData.AnyComponentConfigs, configs);
            CheckDistinctConfigs(query.QueryData.NoneComponentConfigs, configs);

            QueryData = new EntityQueryData(
                category == QueryCategory.All
                    ? AddOrReplaceConfigs(query.QueryData.AllComponentConfigs, configs, false)
                    : query.QueryData.AllComponentConfigs,
                category == QueryCategory.Any
                    ? AddOrReplaceConfigs(query.QueryData.AnyComponentConfigs, configs, false)
                    : query.QueryData.AnyComponentConfigs,
                category == QueryCategory.None
                    ? AddOrReplaceConfigs(query.QueryData.NoneComponentConfigs, configs, false)
                    : query.QueryData.NoneComponentConfigs,
                query.QueryData.FilterComponentDatas);
        }

        private EntityQuery(EntityQuery query, IComponentData filterComponent, bool replace)
        {
            CheckDistinctConfig(query.QueryData.AnyComponentConfigs, filterComponent.Config);
            CheckDistinctConfig(query.QueryData.NoneComponentConfigs, filterComponent.Config);

            QueryData = new EntityQueryData(
                AddOrReplaceConfig(query.QueryData.AllComponentConfigs, filterComponent.Config, true),
                query.QueryData.AnyComponentConfigs,
                query.QueryData.NoneComponentConfigs,
                AddOrReplaceFilterComponent(query.QueryData.FilterComponentDatas, filterComponent, replace));
        }

        private EntityQuery(EntityQuery query, IComponentData[] filterComponents, bool replace)
        {
            var configs = filterComponents.Select(x => x.Config).ToArray();

            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(query.QueryData.AnyComponentConfigs, configs);
            CheckDistinctConfigs(query.QueryData.NoneComponentConfigs, configs);

            QueryData = new EntityQueryData(
                AddOrReplaceConfigs(query.QueryData.AllComponentConfigs, configs, true),
                query.QueryData.AnyComponentConfigs,
                query.QueryData.NoneComponentConfigs,
                AddOrReplaceFilterComponents(query.QueryData.FilterComponentDatas, filterComponents, replace));
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

        private static IComponentData[] AddOrReplaceFilterComponent(IComponentData[] sourceFilterComponents, IComponentData filterComponent, bool replace)
        {
            IComponentData[] destFilterComponents;
            var hasFilterComponent = sourceFilterComponents.Any(x => x.Config == filterComponent.Config);
            if (hasFilterComponent && !replace)
            {
                throw new EntityQueryAlreadyFilteredByException(
                    ComponentConfigs.Instance.AllComponentTypes[filterComponent.Config.ComponentIndex]);
            }
            else if (hasFilterComponent)
            {
                destFilterComponents = new IComponentData[sourceFilterComponents.Length];
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
                destFilterComponents = new IComponentData[sourceFilterComponents.Length + 1];
                destFilterComponents[sourceFilterComponents.Length] = filterComponent;
                Array.Copy(sourceFilterComponents, destFilterComponents, sourceFilterComponents.Length);
                Array.Sort(destFilterComponents);
            }

            return destFilterComponents;
        }

        private static IComponentData[] AddOrReplaceFilterComponents(IComponentData[] sourceFilterComponents, IComponentData[] filterComponents, bool replace)
        {
            var destFilterComponents = new List<IComponentData>(sourceFilterComponents);
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

        #endregion Privates

        #endregion WhereOfs Filters

        #region ForEachs

        #region ForEachs

        public unsafe void ForEach(EcsContext context, bool runParallel, EntityQueryActions.R0W0 action) =>
            ForEachRun(context, runParallel, 0,
                new ComponentConfig[0],
                (options) =>
                {
                    action(options.Index, options.CurrentEntity);
                });

        #region Write 0

        public unsafe void ForEach<T1>(EcsContext context, bool runParallel, EntityQueryActions.R1W0<T1> action)
            where T1 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2>(EcsContext context, bool runParallel, EntityQueryActions.R2W0<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3>(EcsContext context, bool runParallel, EntityQueryActions.R3W0<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4>(EcsContext context, bool runParallel, EntityQueryActions.R4W0<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R5W0<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R6W0<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R7W0<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 0,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R8W0<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 0,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 0

        #region Write 1

        public unsafe void ForEach<T1>(EcsContext context, bool runParallel, EntityQueryActions.R0W1<T1> action)
            where T1 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2>(EcsContext context, bool runParallel, EntityQueryActions.R1W1<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3>(EcsContext context, bool runParallel, EntityQueryActions.R2W1<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4>(EcsContext context, bool runParallel, EntityQueryActions.R3W1<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R4W1<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R5W1<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R6W1<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 1,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R7W1<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 1,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        in ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 1

        #region Write 2

        public unsafe void ForEach<T1, T2>(EcsContext context, bool runParallel, EntityQueryActions.R0W2<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(context, runParallel, 2,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3>(EcsContext context, bool runParallel, EntityQueryActions.R1W2<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(context, runParallel, 2,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4>(EcsContext context, bool runParallel, EntityQueryActions.R2W2<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(context, runParallel, 2,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R3W2<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 2,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R4W2<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 2,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R5W2<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 2,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R6W2<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 2,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        in ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 2

        #region Write 3

        public unsafe void ForEach<T1, T2, T3>(EcsContext context, bool runParallel, EntityQueryActions.R0W3<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(context, runParallel, 3,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4>(EcsContext context, bool runParallel, EntityQueryActions.R1W3<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(context, runParallel, 3,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R2W3<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 3,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R3W3<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 3,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R4W3<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 3,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R5W3<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 3,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        in ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 3

        #region Write 4

        public unsafe void ForEach<T1, T2, T3, T4>(EcsContext context, bool runParallel, EntityQueryActions.R0W4<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(context, runParallel, 4,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R1W4<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 4,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R2W4<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 4,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R3W4<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 4,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R4W4<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 4,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        in ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 4

        #region Write 5

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EcsContext context, bool runParallel, EntityQueryActions.R0W5<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(context, runParallel, 5,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R1W5<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 5,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R2W5<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 5,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R3W5<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 5,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        in ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 5

        #region Write 6

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EcsContext context, bool runParallel, EntityQueryActions.R0W6<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(context, runParallel, 6,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R1W6<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 6,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R2W6<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 6,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        in ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 6

        #region Write 7

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EcsContext context, bool runParallel, EntityQueryActions.R0W7<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(context, runParallel, 7,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        ref ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R1W7<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 7,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        ref ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        in ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 7

        #region Write 8

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EcsContext context, bool runParallel, EntityQueryActions.R0W8<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(context, runParallel, 8,
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
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef(),
                        ref ((IComponentAdapter<T2>)options.ComponentAdapters[1]).GetComponentRef(),
                        ref ((IComponentAdapter<T3>)options.ComponentAdapters[2]).GetComponentRef(),
                        ref ((IComponentAdapter<T4>)options.ComponentAdapters[3]).GetComponentRef(),
                        ref ((IComponentAdapter<T5>)options.ComponentAdapters[4]).GetComponentRef(),
                        ref ((IComponentAdapter<T6>)options.ComponentAdapters[5]).GetComponentRef(),
                        ref ((IComponentAdapter<T7>)options.ComponentAdapters[6]).GetComponentRef(),
                        ref ((IComponentAdapter<T8>)options.ComponentAdapters[7]).GetComponentRef());
                });

        #endregion Write 8

        #endregion ForEachs

        #region Privates

        private unsafe void ForEachRun(EcsContext context, bool runParallel, int writeCount, ComponentConfig[] configs, ForEachRunAction action)
        {
            var missingConfigs = configs.Where(x => !QueryData.AllComponentConfigs.Contains(x));
            if (missingConfigs.Count() > 0)
            {
                throw new EntityQueryNotHaveWhereOfAllException(
                    ComponentConfigs.Instance.AllComponentTypes[missingConfigs.First().ComponentIndex]);
            }

            CheckDuplicateConfigs(configs);

            var entities = context.GetEntities(this);
            var contextQueryData = QueryData.ContextQueryData[context];
            if (runParallel)
            {
                var batches = new List<BatchOptions>();
                var batchCount = entities.Length / _threadCount +
                    (entities.Length % _threadCount != 0
                        ? 1
                        : 0);
                for (var i = 0; i < _threadCount; i++)
                {
                    var batchStartIndex = i * batchCount;
                    var batchEndIndex = batchStartIndex + batchCount > entities.Length
                        ? entities.Length
                        : batchStartIndex + batchCount;

                    if (batchStartIndex < batchEndIndex)
                    {
                        batches.Add(new BatchOptions
                        {
                            StartIndex = batchStartIndex,
                            EndIndex = batchEndIndex,
                            WriteCount = writeCount,
                            Context = context,
                            Entities = entities,
                            Configs = configs,
                            ArcheTypeDatas = contextQueryData.ArcheTypeDatas,
                            Action = action
                        });
                    }
                    else
                    {
                        break;
                    }
                }

                var results = Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = _threadCount },
                    batch =>
                    {
                        ForEachBatchRun(batch);
                    });
            }
            else
            {
                var batchOptions = new BatchOptions()
                {
                    StartIndex = 0,
                    EndIndex = entities.Length,
                    WriteCount = writeCount,
                    Context = context,
                    Entities = entities,
                    Configs = configs,
                    ArcheTypeDatas = contextQueryData.ArcheTypeDatas,
                    Action = action
                };

                ForEachBatchRun(batchOptions);
            }
        }

        private static unsafe void ForEachBatchRun(BatchOptions batchOptions)
        {
            var componentAdapters = EntityQueryComponentAdapters.CreateAdapters(batchOptions.Configs,
                batchOptions.Context.SharedIndexDics, batchOptions.Context.ManagePools);
            var sharedComponentAdapters = componentAdapters
                .Where(x => x.Config.IsShared)
                .ToArray();
            var forEachOptions = new ForEachOptions(batchOptions.ArcheTypeDatas, componentAdapters,
                batchOptions.WriteCount);

            for (var i = batchOptions.StartIndex; i < batchOptions.EndIndex; i++)
            {
                var entity = batchOptions.Entities[i];
                if (forEachOptions.StoreComponents(batchOptions.Context, i, batchOptions.Entities[i]))
                {
                    batchOptions.Action(forEachOptions);
                    batchOptions.Context.UpdateForEachArcheType(entity, forEachOptions.WriteComponentAdapters);
                }
            }
        }

        private unsafe delegate void ForEachRunAction(ForEachOptions options);

        private class BatchOptions
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public int WriteCount { get; set; }
            public EcsContext Context { get; set; }
            public Entity[] Entities { get; set; }
            public ComponentConfig[] Configs { get; set; }
            public ArcheTypeData[] ArcheTypeDatas { get; set; }
            public ForEachRunAction Action { get; set; }
        }

        private class ForEachOptions
        {
            public int Index { get; private set; }
            public Entity CurrentEntity { get; private set; }
            public HashSet<ArcheTypeIndex> ArcheTypeDataHash { get; private set; }
            public ArcheTypeData PrevArcheTypeData { get; private set; }
            public IComponentAdapter[] ComponentAdapters { get; private set; }
            public IComponentAdapter[] WriteComponentAdapters { get; private set; }

            public ForEachOptions(ArcheTypeData[] archeTypeDatas, IComponentAdapter[] componentAdapters, int writeCount)
            {
                ArcheTypeDataHash = new HashSet<ArcheTypeIndex>(archeTypeDatas.Select(x => x.ArcheTypeIndex));
                ComponentAdapters = componentAdapters;
                WriteComponentAdapters = componentAdapters.Take(writeCount).ToArray();
            }

            public unsafe bool StoreComponents(EcsContext context, int index, Entity entity)
            {
                lock (context.LockObj)
                {
                    if (!context.HasEntity(entity))
                        return false;

                    Index = index;
                    CurrentEntity = entity;

                    var entityData = context.EntityDatas[CurrentEntity.Id];
                    var archeTypeData = context.ArcheTypeManager.GetArcheTypeData(entityData.ArcheTypeIndex);

                    if (PrevArcheTypeData != archeTypeData)
                    {
                        PrevArcheTypeData = archeTypeData;
                        for (var i = 0; i < ComponentAdapters.Length; i++)
                        {
                            var configOffset = archeTypeData.GetComponentConfigOffset(ComponentAdapters[i].Config);
                            ComponentAdapters[i].SetComponentConfigOffset(configOffset);
                            ComponentAdapters[i].StoreComponent(entityData, archeTypeData);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < ComponentAdapters.Length; i++)
                            ComponentAdapters[i].StoreComponent(entityData, archeTypeData);
                    }
                }

                return true;
            }
        }

        #endregion Privates

        #endregion ForEachs
    }
}