using EcsLte.HybridArcheType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextHybridTests
{
    [TestClass]
    public class EscContext_Hybrid_EntityComponentGetLifeTest
    {
        public EcsContext_Hybrid Context { get; private set; }

        [TestInitialize]
        public void PreTest() => Context = EcsContexts.CreateEcsContext_Hybrid("Test_Hybrid");

        [TestCleanup]
        public void PostTest()
        {
            if (!Context.IsDestroyed)
                EcsContexts.DestroyContext_Hybrid(Context);
            Context = null;
        }

        [TestMethod]
        public void GetAllComponents_Normal() => AssertGetAllComponents(1,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_Normal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_NormalShared() => AssertGetAllComponents(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_NormalShared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_NormalSharedUnique() => AssertGetAllComponents(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_NormalUnique() => AssertGetAllComponents(1,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_NormalUniqueShared() => AssertGetAllComponents(1,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_Shared() => AssertGetAllComponents(1,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_Shared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_SharedNormal() => AssertGetAllComponents(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_SharedNormal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_SharedNormalUnique() => AssertGetAllComponents(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_SharedUnique() => AssertGetAllComponents(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_SharedUniqueNormal() => AssertGetAllComponents(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_Unique() => AssertGetAllComponents(1,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_UniqueNormal() => AssertGetAllComponents(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_UniqueNormalShared() => AssertGetAllComponents(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_UniqueShared() => AssertGetAllComponents(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_UniqueSharedNormal() => AssertGetAllComponents(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Normal() => AssertGetComponent(1,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Normal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_NormalShared() => AssertGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_NormalShared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_NormalSharedUnique() => AssertGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_NormalUnique() => AssertGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_NormalUniqueShared() => AssertGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Shared() => AssertGetComponent(1,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Shared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_SharedNormal() => AssertGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_SharedNormal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_SharedNormalUnique() => AssertGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_SharedUnique() => AssertGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_SharedUniqueNormal() => AssertGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Unique() => AssertGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_UniqueNormal() => AssertGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_UniqueNormalShared() => AssertGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_UniqueShared() => AssertGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_UniqueSharedNormal() => AssertGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void HasComponent_Normal_Has()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Normal_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent2()));

            Assert.IsFalse(Context.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Has()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestSharedComponent1()));

            Assert.IsTrue(Context.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestSharedComponent2()));

            Assert.IsFalse(Context.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Has()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestUniqueComponent1()));

            Assert.IsTrue(Context.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Never()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestUniqueComponent2()));

            Assert.IsFalse(Context.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void Unique_GetUniqueComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestUniqueComponent1 { Prop = 1 }));

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == 1);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestUniqueComponent1()));

            Assert.IsTrue(entity == Context.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_HasUniqueComponent()
        {
            var entity = Context.CreateEntity(new EntityBlueprint_Hybrid()
                .AddComponent(new TestUniqueComponent1()));

            Assert.IsTrue(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        private void AssertGetAllComponents<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1));

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var components = Context.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 1,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(components[0] is T1,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(((T1)components[0]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetAllComponents<T1, T2>(int entityCount, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1)
                .AddComponent(component2));

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[2];
                var entity = entities[i];
                var components = Context.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 2);
                for (var j = 0; j < components.Length; j++)
                {
                    if (components[j] is T1)
                        isOk[0] += ((T1)components[j]).Prop == component1.Prop ? 1 : 0;
                    if (components[j] is T2)
                        isOk[1] += ((T2)components[j]).Prop == component2.Prop ? 1 : 0;
                }

                entitiesIsOk[i] = isOk;
            }

            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = entitiesIsOk[i];
                for (var j = 0; j < isOk.Length; j++)
                {
                    Assert.IsTrue(isOk[j] == 1,
                        $"Enity.Id {entities[i].Id}");
                }
            }
        }

        private void AssertGetAllComponents<T1, T2, T3>(int entityCount, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3));

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[3];
                var entity = entities[i];
                var components = Context.GetAllComponents(entity);
                Assert.IsTrue(components.Length == 3);
                for (var j = 0; j < components.Length; j++)
                {
                    if (components[j] is T1)
                        isOk[0] += ((T1)components[j]).Prop == component1.Prop ? 1 : 0;
                    if (components[j] is T2)
                        isOk[1] += ((T2)components[j]).Prop == component2.Prop ? 1 : 0;
                    if (components[j] is T3)
                        isOk[2] += ((T3)components[j]).Prop == component3.Prop ? 1 : 0;
                }

                entitiesIsOk[i] = isOk;
            }

            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = entitiesIsOk[i];
                for (var j = 0; j < isOk.Length; j++)
                {
                    Assert.IsTrue(isOk[j] == 1,
                        $"Enity.Id {entities[i].Id}");
                }
            }
        }

        private void AssertGetComponent<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1));

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2>(int entityCount, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1)
                .AddComponent(component2));

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2, T3>(int entityCount, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount, new EntityBlueprint_Hybrid()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3));

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T3>(entities[i]).Prop == component3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
