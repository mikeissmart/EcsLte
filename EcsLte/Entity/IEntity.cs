using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal interface IEntityGet
    {
        bool HasEntity(Entity entity);
        Entity[] GetEntities();
    }

    public interface IEntityLife
    {
        Entity CreateEntity();
        Entity[] CreateEntities(int count);
        void DestroyEntity(Entity entity);
        void DestroyEntities(IEnumerable<Entity> entities);
    }

    public interface IEntityComponentGet
    {
        bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        IComponent[] GetAllComponents(Entity entity);
        bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
    }

    public interface IEntityComponentLife
    {
        void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent;
        void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent) where TComponent : unmanaged, IComponent;
        void RemoveComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        void RemoveAllComponents(Entity entity);
        Entity AddUniqueComponent<TComponentUnique>(TComponentUnique componentUnique) where TComponentUnique : unmanaged, IUniqueComponent;
        Entity ReplaceUniqueComponent<TComponentUnique>(TComponentUnique newComponentUnique) where TComponentUnique : unmanaged, IUniqueComponent;
        void RemoveUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
    }
}
