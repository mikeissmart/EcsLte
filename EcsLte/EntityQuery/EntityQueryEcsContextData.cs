using EcsLte.Data;

namespace EcsLte
{
    internal class EntityQueryEcsContextData
    {
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        internal PtrWrapper[] ArcheTypeDatas { get; set; } = new PtrWrapper[0];

        internal int? ArcheTypeChangeVersion { get; set; }
        internal int EntityQueryDataIndex { get; set; }
    }
}