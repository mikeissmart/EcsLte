using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetEntity_HasEntity : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            bool hasEntity;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                hasEntity = _context.HasEntity(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasEntity;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasEntity = _context.HasEntity(_entities[i]); });
        }
    }
}