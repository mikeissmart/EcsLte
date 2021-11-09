using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_ComponentLife_ReplaceComponent_Unique : BasePerformanceTest
    {
        private Entity _entity;
        private TestUniqueComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
            _component = new TestUniqueComponent1 { Prop = 1 };
            _context.AddUniqueComponent(_entity, _component);
        }

        public override void Run()
        {
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.DefaultCommand.ReplaceUniqueComponent(_entity, _component);
            }
            _context.DefaultCommand.RunCommands();
        }
    }
}