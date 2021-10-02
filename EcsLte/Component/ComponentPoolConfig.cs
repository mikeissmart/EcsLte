namespace EcsLte
{
    internal struct ComponentPoolConfig
    {
        internal int Index { get; set; }
        internal bool IsRecordable { get; set; }
        internal bool IsUnique { get; set; }
        internal bool IsShared { get; set; }
    }
}