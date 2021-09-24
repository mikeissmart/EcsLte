using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_GetEntity_HasEntity : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestComponent1();
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < _entities.Length; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            bool hasEntity;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
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