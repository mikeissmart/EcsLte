using System;
using System.Collections.Generic;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public interface ISharedKey
    {
        Entity[] GetEntities(IComponent component);
    }

    public class SharedKey<TComponent> : ISharedKey
        where TComponent : IComponent
    {
        private Dictionary<TComponent, DataCache<List<Entity>, Entity[]>> _keyes;

        internal SharedKey(KeyManager keyManager, IComponentPool componentPool)
        {
            _keyes = new Dictionary<TComponent, DataCache<List<Entity>, Entity[]>>();

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

        public Entity[] GetEntities(IComponent component)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            if (!(component is TComponent))
                throw new SharedKeyWrongTypeException(typeof(TComponent), component.GetType());

            return GetEntities((TComponent)component);
        }

        public Entity[] GetEntities(TComponent component)
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            lock (_keyes)
            {
                if (_keyes.TryGetValue(component, out var entities))
                    return entities.CachedData;
            }

            return new Entity[0];
        }

        internal void OnEntityComponentAdded(Entity entity, TComponent component)
        {
            lock (_keyes)
            {
                if (!_keyes.TryGetValue(component, out var entities))
                {
                    entities = new DataCache<List<Entity>, Entity[]>(
                        new List<Entity>(),
                        UpdateEntitiesCache);
                    _keyes.Add(component, entities);
                }
                entities.UncachedData.Add(entity);
            }
        }

        internal void OnEntityComponentRemoved(Entity entity, TComponent component)
        {
            lock (_keyes)
            {
                if (_keyes.TryGetValue(component, out var entities))
                {
                    entities.UncachedData.Remove(entity);
                    if (entities.UncachedData.Count == 0)
                        _keyes.Remove(component);
                }
            }
        }

        internal void OnEntityComponentReplaced(Entity entity, TComponent oldComponent, TComponent newComponent)
        {
            OnEntityComponentRemoved(entity, oldComponent);
            OnEntityComponentAdded(entity, newComponent);
        }

        private static Entity[] UpdateEntitiesCache(List<Entity> uncachedData)
        {
            return uncachedData.ToArray();
        }
    }
}