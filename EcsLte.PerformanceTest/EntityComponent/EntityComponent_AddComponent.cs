namespace EcsLte.PerformanceTest
{
    internal class EntityComponent_AddComponent : BasePerformanceTest
    {
        private Entity[] _entities;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];

            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _entities[i] = _world.EntityManager.CreateEntity();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
        }

        public override int ParallelRunCount()
        {
            return TestConsts.EntityLoopCount;
        }

        public override void RunParallel(int index, int startIndex, int endIndex)
        {
            _world.EntityManager.AddComponent(_entities[index], new TestComponent1());
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}