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
    }
}
