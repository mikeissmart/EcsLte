using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityQuery : IEntityQuery
    {
        private bool _addedToMaster;

        public ComponentConfig[] AllConfigs => Data.AllConfigs;
        public ComponentConfig[] AnyConfigs => Data.AnyConfigs;
        public ComponentConfig[] NoneConfigs => Data.NoneConfigs;
        internal EntityQueryData Data { get; set; }

        internal EntityQuery(IComponentEntityFactory componentEntityFactory) => Data = new EntityQueryData(componentEntityFactory);

        private EntityQuery(EntityQuery query, EntityQueryCategory category, ComponentConfig config) : this(query.Data.ComponentEntityFactory)
        {
            if (config.IsUnique)
                throw new EntityQueryUniqueComponentException();
            CheckDistinctConfig(query.Data.AllConfigs, config);
            CheckDistinctConfig(query.Data.AnyConfigs, config);
            CheckDistinctConfig(query.Data.NoneConfigs, config);

            switch (category)
            {
                case EntityQueryCategory.All:
                    Data.AllConfigs = AddConfig(query.Data.AllConfigs, config);
                    Data.AnyConfigs = query.Data.AnyConfigs;
                    Data.NoneConfigs = query.Data.NoneConfigs;
                    break;
                case EntityQueryCategory.Any:
                    Data.AllConfigs = query.Data.AllConfigs;
                    Data.AnyConfigs = AddConfig(query.Data.AnyConfigs, config);
                    Data.NoneConfigs = query.Data.NoneConfigs;
                    break;
                case EntityQueryCategory.None:
                    Data.AllConfigs = query.Data.AllConfigs;
                    Data.AnyConfigs = query.Data.AnyConfigs;
                    Data.NoneConfigs = AddConfig(query.Data.NoneConfigs, config);
                    break;
            }

            Data.SharedComponents = query.Data.SharedComponents;
        }

        private EntityQuery(EntityQuery query, EntityQueryCategory category, ComponentConfig[] configs) : this(query.Data.ComponentEntityFactory)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                if (configs[i].IsUnique)
                    throw new EntityQueryUniqueComponentException();
            }
            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(query.Data.AllConfigs, configs);
            CheckDistinctConfigs(query.Data.AnyConfigs, configs);
            CheckDistinctConfigs(query.Data.NoneConfigs, configs);

            switch (category)
            {
                case EntityQueryCategory.All:
                    Data.AllConfigs = AddConfigs(query.Data.AllConfigs, configs);
                    Data.AnyConfigs = query.Data.AnyConfigs;
                    Data.NoneConfigs = query.Data.NoneConfigs;
                    break;
                case EntityQueryCategory.Any:
                    Data.AllConfigs = query.Data.AllConfigs;
                    Data.AnyConfigs = AddConfigs(query.Data.AnyConfigs, configs);
                    Data.NoneConfigs = query.Data.NoneConfigs;
                    break;
                case EntityQueryCategory.None:
                    Data.AllConfigs = query.Data.AllConfigs;
                    Data.AnyConfigs = query.Data.AnyConfigs;
                    Data.NoneConfigs = AddConfigs(query.Data.NoneConfigs, configs);
                    break;
            }

            Data.SharedComponents = query.Data.SharedComponents;
        }

        private EntityQuery(EntityQuery query, ComponentConfig config, IEntityQuery_SharedComponentData sharedComponent) : this(query.Data.ComponentEntityFactory)
        {
            CheckDistinctConfig(query.Data.AnyConfigs, config);
            CheckDistinctConfig(query.Data.NoneConfigs, config);
            CheckDistinctShareComponent(query.Data.SharedComponents, sharedComponent);

            Data.AllConfigs = AddConfigIgnoreDuplicate(query.Data.AllConfigs, config);
            Data.AnyConfigs = query.Data.AnyConfigs;
            Data.NoneConfigs = query.Data.NoneConfigs;

            Data.SharedComponents = AddShareComponent(query.Data.SharedComponents, sharedComponent);
        }

        private EntityQuery(EntityQuery query, ComponentConfig[] configs, IEntityQuery_SharedComponentData[] sharedComponents) : this(query.Data.ComponentEntityFactory)
        {
            CheckDuplicateConfigs(configs);
            CheckDistinctConfigs(query.Data.AnyConfigs, configs);
            CheckDistinctConfigs(query.Data.NoneConfigs, configs);
            CheckDistinctShareComponents(query.Data.SharedComponents, sharedComponents);


            Data.AllConfigs = AddConfigsIgnoreDuplicates(query.Data.AllConfigs, configs);
            Data.AnyConfigs = query.Data.AnyConfigs;
            Data.NoneConfigs = query.Data.NoneConfigs;

            Data.SharedComponents = AddShareComponents(query.Data.SharedComponents, sharedComponents);
        }

        public bool HasEntity(Entity entity)
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            return Data.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            return Data.Entities;
        }

        public IEntityQuery WhereAllOf<T1>()
            where T1 : IComponent => new EntityQuery(this, EntityQueryCategory.All, ComponentConfig<T1>.Config);

        public IEntityQuery WhereAllOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent => new EntityQuery(this, EntityQueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public IEntityQuery WhereAllOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => new EntityQuery(this, EntityQueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public IEntityQuery WhereAllOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => new EntityQuery(this, EntityQueryCategory.All,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public IEntityQuery WhereAnyOf<T1>()
            where T1 : IComponent => new EntityQuery(this, EntityQueryCategory.Any, ComponentConfig<T1>.Config);

        public IEntityQuery WhereAnyOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent => new EntityQuery(this, EntityQueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public IEntityQuery WhereAnyOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => new EntityQuery(this, EntityQueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public IEntityQuery WhereAnyOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => new EntityQuery(this, EntityQueryCategory.Any,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public IEntityQuery WhereNoneOf<T1>()
            where T1 : IComponent => new EntityQuery(this, EntityQueryCategory.None, ComponentConfig<T1>.Config);

        public IEntityQuery WhereNoneOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent => new EntityQuery(this, EntityQueryCategory.None,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public IEntityQuery WhereNoneOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => new EntityQuery(this, EntityQueryCategory.None, new[]
            {
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config
            });

        public IEntityQuery WhereNoneOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => new EntityQuery(this, EntityQueryCategory.None,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public IEntityQuery FilterBy<T1>(T1 component)
            where T1 : ISharedComponent => new EntityQuery(this,
                ComponentConfig<T1>.Config,
                new EntityQuery_SharedComponentData<T1>(ComponentConfig<T1>.Config, component));

        public IEntityQuery FilterBy<T1, T2>(T1 component1, T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent => new EntityQuery(this,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                new IEntityQuery_SharedComponentData[]
                {
                    new EntityQuery_SharedComponentData<T1>(ComponentConfig<T1>.Config, component1),
                    new EntityQuery_SharedComponentData<T2>(ComponentConfig<T2>.Config, component2)
                });

        public IEntityQuery FilterBy<T1, T2, T3>(T1 component1, T2 component2, T3 component3)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent => new EntityQuery(this,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                new IEntityQuery_SharedComponentData[]
                {
                    new EntityQuery_SharedComponentData<T1>(ComponentConfig<T1>.Config, component1),
                    new EntityQuery_SharedComponentData<T2>(ComponentConfig<T2>.Config, component2),
                    new EntityQuery_SharedComponentData<T3>(ComponentConfig<T3>.Config, component3)
                });

        public IEntityQuery FilterBy<T1, T2, T3, T4>(T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            where T4 : ISharedComponent => new EntityQuery(this,
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                new IEntityQuery_SharedComponentData[]
                {
                    new EntityQuery_SharedComponentData<T1>(ComponentConfig<T1>.Config, component1),
                    new EntityQuery_SharedComponentData<T2>(ComponentConfig<T2>.Config, component2),
                    new EntityQuery_SharedComponentData<T3>(ComponentConfig<T3>.Config, component3),
                    new EntityQuery_SharedComponentData<T4>(ComponentConfig<T4>.Config, component4)
                });
        public void ForEach(EntityQueryActions.EntityQueryAction action)
        {
            var entities = GetEntities();
            for (var i = 0; i < entities.Length; i++)
                action(entities[i]);
        }

        public void ForEach<T1>(EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2>(EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3>(EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3, T4>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        public void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            if (!_addedToMaster)
            {
                Data.ComponentEntityFactory.EntityQueryAddToMaster(this);
                _addedToMaster = true;
            }
            Data.ComponentEntityFactory.ForEach(Data, action);
        }

        private static void CheckDuplicateConfigs(ComponentConfig[] configs)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                for (var j = i + 1; j < configs.Length; j++)
                {
                    if (configs[i] == configs[j])
                    {
                        throw new EntityQueryAlreadyHasWhereException(
                            ComponentConfigs.Instance.AllComponentTypes[configs[i].ComponentIndex]);
                    }
                }
            }
        }

        private static ComponentConfig[] AddConfig(ComponentConfig[] sourceConfigs, ComponentConfig config)
        {
            var destConfigs = new ComponentConfig[sourceConfigs.Length + 1];
            destConfigs[sourceConfigs.Length] = config;
            Array.Copy(sourceConfigs, destConfigs, sourceConfigs.Length);
            Array.Sort(destConfigs);

            return destConfigs;
        }

        private static ComponentConfig[] AddConfigIgnoreDuplicate(ComponentConfig[] sourceConfigs, ComponentConfig config)
        {
            ComponentConfig[] destConfigs = null;
            if (!sourceConfigs.Any(x => x == config))
            {
                destConfigs = new ComponentConfig[sourceConfigs.Length + 1];
                destConfigs[sourceConfigs.Length] = config;
                Array.Copy(sourceConfigs, destConfigs, sourceConfigs.Length);
                Array.Sort(destConfigs);
            }
            else
            {
                destConfigs = sourceConfigs;
            }

            return destConfigs;
        }

        private static ComponentConfig[] AddConfigs(ComponentConfig[] sourceConfigs, ComponentConfig[] configs)
        {
            var destConfigs = new ComponentConfig[sourceConfigs.Length + configs.Length];
            Array.Copy(sourceConfigs, destConfigs, sourceConfigs.Length);
            Array.Copy(configs, 0, destConfigs, sourceConfigs.Length, configs.Length);
            Array.Sort(destConfigs);

            return destConfigs;
        }

        private static ComponentConfig[] AddConfigsIgnoreDuplicates(ComponentConfig[] sourceConfigs, ComponentConfig[] configs)
        {
            ComponentConfig[] destConfigs;
            if (!sourceConfigs.Any(x => configs.Contains(x)))
            {
                destConfigs = new ComponentConfig[sourceConfigs.Length + configs.Length];
                Array.Copy(sourceConfigs, destConfigs, sourceConfigs.Length);
                Array.Copy(configs, 0, destConfigs, sourceConfigs.Length, configs.Length);
            }
            else
            {
                var distinctConfigs = new HashSet<ComponentConfig>();
                foreach (var config in sourceConfigs)
                    distinctConfigs.Add(config);
                foreach (var config in configs)
                    distinctConfigs.Add(config);
                destConfigs = distinctConfigs.ToArray();
            }
            Array.Sort(destConfigs);

            return destConfigs;
        }

        private static IEntityQuery_SharedComponentData[] AddShareComponent(IEntityQuery_SharedComponentData[] sourceSharedComponents, IEntityQuery_SharedComponentData sharedComponent)
        {
            var destSharedComponents = new IEntityQuery_SharedComponentData[sourceSharedComponents.Length + 1];
            destSharedComponents[sourceSharedComponents.Length] = sharedComponent;
            Array.Copy(sourceSharedComponents, destSharedComponents, sourceSharedComponents.Length);
            Array.Sort(destSharedComponents);

            return destSharedComponents;
        }

        private static IEntityQuery_SharedComponentData[] AddShareComponents(IEntityQuery_SharedComponentData[] sourceSharedComponents, IEntityQuery_SharedComponentData[] sharedComponent)
        {
            var destSharedComponents = new IEntityQuery_SharedComponentData[sourceSharedComponents.Length + sharedComponent.Length];
            Array.Copy(sourceSharedComponents, destSharedComponents, sourceSharedComponents.Length);
            Array.Copy(sharedComponent, 0, destSharedComponents, sourceSharedComponents.Length, sharedComponent.Length);
            Array.Sort(destSharedComponents);

            return destSharedComponents;
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

        private static void CheckDistinctShareComponent(IEntityQuery_SharedComponentData[] sourceSharedComponents, IEntityQuery_SharedComponentData sharedComponent)
        {
            if (sourceSharedComponents.Contains(sharedComponent))
            {
                throw new EntityQueryAlreadyHasFilterException(
                    ComponentConfigs.Instance.AllComponentTypes[sharedComponent.Config.ComponentIndex]);
            }
        }

        private static void CheckDistinctShareComponents(IEntityQuery_SharedComponentData[] sourceSharedComponents, IEntityQuery_SharedComponentData[] sharedComponents)
        {
            if (sourceSharedComponents.Any(x => sharedComponents.Contains(x)))
            {
                throw new EntityQueryAlreadyHasFilterException(
                    ComponentConfigs.Instance.AllComponentTypes[sourceSharedComponents.First(x => sharedComponents.Contains(x)).Config.ComponentIndex]);
            }
        }
    }
}
