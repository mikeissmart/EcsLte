using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityGroup_SharedComponent : BasePerformanceTest
    {
        public override void Run()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityGroup = _context.GroupWith(component); });
        }
    }
}