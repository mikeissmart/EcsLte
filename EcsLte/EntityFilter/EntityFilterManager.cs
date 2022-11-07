namespace EcsLte
{
    public class EntityFilterManager
    {
        public EcsContext Context { get; private set; }

        internal EntityFilterManager(EcsContext context) => Context = context;

        public EntityFilter CreateFilter()
            => new EntityFilter(Context);

        public EntityFilter WhereAllOf<TComponent>()
            where TComponent : IComponent
            => new EntityFilter(Context)
                .WhereAllOf<TComponent>();

        public EntityFilter WhereAllOf(EntityArcheType archeType)
            => new EntityFilter(Context)
                .WhereAllOf(archeType);

        public EntityFilter WhereAnyOf<TComponent>()
            where TComponent : IComponent
            => new EntityFilter(Context)
                .WhereAnyOf<TComponent>();

        public EntityFilter WhereAnyOf(EntityArcheType archeType)
            => new EntityFilter(Context)
                .WhereAnyOf(archeType);

        public EntityFilter WhereNoneOf<TComponent>()
            where TComponent : IComponent
            => new EntityFilter(Context)
                .WhereNoneOf<TComponent>();

        public EntityFilter WhereNoneOf(EntityArcheType archeType)
            => new EntityFilter(Context)
                .WhereNoneOf(archeType);

        public EntityFilter FilterBy<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
            => new EntityFilter(Context)
                .FilterBy(component);

        public EntityFilter FilterBy(EntityArcheType archeType)
            => new EntityFilter(Context)
                .FilterBy(archeType);

        internal void InternalDestroy()
        {

        }
    }
}
