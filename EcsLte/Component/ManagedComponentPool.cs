using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal interface IManagedComponentPool
    {
        IComponent GetComponent(int index);
        int AllocateComponent();
        int[] AllocateComponents(int count);
        void SetComponent(int index, IComponent component);
        void ClearComponent(int index);
        void Clear();
    }

    internal static class ManagedComponentPool
    {
        internal static IManagedComponentPool[] CreateManagedComponentPools()
        {
            var managedPools = new IManagedComponentPool[ComponentConfigs.Instance.AllManagedCount];
            var poolType = typeof(ManagedComponentPool<>);
            for (var i = 0; i < managedPools.Length; i++)
            {
                managedPools[i] = (IManagedComponentPool)Activator
                    .CreateInstance(poolType
                        .MakeGenericType(ComponentConfigs.Instance.AllManagedTypes[i]));
            }

            return managedPools;
        }
    }

    internal class ManagedComponentPool<TComponent> : IManagedComponentPool
        where TComponent : IComponent
    {
        private TComponent[] _components;
        private readonly Stack<int> _reusableComponents;
        private int _nextIndex;

        internal ManagedComponentPool()
        {
            _components = new TComponent[1];
            _reusableComponents = new Stack<int>();
            _reusableComponents.Push(0);
            _nextIndex = 1;
        }

        public IComponent GetComponent(int index) => _components[index];

        public int AllocateComponent()
        {
            CheckCapacity(1);
            var index = 0;
            if (_reusableComponents.Count > 0)
                index = _reusableComponents.Pop();
            else
                index = _nextIndex++;

            return index;
        }

        public int[] AllocateComponents(int count)
        {
            CheckCapacity(count);
            var indexes = new int[count];
            for (var i = 0; i < count; i++)
            {
                var index = 0;
                if (_reusableComponents.Count > 0)
                    index = _reusableComponents.Pop();
                else
                    index = _nextIndex++;
                indexes[i] = index;
            }

            return indexes;
        }

        public void SetComponent(int index, IComponent component) => _components[index] = (TComponent)component;

        public void SetComponent(int index, TComponent component) => _components[index] = component;

        public void ClearComponent(int index) => _reusableComponents.Push(index);

        private void CheckCapacity(int count)
        {
            var unusedCount = _components.Length - (_nextIndex - _reusableComponents.Count);
            if (unusedCount < count)
            {
                var newCapacity = (int)Math.Pow(2, (int)Math.Log(_components.Length + count, 2) + 1);
                Array.Resize(ref _components, newCapacity);
            }
        }

        public void Clear()
        {
            Array.Clear(_components, 0, _components.Length);
            _reusableComponents.Clear();
            _nextIndex = 0;
        }
    }
}
