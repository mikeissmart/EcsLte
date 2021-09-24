namespace EcsLte.PerformanceTest.EntityCommandQueue
{
    internal class EntityCommandQueue_EntityLife_CreateEntities : BasePerformanceTest
    {
        Entity[] _entities;

        public override void Run()
        {
            _entities = _context.DefaultCommand.CreateEntities(TestConsts.EntityLoopCount);
            _context.DefaultCommand.RunCommands();
        }
    }
}