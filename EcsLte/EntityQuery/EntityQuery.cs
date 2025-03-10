﻿using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public partial class EntityQuery
    {
        private static readonly int _threadCount = (int)(Environment.ProcessorCount * 0.75);
        private Data _data;

        public EcsContext Context => _data.Context;
        public EntityCommands Commands => _data.Commands;
        public EntityFilter Filter => _data.Filter;
        public EntityTracker Tracker => _data.Tracker;
        public int ThreadCount => _threadCount;

        internal EntityQuery(EcsContext context) => _data = new Data(context);

        public EntityQuery SetCommands(EntityCommands commands)
        {
            Context.AssertContext();
            EntityCommands.AssertEntityCommands(commands, Context);

            _data = new Data(_data)
            {
                Commands = commands
            };

            return this;
        }

        public EntityQuery ClearCommands()
        {
            Context.AssertContext();

            _data = new Data(_data)
            {
                Commands = null
            };

            return this;
        }

        public EntityQuery SetFilter(EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            _data = new Data(_data)
            {
                Filter = filter
            };

            return this;
        }

        public EntityQuery ClearFilter()
        {
            Context.AssertContext();

            _data = new Data(_data)
            {
                Filter = null
            };

            return this;
        }

        public EntityQuery SetTracker(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            _data = new Data(_data)
            {
                Tracker = tracker
            };

            return this;
        }

        public EntityQuery ClearTracker()
        {
            Context.AssertContext();

            _data = new Data(_data)
            {
                Tracker = null
            };

            return this;
        }

        public void Run()
            => InternalRun(false, 1);

        public void RunParallel()
            => InternalRun(true, _threadCount);

        public void RunParallel(int threadCount)
            => InternalRun(true, threadCount);

        #region Assert

        internal static void AssertEntityQuery(EntityQuery query, EcsContext context)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (query.Context != context)
                throw new EcsContextNotSameException(query.Context, context);
            if (query.Tracker != null)
                EntityTracker.AssertEntityTracker(query.Tracker, context);
        }

        #endregion

        private delegate void ForEachRunAction(ForEachOptions options);

        private class Data
        {
            public EcsContext Context;
            public EntityCommands Commands;
            public EntityFilter Filter;
            public EntityTracker Tracker;
            public ComponentConfig[] ReadConfigs;
            public ComponentConfig[] WriteConfigs;
            public ForEachRunAction Action;
            public ForEachRunAction CommandAction;
            public ForEachRunAction OtherAction;
            public int OtherCount;
            public Entity[] Entities;
            public List<IEntityCommand>[] EntityCommandsCaches;
            public EntityQueryArcheTypeCacheRoot[] EntityQueryCacheRoots;

            public Data(EcsContext context)
            {
                Context = context;

                ReadConfigs = new ComponentConfig[0];
                WriteConfigs = new ComponentConfig[0];
                Entities = new Entity[0];
                EntityCommandsCaches = new List<IEntityCommand>[_threadCount];
                EntityQueryCacheRoots = new EntityQueryArcheTypeCacheRoot[_threadCount];

                for (var i = 0; i < _threadCount; i++)
                {
                    EntityCommandsCaches[i] = new List<IEntityCommand>();
                    EntityQueryCacheRoots[i] = new EntityQueryArcheTypeCacheRoot();
                }
            }

            public Data(Data data)
            {
                Context = data.Context;
                Commands = data.Commands;
                Filter = data.Filter;
                Tracker = data.Tracker;
                ReadConfigs = data.ReadConfigs;
                WriteConfigs = data.WriteConfigs;
                Action = data.Action;
                CommandAction = data.CommandAction;
                OtherAction = data.OtherAction;
                OtherCount = data.OtherCount;
                Entities = data.Entities;
                EntityCommandsCaches = data.EntityCommandsCaches;
                EntityQueryCacheRoots = data.EntityQueryCacheRoots;
            }
        }
    }
}
