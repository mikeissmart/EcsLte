using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BencharkTest
{
    public static class SetupCleanupTest
    {
        private static TestComponent1 _component1 = new TestComponent1 { Prop = 1 };
        private static TestComponent2 _component2 = new TestComponent2 { Prop = 2 };
        private static TestSharedComponent1 _componentShared1 = new TestSharedComponent1 { Prop = 3 };
        private static TestSharedComponent2 _componentShared2 = new TestSharedComponent2 { Prop = 4 };

        public static EcsContext EcsContext_Setup(EcsContextType contextType)
        {
            EcsContext context;
            switch (contextType)
            {
                case EcsContextType.Managed:
                    context = EcsContexts.CreateEcsContext_Managed("Managed_Test");
                    break;
                case EcsContextType.Managed_ArcheType:
                    context = EcsContexts.CreateEcsContext_ArcheType_Managed("ArcheType_Managed_Test");
                    break;
                case EcsContextType.Native:
                    context = EcsContexts.CreateEcsContext_Native("Native_Test");
                    break;
                case EcsContextType.Native_ArcheType:
                    context = EcsContexts.CreateEcsContext_ArcheType_Native("ArcheType_Native_Test");
                    break;
                case EcsContextType.Native_ArcheType_Continous:
                    context = EcsContexts.CreateEcsContext_ArcheType_Native_Continuous("ArcheType_Native_Test_Continuous");
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return context;
        }

        public static void EcsContext_Cleanup(EcsContext context)
        {
            if (!context.IsDestroyed)
                EcsContexts.DestroyContext(context);
        }

        public static void EntityComponent_AddComponent(EntityComponentArrangement arrangement, EcsContext context, Entity[] entities)
        {
            switch (arrangement)
            {
                case EntityComponentArrangement.Normal_x1:
                    for (int i = 0; i < entities.Length; i++)
                        context.AddComponent(entities[i], _component1);
                    break;
                case EntityComponentArrangement.Normal_x2:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _component1);
                        context.AddComponent(entities[i], _component2);
                    }
                    break;
                case EntityComponentArrangement.Shared_x1:
                    for (int i = 0; i < entities.Length; i++)
                        context.AddComponent(entities[i], _componentShared1);
                    break;
                case EntityComponentArrangement.Shared_x2:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _componentShared1);
                        context.AddComponent(entities[i], _componentShared2);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x1:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _component1);
                        context.AddComponent(entities[i], _componentShared1);
                    }
                    break;
                case EntityComponentArrangement.Normal_x1_Shared_x2:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _component1);
                        context.AddComponent(entities[i], _componentShared1);
                        context.AddComponent(entities[i], _componentShared2);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x1:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _component1);
                        context.AddComponent(entities[i], _component2);
                        context.AddComponent(entities[i], _componentShared1);
                    }
                    break;
                case EntityComponentArrangement.Normal_x2_Shared_x2:
                    for (int i = 0; i < entities.Length; i++)
                    {
                        context.AddComponent(entities[i], _component1);
                        context.AddComponent(entities[i], _component2);
                        context.AddComponent(entities[i], _componentShared1);
                        context.AddComponent(entities[i], _componentShared2);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
