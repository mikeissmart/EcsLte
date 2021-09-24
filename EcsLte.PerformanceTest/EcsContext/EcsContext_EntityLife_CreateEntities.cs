namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntities : BasePerformanceTest
    {
        Entity[] _entities;

        public override void Run()
        {
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }
    }
}