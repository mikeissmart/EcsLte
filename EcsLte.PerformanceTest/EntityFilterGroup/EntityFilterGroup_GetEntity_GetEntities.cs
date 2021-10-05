namespace EcsLte.PerformanceTest
{
    internal class EntityFilterGroup_GetEntity_GetEntities : BasePerformanceTest
    {
        private EntityFilterGroup _entityFilterGroup;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestSharedComponent1 { Prop = 1 };
            _context.CreateEntities(TestConsts.EntityLoopCount, new EntityBlueprint()
                .AddComponent(component));
            _entityFilterGroup = _context.FilterByGroupWith(Filter.AllOf<TestSharedComponent1>(), component);
        }

        public override void Run()
        {
            Entity[] entities;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entities = _entityFilterGroup.GetEntities();
        }
    }
}