namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_Create_AfterEntities : BasePerformanceTest
    {
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _filter = Filter.AllOf<TestComponent1>();
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(entities[i], component);
        }

        public override void Run()
        {
            EntityFilter entityFilter;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityFilter = _context.FilterBy(_filter);
        }
    }
}