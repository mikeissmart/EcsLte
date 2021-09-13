using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class PrimaryKey_CreateGetAfterEntities : BasePerformanceTest
    {
        private Entity[] _entities;
        private Filter _filter;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestPrimaryKeyComponent1>();
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _world.EntityManager.AddComponent(_entities[i], new TestPrimaryKeyComponent1 { Prop = i });
        }

        public override void Run()
        {
            var key = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>();
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
                    var key = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>();
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}