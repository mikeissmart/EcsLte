using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityFilterGroup_GetEntity_HasEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private EntityFilterGroup _entityFilterGroup;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestSharedComponent1 { Prop = 1 };
            _entityFilterGroup = _context.FilterByGroupWith(Filter.AllOf<TestSharedComponent1>(), component);
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            bool hasEntity;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                hasEntity = _entityFilterGroup.HasEntity(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasEntity;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasEntity = _entityFilterGroup.HasEntity(_entities[i]); });
        }
    }
}