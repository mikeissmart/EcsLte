using EcsLte.Utilities;

namespace EcsLte.PerformanceTest.EntityCommandQueue
{
    internal class EntityCommandQueue_ComponentLife_ReplaceComponent : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            var component = new TestComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.DefaultCommand.ReplaceComponent(_entities[i], component);
            _context.DefaultCommand.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestComponent1();
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.DefaultCommand.ReplaceComponent(_entities[i], component); });
            _context.DefaultCommand.RunCommands();
        }
    }
}