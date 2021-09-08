using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class CollectorTrigger_Create : BasePerformanceTest
    {
        private CollectorTrigger[] _collectorTriggers;

        public override void PreRun()
        {
            _collectorTriggers = new CollectorTrigger[TestConsts.EntityLoopCount];
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _collectorTriggers[i] = CollectorTrigger
                    .Added<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();
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
                    _collectorTriggers[index] = CollectorTrigger
                        .Added<TestComponent1, TestComponent2, TestRecordableComponent1, TestRecordableComponent2>();
                });
        }

        public override void PostRun()
        {
        }
    }
}