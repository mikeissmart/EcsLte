using EcsLte;

public struct TestComponent1 : IComponent
{
    public int Prop { get; set; }
}

public struct TestComponent2 : IComponent
{
    public int Prop { get; set; }
}

public struct TestRecordableComponent1 : IRecordableComponent
{
    public int Prop { get; set; }
}

public struct TestRecordableComponent2 : IRecordableComponent
{
    public int Prop { get; set; }
}

public struct TestComponentUnique1 : IUniqueComponent
{
    public int Prop { get; set; }
}

public struct TestComponentUnique2 : IUniqueComponent
{
    public int Prop { get; set; }
}

public struct TestSharedKeyComponent1 : ISharedComponent
{
    public int Prop { get; set; }
}

public struct TestSharedKeyComponent2 : ISharedComponent
{
    public int Prop { get; set; }
}