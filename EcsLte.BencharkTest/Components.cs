using EcsLte;
using System;

public unsafe struct TestComponent1 : IComponent
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

public struct TestSharedComponent1 : ISharedComponent, IEquatable<TestSharedComponent1>
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent1 other)
        => Prop == other.Prop;
    public override int GetHashCode() => -1159918376 + Prop.GetHashCode();
}

public struct TestSharedComponent2 : ISharedComponent, IEquatable<TestSharedComponent2>
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent2 other)
        => Prop == other.Prop;
    public override int GetHashCode() => -1159918376 + Prop.GetHashCode();
}