using EcsLte;
using System;

public interface ITestComponent
{
    int Prop { get; set; }
}

public struct TestComponent1 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent2 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent3 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent4 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent5 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent6 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent7 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestComponent8 : IComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestManageComponent1 : IComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestManageComponent2 : IComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestRecordableComponent1 : IRecordableComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestRecordableComponent2 : IRecordableComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestManageRecordableComponent1 : IRecordableComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestManageRecordableComponent2 : IRecordableComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestUniqueComponent1 : IUniqueComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestUniqueComponent2 : IUniqueComponent, ITestComponent
{
    public int Prop { get; set; }
}

public struct TestManageUniqueComponent1 : IUniqueComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestManageUniqueComponent2 : IUniqueComponent, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }
}

public struct TestSharedComponent1 : ISharedComponent, IEquatable<TestSharedComponent1>, ITestComponent
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent1 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}

public struct TestSharedComponent2 : ISharedComponent, IEquatable<TestSharedComponent2>, ITestComponent
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent2 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}

public struct TestManageSharedComponent1 : ISharedComponent, IEquatable<TestManageSharedComponent1>, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }

    public bool Equals(TestManageSharedComponent1 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}

public struct TestManageSharedComponent2 : ISharedComponent, IEquatable<TestManageSharedComponent2>, ITestComponent
{
    public string PropString { get; set; }
    public int Prop { get; set; }

    public bool Equals(TestManageSharedComponent2 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}