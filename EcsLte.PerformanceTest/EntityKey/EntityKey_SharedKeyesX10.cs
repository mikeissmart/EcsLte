using System.Linq;
using EcsLte.Utilities;

namespace EcsLte.PerformanceTest
{
    internal class EntityKey_SharedKeyesX10 : BasePerformanceTest
    {
        private IComponentSharedKey[] _components;

        public override void PreRun()
        {
            base.PreRun();

            _components = new IComponentSharedKey[]
            {
                new TestSharedKeyComponent1 { Prop = 1 },
                new TestSharedKeyComponent2 { Prop = 1 },
                new TestSharedKeyComponent3 { Prop = 1 },
                new TestSharedKeyComponent4 { Prop = 1 },
                new TestSharedKeyComponent5 { Prop = 1 },
                new TestSharedKeyComponent6 { Prop = 1 },
                new TestSharedKeyComponent7 { Prop = 1 },
                new TestSharedKeyComponent8 { Prop = 1 },
                new TestSharedKeyComponent9 { Prop = 1 },
            };
            var entities = _context.CreateEntities(TestConsts.EntityLoopCount);
            for (int i = 0; i < entities.Length; i++)
            {
                _context.AddComponent(entities[i], new TestSharedKeyComponent1 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent2 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent3 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent4 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent5 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent6 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent7 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent8 { Prop = 1 });
                _context.AddComponent(entities[i], new TestSharedKeyComponent9 { Prop = 1 });
            }
        }

        public override void Run()
        {
            EntityKey entityKey;
            for (int i = 0; i < TestConsts.EntityLoopCount; i++)
                entityKey = _context.WithKey(_components);
        }

        public override bool CanRunParallel()
        {
            return true;
        }

        public override void RunParallel()
        {
            EntityKey entityKey;
            ParallelRunner.RunParallelFor(TestConsts.EntityLoopCount,
                i => { entityKey = _context.WithKey(_components); });
        }
    }
}