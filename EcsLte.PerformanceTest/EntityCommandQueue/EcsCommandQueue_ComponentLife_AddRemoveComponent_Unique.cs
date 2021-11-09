using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_ComponentLife_AddRemoveComponent_Unique : BasePerformanceTest
    {
        private Entity _entity;

        public override void PreRun()
        {
            base.PreRun();

            _entity = _context.CreateEntity();
        }

        public override void Run()
        {
            var component = new TestUniqueComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
            {
                _context.DefaultCommand.AddUniqueComponent(_entity, component);
                _context.DefaultCommand.RemoveUniqueComponent<TestUniqueComponent1>(_entity);
            }
            _context.DefaultCommand.RunCommands();
        }
    }
}