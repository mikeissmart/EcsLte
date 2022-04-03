using System;

namespace EcsLte.ManagedArcheType
{
    public interface IComponentPool_ArcheType_Managed
    {
        IComponent GetComponent(int index);
        void SetComponent(int index, IComponent component);
        void MoveComponent(int sourceIndex, int destinationIndex);
        void Resize(int newCapacity);
    }

    public class ComponentPool_ArcheType_Managed<TComponent> : IComponentPool_ArcheType_Managed
        where TComponent : IComponent
    {
        private TComponent[] _components;

        public ComponentPool_ArcheType_Managed(int initCapacity) => _components = new TComponent[initCapacity];

        public IComponent GetComponent(int index) => _components[index];

        public void SetComponent(int index, IComponent component) => _components[index] = (TComponent)component;

        public void MoveComponent(int sourceIndex, int destinationIndex) => _components[destinationIndex] = _components[sourceIndex];

        public void Resize(int newCapacity) => Array.Resize(ref _components, newCapacity);
    }
}
