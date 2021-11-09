namespace EcsLte.PerformanceTest
{
    internal class EntityFilterGroup_Create_AfterEntities : BasePerformanceTest
    {
        private TestSharedComponent1 _sharedComponent;
        private TestStandardComponent1 _standardComponent;
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _sharedComponent = new TestSharedComponent1 { Prop = 1 };
            _standardComponent = new TestStandardComponent1 { Prop = 1 };
            _filter = Filter.AllOf<TestSharedComponent1, TestStandardComponent1>();
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.AddComponent(entities[i], _sharedComponent);
                _context.AddComponent(entities[i], _standardComponent);
            }
        }

        public override void Run()
        {
            EntityFilterGroup entityGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.FilterByGroupWith(_filter, _sharedComponent);
        }
    }
}