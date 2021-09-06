using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class World
    {
        private static readonly DataCache<Dictionary<string, World>, World[]> _worlds =
            new DataCache<Dictionary<string, World>, World[]>(new Dictionary<string, World>(), UpdateWorldsCache);

        private World(string name)
        {
            Name = name;
            EntityManager = new EntityManager(this);
            GroupManager = new GroupManager(this);
        }

        public static World[] Worlds => _worlds.CachedData;
        public static World DefaultWorld { get; set; } = CreateWorld("Default");

        public string Name { get; }
        public bool IsDestroyed { get; private set; }
        public EntityManager EntityManager { get; }
        public GroupManager GroupManager { get; }

        public static bool HasWorld(string name)
        {
            return _worlds.UncachedData.ContainsKey(name);
        }

        public static World GetWorld(string name)
        {
            if (!HasWorld(name))
                throw new WorldDoesNotExistException(name);

            return _worlds.UncachedData[name];
        }

        public static World CreateWorld(string name)
        {
            if (!ParallelRunner.IsMainThread)
                throw new WorldCreateOffThreadException(name);
            if (HasWorld(name))
                throw new WorldNameAlreadyExistException(name);

            var world = new World(name);
            _worlds.UncachedData.Add(name, world);
            _worlds.IsDirty = true;

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

            _worlds.UncachedData.Remove(world.Name);
            _worlds.IsDirty = true;
        }

        public override string ToString()
        {
            return Name;
        }

        private static World[] UpdateWorldsCache(Dictionary<string, World> uncached)
        {
            return uncached.Values.ToArray();
        }

        private void SelfDestroy()
        {
            GroupManager.InternalDestroy();
            EntityManager.InternalDestroy();

            IsDestroyed = true;
        }
    }
}