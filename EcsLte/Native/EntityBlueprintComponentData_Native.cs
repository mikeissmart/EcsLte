namespace EcsLte.Native
{
    internal interface IEntityBlueprintComponentData_Native
    {
        unsafe void* GetData();
    }

    public class EntityBlueprintComponentData_Native<TComponent> : IEntityBlueprintComponentData_Native where TComponent : unmanaged, IComponent
    {
        public TComponent Component;

        public unsafe void* GetData()
        {
            fixed (void* ptr = &Component)
            {
                return ptr;
            }
        }
    }
}
