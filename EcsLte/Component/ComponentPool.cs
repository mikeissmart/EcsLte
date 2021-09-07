using System;

namespace EcsLte
{
    internal interface IComponentPool
    {
        bool HasComponent(int entityId);
        IComponent GetComponent(int entityid);
        void AddComponent(int entityId, IComponent component);
        void RemoveComponent(int entityId);
        void ReplaceComponent(int entityId, IComponent component);
        void Resize(int newSize);
        void Clear();
    }

    internal class ComponentPool<TComponent> : IComponentPool
        where TComponent : IComponent
    {
        private TComponent[] _components;
        private bool[] _hasComponents;

        public ComponentPool(int initialSize)
        {
            _components = new TComponent[initialSize];
            _hasComponents = new bool[initialSize];
        }

        public bool HasComponent(int entityId)
        {
            return _hasComponents[entityId];
        }

        public IComponent GetComponent(int entityId)
        {
            return _components[entityId];
        }

        public void AddComponent(int entityId, IComponent component)
        {
            _components[entityId] = (TComponent)component;
            _hasComponents[entityId] = true;
        }

        public void RemoveComponent(int entityId)
        {
            _hasComponents[entityId] = false;
        }

        public void ReplaceComponent(int entityId, IComponent component)
        {
            _components[entityId] = (TComponent)component;
        }

        public void Resize(int newSize)
        {
            if (newSize > _components.Length)
            {
                Array.Resize(ref _components, newSize);
                Array.Resize(ref _hasComponents, newSize);
            }
        }

        public void Clear()
        {
            Array.Clear(_components, 0, _components.Length);
            Array.Clear(_hasComponents, 0, _hasComponents.Length);
        }
    }
}