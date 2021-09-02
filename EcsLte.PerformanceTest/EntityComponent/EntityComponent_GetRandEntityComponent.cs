using System;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityComponent_GetRandEntityComponent : BasePerformanceTest
    {
        private Entity[] _entities;
        private Random _randComponent;
        private Random _randEntity;
        private World _world;

        public override void PreRun()
        {
            _world = World.CreateWorld("Test");
            _entities = new Entity[TestConsts.EntityLoopCount];
            _randEntity = new Random((int) DateTime.UtcNow.Ticks);
            _randComponent = new Random(DateTime.UtcNow.Second);


            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var entity = _world.EntityManager.CreateEntity();
                _world.EntityManager.AddComponent(entity, new TestComponent1());
                _world.EntityManager.AddComponent(entity, new TestComponent2());
                _world.EntityManager.AddComponent(entity, new TestRecordableComponent1());
                _world.EntityManager.AddComponent(entity, new TestRecordableComponent2());
                _entities[i] = entity;
            }
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var entity = _entities[_randEntity.Next(0, TestConsts.EntityLoopCount)];
                var componentIndex = _randComponent.Next(0, 3);
                IComponent component;

                switch (componentIndex)
                {
                    case 0:
                        component = _world.EntityManager.GetComponent<TestComponent1>(entity);
                        break;
                    case 1:
                        component = _world.EntityManager.GetComponent<TestComponent2>(entity);
                        break;
                    case 2:
                        component = _world.EntityManager.GetComponent<TestRecordableComponent1>(entity);
                        break;
                    case 3:
                        component = _world.EntityManager.GetComponent<TestRecordableComponent2>(entity);
                        break;
                }
            }
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
                    var entity = _entities[_randEntity.Next(0, TestConsts.EntityLoopCount)];
                    var componentIndex = _randComponent.Next(0, 3);
                    IComponent component;

                    switch (componentIndex)
                    {
                        case 0:
                            component = _world.EntityManager.GetComponent<TestComponent1>(entity);
                            break;
                        case 1:
                            component = _world.EntityManager.GetComponent<TestComponent2>(entity);
                            break;
                        case 2:
                            component = _world.EntityManager.GetComponent<TestRecordableComponent1>(entity);
                            break;
                        case 3:
                            component = _world.EntityManager.GetComponent<TestRecordableComponent2>(entity);
                            break;
                    }
                });
        }

        public override void PostRun()
        {
            World.DestroyWorld(_world);
        }
    }
}