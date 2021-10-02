namespace EcsLte.PerformanceTest
{
    internal class EcsContext_GetEntity_GetEntities : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            Entity[] entities;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entities = _context.GetEntities();
        }
    }
}