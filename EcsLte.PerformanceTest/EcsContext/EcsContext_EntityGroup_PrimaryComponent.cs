using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityGroup_PrimaryComponent : BasePerformanceTest
    {
        public override void Run()
        {
            var component = new TestPrimaryKeyComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestPrimaryKeyComponent1 { Prop = 1 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityGroup = _context.GroupWith(component); });
        }
    }
}