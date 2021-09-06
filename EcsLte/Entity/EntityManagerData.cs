using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityManagerData
    {
        private const int _arrayInitSize = 4;

        public IComponent[][] ComponentPools = new IComponent[ComponentIndexes.Instance.Count][];
        public DataCache<Entity[], Entity[]> Entities = new DataCache<Entity[], Entity[]>(new Entity[_arrayInitSize], UpdateEntitiesCache);
        public Dictionary<string, EntityCommandPlayback> EntityCommandPlaybacks = new Dictionary<string, EntityCommandPlayback>();
        public Queue<Entity> ReuseableEntities = new Queue<Entity>();
        public Entity[] UniqueEntities = new Entity[ComponentIndexes.Instance.Count];
        public List<int>[] EntityComponentIndexes = new List<int>[_arrayInitSize];
        public int NextId;

        public EntityManagerData()
        {
            for (var i = 0; i < EntityComponentIndexes.Length; i++)
                EntityComponentIndexes[i] = new List<int>();
            for (var i = 0; i < ComponentPools.Length; i++)
                ComponentPools[i] = new IComponent[_arrayInitSize];
        }

        public void Reset()
        {
            Array.Clear(Entities.UncachedData, 0, Entities.UncachedData.Length);
            Entities.IsDirty = true;
            ReuseableEntities.Clear();
            Array.Clear(UniqueEntities, 0, UniqueEntities.Length);
            EntityCommandPlaybacks.Clear();
            NextId = 0;

            foreach (var entityIndexes in EntityComponentIndexes)
                entityIndexes.Clear();
            foreach (var pool in ComponentPools)
                Array.Clear(pool, 0, pool.Length);
        }

        private static Entity[] UpdateEntitiesCache(Entity[] uncachedData)
        {
            return uncachedData
                .Where(x => x != Entity.Null)
                .ToArray();
        }
    }
}