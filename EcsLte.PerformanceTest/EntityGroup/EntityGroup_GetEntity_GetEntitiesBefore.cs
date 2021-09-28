using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetEntity_GetEntitiesBefore : BasePerformanceTest
    {
        private TestSharedKeyComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestSharedKeyComponent1 { Prop = 1 };
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(entities[i], _component);
        }

        public override void Run()
        {
            EntityGroup entityGroup;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(_component);
        }
    }
}