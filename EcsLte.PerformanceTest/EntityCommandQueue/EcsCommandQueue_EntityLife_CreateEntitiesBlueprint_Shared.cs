namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_EntityLife_CreateEntitiesBlueprint_Shared : BasePerformanceTest
    {
        private EntityBlueprint _blueprint;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestSharedComponent1());
        }

        public override void Run()
        {
            var entities = _context.DefaultCommand.CreateEntities(TestConsts.EntityLoopCount, _blueprint);
            _context.DefaultCommand.RunCommands();
        }
    }
}