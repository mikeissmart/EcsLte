using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityKey_SharedKey : BasePerformanceTest
    {
        private TestSharedKeyComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestSharedKeyComponent1 { Prop = 1 };
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < entities.Length; i++)
                _context.AddComponent(entities[i], _component);
        }

        public override void Run()
        {
            EntityKey entityKey;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityKey = _context.WithKey(_component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            EntityKey entityKey;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityKey = _context.WithKey(_component); });
        }
    }
}