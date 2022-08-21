using EcsLte;
using System.Collections.Generic;

public class SystemOrder
{
    public List<SystemBase> Order { get; set; } = new List<SystemBase>();
}

public abstract class SystemTest : SystemBase
{
    public SystemOrder SystemOrder { get; set; }
    public int ActivatedCalledCount { get; set; }
    public int DeactivatedCalledCount { get; set; }
    public int InitializeCalledCount { get; set; }
    public int UpdateCalledCount { get; set; }
    public int UninitializeCalledCount { get; set; }

    public override void Activated() => ActivatedCalledCount++;

    public override void Deactivated() => DeactivatedCalledCount++;

    public override void Initialize() => InitializeCalledCount++;

    public override void Update()
    {
        if (SystemOrder != null)
            SystemOrder.Order.Add(this);
        UpdateCalledCount++;
    }

    public override void Uninitialize() => UninitializeCalledCount++;
}

public class System_A : SystemTest
{
}

[AfterSystem(typeof(System_A))]
[BeforeSystem(typeof(System_C))]
public class System_B : SystemTest
{
}

public class System_C : SystemTest
{
}

[SystemNotAutoAdd]
public class System_NotAutoAdd : SystemTest
{
}