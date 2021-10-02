namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_GetEntity_GetEntitiesBefore : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(entities[i], component);
        }

        public override void Run()
        {
            EntityFilter entityFilter;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityFilter = _context.FilterBy(Filter.AllOf<TestComponent1>());
        }
    }
}