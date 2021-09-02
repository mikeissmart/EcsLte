using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class Filter_Create : BasePerformanceTest
    {
        private Filter[] _filters;

        public override void PreRun()
        {
            _filters = new Filter[TestConsts.EntityLoopCount];
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _filters[i] =
                    Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();
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
                    _filters[index] =
                        Filter
                            .AllOf<TestComponent1, TestComponent2, TestRecordableComponent1,
                                TestRecordableComponent2>();
                });
        }

        public override void PostRun()
        {
        }
    }
}