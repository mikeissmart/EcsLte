using System.Collections.Concurrent;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.Misc
{
    internal class Misc_ConcurrentDic_TryAdd : BasePerformanceTest
    {
        private ConcurrentDictionary<int, int> _dic;

        public override void PreRun()
        {
            _dic = new ConcurrentDictionary<int, int>();
        }

        public override void Run()
        {
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index => { _dic.TryAdd(index, index); });
        }

        public override void PostRun()
        {
        }
    }
}