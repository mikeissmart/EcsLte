using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_HasUniqueComponent : BasePerformanceTest
    {
        public override void Run()
        {
            bool hasComponent;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                hasComponent = _context.HasUniqueComponent<TestUniqueComponent1>();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasComponent;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasComponent = _context.HasUniqueComponent<TestUniqueComponent1>(); });
        }
    }
}