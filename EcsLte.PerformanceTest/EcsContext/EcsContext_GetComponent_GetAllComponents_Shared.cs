using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetComponent_GetAllComponents_Shared : BasePerformanceTest
    {
        private Entity _entity;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
            _context.AddComponent(_entity, new TestSharedComponent1());
        }

        public override void Run()
        {
            IComponent[] components;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                components = _context.GetAllComponents(_entity);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            IComponent[] components;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { components = _context.GetAllComponents(_entity); });
        }
    }
}