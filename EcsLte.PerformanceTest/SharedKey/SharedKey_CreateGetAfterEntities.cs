using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class SharedKey_CreateGetAfterEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestComponent1>();
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestSharedKeyComponent1());
        }

        public override void Run()
        {
            var key = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var key = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>();
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}