using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_EntityLife_CreateEntity : BasePerformanceTest
    {
        public override void Run()
        {
            Entity entity;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entity = _context.DefaultCommand.CreateEntity();
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
                i => { entity = _context.DefaultCommand.CreateEntity(); });
            _context.DefaultCommand.RunCommands();
        }
    }
}