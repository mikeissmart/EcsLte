namespace EcsLte
{
    internal unsafe struct EntityData
    {
        internal ArcheTypeIndex ArcheTypeIndex;
        internal byte* ComponentsBuffer;
        internal int EntityIndex;
    }
}