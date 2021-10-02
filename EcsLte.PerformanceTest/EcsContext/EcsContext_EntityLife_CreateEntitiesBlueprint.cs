namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntitiesBlueprint : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1())
                .AddComponent(new TestComponent2());
        }

        public override void Run()
        {
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount, _blueprint);
        }
    }
}