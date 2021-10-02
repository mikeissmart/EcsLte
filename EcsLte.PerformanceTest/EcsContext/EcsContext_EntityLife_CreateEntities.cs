namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntities : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void Run()
        {
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }
    }
}