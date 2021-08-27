using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class World
    {
        private static readonly Dictionary<string, World> _worldsLookup = new Dictionary<string, World>();
        private static readonly DataCache<World[]> _worldsCache = new DataCache<World[]>(UpdateWorldsCache);

        private World(string name)
        {
            Name = name;
            EntityManager = new EntityManager(this);
            GroupManager = new GroupManager(EntityManager);
        }

        public static World[] Worlds => _worldsCache.Data;
        public static World DefaultWorld { get; set; } = CreateWorld("Default");

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public EntityManager EntityManager { get; }
        public GroupManager GroupManager { get; }

        public static bool HasWorld(string name)
        {
            return _worldsLookup.ContainsKey(name);
        }

        public static World GetWorld(string name)
        {
            if (!HasWorld(name))
                throw new WorldDoesNotExistException(name);

            return _worldsLookup[name];
        }

        public static World CreateWorld(string name)
        {
            if (!ParallelRunner.IsMainThread)
                throw new WorldCreateOffThreadException(name);
            if (HasWorld(name))
                throw new WorldNameAlreadyExistException(name);

            var world = new World(name);
            _worldsLookup.Add(name, world);
            _worldsCache.IsDirty = true;

            return world;
        }

        public static void DestroyWorld(World world)
        {
            if (world == null)
                throw new ArgumentNullException();
            if (world.IsDestroyed)
                throw new WorldIsDestroyedException(world);
            if (!ParallelRunner.IsMainThread)
                throw new WorldDestroyOffThreadException(world);

            world.SelfDestroy();

            _worldsLookup.Remove(world.Name);
            _worldsCache.IsDirty = true;
        }

        public override string ToString()
        {
            return Name;
        }

        private static World[] UpdateWorldsCache()
        {
            return _worldsLookup.Values.ToArray();
        }

        private void SelfDestroy()
        {
            GroupManager.InternalDestroy();
            EntityManager.InternalDestroy();

            IsDestroyed = true;
        }
    }
}