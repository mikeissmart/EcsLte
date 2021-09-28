using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityGroup_SharedComponentX2 : BasePerformanceTest
    {
        public override void Run()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var component2 = new TestSharedKeyComponent2 { Prop = 2 };
            EntityGroup entityGroup;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(component1, component2);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var component2 = new TestSharedKeyComponent2 { Prop = 2 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityGroup = _context.GroupWith(component1, component2); });
        }
    }
}