using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetEntity_HasEntity : BasePerformanceTest
    {
        private Entity[] _entities;
        private EntityGroup _entityGroup;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestSharedKeyComponent1 { Prop = 1 };
            _entityGroup = _context.GroupWith(component);
            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            bool hasEntity;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                hasEntity = _entityGroup.HasEntity(_entities[i]);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            bool hasEntity;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { hasEntity = _entityGroup.HasEntity(_entities[i]); });
        }
    }
}