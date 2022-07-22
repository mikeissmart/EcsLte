using System;

namespace EcsLte
{
    internal interface IComponentData : IComparable<IComponentData>, IEquatable<IComponentData>
    {
        ComponentConfig Config { get; }
        IComponent Component { get; }

        unsafe void CopyComponentData(byte* componentPtr);

        bool ComponentEquals<TComponentEqual>(TComponentEqual component) where TComponentEqual : unmanaged, IComponent;

        unsafe bool ComponentEquals(byte* sharedComponentPtr);

        SharedComponentDataIndex GetSharedComponentDataIndex(SharedComponentIndexDictionaries sharedIndexDics);
    }

    internal class ComponentData<TComponent> : IComponentData
        where TComponent : unmanaged, IComponent
    {
        private readonly TComponent _component;

        public ComponentConfig Config { get; private set; }
        public IComponent Component => _component;

        internal ComponentData(TComponent component)
        {
            _component = component;
            Config = ComponentConfig<TComponent>.Config;
        }

        public unsafe void CopyComponentData(byte* componentPtr) => *(TComponent*)componentPtr = _component;

        public bool ComponentEquals<TComponentEqual>(TComponentEqual component)
             where TComponentEqual : unmanaged, IComponent => Config == ComponentConfig<TComponentEqual>.Config &&
                _component.Equals(component);

        public unsafe bool ComponentEquals(byte* sharedComponentPtr)
        {
            var checkComponent = *(TComponent*)sharedComponentPtr;
            return _component.GetHashCode() == checkComponent.GetHashCode() &&
                _component.Equals(checkComponent);
        }

        public SharedComponentDataIndex GetSharedComponentDataIndex(SharedComponentIndexDictionaries sharedIndexDics) =>
            sharedIndexDics.GetDataIndex(_component);

        public int CompareTo(IComponentData other)
            => Config.CompareTo(other.Config);

        public bool Equals(IComponentData other)
            => Config.Equals(other.Config) &&
                _component.Equals((TComponent)other.Component);
    }
}