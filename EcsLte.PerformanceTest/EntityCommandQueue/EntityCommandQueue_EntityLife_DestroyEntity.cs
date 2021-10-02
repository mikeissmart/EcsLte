using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandQueue
{
    internal class EntityCommandQueue_EntityLife_DestroyEntity : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.DefaultCommand.DestroyEntity(_entities[i]);
            _context.DefaultCommand.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.DefaultCommand.DestroyEntity(_entities[i]); });
            _context.DefaultCommand.RunCommands();
        }
    }
}