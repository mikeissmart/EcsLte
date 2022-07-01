namespace EcsLte
{
    internal class EntityQueryEcsContextData
    {
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        internal ArcheTypeData[] ArcheTypeDatas { get; set; } = new ArcheTypeData[0];

        internal int? ArcheTypeChangeVersion { get; set; }
        internal int EntityQueryDataIndex { get; set; }
    }
}