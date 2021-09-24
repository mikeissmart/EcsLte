using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    /*
    Way to long
    internal class EcsContext_WithKey_PrimaryKey : BasePerformanceTest
    {
        private TestPrimaryKeyComponent1 _component;
        private Entity _entity;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestPrimaryKeyComponent1 { Prop = 1 };
            _entity = _context.CreateEntity();
            _context.AddComponent(_entity, _component);
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
    }*/
}