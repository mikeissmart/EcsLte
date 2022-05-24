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

    internal class ManagedComponentPools
    {
        private IManagedComponentPool[] _managePools;

        internal ManagedComponentPools()
        {
            _managePools = new IManagedComponentPool[ComponentConfigs.Instance.AllManagedCount];
            var poolType = typeof(ManagedComponentPool<>);
            for (var i = 0; i < _managePools.Length; i++)
            {
                _managePools[i] = (IManagedComponentPool)Activator
                    .CreateInstance(poolType
                        .MakeGenericType(ComponentConfigs.Instance.AllManagedTypes[i]));
            }
        }

        internal IManagedComponentPool GetPool(ComponentConfig config) => _managePools[config.ManagedIndex];

        internal ManagedComponentPool<TComponent> GetPool<TComponent>()
            where TComponent : IComponent =>
            (ManagedComponentPool<TComponent>)_managePools[ComponentConfig<TComponent>.Config.ManagedIndex];
    }

    internal class ManagedComponentPool<TComponent> : IManagedComponentPool
        where TComponent : IComponent
    {
        private TComponent[] _components;
        private readonly Stack<int> _reusableComponents;
        private int _nextIndex;

        public ManagedComponentPool()
        {
            _components = new TComponent[1];
            _reusableComponents = new Stack<int>();
            _reusableComponents.Push(0);
            _nextIndex = 1;
        }

        public int AllocateComponent()
        {
            CheckCapacity(1);
            if (_reusableComponents.Count > 0)
                return _reusableComponents.Pop();

            return _nextIndex++;
        }

        public int[] AllocateComponents(int count)
        {
            CheckCapacity(count);
            var indexes = new int[count];
            for (var i = 0; i < count; i++)
            {
                if (_reusableComponents.Count > 0)
                    indexes[i] = _reusableComponents.Pop();
                else
                    indexes[i] = _nextIndex++;
            }

            return indexes;
        }

        public TComponent GetComponent(int index) => _components[index];

        public ref TComponent GetComponentRef(int index) => ref _components[index];

        IComponent IManagedComponentPool.GetComponent(int index) => _components[index];

        public void SetComponent(int index, TComponent component) => _components[index] = component;

        void IManagedComponentPool.SetComponent(int index, IComponent component) => _components[index] = (TComponent)component;

        public void ClearComponent(int index)
        {
            _components[index] = default;
            _reusableComponents.Push(index);
        }

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
