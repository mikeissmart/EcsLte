namespace EcsLte
{
    internal struct EntityData
    {
        public static readonly EntityData Null = new EntityData();

        internal ArcheTypeIndex ArcheTypeIndex;
        internal int ChunkIndex;
        internal int EntityIndex;
    }
}
