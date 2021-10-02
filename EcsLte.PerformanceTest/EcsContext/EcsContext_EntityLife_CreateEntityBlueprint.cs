using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntityBlueprint : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.CreateEntity(_blueprint);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.CreateEntity(_blueprint); });
        }
    }
}