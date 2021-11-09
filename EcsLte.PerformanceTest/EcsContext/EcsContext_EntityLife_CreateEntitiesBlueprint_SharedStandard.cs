namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntitiesBlueprint_SharedStandard : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestSharedComponent1())
                .AddComponent(new TestStandardComponent1());
        }

        public override void Run()
        {
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount, _blueprint);
        }
    }
}