using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_AddComponent_Standard : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            var component = new TestStandardComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestStandardComponent1();
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.AddComponent(_entities[i], component); });
        }
    }
}