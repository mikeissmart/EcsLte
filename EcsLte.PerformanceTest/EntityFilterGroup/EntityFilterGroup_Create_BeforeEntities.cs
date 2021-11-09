using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilterGroup_Create_BeforeEntities : BasePerformanceTest
    {
        private TestSharedComponent1 _sharedComponent;
        private TestStandardComponent1 _standardComponent;
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _sharedComponent = new TestSharedComponent1 { Prop = 1 };
            _standardComponent = new TestStandardComponent1 { Prop = 1 };
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            _context.FilterByGroupWith(
                Filter.AllOf<TestSharedComponent1, TestStandardComponent1>(), _sharedComponent);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.AddComponent(_entities[i], _sharedComponent);
                _context.AddComponent(_entities[i], _standardComponent);
            }
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i =>
                {
                    _context.AddComponent(_entities[i], _sharedComponent);
                    _context.AddComponent(_entities[i], _standardComponent);
                });
        }
    }
}