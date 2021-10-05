using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsLte.PerformanceTest
{
    internal class EntityGroup_GetEntity_GetEntities : BasePerformanceTest
    {
        private EntityGroup _entityGroup;

        public override void PreRun()
        {
            base.PreRun();

            var component = new TestSharedComponent1 { Prop = 1 };
            _context.CreateEntities(TestConsts.EntityLoopCount, new EntityBlueprint()
                .AddComponent(component));
            _entityGroup = _context.GroupWith(component);
        }

        public override void Run()
        {
            Entity[] entities;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                entities = _entityGroup.GetEntities();
        }
    }
}