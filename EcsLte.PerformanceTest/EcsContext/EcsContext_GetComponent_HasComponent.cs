using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_HasComponent : BasePerformanceTest
    {
        private Entity _entity;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
            _context.AddComponent(_entity, new TestComponent1());
        }

        public override void Run()
        {
            bool hasComponent;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                hasComponent = _context.HasComponent<TestComponent1>(_entity);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasComponent;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasComponent = _context.HasComponent<TestComponent1>(_entity); });
        }
    }
}