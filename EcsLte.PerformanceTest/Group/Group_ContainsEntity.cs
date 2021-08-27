using System;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Group_ContainsEntity : BasePerformanceTest
    {
        private World _world;
        private Group _group;
        private Entity[] _entities;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _group = _world.GroupManager.GetGroup(Filter.AllOf<TestComponent1>());
            _entities = _world.EntityManager.CreateEntities(TestConsts.EntityLoopCount);
            foreach (var entity in _entities)
                _world.EntityManager.AddComponent(entity, new TestComponent1());
        }

        public override void Run()
        {
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
            { var result = _group.ContainsEntity(_entities[i]); }
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var result = _group.ContainsEntity(_entities[index]);
                });
        }

        public override void PostRun()
            => World.DestroyWorld(_world);
    }
}