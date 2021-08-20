namespace EcsLte.PerformanceTest
{
    internal class Filter_Equals : BasePerformanceTest
    {
        private Filter _filter1;
        private Filter _filter2;

        public override void PreRun()
        {
            _filter1 = Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1>();
            _filter2 = Filter.AllOf<TestComponent1, TestComponent2, TestRecordableComponent1>();
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _filter1.Equals(_filter2);
        }

        public override int ParallelRunCount()
        {
            return TestConsts.EntityLoopCount;
        }

        public override void RunParallel(int index, int startIndex, int endIndex)
        {
            _filter1.Equals(_filter2);
        }

        public override void PostRun()
        {
        }
    }
}