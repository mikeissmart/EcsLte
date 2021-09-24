using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    /*
    Way to long
    internal class EcsContext_WithKey_SharedKeyes : BasePerformanceTest
    {
        private TestSharedKeyComponent1 _component1;
        private TestSharedKeyComponent2 _component2;

        public override void PreRun()
        {
            base.PreRun();

            _component1 = new TestSharedKeyComponent1 { Prop = 1 };
            _component2 = new TestSharedKeyComponent2 { Prop = 2 };
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < entities.Length; i++)
            {
                _context.AddComponent(entities[i], _component1);
                _context.AddComponent(entities[i], _component2);
            }
        }

        public override void Run()
        {
            EntityKey entityKey;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityKey = _context.WithKey(_component1, _component2);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            EntityKey entityKey;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityKey = _context.WithKey(_component1, _component2); });
        }
    }*/
}