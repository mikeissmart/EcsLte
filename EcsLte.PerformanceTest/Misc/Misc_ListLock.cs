using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.Misc
{
    internal class Misc_ListLock : BasePerformanceTest
    {
        private List<int> _list;

        public override void PreRun()
        {
            _list = new List<int>();
        }

        public override void Run()
        {
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    lock (_list)
                    {
                        _list.Add(index);
                    }
                });
        }

        public override void PostRun()
        {
        }
    }
}