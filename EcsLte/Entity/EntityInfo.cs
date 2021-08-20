using System;
using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityInfo
    {
        private static readonly List<IComponent> _componentsBuffer = new List<IComponent>();
        private readonly IComponent[] _components;

        private readonly DataCache<IComponent[]> _componentsCache;

        public EntityInfo()
        {
            _components = new IComponent[ComponentIndexes.Instance.Count];
            _componentsCache = new DataCache<IComponent[]>(UpdateComponentsCache);
        }

        public int Id { get; set; }
        public int Version { get; set; }

        public IComponent this[int componentIndex]
        {
            get => _components[componentIndex];
            set
            {
                _components[componentIndex] = value;
                _componentsCache.IsDirty = true;
            }
        }

        public IComponent[] GetComponents()
        {
            return _componentsCache.Data;
        }

        public void ClearComponents()
        {
            Array.Clear(_components, 0, _components.Length);
            _componentsCache.IsDirty = true;
        }

        public void Reset()
        {
            ClearComponents();
        }

        private IComponent[] UpdateComponentsCache()
        {
            var components = new List<IComponent>();
            for (var i = 0; i < _components.Length; i++)
                if (_components[i] != null)
                    _componentsBuffer.Add(_components[i]);

            return components.ToArray();
        }
    }
}