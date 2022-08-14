using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal interface IComponentPool
    {
        int Length { get; }

        IComponent GetComponent(int index);
        void SetComponent(int index, IComponent component);

        void CopySameArray(int srcIndex, int destIndex, int count);
        void CopyFrom(IComponentPool srcPool, int srcStartingIndex, int destStartingIndex, int count);
        void Resize(int newSize);
        void Clear(int index);
        void ClearRange(int startingIndex, int count);
        void ClearAll(int index);
    }

    internal class ComponentPool<TComponent> : IComponentPool
        where TComponent : IManagedComponent
    {
        private TComponent[] _components;

        public ComponentPool()
        {
            _components = new TComponent[1];
        }

        public int Length { get => _components.Length; }

        IComponent IComponentPool.GetComponent(int index)
            => GetComponent(index);

        public TComponent GetComponent(int index)
        {
            return _components[index];
        }

        public ref TComponent GetComponentRef(int index)
        {
            return ref _components[index];
        }

        public void GetComponents(ref TComponent[] components, int startingIndex)
        {
            Array.Copy(_components, 0, components, startingIndex, _components.Length);
        }

        void IComponentPool.SetComponent(int index, IComponent component)
            => SetComponent(index, (TComponent)component);

        public void SetComponent(int index, in TComponent component)
        {
            _components[index] = component;
        }

        public void SetComponents(int index, int count, in TComponent component)
        {
            for (var i = 0; i < count; i++, index++)
                _components[index] = component;
        }

        public void CopySameArray(int srcIndex, int destIndex, int count)
        {
            Array.Copy(_components, srcIndex, _components, destIndex, count);
        }

        void IComponentPool.CopyFrom(IComponentPool srcPool, int srcStartingIndex, int destStartingIndex, int count)
            => CopyFrom((ComponentPool<TComponent>)srcPool, srcStartingIndex, destStartingIndex, count);

        public void CopyFrom(ComponentPool<TComponent> srcPool, int srcStartingIndex, int destStartingIndex, int count)
        {
            Array.Copy(srcPool._components, srcStartingIndex,
                _components, destStartingIndex,
                count);
        }

        public void Resize(int newSize)
        {
            Array.Resize(ref _components, newSize);
        }

        public void Clear(int index)
        {
            _components[index] = default;
        }

        public void ClearRange(int startingIndex, int count)
        {
            Array.Clear(_components, startingIndex, count);
        }

        public void ClearAll(int index)
        {
            Array.Clear(_components, 0, _components.Length);
        }
    }
}
