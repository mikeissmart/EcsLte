using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public interface IEntityGet
    {
        bool HasEntity(Entity entity);
        Entity[] GetEntities();
    }

    public interface IEntityLife
    {
        Entity CreateEntity(EntityBlueprint blueprint);
        Entity[] CreateEntities(int count, EntityBlueprint blueprint);
        void DestroyEntity(Entity entity);
        void DestroyEntities(IEnumerable<Entity> entities);
    }

    public interface IEntityComponentGet
    {
        bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent;
        IComponent[] GetAllComponents(Entity entity);
    }

    public interface IEntityComponentLife
    {
        void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent;
    }

    public interface IEntityComponentUniqueGet
    {
        bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
        Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent;
    }
}
