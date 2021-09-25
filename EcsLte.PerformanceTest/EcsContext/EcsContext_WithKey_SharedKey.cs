using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_WithKey_SharedKey : BasePerformanceTest
    {
        public override void Run()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            EntityKey entityKey;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityKey = _context.WithKey(component);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestSharedKeyComponent1 { Prop = 1 };
            EntityKey entityKey;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityKey = _context.WithKey(component); });
        }
    }
}