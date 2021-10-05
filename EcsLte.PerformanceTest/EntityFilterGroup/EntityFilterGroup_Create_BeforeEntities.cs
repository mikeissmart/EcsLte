using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilterGroup_Create_BeforeEntities : BasePerformanceTest
    {
        private TestSharedComponent1 _component;
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestSharedComponent1 { Prop = 1 };
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            _context.FilterByGroupWith(Filter.AllOf<TestSharedComponent1>(), _component);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], _component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.AddComponent(_entities[i], _component); });
        }
    }
}