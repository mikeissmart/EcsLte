using System;
namespace EcsLte.PerformanceTest.Misc
{
    internal class Misc_GrowArray : BasePerformanceTest
    {
        private int[] _bag;

        public override void PreRun()
        {
            _bag = new int[4];
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount * 10; i++)
            {
                if (_bag.Length == i)
                    Array.Resize(ref _bag, _bag.Length << 1);
                _bag[i] = i;
            }
        }

        public override bool CanRunParallel()
            => false;

        public override void RunParallel()
        {
        }

        public override void PostRun()
        {
        }
    }
}