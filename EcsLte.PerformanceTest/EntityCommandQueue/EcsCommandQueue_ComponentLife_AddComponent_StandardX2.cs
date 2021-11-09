using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityCommandQueue_ComponentLife_AddComponent_StandardX2 : BasePerformanceTest
    {
        private Entity[] _entities;

        public override void PreRun()
        {
            base.PreRun();

            _entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            var component = new TestStandardComponent1();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.AddComponent(_entities[i], component);
        }

        public override void Run()
        {
            var component = new TestStandardComponent2();
            for (var i = 0; i < TestConsts.EntityLoopCount; i++)
                _context.DefaultCommand.AddComponent(_entities[i], component);
            _context.DefaultCommand.RunCommands();
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            var component = new TestStandardComponent2();
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { _context.DefaultCommand.AddComponent(_entities[i], component); });
            _context.DefaultCommand.RunCommands();
        }
    }
}