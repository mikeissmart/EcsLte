namespace EcsLte.PerformanceTest.EntityCommandQueue
{
    internal class EntityCommandQueue_EntityLife_DestroyEntities : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            _context.DefaultCommand.DestroyEntities(_entities);
            _context.DefaultCommand.RunCommands();
        }
    }
}