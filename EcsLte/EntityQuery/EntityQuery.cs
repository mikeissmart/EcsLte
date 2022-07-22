using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsLte
{
    public class EntityQuery
    {
        private static readonly int _threadCount = (int)(Environment.ProcessorCount * 0.75);
        private static bool _queryRunning = false;

        private Entity[] _cachedEntities;

        public EcsContext Context { get; private set; }
        public EntityFilter Filter { get; private set; }
        public EntityTracker Tracker { get; private set; }

        public EntityQuery(EcsContext context, EntityFilter filter)
        {
            EcsContext.AssertContext(context);
            EntityFilter.AssertEntityFilter(filter);

            _cachedEntities = new Entity[0];

            Context = context;
            Filter = filter;
        }

        public EntityQuery(EntityTracker tracker, EntityFilter filter)
        {
            EntityTracker.AssertEntityTracker(tracker, tracker?.Context);
            EntityFilter.AssertEntityFilter(filter);

            _cachedEntities = new Entity[0];

            Context = tracker.Context;
            Filter = filter;
            Tracker = tracker;
        }

        internal EntityQuery(EntityQuery clone)
        {
            _cachedEntities = new Entity[0];

            Context = clone.Context;
            Filter = new EntityFilter(clone.Filter);
            if (clone.Tracker != null)
                Tracker = new EntityTracker(clone.Tracker);
        }

        internal static void AssertEntityQuery(EntityQuery query, EcsContext context)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (query.Context != context)
                throw new EcsContextDifferentException(query.Context, context);
            if (query.Tracker != null)
                EntityTracker.AssertEntityTracker(query.Tracker, context);
        }

        #region ForEachs

        #region ForEachs

        public unsafe void ForEach(EntityQueryActions.R0W0 action, bool runParallel, EntityCommands commandQueue = null) =>
            ForEachRun(commandQueue, runParallel, 0,
                new ComponentConfig[0],
                (options) =>
                {
                    action(options.Index, options.CurrentEntity);
                });

        #region Write 0

        public unsafe void ForEach<T1>(EntityQueryActions.R1W0<T1> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent => ForEachRun(commandQueue, runParallel, 0,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        in ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2>(EntityQueryActions.R2W0<T1, T2> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3>(EntityQueryActions.R3W0<T1, T2, T3> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3, T4>(EntityQueryActions.R4W0<T1, T2, T3, T4> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R5W0<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R6W0<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R7W0<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R8W0<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 0,
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

        public unsafe void ForEach<T1>(EntityQueryActions.R0W1<T1> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent => ForEachRun(commandQueue, runParallel, 1,
                new[]
                {
                    ComponentConfig<T1>.Config
                },
                (options) =>
                {
                    action(options.Index, options.CurrentEntity,
                        ref ((IComponentAdapter<T1>)options.ComponentAdapters[0]).GetComponentRef());
                });

        public unsafe void ForEach<T1, T2>(EntityQueryActions.R1W1<T1, T2> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3>(EntityQueryActions.R2W1<T1, T2, T3> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3, T4>(EntityQueryActions.R3W1<T1, T2, T3, T4> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R4W1<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R5W1<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R6W1<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R7W1<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 1,
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

        public unsafe void ForEach<T1, T2>(EntityQueryActions.R0W2<T1, T2> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3>(EntityQueryActions.R1W2<T1, T2, T3> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3, T4>(EntityQueryActions.R2W2<T1, T2, T3, T4> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R3W2<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R4W2<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R5W2<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R6W2<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 2,
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

        public unsafe void ForEach<T1, T2, T3>(EntityQueryActions.R0W3<T1, T2, T3> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4>(EntityQueryActions.R1W3<T1, T2, T3, T4> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R2W3<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R3W3<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R4W3<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R5W3<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 3,
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

        public unsafe void ForEach<T1, T2, T3, T4>(EntityQueryActions.R0W4<T1, T2, T3, T4> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent => ForEachRun(commandQueue, runParallel, 4,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R1W4<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 4,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R2W4<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 4,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R3W4<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 4,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R4W4<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 4,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R0W5<T1, T2, T3, T4, T5> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent => ForEachRun(commandQueue, runParallel, 5,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R1W5<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 5,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R2W5<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 5,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R3W5<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 5,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R0W6<T1, T2, T3, T4, T5, T6> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent => ForEachRun(commandQueue, runParallel, 6,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R1W6<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 6,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R2W6<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 6,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R0W7<T1, T2, T3, T4, T5, T6, T7> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent => ForEachRun(commandQueue, runParallel, 7,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R1W7<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 7,
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

        public unsafe void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R0W8<T1, T2, T3, T4, T5, T6, T7, T8> action, bool runParallel, EntityCommands commandQueue = null)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent => ForEachRun(commandQueue, runParallel, 8,
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

        private unsafe void ForEachRun(EntityCommands commandQueue, bool runParallel, int writeCount, ComponentConfig[] configs, ForEachRunAction action)
        {
            if (_queryRunning)
                throw new EntityQueryAlreadyRunningException();
            _queryRunning = true;
            if (commandQueue != null && commandQueue.Context != Context)
                throw new EcsContextDifferentException(Context, commandQueue.Context);

            Context.AssertContext();
            Helper.AssertDuplicateConfigs(configs);

            var missingConfigs = configs.Where(x => !Filter.AllOfComponentConfigs.Contains(x));
            if (missingConfigs.Count() > 0)
                throw new EntityFilterNotHaveWhereAllOfException(missingConfigs.First().ComponentType);

            Context.Entities.GetEntities(this, ref _cachedEntities);
            var archeTypeDatas = Filter.GetContextData(Context).ArcheTypeDatas;
            if (runParallel)
            {
                var batches = new List<BatchOptions>();
                var batchCount = _cachedEntities.Length / _threadCount +
                    (_cachedEntities.Length % _threadCount != 0
                        ? 1
                        : 0);
                for (var i = 0; i < _threadCount; i++)
                {
                    var batchStartIndex = i * batchCount;
                    var batchEndIndex = batchStartIndex + batchCount > _cachedEntities.Length
                        ? _cachedEntities.Length
                        : batchStartIndex + batchCount;

                    if (batchStartIndex < batchEndIndex)
                    {
                        batches.Add(new BatchOptions
                        {
                            StartIndex = batchStartIndex,
                            EndIndex = batchEndIndex,
                            WriteCount = writeCount,
                            Context = Context,
                            Entities = _cachedEntities,
                            Configs = configs,
                            ArcheTypeDatas = archeTypeDatas,
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
                    EndIndex = _cachedEntities.Length,
                    WriteCount = writeCount,
                    Context = Context,
                    Entities = _cachedEntities,
                    Configs = configs,
                    ArcheTypeDatas = archeTypeDatas,
                    Action = action
                };

                ForEachBatchRun(batchOptions);
            }

            _queryRunning = false;
        }

        private static unsafe void ForEachBatchRun(BatchOptions batchOptions)
        {
            var componentAdapters = EntityQueryComponentAdapters.CreateAdapters(batchOptions.Configs,
                batchOptions.Context.SharedIndexDics);
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
                    batchOptions.Context.Entities.UpdateForEach(entity, forEachOptions.WriteComponentAdapters);
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
            private ArcheTypeData _prevArcheTypeData;
            private IComponentAdapter[] _componentAdapters;

            public int Index { get; private set; }
            public Entity CurrentEntity { get; private set; }
            public HashSet<ArcheTypeIndex> ArcheTypeDataHash { get; private set; }
            public IComponentAdapter[] ComponentAdapters { get => _componentAdapters; }
            public IComponentAdapter[] WriteComponentAdapters { get; private set; }

            public ForEachOptions(ArcheTypeData[] archeTypeDatas, IComponentAdapter[] componentAdapters, int writeCount)
            {
                ArcheTypeDataHash = new HashSet<ArcheTypeIndex>(archeTypeDatas.Select(x => x.ArcheTypeIndex));
                _componentAdapters = componentAdapters;
                WriteComponentAdapters = componentAdapters.Take(writeCount).ToArray();
            }

            public unsafe bool StoreComponents(EcsContext context, int index, Entity entity)
            {
                Index = index;
                CurrentEntity = entity;

                return context.Entities.GetForEach(entity, ref _prevArcheTypeData, ref _componentAdapters);
            }
        }

        #endregion Privates

        #endregion ForEachs
    }
}