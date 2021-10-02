using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_DestroyEntity : BasePerformanceTest
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
                _context.DestroyEntity(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.DestroyEntity(_entities[i]); });
        }
    }
}