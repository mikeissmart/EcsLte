using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EcsContext_ComponentLife_AddRemoveComponent_Unique : BasePerformanceTest
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
                _context.AddUniqueComponent(_entity, component);
                _context.RemoveUniqueComponent<TestUniqueComponent1>(_entity);
            }
        }
    }
}