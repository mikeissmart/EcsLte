namespace EcsLte
{
    public static class EntityQueryActions
    {
        public delegate void EntityQueryAction(Entity entity);
        public delegate void EntityQueryAction<T1>(Entity entity, T1 component1)
            where T1 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2>(Entity entity, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3>(Entity entity, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3, T4>(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent;
        public delegate void EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7, T8 component8)
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
