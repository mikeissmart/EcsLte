namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityLife_CreateEntities : BasePerformanceTest
    {
        public override void Run()
        {
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
        }
    }
}