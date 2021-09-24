using EcsLte;

/*public class SystemTest : SystemBase
{
    public bool CostructorCalled { get; set; }
    public int CleanupCalled { get; set; }
    public int ExecuteCalled { get; set; }
    public int InitializeCalled { get; set; }
    public int TearDownCalled { get; set; }

    public SystemTest()
    {
        CostructorCalled = true;
    }

    public override void Cleanup()
    {
        CleanupCalled++;
    }

    public override void Execute()
    {
        ExecuteCalled++;
    }

    public override void Initialize()
    {
        InitializeCalled++;
    }

    public override void TearDown()
    {
        TearDownCalled++;
    }
}

#region AutoAdd

[SystemAutoAdd]
public class TestAutoAddSystem1 : SystemTest
{
}

[SystemAutoAdd]
public class TestAutoAddSystem2 : SystemTest
{ }

#endregion

#region Sort

public class TestSortSystem1 : SystemTest
{ }

[AfterSystem(typeof(TestSortSystem1))]
[BeforeSystem(typeof(TestSortSystem3))]
public class TestSortSystem2 : SystemTest
{ }

public class TestSortSystem3 : SystemTest
{ }

#endregion

#region SortSelf

[AfterSystem(typeof(TestSortSelfSystem1))]
public class TestSortSelfSystem1 : SystemTest
{ }

#endregion

#region SortLoop

[BeforeSystem(typeof(TestSortLoopSystem2))]
public class TestSortLoopSystem1 : SystemTest
{ }

[BeforeSystem(typeof(TestSortLoopSystem1))]
public class TestSortLoopSystem2 : SystemTest
{ }

#endregion*/