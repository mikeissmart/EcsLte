using EcsLte;

public struct TestComponent1 : IComponent
{
    public int Prop { get; set; }
}

public struct TestComponent2 : IComponent
{
    public int Prop { get; set; }
}

public struct TestRecordableComponent1 : IComponentRecordable
{
    public int Prop { get; set; }
}

public struct TestRecordableComponent2 : IComponentRecordable
{
    public int Prop { get; set; }
}

public struct TestComponentUnique1 : IComponentUnique
{
    public int Prop { get; set; }
}

public struct TestComponentUnique2 : IComponentUnique
{
    public int Prop { get; set; }
}

public struct TestSharedKeyComponent1 : IComponentSharedKey
{
    public int Prop { get; set; }
}

public struct TestSharedKeyComponent2 : IComponentSharedKey
{
    public int Prop { get; set; }
}

public struct TestPrimaryKeyComponent1 : IComponentPrimaryKey
{
    public int Prop { get; set; }
}

public struct TestPrimaryKeyComponent2 : IComponentPrimaryKey
{
    public int Prop { get; set; }
}