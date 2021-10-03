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

public struct TestUniqueComponent1 : IUniqueComponent
{
    public int Prop { get; set; }
}

public struct TestUniqueComponent2 : IUniqueComponent
{
    public int Prop { get; set; }
}

public struct TestSharedComponent1 : ISharedComponent
{
    public int Prop { get; set; }
}

public struct TestSharedComponent2 : ISharedComponent
{
    public int Prop { get; set; }
}