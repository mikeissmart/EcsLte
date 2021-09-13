using System;
using System.Collections.Generic;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class KeyManager
    {
        private readonly Dictionary<int, IPrimaryKey> _primaryKeyes;
        private readonly Dictionary<int, ISharedKey> _sharedKeyes;

        internal KeyManager(World world)
        {
            _primaryKeyes = new Dictionary<int, IPrimaryKey>();
            _sharedKeyes = new Dictionary<int, ISharedKey>();

            CurrentWorld = world;
        }

        public World CurrentWorld { get; }

        public PrimaryKey<TComponent> GetPrimaryKey<TComponent>()
            where TComponent : IComponentPrimaryKey
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            lock (_primaryKeyes)
            {
                if (!_primaryKeyes.TryGetValue(componentPoolIndex, out var key))
                {
                    key = new PrimaryKey<TComponent>(this,
                        CurrentWorld.EntityManager.GetComponentPool(componentPoolIndex));
                    _primaryKeyes.Add(componentPoolIndex, key);
                }

                return (PrimaryKey<TComponent>)key;
            }
        }

        public SharedKey<TComponent> GetSharedKey<TComponent>()
            where TComponent : IComponentSharedKey
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var componentPoolIndex = ComponentIndex<TComponent>.Index;
            lock (_sharedKeyes)
            {
                if (!_sharedKeyes.TryGetValue(componentPoolIndex, out var key))
                {
                    key = new SharedKey<TComponent>(this,
                        CurrentWorld.EntityManager.GetComponentPool(componentPoolIndex));
                    _sharedKeyes.Add(componentPoolIndex, key);
                }

                return (SharedKey<TComponent>)key;
            }
        }

        internal void OnEntityComponentAdded<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            lock (_primaryKeyes)
            {
                if (_primaryKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((PrimaryKey<TComponent>)key).OnEntityComponentRemoved(entity, component);
            }

            lock (_sharedKeyes)
            {
                if (_sharedKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((SharedKey<TComponent>)key).OnEntityComponentAdded(entity, component);
            }
        }

        internal void OnEntityComponentRemoved<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            lock (_primaryKeyes)
            {
                if (_primaryKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((PrimaryKey<TComponent>)key).OnEntityComponentRemoved(entity, component);
            }

            lock (_sharedKeyes)
            {
                if (_sharedKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((SharedKey<TComponent>)key).OnEntityComponentRemoved(entity, component);
            }
        }

        internal void OnEntityComponentReplaced<TComponent>(Entity entity, TComponent oldComponent, TComponent newComponent)
            where TComponent : IComponent
        {
            var componentPoolIndex = ComponentIndex<TComponent>.Index;

            lock (_primaryKeyes)
            {
                if (_primaryKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((PrimaryKey<TComponent>)key).OnEntityComponentReplaced(entity, oldComponent, newComponent);
            }

            lock (_sharedKeyes)
            {
                if (_sharedKeyes.TryGetValue(componentPoolIndex, out var key))
                    ((SharedKey<TComponent>)key).OnEntityComponentReplaced(entity, oldComponent, newComponent);
            }
        }

        internal void InternalDestroy()
        {
            lock (_sharedKeyes)
            {
                _sharedKeyes.Clear();
            }
        }
    }
}