using System.Collections.Concurrent;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.Misc
{
    internal class Misc_ConcurrentDic_TryGetValue_DoesHave : BasePerformanceTest
    {
        private ConcurrentDictionary<int, int> _dic;

        public override void PreRun()
        {
            _dic = new ConcurrentDictionary<int, int>();
            _dic.TryAdd(0, 0);
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
                    var result = _dic.TryGetValue(0, out var value);
                });
        }

        public override void PostRun()
        {
        }
    }
}