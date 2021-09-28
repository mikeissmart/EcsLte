using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_EntityGroup_SharedComponentX10 : BasePerformanceTest
    {
        public override void Run()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var component2 = new TestSharedKeyComponent2 { Prop = 2 };
            var component3 = new TestSharedKeyComponent3 { Prop = 3 };
            var component4 = new TestSharedKeyComponent4 { Prop = 4 };
            var component5 = new TestSharedKeyComponent5 { Prop = 5 };
            var component6 = new TestSharedKeyComponent6 { Prop = 6 };
            var component7 = new TestSharedKeyComponent7 { Prop = 7 };
            var component8 = new TestSharedKeyComponent8 { Prop = 8 };
            var component9 = new TestSharedKeyComponent9 { Prop = 9 };
            EntityGroup entityGroup;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityGroup = _context.GroupWith(
                    component1,
                    component2,
                    component3,
                    component4,
                    component5,
                    component6,
                    component7,
                    component8,
                    component9);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component1 = new TestSharedKeyComponent1 { Prop = 1 };
            var component2 = new TestSharedKeyComponent2 { Prop = 2 };
            var component3 = new TestSharedKeyComponent3 { Prop = 3 };
            var component4 = new TestSharedKeyComponent4 { Prop = 4 };
            var component5 = new TestSharedKeyComponent5 { Prop = 5 };
            var component6 = new TestSharedKeyComponent6 { Prop = 6 };
            var component7 = new TestSharedKeyComponent7 { Prop = 7 };
            var component8 = new TestSharedKeyComponent8 { Prop = 8 };
            var component9 = new TestSharedKeyComponent9 { Prop = 9 };
            EntityGroup entityGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i =>
                {
                    entityGroup = _context.GroupWith(
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7,
                        component8,
                        component9);
                });
        }
    }
}