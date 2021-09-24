namespace EcsLte.PerformanceTest
{
    internal class EntityFilter_GetEntity_GetEntitiesBefore : BasePerformanceTest
    {
        public override void PreRun()
        {
            base.PreRun();

            _context.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            Entity[] entities;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entities = _context.GetEntities();
        }
    }
}