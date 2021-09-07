using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    /*internal class EntityTable
    {
        private const int _arrayInitSize = 4;

        private IComponent[][] _componentPools;
        private List<EntityCollection> _entityCollections;
        private Entity[] _uniqueEntities;
        private List<int>[] _entityComponentIndexes;
        private int _nextId;
        private int _arrayCurrentSize;

        internal EntityCollection Entities { get; private set; }

        internal EntityTable()
        {
            _componentPools = new IComponent[ComponentIndexes.Instance.Count][];
            _entityCollections = new List<EntityCollection>();
            _uniqueEntities = new Entity[ComponentIndexes.Instance.Count];
            _entityComponentIndexes = new List<int>[_arrayInitSize];
            _nextId = 1;
            _arrayCurrentSize = _arrayInitSize;

            for (var i = 0; i < _entityComponentIndexes.Length; i++)
                _entityComponentIndexes[i] = new List<int>();
            for (int i = 0; i < _componentPools.Length; i++)
                _componentPools[i] = new IComponent[_arrayInitSize];
            Entities = CreateCollection();
        }

        internal EntityCollection CreateCollection()
        {
            if (!ObjectCache.Pop<EntityCollection>(out var entityCollection))
                entityCollection = new EntityCollection(_arrayCurrentSize);
            else
                entityCollection.Resize(_arrayCurrentSize);

            lock (_entityCollections)
            {
                _entityCollections.Add(entityCollection);
            }

            return entityCollection;
        }

        internal void RemoveCollection(EntityCollection entityCollection)
        {
            lock (_entityCollections)
            {
                _entityCollections.Remove(entityCollection);
                ObjectCache.Push(entityCollection);
            }
        }

        internal void Reset()
        {
            foreach (var pool in _componentPools)
                Array.Clear(pool, 0, pool.Length);
            foreach (var entityCollection in _entityCollections)
            {
                entityCollection.Reset();
                ObjectCache.Push(entityCollection);
            }
            Array.Clear(_uniqueEntities, 0, _uniqueEntities.Length);
            foreach (var entityIndexes in _entityComponentIndexes)
                entityIndexes.Clear();
            _nextId = 1;
        }
    }*/
}