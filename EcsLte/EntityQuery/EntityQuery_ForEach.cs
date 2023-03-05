using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcsLte
{
    public partial class EntityQuery
    {
        #region Other

        public EntityQuery ForEach(int count, EntityQueryActions.Other action)
            => ForEachOtherStore(count,
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index);
                });

        #endregion

        #region Write 0

        public EntityQuery ForEach(EntityQueryActions.R0W0 action)
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity);
                },
                (options) =>
                {

                },
                new ComponentConfig[0],
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1>(EntityQueryActions.R1W0<T1> action)
            where T1 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R2W0<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R3W0<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R4W0<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R5W0<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R6W0<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R7W0<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
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
                new ComponentConfig[0]);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R8W0<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));
                },
                (options) =>
                {

                },
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
                new ComponentConfig[0]);

        #endregion

        #region Write 1

        public EntityQuery ForEach<T1>(EntityQueryActions.R0W1<T1> action)
            where T1 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R1W1<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R2W1<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R3W1<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R4W1<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R5W1<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R6W1<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R7W1<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1));
                },
                new[]
                {
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config
                });

        #endregion

        #region Write 2

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R0W2<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R1W2<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R2W2<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R3W2<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R4W2<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R5W2<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R6W2<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2));
                },
                new[]
                {
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config
                });

        #endregion

        #region Write 3

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R0W3<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R1W3<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new[]
                {
                    ComponentConfig<T4>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R2W3<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new[]
                {
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R3W3<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new[]
                {
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R4W3<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new[]
                {
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R5W3<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3));
                },
                new[]
                {
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config
                });

        #endregion

        #region Write 4

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R0W4<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R1W4<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4));
                },
                new[]
                {
                    ComponentConfig<T5>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R2W4<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4));
                },
                new[]
                {
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R3W4<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4));
                },
                new[]
                {
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R4W4<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4));
                },
                new[]
                {
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config
                });

        #endregion

        #region Write 5

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R0W5<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R1W5<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
                },
                new[]
                {
                    ComponentConfig<T6>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R2W5<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
                },
                new[]
                {
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R3W5<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
                },
                new[]
                {
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config
                });

        #endregion

        #region Write 6

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R0W6<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R1W6<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6));
                },
                new[]
                {
                    ComponentConfig<T7>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R2W6<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6));
                },
                new[]
                {
                    ComponentConfig<T7>.Config,
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config
                });

        #endregion

        #region Write 7

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R0W7<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5])
                            .GetCache<T7>(options.Adapters[6], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7));
                },
                new ComponentConfig[0],
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                });

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R1W7<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5])
                            .GetCache<T7>(options.Adapters[6], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7));
                },
                new[]
                {
                    ComponentConfig<T8>.Config
                },
                new[]
                {
                    ComponentConfig<T1>.Config,
                    ComponentConfig<T2>.Config,
                    ComponentConfig<T3>.Config,
                    ComponentConfig<T4>.Config,
                    ComponentConfig<T5>.Config,
                    ComponentConfig<T6>.Config,
                    ComponentConfig<T7>.Config
                });

        #endregion

        #region Write 8

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R0W8<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData),
                        ref ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData));

                    if (options.BatchOptions.UseEntityQueryCache)
                    {
                        options.Cache
                            .GetCache<T1>(options.Adapters[0])
                            .GetCache<T2>(options.Adapters[1])
                            .GetCache<T3>(options.Adapters[2])
                            .GetCache<T4>(options.Adapters[3])
                            .GetCache<T5>(options.Adapters[4])
                            .GetCache<T6>(options.Adapters[5])
                            .GetCache<T7>(options.Adapters[6])
                            .GetCache<T8>(options.Adapters[7], options.BatchOptions.CacheRoot)
                            .Entities.Add(options.CurrentEntity);
                    }
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityData);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityData);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityData);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityData);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityData);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityData);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityData);
                    var component8 = ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityData);

                    action(options.BatchOptions.ThreadIndex, options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        ref component8);

                    options.BatchOptions.EntityCommandsCache.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(options.CurrentEntity,
                        options.EntityData.EntityIndex,
                        options.ArcheTypeIndex,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7,
                        component8));
                },
                new ComponentConfig[0],
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
                });

        #endregion

        private EntityQuery ForEachOtherStore(int count, ForEachRunAction action)
        {
            _data = new Data(_data)
            {
                ReadConfigs = new ComponentConfig[0],
                WriteConfigs = new ComponentConfig[0],
                Action = null,
                CommandAction = null,
                OtherAction = action,
                OtherCount = count
            };

            return this;
        }

        private EntityQuery ForEachStore(ForEachRunAction action, ForEachRunAction commandAction, ComponentConfig[] readConfigs, ComponentConfig[] writeConfigs)
        {
            _data = new Data(_data)
            {
                ReadConfigs = readConfigs,
                WriteConfigs = writeConfigs,
                Action = action,
                CommandAction = commandAction,
                OtherAction = null,
                OtherCount = 0
            };

            return this;
        }

        private void InternalRun(bool isParallel)
        {
            _data.Context.AssertContext();
            _data.Context.AssertStructualChangeAvailable();

            _data.Context.StructChangeAvailable = false;

            if (_data.Action != null)
                EntityForEach(_data, isParallel);
            else if (_data.OtherAction != null)
                OtherForEach(_data, isParallel);
            else
            {
                _data.Context.StructChangeAvailable = true;
                throw new EntityQueryHasNoForEachException();
            }

            _data.Context.StructChangeAvailable = true;
        }

        private void EntityForEach(Data runningData, bool isParallel)
        {
            // Switch to temp filter that whereAllOf action configs
            var dataFilter = runningData.Filter;
            var useEntityQueryCache = false;
            var useQueryCommandsCount = 0;

            Helper.AssertDuplicateConfigs(runningData.ReadConfigs);

            try
            {
                if (dataFilter != null)
                {
                    runningData.Filter = new EntityFilter(dataFilter);
                    for (var i = 0; i < runningData.ReadConfigs.Length; i++)
                    {
                        var config = runningData.ReadConfigs[i];
                        if (runningData.Filter.HasWhereNoneOf(config))
                        {
                            // Cant have none of read/write component
                            throw new Exception();
                        }
                        else
                        {
                            // Component is now required
                            // Remove any and set all
                            runningData.Filter.RemoveAnyOf(config);
                            if (!runningData.Filter.HasWhereAllOf(config))
                                runningData.Filter.WhereAllOf(config);
                        }
                    }
                    for (var i = 0; i < runningData.WriteConfigs.Length; i++)
                    {
                        var config = runningData.WriteConfigs[i];
                        useEntityQueryCache |= config.IsShared;
                        if (runningData.Filter.HasWhereNoneOf(config))
                        {
                            // Cant have none of read/write component
                            throw new Exception();
                        }
                        else
                        {
                            // Component is now required
                            // Remove any and set all
                            runningData.Filter.RemoveAnyOf(config);
                            if (!runningData.Filter.HasWhereAllOf(config))
                                runningData.Filter.WhereAllOf(config);
                        }
                    }
                }
                else
                {
                    runningData.Filter = new EntityFilter(runningData.Context);
                    for (var i = 0; i < runningData.ReadConfigs.Length; i++)
                        runningData.Filter.WhereAllOf(runningData.ReadConfigs[i]);
                    for (var i = 0; i < runningData.WriteConfigs.Length; i++)
                    {
                        useEntityQueryCache |= runningData.WriteConfigs[i].IsShared;
                        runningData.Filter.WhereAllOf(runningData.WriteConfigs[i]);
                    }
                }

                useEntityQueryCache = useEntityQueryCache && runningData.Commands == null;

                // TODO reformat for chunks instead of entities
                var entitiesCount = Context.Entities.GetEntities(this, ref runningData.Entities);

                if (isParallel)
                {
                    var batches = new List<BatchOptions>();
                    var batchCount = entitiesCount / _threadCount +
                        (entitiesCount % _threadCount != 0 ? 1 : 0);

                    for (var i = 0; i < _threadCount; i++)
                    {
                        var batchStartIndex = i * batchCount;
                        var batchEndIndex = batchStartIndex + batchCount > entitiesCount
                            ? entitiesCount : batchStartIndex + batchCount;

                        if (batchStartIndex < batchEndIndex)
                        {
                            batches.Add(new BatchOptions
                            {
                                StartIndex = batchStartIndex,
                                EndIndex = batchEndIndex,
                                ThreadIndex = i,
                                Context = runningData.Context,
                                EntityManager = runningData.Context.Entities,
                                Entities = runningData.Entities,
                                Commands = runningData.Commands,
                                ReadConfigs = runningData.ReadConfigs,
                                WriteConfigs = runningData.WriteConfigs,
                                Action = runningData.Action,
                                CommandAction = runningData.CommandAction,
                                CacheRoot = runningData.EntityQueryCacheRoots[i],
                                UseEntityQueryCache = useEntityQueryCache
                            });
                        }
                        else
                            break;
                    }

                    useQueryCommandsCount = useEntityQueryCache
                        ? batches.Count
                        : 0;
                    var result = Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = _threadCount },
                        batch =>
                        {
                            new ForEachOptions(batch)
                                .InvokeForEachAction();
                        });
                }
                else
                {
                    useQueryCommandsCount = useEntityQueryCache
                        ? 1
                        : 0;
                    new ForEachOptions(new BatchOptions
                    {
                        StartIndex = 0,
                        EndIndex = entitiesCount,
                        ThreadIndex = 1,
                        Context = runningData.Context,
                        EntityManager = runningData.Context.Entities,
                        Entities = runningData.Entities,
                        Commands = runningData.Commands,
                        ReadConfigs = runningData.ReadConfigs,
                        WriteConfigs = runningData.WriteConfigs,
                        Action = runningData.Action,
                        CommandAction = runningData.CommandAction,
                        EntityCommandsCache = runningData.EntityCommandsCaches[0],
                        CacheRoot = runningData.EntityQueryCacheRoots[0],
                        UseEntityQueryCache = useEntityQueryCache,
                    }).InvokeForEachAction();
                }

                if (useEntityQueryCache)
                {
                    for (var i = 0; i < useQueryCommandsCount; i++)
                    {
                        var commands = runningData.EntityQueryCacheRoots[i];
                        foreach (var cache in commands.SharedCaches)
                        {
                            cache.Entities.CopyTo(runningData.Entities);
                            runningData.Context.Entities.EntityQuery_TransferNextArtcheType(runningData.Entities,
                                cache.Entities.Count, cache.ArcheTypeIndex, cache.Components);

                            cache.Entities.Clear();
                            commands.EntitiesCache.Enqueue(cache.Entities);
                        }
                        commands.Clear();
                    }
                    useQueryCommandsCount = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                runningData.Filter = dataFilter;
                for (var i = 0; i < useQueryCommandsCount; i++)
                {
                    var commands = runningData.EntityQueryCacheRoots[i];
                    foreach (var cache in commands.SharedCaches)
                    {
                        cache.Entities.Clear();
                        commands.EntitiesCache.Enqueue(cache.Entities);
                    }
                    commands.Clear();
                }
            }
        }

        private void OtherForEach(Data runningData, bool isParallel)
        {
            if (runningData.OtherCount <= 0)
                throw new ArgumentOutOfRangeException("ForEachOther.Count", "Must be greater than 0.");

            try
            {
                if (isParallel)
                {
                    var batches = new List<BatchOptions>();
                    var batchCount = runningData.OtherCount / _threadCount +
                        (runningData.OtherCount % _threadCount != 0 ? 1 : 0);

                    for (var i = 0; i < _threadCount; i++)
                    {
                        var batchStartIndex = i * batchCount;
                        var batchEndIndex = batchStartIndex + batchCount > runningData.OtherCount
                            ? runningData.OtherCount : batchStartIndex + batchCount;

                        if (batchStartIndex < batchEndIndex)
                        {
                            batches.Add(new BatchOptions
                            {
                                StartIndex = batchStartIndex,
                                EndIndex = batchEndIndex,
                                OtherAction = runningData.OtherAction
                            });
                        }
                        else
                            break;
                    }

                    var result = Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = _threadCount },
                        batch =>
                        {
                            new ForEachOptions(batch)
                                .InvokeForEachOtherAction();
                        });
                }
                else
                {
                    new ForEachOptions(new BatchOptions
                    {
                        StartIndex = 0,
                        EndIndex = runningData.OtherCount,
                        OtherAction = runningData.OtherAction
                    }).InvokeForEachOtherAction();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static IEntityQueryAdapter[] CreateAdapters(bool useCommands, ComponentConfig[] writeConfigs, ComponentConfig[] readConfigs)
        {
            var adapters = new IEntityQueryAdapter[writeConfigs.Length + readConfigs.Length];
            for (var i = 0; i < writeConfigs.Length; i++)
            {
                var config = writeConfigs[i];
                if (config.IsGeneral)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQueryGeneralAdapter<>).MakeGenericType(config.ComponentType));
                }
                else if (config.IsManaged)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQueryManagedAdapter<>).MakeGenericType(config.ComponentType));
                }
                else if (config.IsShared)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQuerySharedAdapter<>).MakeGenericType(config.ComponentType));
                }
            }
            for (int i = writeConfigs.Length, configIndex = 0;
                configIndex < readConfigs.Length;
                i++, configIndex++)
            {
                var config = readConfigs[configIndex];
                if (config.IsGeneral)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQueryGeneralAdapter<>).MakeGenericType(config.ComponentType));
                }
                else if (config.IsManaged)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQueryManagedAdapter<>).MakeGenericType(config.ComponentType));
                }
                else if (config.IsShared)
                {
                    adapters[i] = (IEntityQueryAdapter)Activator.CreateInstance(
                        typeof(EntityQuerySharedAdapter<>).MakeGenericType(config.ComponentType));
                }
            }

            return adapters;
        }

        private class BatchOptions
        {
            internal int StartIndex { get; set; }
            internal int EndIndex { get; set; }
            internal int ThreadIndex { get; set; }
            internal ChangeVersion ForEachChangeVersion { get; set; }
            internal EcsContext Context { get; set; }
            internal EntityManager EntityManager { get; set; }
            internal Entity[] Entities { get; set; }
            internal EntityCommands Commands { get; set; }
            internal ComponentConfig[] ReadConfigs { get; set; }
            internal ComponentConfig[] WriteConfigs { get; set; }
            internal ForEachRunAction Action { get; set; }
            internal ForEachRunAction CommandAction { get; set; }
            internal ForEachRunAction OtherAction { get; set; }
            internal List<IEntityCommand> EntityCommandsCache { get; set; }
            internal bool UseEntityQueryCache { get; set; }
            internal EntityQueryArcheTypeCacheRoot CacheRoot { get; set; }
        }

        private class ForEachOptions
        {
            private readonly EcsContext _context;
            private ArcheTypeData _prevArcheTypeData = null;
            private ArcheTypeDataChunk _prevChunk;
            private ComponentConfigOffset[] _configOffsets;

            internal int Index { get; set; }
            internal Entity CurrentEntity { get; set; }
            internal EntityData EntityData { get; set; }
            internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
            internal IEntityQueryAdapter[] Adapters { get; set; }
            internal BatchOptions BatchOptions { get; private set; }
            internal EntityQuerySharedCache Cache { get; private set; }

            internal ForEachOptions(BatchOptions batchOptions)
            {
                _context = batchOptions.Context;
                _configOffsets = new ComponentConfigOffset[batchOptions.WriteConfigs.Length];

                Adapters = CreateAdapters(batchOptions.Commands != null,
                    batchOptions.WriteConfigs, batchOptions.ReadConfigs);
                BatchOptions = batchOptions;
            }

            internal void InvokeForEachAction()
            {
                if (BatchOptions.Commands == null)
                {
                    for (Index = BatchOptions.StartIndex; Index < BatchOptions.EndIndex; Index++)
                    {
                        var entity = BatchOptions.Entities[Index];
                        UpdateEntityData(entity, BatchOptions.EntityManager.GetEntityData(entity));

                        BatchOptions.Action.Invoke(this);
                    }

                    // Need to update last ArcheTypeDataChunk aswell
                    for (var i = 0; i < _configOffsets.Length; i++)
                        _prevChunk.UpdateComponentVersion(BatchOptions.ForEachChangeVersion, _configOffsets[i]);
                }
                else
                {
                    for (Index = BatchOptions.StartIndex; Index < BatchOptions.EndIndex; Index++)
                    {
                        var entity = BatchOptions.Entities[Index];
                        UpdateEntityData(entity, BatchOptions.EntityManager.GetEntityData(entity));

                        BatchOptions.CommandAction.Invoke(this);
                    }

                    BatchOptions.Commands.AppendQueryCommands(BatchOptions.EntityCommandsCache);
                    BatchOptions.EntityCommandsCache.Clear();
                }
            }

            internal void InvokeForEachOtherAction()
            {
                for (Index = BatchOptions.StartIndex; Index < BatchOptions.EndIndex; Index++)
                    BatchOptions.OtherAction.Invoke(this);
            }

            private void UpdateEntityData(in Entity entity, in EntityData entityData)
            {
                if (ArcheTypeIndex != entityData.ArcheTypeIndex)
                {
                    if (BatchOptions.Commands == null && _prevArcheTypeData != null)
                    {
                        for (var i = 0; i < _configOffsets.Length; i++)
                            _prevChunk.UpdateComponentVersion(BatchOptions.ForEachChangeVersion, _configOffsets[i]);
                    }

                    var archeTypeData = _context.ArcheTypes
                        .GetArcheTypeData(entityData.ArcheTypeIndex);
                    for (var i = 0; i < Adapters.Length; i++)
                    {
                        var adapter = Adapters[i];
                        adapter.ChangeArcheTypeData(archeTypeData);
                    }

                    _prevArcheTypeData = archeTypeData;
                    _prevChunk = archeTypeData.Chunks[entityData.ChunkIndex];
                    ArcheTypeIndex = entityData.ArcheTypeIndex;
                    if (BatchOptions.UseEntityQueryCache)
                        Cache = BatchOptions.CacheRoot.GetCache(ArcheTypeIndex);

                    for (var i = 0; i < _configOffsets.Length; i++)
                        _configOffsets[i] = archeTypeData.GetConfigOffset(BatchOptions.WriteConfigs[i]);
                }
                else if (_prevChunk.ChunkIndex != entityData.ChunkIndex)
                {
                    for (var i = 0; i < _configOffsets.Length; i++)
                        _prevChunk.UpdateComponentVersion(BatchOptions.ForEachChangeVersion, _configOffsets[i]);

                    _prevChunk = _prevArcheTypeData.Chunks[entityData.ChunkIndex];
                }

                CurrentEntity = entity;
                EntityData = entityData;
            }
        }
    }
}
