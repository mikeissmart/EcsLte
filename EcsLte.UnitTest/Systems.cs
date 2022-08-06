using EcsLte;
using System.Collections.Generic;

/*public class SystemOrder
{
    public List<SystemBase> Order { get; set; } = new List<SystemBase>();
}

public class System_A : SystemBase,
    IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int InitializeCalledCount { get; private set; }
    public int ExecuteCalledCount { get; private set; }
    public int CleanupCalledCount { get; private set; }
    public int TearDownCalledCount { get; private set; }

    public System_A(EcsContext context) : base(context)
    {
    }

    public void Initialize()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        InitializeCalledCount++;
    }

    public void Execute()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        ExecuteCalledCount++;
    }

    public void Cleanup()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        CleanupCalledCount++;
    }

    public void TearDown()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        TearDownCalledCount++;
    }
}

[AfterSystem(typeof(System_A))]
[BeforeSystem(typeof(System_C))]
public class System_B : SystemBase,
    IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int InitializeCalledCount { get; private set; }
    public int ExecuteCalledCount { get; private set; }
    public int CleanupCalledCount { get; private set; }
    public int TearDownCalledCount { get; private set; }

    public System_B(EcsContext context) : base(context)
    {
    }

    public void Initialize()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        InitializeCalledCount++;
    }

    public void Execute()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        ExecuteCalledCount++;
    }

    public void Cleanup()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        CleanupCalledCount++;
    }

    public void TearDown()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        TearDownCalledCount++;
    }
}

public class System_C : SystemBase,
    IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int InitializeCalledCount { get; private set; }
    public int ExecuteCalledCount { get; private set; }
    public int CleanupCalledCount { get; private set; }
    public int TearDownCalledCount { get; private set; }

    public System_C(EcsContext context) : base(context)
    {
    }

    public void Initialize()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        InitializeCalledCount++;
    }

    public void Execute()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        ExecuteCalledCount++;
    }

    public void Cleanup()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        CleanupCalledCount++;
    }

    public void TearDown()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        TearDownCalledCount++;
    }
}

[DontAutoAddSystem]
public class System_Initialize : SystemBase, IInitializeSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int InitializeCalledCount { get; private set; }

    public System_Initialize(EcsContext context) : base(context)
    {
    }

    public void Initialize()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        InitializeCalledCount++;
    }
}

[DontAutoAddSystem]
public class System_Execute : SystemBase, IExecuteSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int ExecuteCalledCount { get; private set; }

    public System_Execute(EcsContext context) : base(context)
    {
    }

    public void Execute()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        ExecuteCalledCount++;
    }
}

[DontAutoAddSystem]
public class System_Cleanup : SystemBase, ICleanupSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int CleanupCalledCount { get; private set; }

    public System_Cleanup(EcsContext context) : base(context)
    {
    }

    public void Cleanup()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        CleanupCalledCount++;
    }
}

[DontAutoAddSystem]
public class System_TearDown : SystemBase, ITearDownSystem
{
    public SystemOrder SystemOrder { get; set; }
    public int TearDownCalledCount { get; private set; }

    public System_TearDown(EcsContext context) : base(context)
    {
    }

    public void TearDown()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        TearDownCalledCount++;
    }
}*/