using EcsLte.Exceptions;
using System.Collections.Generic;

namespace EcsLte.HybridArcheType
{
    public class EcsContext_Hybrid
    {
        private ComponentEntityFactory_Hybrid _componentEntityFactory;

        internal EcsContext_Hybrid(string name, ComponentEntityFactory_Hybrid componentEntityFactory)
        {
            _componentEntityFactory = componentEntityFactory;
            Name = name;
        }

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public int EntityCount => _componentEntityFactory?.Count ?? 0;
        public int EntityCapacity => _componentEntityFactory?.Capacity ?? 0;

        internal void InternalDestroy()
        {
            _componentEntityFactory.Dispose();
            _componentEntityFactory = null;
            IsDestroyed = true;
        }

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.GetEntities();
        }

        public Entity CreateEntity(EntityBlueprint_Hybrid blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.CreateEntity(blueprint);
        }

        public Entity[] CreateEntities(int count, EntityBlueprint_Hybrid blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.CreateEntities(count, blueprint);
        }

        public void DestroyEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            _componentEntityFactory.DestroyEntity(entity);
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            _componentEntityFactory.DestroyEntities(entities);
        }

        public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.HasComponent<TComponent>(entity);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.GetComponent<TComponent>(entity);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.GetAllComponents(entity);
        }
        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.HasUniqueComponent<TComponentUnique>();
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.GetUniqueComponent<TComponentUnique>();
        }

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            return _componentEntityFactory.GetUniqueEntity<TComponentUnique>();
        }

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(new EcsContext(Name, null));

            _componentEntityFactory.UpdateComponent(entity, component);
        }
    }
}
