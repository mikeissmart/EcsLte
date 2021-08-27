using System;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_CreateGetBeforeEntities : BasePerformanceTest
    {
        private World _world;
        private Filter _filter;
        private Entity[] _entities;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _filter = Filter.AllOf<TestComponent1>();
            _world.GroupManager.GetGroup(_filter);
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _world.EntityManager.AddComponent(_entities[i], new TestComponent1());
            }
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    _world.EntityManager.AddComponent(_entities[index], new TestComponent1());
                });
        }

        public override void PostRun()
            => World.DestroyWorld(_world);
    }
}