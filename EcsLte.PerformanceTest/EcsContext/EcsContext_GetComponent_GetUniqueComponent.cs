using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_GetUniqueComponent : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            _context.AddUniqueComponent(new TestUniqueComponent1());
        }

        public override void Run()
        {
            TestUniqueComponent1 component;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                component = _context.GetUniqueComponent<TestUniqueComponent1>();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            TestUniqueComponent1 component;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { component = _context.GetUniqueComponent<TestUniqueComponent1>(); });
        }
    }
}