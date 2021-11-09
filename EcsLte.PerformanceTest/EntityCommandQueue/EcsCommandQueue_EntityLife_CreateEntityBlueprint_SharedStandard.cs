using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_EntityLife_CreateEntityBlueprint_SharedStandard : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestSharedComponent1())
                .AddComponent(new TestStandardComponent1());
        }

        public override void Run()
        {
            Entity entity;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entity = _context.DefaultCommand.CreateEntity(_blueprint);
            _context.DefaultCommand.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            Entity entity;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entity = _context.DefaultCommand.CreateEntity(_blueprint); });
            _context.DefaultCommand.RunCommands();
        }
    }
}