namespace EcsLte.UnitTest
{
	public struct TestComponent1 : IComponent, IComponentRecordable { public int Prop { get; set; } }

	public struct TestComponent2 : IComponent { public int Prop { get; set; } }

	public struct TestComponent3 : IComponent, IComponentRecordable { public int Prop { get; set; } }

	public struct TestComponent4 : IComponent { public int Prop { get; set; } }

	public struct TestComponent5 : IComponent, IComponentRecordable { public int Prop { get; set; } }
}