using System;
using System.Collections.Generic;
using EcsLte.Exceptions;

namespace EcsLte
{
    public interface IPrimaryKey
    {
        bool GetEntity(IComponent component, out Entity entity);
    }

    public class PrimaryKey<TComponent> : IPrimaryKey
        where TComponent : IComponent
    {
        private Dictionary<TComponent, Entity> _keyes;

        internal PrimaryKey(KeyManager keyManager, IComponentPool componentPool)
        {
            _keyes = new Dictionary<TComponent, Entity>();

            var entities = keyManager.CurrentWorld.EntityManager.GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (componentPool.HasComponent(entity.Id))
                    OnEntityComponentAdded(entity, (TComponent)componentPool.GetComponent(entity.Id));
            }

            CurrentWorld = keyManager.CurrentWorld;
        }

        public World CurrentWorld { get; private set; }

        public bool GetEntity(IComponent component, out Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            if (!(component is TComponent))
                throw new PrimaryKeyWrongTypeException(typeof(TComponent), component.GetType());

            return GetEntity((TComponent)component, out entity);
        }

        public bool GetEntity(TComponent component, out Entity entity)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            lock (_keyes)
            {
                return _keyes.TryGetValue(component, out entity);
            }
        }

        internal void OnEntityComponentAdded(Entity entity, TComponent component)
        {
            lock (_keyes)
            {
                if (_keyes.TryGetValue(component, out var currentEntity))
                    // TODO
                    throw new Exception();
                _keyes.Add(component, entity);
            }
        }

        internal void OnEntityComponentRemoved(Entity entity, TComponent component)
        {
            lock (_keyes)
            {
                if (_keyes.ContainsKey(component))
                    _keyes.Remove(component);
            }
        }

        internal void OnEntityComponentReplaced(Entity entity, TComponent oldComponent, TComponent newComponent)
        {
            OnEntityComponentRemoved(entity, oldComponent);
            OnEntityComponentAdded(entity, newComponent);
        }
    }
}