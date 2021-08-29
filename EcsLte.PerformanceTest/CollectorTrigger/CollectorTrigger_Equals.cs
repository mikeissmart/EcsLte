using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class CollectorTrigger_Equals : BasePerformanceTest
    {
        private CollectorTrigger _collectorTrigger1;
        private CollectorTrigger _collectorTrigger2;

        public override void PreRun()
        {
            _collectorTrigger1 = CollectorTrigger.Added<TestComponent1, TestComponent2, TestRecordableComponent1>();
            _collectorTrigger2 = CollectorTrigger.Added<TestComponent1, TestComponent2, TestRecordableComponent1>();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _collectorTrigger1.Equals(_collectorTrigger2);
        }

        public override bool CanRunParallel()
            => true;

        public override void RunParallel()
        {
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                index =>
                {
                    var result = _collectorTrigger1.Equals(_collectorTrigger2);
                });
        }

        public override void PostRun()
        {
        }
    }
}