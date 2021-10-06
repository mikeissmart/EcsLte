using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_AddSharedComponentX3 : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component1 = new TestSharedComponent1();
            var component2 = new TestSharedComponent2();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.AddComponent(_entities[i], component1);
                _context.AddComponent(_entities[i], component2);
            }
        }

        public override void Run()
        {
            var component = new TestSharedComponent3();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestSharedComponent3();
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.AddComponent(_entities[i], component); });
        }
    }
}