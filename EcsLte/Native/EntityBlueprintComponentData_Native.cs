namespace EcsLte.Native
{
    internal interface IEntityBlueprintComponentData_Native
    {
        unsafe void CopyToBuffer(void* buffer);
    }

    public class EntityBlueprintComponentData_Native<TComponent> : IEntityBlueprintComponentData_Native where TComponent : unmanaged, IComponent
    {
        public TComponent Component;

        public unsafe void CopyToBuffer(void* buffer) => *(TComponent*)buffer = Component;
    }
}
