using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_GetComponent : BasePerformanceTest
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
            TestComponent1 component;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                component = _context.GetComponent<TestComponent1>(_entity);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            TestComponent1 component;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { component = _context.GetComponent<TestComponent1>(_entity); });
        }
    }
}