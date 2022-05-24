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

        public static TestManageComponent1 ManageComponent1 = new TestManageComponent1 { Prop = 11 };
        public static TestManageComponent2 ManageComponent2 = new TestManageComponent2 { Prop = 12 };
        public static TestManageComponent3 ManageComponent3 = new TestManageComponent3 { Prop = 13 };
        public static TestManageComponent4 ManageComponent4 = new TestManageComponent4 { Prop = 14 };
        public static TestManageSharedComponent1 ManageSharedComponent1 = new TestManageSharedComponent1 { Prop = 15 };
        public static TestManageSharedComponent2 ManageSharedComponent2 = new TestManageSharedComponent2 { Prop = 16 };
        public static TestManageSharedComponent3 ManageSharedComponent3 = new TestManageSharedComponent3 { Prop = 17 };
        public static TestManageSharedComponent4 ManageSharedComponent4 = new TestManageSharedComponent4 { Prop = 18 };

        public static EntityBlueprint CreateBlueprint(ComponentArrangement compArr)
        {
            var blueprint = new EntityBlueprint();
            switch (compArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(Component3)
                        .AddComponent(Component4);
                    break;

                case ComponentArrangement.Shared_Bx4:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(SharedComponent3)
                        .AddComponent(SharedComponent4);
                    break;

                case ComponentArrangement.Normal_Mx4:
                    blueprint = blueprint
                        .AddComponent(ManageComponent1)
                        .AddComponent(ManageComponent2)
                        .AddComponent(ManageComponent3)
                        .AddComponent(ManageComponent4);
                    break;

                case ComponentArrangement.Shared_Mx4:
                    blueprint = blueprint
                        .AddComponent(ManageSharedComponent1)
                        .AddComponent(ManageSharedComponent2)
                        .AddComponent(ManageSharedComponent3)
                        .AddComponent(ManageSharedComponent4);
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(ManageComponent1)
                        .AddComponent(ManageComponent2);
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(ManageSharedComponent1)
                        .AddComponent(ManageSharedComponent2);
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

                case EntityQuery_ForEach.ReadWriteType.R0W4_Normal_Bx4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Normal_Bx4:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(Component3)
                        .AddComponent(Component4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Normal_Mx4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Normal_Mx4:
                    blueprint = blueprint
                        .AddComponent(ManageComponent1)
                        .AddComponent(ManageComponent2)
                        .AddComponent(ManageComponent3)
                        .AddComponent(ManageComponent4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Shared_Bx4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Shared_Bx4:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(SharedComponent3)
                        .AddComponent(SharedComponent4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Shared_Mx4:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Shared_Mx4:
                    blueprint = blueprint
                        .AddComponent(ManageSharedComponent1)
                        .AddComponent(ManageSharedComponent2)
                        .AddComponent(ManageSharedComponent3)
                        .AddComponent(ManageSharedComponent4);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R0W4_Normal_Bx2_Mx2:
                case EntityQuery_ForEach.ReadWriteType.R4W0_Normal_Bx2_Mx2:
                    blueprint = blueprint
                        .AddComponent(Component1)
                        .AddComponent(Component2)
                        .AddComponent(ManageComponent1)
                        .AddComponent(ManageComponent2);
                    break;

                case EntityQuery_ForEach.ReadWriteType.R4W0_Shared_Bx2_Mx2:
                case EntityQuery_ForEach.ReadWriteType.R0W4_Shared_Bx2_Mx2:
                    blueprint = blueprint
                        .AddComponent(SharedComponent1)
                        .AddComponent(SharedComponent2)
                        .AddComponent(ManageSharedComponent1)
                        .AddComponent(ManageSharedComponent2);
                    break;
            }
            return blueprint;
        }
    }
}