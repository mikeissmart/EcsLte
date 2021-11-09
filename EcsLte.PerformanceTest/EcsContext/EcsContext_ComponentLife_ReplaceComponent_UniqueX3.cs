using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_ReplaceComponent_UniqueX3 : BasePerformanceTest
    {
        private Entity _entity;
        private TestUniqueComponent1 _component1;
        private TestUniqueComponent1 _component2;
        private TestUniqueComponent1 _component3;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
            _component1 = new TestUniqueComponent1 { Prop = 1 };
            _component2 = new TestUniqueComponent1 { Prop = 2 };
            _component3 = new TestUniqueComponent1 { Prop = 3 };
            _context.AddUniqueComponent(_entity, _component1);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.ReplaceUniqueComponent(_entity, _component1);
                _context.ReplaceUniqueComponent(_entity, _component2);
                _context.ReplaceUniqueComponent(_entity, _component3);
            }
        }
    }
}