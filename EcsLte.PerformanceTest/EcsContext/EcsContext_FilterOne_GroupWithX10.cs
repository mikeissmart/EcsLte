using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_FilterOne_GroupWithX10 : BasePerformanceTest
    {
        private TestSharedComponent1 _component1;
        private TestSharedComponent2 _component2;
        private TestSharedComponent3 _component3;
        private TestSharedComponent4 _component4;
        private TestSharedComponent5 _component5;
        private TestSharedComponent6 _component6;
        private TestSharedComponent7 _component7;
        private TestSharedComponent8 _component8;
        private TestSharedComponent9 _component9;
        private Filter _filter;

        public override void PreRun()
        {
            base.PreRun();

            _component1 = new TestSharedComponent1 { Prop = 1 };
            _component2 = new TestSharedComponent2 { Prop = 2 };
            _component3 = new TestSharedComponent3 { Prop = 3 };
            _component4 = new TestSharedComponent4 { Prop = 4 };
            _component5 = new TestSharedComponent5 { Prop = 5 };
            _component6 = new TestSharedComponent6 { Prop = 6 };
            _component7 = new TestSharedComponent7 { Prop = 7 };
            _component8 = new TestSharedComponent8 { Prop = 8 };
            _component9 = new TestSharedComponent9 { Prop = 9 };
            _filter = Filter.AllOf<TestComponent1>();
        }

        public override void Run()
        {
            EntityFilterGroup filterGroup;
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                filterGroup = _context.FilterByGroupWith(_filter,
                    _component1,
                    _component2,
                    _component3,
                    _component4,
                    _component5,
                    _component6,
                    _component7,
                    _component8,
                    _component9);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            EntityFilterGroup filterGroup;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i =>
                {
                    filterGroup = _context.FilterByGroupWith(_filter,
                        _component1,
                        _component2,
                        _component3,
                        _component4,
                        _component5,
                        _component6,
                        _component7,
                        _component8,
                        _component9);
                });
        }
    }
}