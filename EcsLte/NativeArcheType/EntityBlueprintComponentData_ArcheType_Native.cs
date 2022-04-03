namespace EcsLte.NativeArcheType
{
    internal interface IEntityBlueprintComponentData_ArcheType_Native
    {
        unsafe void* GetData();
        IComponent GetComponent();
    }

    public class EntityBlueprintComponentData_ArcheType_Native<TComponent> : IEntityBlueprintComponentData_ArcheType_Native where TComponent : unmanaged, IComponent
    {
        public TComponent Component;

        public unsafe void* GetData()
        {
            fixed (void* ptr = &Component)
            {
                return ptr;
            }
        }

        public IComponent GetComponent() => Component;
    }
}
