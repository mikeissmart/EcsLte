using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_HasComponent_Standard : BasePerformanceTest
    {
        private Entity _entity;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
            _context.AddComponent(_entity, new TestStandardComponent1());
        }

        public override void Run()
        {
            bool hasComponent;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                hasComponent = _context.HasComponent<TestStandardComponent1>(_entity);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasComponent;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasComponent = _context.HasComponent<TestStandardComponent1>(_entity); });
        }
    }
}