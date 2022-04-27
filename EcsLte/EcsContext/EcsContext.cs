using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public class EcsContext : IEntityGet, IEntityLife, IEntityComponentGet, IEntityComponentLife
    {
        internal EcsContext(string name)
        {
            ArcheTypeFactory = new ArcheTypeFactory();
            ComponentEntityFactory = new ComponentEntityFactory();
            EntityQueryFactory = new EntityQueryFactory();

            ComponentEntityFactory.SetDependentFactories(ArcheTypeFactory);

            Name = name;
        }

        #region EcsContext

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public int EntityCount => ComponentEntityFactory?.EntityCount ?? 0;
        public int EntityCapacity => ComponentEntityFactory?.EntityCapacity ?? 0;

        internal ArcheTypeFactory ArcheTypeFactory { get; private set; }
        internal ComponentEntityFactory ComponentEntityFactory { get; private set; }
        internal EntityQueryFactory EntityQueryFactory { get; private set; }

        public EntityQuery CreateQuery()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return new EntityQuery(this);
        }

        internal void InternalDestroy()
        {
            ArcheTypeFactory.Dispose();
            ArcheTypeFactory = null;
            ComponentEntityFactory.Dispose();
            ComponentEntityFactory = null;
            EntityQueryFactory.Dispose();
            EntityQueryFactory = null;

            IsDestroyed = true;
        }

        #endregion

        #region EntityGet

        public bool HasEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.HasEntity(entity);
        }

        public Entity[] GetEntities()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.GetEntities();
        }

        #endregion

        #region EntityLife

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.CreateEntity(blueprint);
        }

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.CreateEntities(count, blueprint);
        }

        public void DestroyEntity(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            ComponentEntityFactory.DestroyEntity(entity);
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            ComponentEntityFactory.DestroyEntities(entities);
        }

        #endregion

        #region ComponentGet

        public bool HasComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.HasComponent<TComponent>(entity);
        }

        public TComponent GetComponent<TComponent>(Entity entity) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.GetComponent<TComponent>(entity);
        }

        public IComponent[] GetAllComponents(Entity entity)
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.GetAllComponents(entity);
        }
        public bool HasUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.HasUniqueComponent<TComponentUnique>();
        }

        public TComponentUnique GetUniqueComponent<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.GetUniqueComponent<TComponentUnique>();
        }

        public Entity GetUniqueEntity<TComponentUnique>() where TComponentUnique : unmanaged, IUniqueComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            return ComponentEntityFactory.GetUniqueEntity<TComponentUnique>();
        }

        #endregion

        #region ComponentLife

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            ComponentEntityFactory.UpdateComponent(entity, component);
        }

        public void UpdateComponent<TComponent>(EntityQuery query, TComponent component) where TComponent : unmanaged, IComponent
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);

            ComponentEntityFactory.UpdateComponent(query, component);
        }

        #endregion
    }
}
