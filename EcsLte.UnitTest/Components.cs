namespace EcsLte
{
	public struct TestComponent1 : IComponent { public int Prop { get; set; } }

	public struct TestComponent2 : IComponent { public int Prop { get; set; } }

	public struct TestrecordableComponent1 : IComponent, IComponentRecordable { public int Prop { get; set; } }

	public struct TestRecordableComponent2 : IComponent, IComponentRecordable { public int Prop { get; set; } }

	[SharedKey]
	public struct TestSharedKeyComponent1 : IComponent { public int Prop { get; set; } }

	[SharedKey]
	public struct TestSharedKeyComponent2 : IComponent { public int Prop { get; set; } }

	[PrimaryKey]
	public struct TestPrimaryKeyComponent1 : IComponent { public int Prop { get; set; } }

	[PrimaryKey]
	public struct TestPrimaryKeyComponent2 : IComponent { public int Prop { get; set; } }
}