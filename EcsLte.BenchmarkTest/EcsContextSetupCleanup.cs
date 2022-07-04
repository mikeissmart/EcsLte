using EcsLte.BenchmarkTest.EntityQueryTests;
using System;

namespace EcsLte.BenchmarkTest
{
    public static class EcsContextSetupCleanup
    {
        public static TestComponent1 Component1 = new TestComponent1 { Prop = 1 };
        public static TestComponent2 Component2 = new TestComponent2 { Prop = 2 };
        public static TestComponent3 Component3 = new TestComponent3 { Prop = 3 };
        public static TestComponent4 Component4 = new TestComponent4 { Prop = 4 };
        public static TestSharedComponent1 SharedComponent1 = new TestSharedComponent1 { Prop = 5 };
        public static TestSharedComponent2 SharedComponent2 = new TestSharedComponent2 { Prop = 6 };
        public static TestSharedComponent3 SharedComponent3 = new TestSharedComponent3 { Prop = 7 };
        public static TestSharedComponent4 SharedComponent4 = new TestSharedComponent4 { Prop = 8 };

        public static EntityBlueprint CreateBlueprint(ComponentArrangement compArr)
        {
            var blueprint = new EntityBlueprint();
            switch (compArr)
            {
                case ComponentArrangement.Normal_x4:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(Component3)
                        .AddComponent(Component4);
                    break;

                case ComponentArrangement.Shared_x4:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(SharedComponent3)
                        .AddComponent(SharedComponent4);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return blueprint;
        }

        public static EntityBlueprint CreateBlueprint(EntityQuery_ForEach.ReadWriteType rwType)
        {
            var blueprint = new EntityBlueprint();
            switch (rwType)
            {
                case EntityQuery_ForEach.ReadWriteType.R0W0:
                    blueprint = blueprint
                        .AddComponent(Component1);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Normal_x4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Normal_x4:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(Component3)
                        .AddComponent(Component4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Shared_x4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Shared_x4:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(SharedComponent3)
                        .AddComponent(SharedComponent4);
                    break;
            }
            return blueprint;
        }
    }
}