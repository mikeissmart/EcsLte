using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public class EntityQuery
    {
        /*
        var query = Context.Queries
            .SetFilter(new EntityFilter())
            .SetTracking(Context.Tracking.CreateTracker("Name"))
            .SetCommands(Context.Commands.CreateCommands("Name"))
            .ForEach(
                (int index, Entity entity, ref/in component1 - 8) =>
                {
                    bool applyChanges = false;
                    return applyChanges; // Only when there is at least 1 ref
                })
            .Run(isParallel);
        query.ClearFilter();
        query.Tracker.StopTracking();
        query.ForEach(
            (int index, Entity entity) =>
            {
                // Something
            });
        query.Run(isParallel);
         */
        public EcsContext Context { get; private set; }
        public EntityCommands Commands { get; private set; }
        public EntityFilter Filter { get; private set; }
        public EntityTracker Tracker { get; private set; }

        internal EntityQuery(EcsContext context)
        {
            Context = context;
        }

        public EntityQuery SetCommands(EntityCommands commands)
        {
            Context.AssertContext();
            EntityCommands.AssertEntityCommands(commands, Context);

            Commands = commands;

            return this;
        }

        public EntityQuery ClearCommands()
        {
            Context.AssertContext();

            Commands = null;

            return this;
        }

        public EntityQuery SetFilter(EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            Filter = filter;

            return this;
        }

        public EntityQuery ClearFilter()
        {
            Context.AssertContext();

            Filter = null;

            return this;
        }

        public EntityQuery SetTracker(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            Tracker = tracker;

            return this;
        }

        public EntityQuery ClearTracker()
        {
            Context.AssertContext();

            Tracker = null;

            return this;
        }

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
    }
}
