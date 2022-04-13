namespace EcsLte
{
    public interface IEntityQuery : IEntityGet
    {
        ComponentConfig[] AllConfigs { get; }
        ComponentConfig[] AnyConfigs { get; }
        ComponentConfig[] NoneConfigs { get; }

        IEntityQuery WhereAllOf<T1>()
            where T1 : IComponent;
        IEntityQuery WhereAllOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent;
        IEntityQuery WhereAllOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;
        IEntityQuery WhereAllOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;
        IEntityQuery WhereAnyOf<TComponent>()
            where TComponent : IComponent;
        IEntityQuery WhereAnyOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent;
        IEntityQuery WhereAnyOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;
        IEntityQuery WhereAnyOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;
        IEntityQuery WhereNoneOf<TComponent>()
            where TComponent : IComponent;
        IEntityQuery WhereNoneOf<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent;
        IEntityQuery WhereNoneOf<T1, T2, T3>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;
        IEntityQuery WhereNoneOf<T1, T2, T3, T4>()
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;
        IEntityQuery FilterBy<T1>(T1 component)
            where T1 : ISharedComponent;
        IEntityQuery FilterBy<T1, T2>(T1 component1, T2 component2)
            where T1 : ISharedComponent
            where T2 : ISharedComponent;
        IEntityQuery FilterBy<T1, T2, T3>(T1 component1, T2 component2, T3 component3)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent;
        IEntityQuery FilterBy<T1, T2, T3, T4>(T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : ISharedComponent
            where T2 : ISharedComponent
            where T3 : ISharedComponent
            where T4 : ISharedComponent;
        void ForEach(EntityQueryActions.EntityQueryAction action);
        void ForEach<T1>(EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent;
        void ForEach<T1, T2>(EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        void ForEach<T1, T2, T3>(EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent;
    }
}
