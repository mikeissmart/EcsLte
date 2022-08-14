namespace EcsLte
{
    public class EntityQueryManager
    {
        public EcsContext Context { get; private set; }

        internal EntityQueryManager(EcsContext context) => Context = context;

        public EntityQuery SetCommands(EntityCommands commands)
            => new EntityQuery(Context)
                .SetCommands(commands);

        public EntityQuery SetFilter(EntityFilter filter)
            => new EntityQuery(Context)
                .SetFilter(filter);

        public EntityQuery SetTracker(EntityTracker tracker)
            => new EntityQuery(Context)
                .SetTracker(tracker);

        internal void InternalDestroy()
        {

        }
    }
}
