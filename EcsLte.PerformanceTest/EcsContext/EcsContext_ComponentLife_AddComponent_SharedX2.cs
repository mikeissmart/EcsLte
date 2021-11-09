using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_AddComponent_SharedX2 : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestSharedComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            var component = new TestSharedComponent2();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestSharedComponent2();
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.AddComponent(_entities[i], component); });
        }
    }
}