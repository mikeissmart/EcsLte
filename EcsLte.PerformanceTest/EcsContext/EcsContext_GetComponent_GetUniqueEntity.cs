using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_GetUniqueEntity : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            _context.AddUniqueComponent(new TestUniqueComponent1());
        }

        public override void Run()
        {
            Entity entity;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entity = _context.GetUniqueEntity<TestUniqueComponent1>();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            Entity entity;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entity = _context.GetUniqueEntity<TestUniqueComponent1>(); });
        }
    }
}