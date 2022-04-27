using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BenchmarkTest
{
    public static class EcsContextSetupCleanup
    {
        public static TestComponent1 Component1 = new TestComponent1 { Prop = 1 };
        public static TestComponent2 Component2 = new TestComponent2 { Prop = 2 };
        public static TestSharedComponent1 SharedComponent1 = new TestSharedComponent1 { Prop = 3 };
        public static TestSharedComponent2 SharedComponent2 = new TestSharedComponent2 { Prop = 4 };

        public static EntityBlueprint CreateBlueprint(ComponentArrangement compArr)
        {
            var blueprint = new EntityBlueprint();
            switch (compArr)
            {
                case ComponentArrangement.Normal_x1:
                    blueprint = blueprint
                        .AddComponent(Component1);
                    break;
                case ComponentArrangement.Normal_x2:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2);
                    break;
                case ComponentArrangement.Shared_x1:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1);
                    break;
                case ComponentArrangement.Shared_x2:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x1:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(SharedComponent1);
                    break;
                case ComponentArrangement.Normal_x1_Shared_x2:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x1:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(SharedComponent1);
                    break;
                case ComponentArrangement.Normal_x2_Shared_x2:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return blueprint;
        }
    }
}
