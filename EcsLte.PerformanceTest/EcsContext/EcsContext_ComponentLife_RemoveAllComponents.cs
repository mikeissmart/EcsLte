using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_RemoveAllComponents : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestComponent1();
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.RemoveAllComponents(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.RemoveAllComponents(_entities[i]); });
        }
    }
}