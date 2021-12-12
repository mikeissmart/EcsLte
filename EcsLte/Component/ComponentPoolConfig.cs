namespace EcsLte
{
	internal struct ComponentPoolConfig
	{
		internal int PoolIndex { get; set; }
		internal int RecordableIndex { get; set; }
		internal int UniqueIndex { get; set; }
		internal int SharedIndex { get; set; }
		internal bool IsRecordable { get; set; }
		internal bool IsUnique { get; set; }
		internal bool IsShared { get; set; }
	}
}