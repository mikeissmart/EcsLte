namespace EcsLte.HybridArcheType
{
    internal interface IEntityBlueprintComponentData
    {
        ComponentConfig Config { get; }
        IComponent Component { get; }

        unsafe void CopyComponentData(byte* componentPtr);
    }

    internal class EntityBlueprintComponentData<TComponent> : IEntityBlueprintComponentData
        where TComponent : unmanaged, IComponent
    {
        private readonly TComponent _component;

        public ComponentConfig Config { get; private set; }
        public IComponent Component => _component;

        public EntityBlueprintComponentData(TComponent component, ComponentConfig config)
        {
            _component = component;
            Config = config;
        }

        public unsafe void CopyComponentData(byte* componentPtr) => *(TComponent*)componentPtr = _component;
    }
}
