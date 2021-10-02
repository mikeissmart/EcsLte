using System.Collections.Generic;

namespace EcsLte
{
    public interface IGetEntity
    {
        bool HasEntity(Entity entity);
        Entity[] GetEntities();
    }

    public interface IEntityLife
    {
        Entity CreateEntity();
        Entity CreateEntity(EntityBlueprint blueprint);
        Entity[] CreateEntities(int count);
        void DestroyEntity(Entity entity);
        void DestroyEntities(ICollection<Entity> entities);
    }

    public interface IGetComponent
    {
        bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : IUniqueComponent;
        TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : IUniqueComponent;
        Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : IUniqueComponent;
        bool HasComponent<TComponent>(Entity entity) where TComponent : IComponent;
        TComponent GetComponent<TComponent>(Entity entity) where TComponent : IComponent;
        IComponent[] GetAllComponents(Entity entity);
    }

    public interface IComponentLife
    {
        void AddUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique componentUnique)
            where TComponentUnique : IUniqueComponent;
        void ReplaceUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique newComponentUnique)
            where TComponentUnique : IUniqueComponent;
        void RemoveUniqueComponent<TComponentUnique>(Entity entity) where TComponentUnique : IUniqueComponent;
        void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent;
        void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent) where TComponent : IComponent;
        void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent;
        void RemoveAllComponents(Entity entity);
    }
}