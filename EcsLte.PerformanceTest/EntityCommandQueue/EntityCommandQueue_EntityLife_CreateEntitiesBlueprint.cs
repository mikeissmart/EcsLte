namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_EntityLife_CreateEntitiesBlueprint : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 });
        }

        public override void Run()
        {
            _entities = _context.DefaultCommand.CreateEntities(TestConsts.EntityLoopCount, _blueprint);
            _context.DefaultCommand.RunCommands();
        }
    }
}