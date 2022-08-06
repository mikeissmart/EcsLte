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
        public static TestManagedComponent1 ManagedComponent1 = new TestManagedComponent1 { Prop = 10 };
        public static TestManagedComponent2 ManagedComponent2 = new TestManagedComponent2 { Prop = 11 };
        public static TestManagedComponent3 ManagedComponent3 = new TestManagedComponent3 { Prop = 12 };
        public static TestManagedComponent4 ManagedComponent4 = new TestManagedComponent4 { Prop = 13 };

        public static EntityBlueprint CreateBlueprint(ComponentArrangement compArr)
        {
            var blueprint = new EntityBlueprint();
            switch (compArr)
            {
                case ComponentArrangement.Normal_x4:
                    blueprint = blueprint
                        .SetComponent(Component1)
                        .SetComponent(Component2)
                        .SetComponent(Component3)
                        .SetComponent(Component4);
                    break;

                case ComponentArrangement.Managed_x4:
                    blueprint = blueprint
                        .SetManagedComponent(ManagedComponent1)
                        .SetManagedComponent(ManagedComponent2)
                        .SetManagedComponent(ManagedComponent3)
                        .SetManagedComponent(ManagedComponent4);
                    break;

                case ComponentArrangement.Shared_x4:
                    blueprint = blueprint
                        .SetSharedComponent(SharedComponent1)
                        .SetSharedComponent(SharedComponent2)
                        .SetSharedComponent(SharedComponent3)
                        .SetSharedComponent(SharedComponent4);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return blueprint;
        }

        /*public static EntityBlueprint CreateBlueprint(EntityQuery_ForEach.ReadWriteType rwType)
        {
            var blueprint = new EntityBlueprint();
            switch (rwType)
            {
                case EntityQuery_ForEach.ReadWriteType.R0W0:
                    blueprint = blueprint
                        .SetComponent(Component1);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Normal_x4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Normal_x4:
                    blueprint = blueprint
                        .SetComponent(Component1)
                        .SetComponent(Component2)
                        .SetComponent(Component3)
                        .SetComponent(Component4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Shared_x4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Shared_x4:
                    blueprint = blueprint
                        .SetSharedComponent(SharedComponent1)
                        .SetSharedComponent(SharedComponent2)
                        .SetSharedComponent(SharedComponent3)
                        .SetSharedComponent(SharedComponent4);
                    break;
            }
            return blueprint;
        }*/
    }
}