using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_GetUniqueComponent : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            _context.AddUniqueComponent(new TestComponentUnique1());
        }

        public override void Run()
        {
            TestComponentUnique1 component;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                component = _context.GetUniqueComponent<TestComponentUnique1>();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            TestComponentUnique1 component;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { component = _context.GetUniqueComponent<TestComponentUnique1>(); });
        }
    }
}