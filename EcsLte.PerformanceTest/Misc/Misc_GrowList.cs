using System.Collections.Generic;

namespace EcsLte.PerformanceTest.Misc
{
    internal class Misc_GrowList : BasePerformanceTest
    {
        private List<int> _bag;

        public override void PreRun()
        {
            _bag = new List<int>(4);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount * 10; i++)
                _bag.Add(i);
        }

        public override bool CanRunParallel()
        {
            return false;
        }

        public override void RunParallel()
        {
        }

        public override void PostRun()
        {
        }
    }
}