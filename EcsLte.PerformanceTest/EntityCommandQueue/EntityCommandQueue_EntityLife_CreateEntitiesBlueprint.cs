using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_EntityLife_CreateEntitiesBlueprint : BasePerformanceTest
    {
        private Entity[] _entities;
        private EntityBlueprint _blueprint;

        public override void PreRun()
        {
            base.PreRun();

            _blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1 { Prop = 1 });
        }

        public override void Run()
        {
            _entities = _context.DefaultCommand.CreateEntities(TestConsts.EntityLoopCount, _blueprint);
            _context.DefaultCommand.RunCommands();
        }
    }
}