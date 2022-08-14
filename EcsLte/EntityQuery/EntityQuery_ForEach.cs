using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte
{
    public partial class EntityQuery
    {
        public EntityQuery ForEach(EntityQueryActions.R0W0 action)
            => ForEachStore(
                (options) =>
                {
                    action(options.Index, options.CurrentEntity);
                },
                (options) =>
                {

                },
                new ComponentConfig[0],
                new ComponentConfig[0]);

        #region Write 0

        public EntityQuery ForEach<T1>(EntityQueryActions.R1W0<T1> action)
            where T1 : IComponent
            => ForEachStore(
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        in ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        in ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        in ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        in ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        in ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        in ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        in ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        in ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(options.CurrentEntity,
                        options.EntityIndex,
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
                    action(options.Index, options.CurrentEntity,
                        ref ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex),
                        ref ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex));
                },
                (options) =>
                {
                    var component1 = ((IEntityQueryAdapter<T1>)options.Adapters[0]).GetRef(options.EntityIndex);
                    var component2 = ((IEntityQueryAdapter<T2>)options.Adapters[1]).GetRef(options.EntityIndex);
                    var component3 = ((IEntityQueryAdapter<T3>)options.Adapters[2]).GetRef(options.EntityIndex);
                    var component4 = ((IEntityQueryAdapter<T4>)options.Adapters[3]).GetRef(options.EntityIndex);
                    var component5 = ((IEntityQueryAdapter<T5>)options.Adapters[4]).GetRef(options.EntityIndex);
                    var component6 = ((IEntityQueryAdapter<T6>)options.Adapters[5]).GetRef(options.EntityIndex);
                    var component7 = ((IEntityQueryAdapter<T7>)options.Adapters[6]).GetRef(options.EntityIndex);
                    var component8 = ((IEntityQueryAdapter<T8>)options.Adapters[7]).GetRef(options.EntityIndex);

                    action(options.Index, options.CurrentEntity,
                        ref component1,
                        ref component2,
                        ref component3,
                        ref component4,
                        ref component5,
                        ref component6,
                        ref component7,
                        ref component8);

                    options.BatchOptions.CachedCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(options.CurrentEntity,
                        options.EntityIndex,
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

        private EntityQuery ForEachStore(ForEachRunAction action, ForEachRunAction commandAction, ComponentConfig[] readConfigs, ComponentConfig[] writeConfigs)
        {
            _data = new Data(_data)
            {
                ReadConfigs = readConfigs,
                WriteConfigs = writeConfigs,
                Action = action,
                CommandAction = commandAction,
            };

            return this;
        }

        private void InternalRun(bool isParallel)
        {
            var runningData = _data;
            // Switch to temp filter that whereAllOf action configs
            var dataFilter = runningData.Filter;
            var useQueryCommands = false;
            var useQueryCommandsCount = 0;

            runningData.Context.AssertContext();
            runningData.Context.AssertStructualChangeAvailable();
            if (runningData.Action == null)
                throw new EntityQueryHasNoForEachException();
            Helper.AssertDuplicateConfigs(runningData.ReadConfigs);

            runningData.Context.StructChangeAvailable = false;

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
                            runningData.Filter.RemoveAnyOf(config);
                            if (!runningData.Filter.HasWhereAllOf(config))
                                runningData.Filter.WhereAllOf(config);
                        }
                    }
                    for (var i = 0; i < runningData.WriteConfigs.Length; i++)
                    {
                        var config = runningData.WriteConfigs[i];
                        useQueryCommands |= config.IsShared;
                        if (runningData.Filter.HasWhereNoneOf(config))
                        {
                            // Cant have none of read/write component
                            throw new Exception();
                        }
                        else
                        {
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
                        useQueryCommands |= runningData.WriteConfigs[i].IsShared;
                        runningData.Filter.WhereAllOf(runningData.WriteConfigs[i]);
                    }
                }

                useQueryCommands = useQueryCommands && runningData.Commands == null;

                Context.Entities.GetEntities(this, ref runningData.Entities);

                if (isParallel)
                {
                    var batches = new List<BatchOptions>();
                    var batchCount = runningData.Entities.Length / _threadCount +
                        (runningData.Entities.Length % _threadCount != 0 ? 1 : 0);

                    for (var i = 0; i < _threadCount; i++)
                    {
                        var batchStartIndex = i * batchCount;
                        var batchEndIndex = batchStartIndex + batchCount > runningData.Entities.Length
                            ? runningData.Entities.Length : batchStartIndex + batchCount;

                        if (batchStartIndex < batchEndIndex)
                        {
                            batches.Add(new BatchOptions
                            {
                                StartIndex = batchStartIndex,
                                EndIndex = batchEndIndex,
                                Context = runningData.Context,
                                EntityManager = runningData.Context.Entities,
                                Entities = runningData.Entities,
                                Commands = runningData.Commands,
                                ReadConfigs = runningData.ReadConfigs,
                                WriteConfigs = runningData.WriteConfigs,
                                Action = runningData.Action,
                                CommandAction = runningData.CommandAction,
                                CachedCommands = runningData.CachedCommands[i],
                                UseQueryCommands = useQueryCommands
                            });
                        }
                        else
                            break;
                    }

                    useQueryCommandsCount = batches.Count;
                    var result = Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = _threadCount },
                        batch =>
                        {
                            new ForEachOptions(batch)
                                .InvokeForEachAction();
                        });
                }
                else
                {
                    useQueryCommandsCount = 1;
                    new ForEachOptions(new BatchOptions
                    {
                        StartIndex = 0,
                        EndIndex = runningData.Entities.Length,
                        Context = runningData.Context,
                        EntityManager = runningData.Context.Entities,
                        Entities = runningData.Entities,
                        Commands = runningData.Commands,
                        ReadConfigs = runningData.ReadConfigs,
                        WriteConfigs = runningData.WriteConfigs,
                        Action = runningData.Action,
                        CommandAction = runningData.CommandAction,
                        CachedCommands = runningData.CachedCommands[0],
                        UseQueryCommands = useQueryCommands
                    }).InvokeForEachAction();
                }

                runningData.Context.StructChangeAvailable = true;
                if (useQueryCommands)
                {
                    for (var i = 0; i < useQueryCommandsCount; i++)
                    {
                        var commands = runningData.CachedCommands[i];
                        for (var j = 0; j < commands.Count; j++)
                            commands[j].QueryExecute(runningData.Context);
                        commands.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                runningData.Filter = dataFilter;
                runningData.Context.StructChangeAvailable = true;
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
                else if(config.IsShared)
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
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public EcsContext Context { get; set; }
            public EntityManager EntityManager { get; set; }
            public Entity[] Entities { get; set; }
            public EntityCommands Commands { get; set; }
            public ComponentConfig[] ReadConfigs { get; set; }
            public ComponentConfig[] WriteConfigs { get; set; }
            public ForEachRunAction Action { get; set; }
            public ForEachRunAction CommandAction { get; set; }
            public List<IEntityQueryCommand> CachedCommands { get; set; }
            public bool UseQueryCommands { get; set; }
        }

        private class ForEachOptions
        {
            private EcsContext _context;
            private ArcheTypeData _prevArcheTypeData = null;
            private ArcheTypeIndex _prevArcheTypeIndex;
            private int _trackStartingIndex;

            internal int Index { get; set; }
            internal Entity CurrentEntity { get; set; }
            internal int EntityIndex { get; set; }
            internal ArcheTypeIndex ArcheTypeIndex { get => _prevArcheTypeIndex; }
            internal IEntityQueryAdapter[] Adapters { get; set; }
            internal BatchOptions BatchOptions { get; private set; }

            internal ForEachOptions(BatchOptions batchOptions)
            {
                _context = batchOptions.Context;

                Adapters = CreateAdapters(batchOptions.Commands != null,
                    batchOptions.WriteConfigs, batchOptions.ReadConfigs);
                BatchOptions = batchOptions;
            }

            internal void InvokeForEachAction()
            {
                _trackStartingIndex = BatchOptions.StartIndex;

                if (BatchOptions.Commands == null && !BatchOptions.UseQueryCommands)
                {
                    for (Index = BatchOptions.StartIndex; Index < BatchOptions.EndIndex; Index++)
                    {
                        var entity = BatchOptions.Entities[Index];
                        UpdateEntityData(entity, BatchOptions.EntityManager.GetEntityData(entity));

                        BatchOptions.Action.Invoke(this);
                    }

                    // Need to update last ArcheTypeData aswell
                    for (var i = 0; i < BatchOptions.WriteConfigs.Length; i++)
                    {
                        _context.Tracking.TrackUpdates(BatchOptions.Entities,
                            _trackStartingIndex, Index, BatchOptions.WriteConfigs[i], _prevArcheTypeData);
                    }
                }
                else
                {
                    for (Index = BatchOptions.StartIndex; Index < BatchOptions.EndIndex; Index++)
                    {
                        var entity = BatchOptions.Entities[Index];
                        UpdateEntityData(entity, BatchOptions.EntityManager.GetEntityData(entity));

                        BatchOptions.CommandAction.Invoke(this);
                    }

                    if (!BatchOptions.UseQueryCommands)
                    {
                        BatchOptions.Commands.AppendQueryCommands(BatchOptions.CachedCommands);
                        BatchOptions.CachedCommands.Clear();
                    }
                }
            }

            private void UpdateEntityData(in Entity entity, in EntityData entityData)
            {
                if (_prevArcheTypeIndex != entityData.ArcheTypeIndex)
                {
                    if (BatchOptions.Commands == null && _prevArcheTypeData != null)
                    {
                        for (var i = 0; i < BatchOptions.WriteConfigs.Length; i++)
                        {
                            _context.Tracking.TrackParallelUpdates(BatchOptions.Entities,
                                _trackStartingIndex, Index, BatchOptions.WriteConfigs[i], _prevArcheTypeData);
                        }
                        _trackStartingIndex = Index;
                    }

                    var archeTypeData = _context.ArcheTypes
                        .GetArcheTypeData(entityData.ArcheTypeIndex);
                    for (var i = 0; i < Adapters.Length; i++)
                    {
                        var adapter = Adapters[i];
                        adapter.ChangeArcheTypeData(archeTypeData);
                    }

                    _prevArcheTypeIndex = entityData.ArcheTypeIndex;
                    _prevArcheTypeData = archeTypeData;
                }

                CurrentEntity = entity;
                EntityIndex = entityData.EntityIndex;
            }
        }
    }
}
