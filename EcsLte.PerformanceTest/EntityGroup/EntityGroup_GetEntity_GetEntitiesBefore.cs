namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetEntity_GetEntitiesBefore : BasePerformanceTest
    {
        private TestSharedComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestSharedComponent1 { Prop = 1 };
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(entities[i], _component);
        }

        public override void Run()
        {
            EntityGroup entityGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(_component);
        }
    }
}