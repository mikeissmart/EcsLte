namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_GetEntity_GetEntities : BasePerformanceTest
    {
        private EntityFilter _filter;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestStandardComponent1();
            _context.CreateEntities(TestConsts.EntityLoopCount, new EntityBlueprint()
                .AddComponent(component));
            _filter = _context.FilterBy(Filter.AllOf<TestStandardComponent1>());
        }

        public override void Run()
        {
            Entity[] entities;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entities = _filter.GetEntities();
        }
    }
}