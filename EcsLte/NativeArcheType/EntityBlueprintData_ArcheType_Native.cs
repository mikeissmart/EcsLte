using EcsLte.Utilities;

namespace EcsLte.NativeArcheType
{
    internal class EntityBlueprintData_ArcheType_Native
    {
        public ComponentConfig[] Configs { get; set; }
        public ComponentConfig[] UniqueConfigs { get; set; }
        public IEntityBlueprintComponentData_ArcheType_Native[] Components { get; set; }
        public IEntityBlueprintComponentData_ArcheType_Native[] UniqueComponents { get; set; }
        public int ComponentsLengthInBytes { get; set; }
        public int UniqueComponentsLengthInBytes { get; set; }

        public unsafe byte* CreateComponentsBuffer()
        {
            var components = (byte*)MemoryHelper.Alloc(ComponentsLengthInBytes);
            for (int i = 0, offset = 0; i < Components.Length; i++)
            {
                var config = Configs[i];
                MemoryHelper.Copy(
                    Components[i].GetData(),
                    components + offset,
                    config.UnmanagedSizeInBytes);
                offset += config.UnmanagedSizeInBytes;
            }

            return components;
        }
    }
}
