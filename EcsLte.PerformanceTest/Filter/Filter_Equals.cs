using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Filter_Equals : BasePerformanceTest
    {
        private Filter _filter1;
        private Filter _filter2;

        public override void PreRun()
        {
            _filter1 =
                Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();
            _filter2 =
                Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                var result = _filter1 == _filter2;
            }
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var result = _filter1 == _filter2;
                });
        }

        public override void PostRun()
        {
        }
    }
}