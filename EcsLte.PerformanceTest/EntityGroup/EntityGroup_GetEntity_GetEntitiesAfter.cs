using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetEntity_GetEntitiesAfter : BasePerformanceTest
    {
        private Entity[] _entities;
        private TestSharedKeyComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestSharedKeyComponent1 { Prop = 1 };
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            _context.GroupWith(_component);
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
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