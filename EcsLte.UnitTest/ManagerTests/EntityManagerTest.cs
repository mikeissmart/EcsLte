using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class EntityManagerTest : BasePrePostTest
    {
        private TestComponent1 _testComponent1_1 = new TestComponent1 { Prop = 1 };
        private TestSharedComponent1 _testSharedComponent1_1 = new TestSharedComponent1 { Prop = 2 };
        private TestUniqueComponent1 _testUniqueComponent1_1 = new TestUniqueComponent1 { Prop = 3 };
        private TestComponent1 _testComponent1_2 = new TestComponent1 { Prop = 4 };
        private TestSharedComponent1 _testSharedComponent1_2 = new TestSharedComponent1 { Prop = 5 };
        private TestUniqueComponent1 _testUniqueComponent1_2 = new TestUniqueComponent1 { Prop = 6 };

        [TestMethod]
        public void CreateEntity()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 1);
        }

        [TestMethod]
        public void CreateEntity_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.CreateEntity(new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntity_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = new Entity[UnitTestConsts.LargeCount];
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.EntityManager.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount == entities.Length);
            for (int i = 0, id = 1; i < entities.Length; i++, id++)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntity_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entity = Context.EntityManager.CreateEntity(blueprint);
            Context.EntityManager.DestroyEntity(entity);
            entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            Assert.IsTrue(entity.Id == 1);
            Assert.IsTrue(entity.Version == 2);
        }

        [TestMethod]
        public void CreateEntity_Reuse_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, blueprint);
            Context.EntityManager.DestroyEntities(entities);
            for (var i = 0; i < entities.Length; i++)
                entities[i] = Context.EntityManager.CreateEntity(blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount == entities.Length);
            for (int i = 0, id = entities.Length; i < entities.Length; i++, id--)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities()
        {
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 1);
        }

        [TestMethod]
        public void CreateEntities_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                    .AddComponent(new TestComponent1())));
        }

        [TestMethod]
        public void CreateEntities_Large()
        {
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == entities.Length);
            for (int i = 0, id = 1; i < entities.Length; i++, id++)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 1,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void CreateEntities_Reuse()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.EntityManager.CreateEntities(1, blueprint);
            Context.EntityManager.DestroyEntities(entities);
            entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.EntityCount == 1);
            Assert.IsTrue(entities[0].Id == 1);
            Assert.IsTrue(entities[0].Version == 2);
        }

        [TestMethod]
        public void CreateEntities_Reuse_Large()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, blueprint);
            Context.EntityManager.DestroyEntities(entities);
            entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, blueprint);

            Assert.IsTrue(Context.EntityManager.EntityCount == entities.Length);
            for (int i = 0, id = entities.Length; i < entities.Length; i++, id--)
            {
                Assert.IsTrue(entities[i].Id == id,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(entities[i].Version == 2,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntity()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.DestroyEntity(entity);

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            Assert.IsFalse(Context.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void DestroyEntity_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntity(Entity.Null));
        }

        [TestMethod]
        public void DestroyEntity_Large()
        {
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            for (var i = 0; i < entities.Length; i++)
                Context.EntityManager.DestroyEntity(entities[i]);

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntity_Never() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                               Context.EntityManager.DestroyEntity(Entity.Null));

        [TestMethod]
        public void DestroyEntities()
        {
            var entities = Context.EntityManager.CreateEntities(1, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_Destroy()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.DestroyEntities(new Entity[0]));
        }

        [TestMethod]
        public void DestroyEntities_Large()
        {
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));
            Context.EntityManager.DestroyEntities(entities);

            Assert.IsTrue(Context.EntityManager.EntityCount == 0);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsFalse(Context.EntityManager.HasEntity(entities[i]),
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void DestroyEntities_Never() => Assert.ThrowsException<EntityDoesNotExistException>(() =>
                                                 Context.EntityManager.DestroyEntities(new[] { Entity.Null }));

        [TestMethod]
        public void HasEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasEntity(Entity.Null));
        }

        [TestMethod]
        public void HasEntity_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void HasEntity_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            Assert.IsTrue(Context.EntityManager.HasEntity(entity));
        }

        [TestMethod]
        public void GetEntities()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.EntityManager.GetEntities();

            Assert.IsTrue(getEntities.Length == 1);
            Assert.IsTrue(getEntities[0] == entity);
        }

        [TestMethod]
        public void GetEntities_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetEntities());
        }

        [TestMethod]
        public void GetEntities_Large()
        {
            var entities = Context.EntityManager.CreateEntities(UnitTestConsts.LargeCount, new EntityBlueprint()
                .AddComponent(new TestComponent1()));

            var getEntities = Context.EntityManager.GetEntities();

            Assert.IsTrue(getEntities.Length == entities.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(getEntities[i] == entities[i],
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void GetAllComponents_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetAllComponents(Entity.Null));
        }

        [TestMethod]
        public void GetAllComponents_Normal() => AssertGetAllComponents(1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_Normal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalShared() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalShared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalSharedUnique() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalUnique() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_NormalUniqueShared() => AssertGetAllComponents(1,
                _testComponent1_1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_Shared() => AssertGetAllComponents(1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_Shared_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormal() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormal_Large() => AssertGetAllComponents(UnitTestConsts.LargeCount,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedNormalUnique() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedUnique() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_SharedUniqueNormal() => AssertGetAllComponents(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_Unique() => AssertGetAllComponents(1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueNormal() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueNormalShared() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testComponent1_1,
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_UniqueShared() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetAllComponents_UniqueSharedNormal() => AssertGetAllComponents(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void GetComponent_Normal() => AssertGetComponent(1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Normal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_NormalShared() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_NormalShared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_NormalSharedUnique() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_NormalUnique() => AssertGetComponent(1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_NormalUniqueShared() => AssertGetComponent(1,
                _testComponent1_1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_Shared() => AssertGetComponent(1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_Shared_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_SharedNormal() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_SharedNormal_Large() => AssertGetComponent(UnitTestConsts.LargeCount,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_SharedNormalUnique() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_SharedUnique() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_SharedUniqueNormal() => AssertGetComponent(1,
                _testSharedComponent1_1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_Unique() => AssertGetComponent(1,
                _testUniqueComponent1_1);

        [TestMethod]
        public void GetComponent_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.GetComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void GetComponent_UniqueNormal() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueNormalShared() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueShared() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1);

        [TestMethod]
        public void GetComponent_UniqueSharedNormal() => AssertGetComponent(1,
                _testUniqueComponent1_1,
                _testSharedComponent1_1,
                _testComponent1_1);

        [TestMethod]
        public void HasComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasComponent<TestComponent1>(Entity.Null));
        }

        [TestMethod]
        public void HasComponent_Normal_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Has()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testUniqueComponent1_1));

            Assert.IsTrue(Context.EntityManager.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.IsFalse(Context.EntityManager.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void Unique_GetUniqueComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.GetUniqueComponent<TestUniqueComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_GetUniqueEntity()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.GetUniqueEntity<TestUniqueComponent1>() == entity);
        }

        [TestMethod]
        public void Unique_HasUniqueComponent_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_HasUniqueComponent()
        {
            var component = new TestUniqueComponent1 { Prop = 1 };
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(component));

            Assert.IsTrue(Context.EntityManager.HasUniqueComponent<TestUniqueComponent1>());
        }

        [TestMethod]
        public void UpdateComponent_Entity_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(Entity.Null, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_Normal() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Normal_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_NormalShared() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalShared_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalSharedUnique() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalUnique() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_NormalUniqueShared() => AssertUpdateComponent_Entity(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testSharedComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormal() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormal_Large() => AssertUpdateComponent_Entity(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedNormalUnique() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedUnique() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_SharedUniqueNormal() => AssertUpdateComponent_Entity(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Unique() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(entity, _testUniqueComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entity_UniqueNormal() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueNormalShared() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueShared() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entity_UniqueSharedNormal() => AssertUpdateComponent_Entity(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.EntityManager.UpdateComponent(new[] { Entity.Null }, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_Normal() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Normal_Large() => AssertUpdateComponent_Entities(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Normal_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testSharedComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_NormalShared() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalShared_Large() => AssertUpdateComponent_Entities(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalSharedUnique() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalUnique() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_NormalUniqueShared() => AssertUpdateComponent_Entities(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Shared() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Shared_Large() => AssertUpdateComponent_Entities(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Shared_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testSharedComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_SharedNormal() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedNormal_Large() => AssertUpdateComponent_Entities(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedNormalUnique() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedUnique() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_SharedUniqueNormal() => AssertUpdateComponent_Entities(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Unique() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_Unique_Never()
        {
            var entity = Context.EntityManager.CreateEntity(new EntityBlueprint()
                .AddComponent(_testComponent1_1));

            Assert.ThrowsException<EntityNotHaveComponentException>(() =>
                Context.EntityManager.UpdateComponent(new[] { entity }, _testUniqueComponent1_1));
        }

        [TestMethod]
        public void UpdateComponent_Entities_UniqueNormal() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueNormalShared() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueShared() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_Entities_UniqueSharedNormal() => AssertUpdateComponent_Entities(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Normal() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Normal_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalShared() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalShared_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalSharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_NormalUniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_2,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Shared() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Shared_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormal() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormal_Large() => AssertUpdateComponent_EntityQuery(UnitTestConsts.LargeCount,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedNormalUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUnique() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_SharedUniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testSharedComponent1_1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_2,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_Unique() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testUniqueComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueNormalShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueShared() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2);

        [TestMethod]
        public void UpdateComponent_EntityQuery_UniqueSharedNormal() => AssertUpdateComponent_EntityQuery(1,
            _testUniqueComponent1_1,
            _testSharedComponent1_1,
            _testComponent1_1,
            _testUniqueComponent1_2,
            _testSharedComponent1_2,
            _testComponent1_2);

        private void AssertGetAllComponents<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
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
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[2];
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
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
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            var entitiesIsOk = new int[entityCount][];
            for (var i = 0; i < entitiesIsOk.Length; i++)
            {
                var isOk = new int[3];
                var entity = entities[i];
                var components = Context.EntityManager.GetAllComponents(entity);
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

        private void AssertGetComponent<T1>(int entityCount,
            T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2>(int entityCount,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertGetComponent<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == component3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            for (var i = 0; i < entities.Length; i++)
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entity<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);
            Context.EntityManager.UpdateComponent(entities, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_Entities<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);

            Context.EntityManager.UpdateComponent(entities, updateComponent1);
            Context.EntityManager.UpdateComponent(entities, updateComponent2);
            Context.EntityManager.UpdateComponent(entities, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1>(int entityCount,
            T1 component1,
            T1 updateComponent1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1);
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<T1>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2>(int entityCount,
            T1 component1,
            T2 component2,
            T1 updateComponent1,
            T2 updateComponent2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2);
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<T1, T2>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);
            Context.EntityManager.UpdateComponent(query, updateComponent2);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertUpdateComponent_EntityQuery<T1, T2, T3>(int entityCount,
            T1 component1,
            T2 component2,
            T3 component3,
            T1 updateComponent1,
            T2 updateComponent2,
            T3 updateComponent3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = TestCreateEntities(Context, entityCount,
                component1,
                component2,
                component3);
            var query = Context.QueryManager.CreateQuery()
                .WhereAllOf<T1, T2, T3>();

            Context.EntityManager.UpdateComponent(query, updateComponent1);
            Context.EntityManager.UpdateComponent(query, updateComponent2);
            Context.EntityManager.UpdateComponent(query, updateComponent3);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.EntityManager.UpdateComponent(entities[i], updateComponent1);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent2);
                Context.EntityManager.UpdateComponent(entities[i], updateComponent3);
            }

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.EntityManager.GetComponent<T1>(entities[i]).Prop == updateComponent1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T2>(entities[i]).Prop == updateComponent2.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<T3>(entities[i]).Prop == updateComponent3.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }
    }
}
