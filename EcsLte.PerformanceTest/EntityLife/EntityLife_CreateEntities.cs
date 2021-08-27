namespace EcsLte.PerformanceTest
{
    internal class EntityLife_CreateEntities : BasePerformanceTest
    {
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
        }

        public override void Run()
        {
            var a = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override bool CanRunParallel()
            => false;

        public override void RunParallel()
        {
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}