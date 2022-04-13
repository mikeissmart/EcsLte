using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal interface IComponentEntityFactory : IDisposable
    {
        int Count { get; }
        int Capacity { get; }

        Entity[] GetEntities();
        bool HasEntity(Entity entity);
        Entity CreateEntity(IEntityBlueprint blueprint);
        Entity[] CreateEntities(int count, IEntityBlueprint blueprint);
        void DestroyEntity(Entity entity);
        void DestroyEntities(IEnumerable<Entity> entities);
        bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        IComponent[] GetAllComponents(Entity entity);
        bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent;
        void ReplaceComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent;
        void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        void RemoveAllComponents(Entity entity);
        Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent;
        Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent;
        void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;

        SharedComponentDataIndex GetSharedComponentDataIndex<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent;

        IEntityQuery EntityQueryCreate();
        void EntityQueryAddToMaster(IEntityQuery query);
        bool EntityQueryHasEntity(IEntityQueryData query, Entity entity);
        Entity[] EntityQueryGetEntities(EntityQueryData_ArcheType entityQueryData);

        void ForEach<T1>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1> action)
            where T1 : unmanaged, IComponent;
        void ForEach<T1, T2>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        void ForEach<T1, T2, T3>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6, T7>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent;
        void ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(IEntityQueryData queryData, EntityQueryActions.EntityQueryAction<T1, T2, T3, T4, T5, T6, T7, T8> action)
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
