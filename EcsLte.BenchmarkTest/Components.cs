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

public struct TestSharedComponent3 : ISharedComponent, IEquatable<TestSharedComponent3>, ITestComponent
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent3 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}

public struct TestSharedComponent4 : ISharedComponent, IEquatable<TestSharedComponent4>, ITestComponent
{
    public int Prop { get; set; }

    public bool Equals(TestSharedComponent4 other)
        => Prop == other.Prop;

    public override int GetHashCode() => Prop.GetHashCode();
}