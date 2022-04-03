using EcsLte.UnitTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    public abstract class BaseEscContext_EntityComponentGetLifeTest<TEcsContextType> : BasePrePostTest<TEcsContextType>,
        IEntityComponentGetLifeTest
        where TEcsContextType : IEcsContextType
    {
        [TestMethod]
        public void GetAllComponents_Normal() => AssertGetAllComponents(
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_NormalShared() => AssertGetAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_NormalSharedUnique() => AssertGetAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_NormalUnique() => AssertGetAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_NormalUniqueShared() => AssertGetAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_Shared() => AssertGetAllComponents(
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_SharedNormal() => AssertGetAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_SharedNormalUnique() => AssertGetAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_SharedUnique() => AssertGetAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_SharedUniqueNormal() => AssertGetAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_Unique() => AssertGetAllComponents(
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void GetAllComponents_UniqueNormal() => AssertGetAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_UniqueNormalShared() => AssertGetAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetAllComponents_UniqueShared() => AssertGetAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetAllComponents_UniqueSharedNormal() => AssertGetAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_Normal() => AssertBlueprintGetComponent(1,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Blueprint_Normal_Large() => AssertBlueprintGetComponent(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Blueprint_NormalShared() => AssertBlueprintGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_NormalShared_Large() => AssertBlueprintGetComponent(UnitTestConsts.LargeCount,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_NormalSharedUnique() => AssertBlueprintGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_NormalUnique() => AssertBlueprintGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_NormalUniqueShared() => AssertBlueprintGetComponent(1,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_Shared() => AssertBlueprintGetComponent(1,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Blueprint_Shared_Large() => AssertBlueprintGetComponent(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Blueprint_SharedNormal() => AssertBlueprintGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_SharedNormal_Large() => AssertBlueprintGetComponent(UnitTestConsts.LargeCount,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_SharedNormalUnique() => AssertBlueprintGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_SharedUnique() => AssertBlueprintGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_SharedUniqueNormal() => AssertBlueprintGetComponent(1,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_Unique() => AssertBlueprintGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void GetComponent_Blueprint_UniqueNormal() => AssertBlueprintGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_UniqueNormalShared() => AssertBlueprintGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void GetComponent_Blueprint_UniqueShared() => AssertBlueprintGetComponent(1,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void GetComponent_Blueprint_UniqueSharedNormal() => AssertBlueprintGetComponent(1,
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
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestComponent1());

            Assert.IsTrue(Context.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Normal_Never()
        {
            var entity = Context.CreateEntity();

            Assert.IsFalse(Context.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Has()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestSharedComponent1());

            Assert.IsTrue(Context.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Shared_Never()
        {
            var entity = Context.CreateEntity();

            Assert.IsFalse(Context.HasComponent<TestSharedComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Has()
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, new TestUniqueComponent1());

            Assert.IsTrue(Context.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void HasComponent_Unique_Never()
        {
            var entity = Context.CreateEntity();

            Assert.IsFalse(Context.HasComponent<TestUniqueComponent1>(entity));
        }

        [TestMethod]
        public void RemoveAllComponents_Normal() => AssertRemoveAllComponents(
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveAllComponents_NormalShared() => AssertRemoveAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_NormalSharedUnique() => AssertRemoveAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveAllComponents_NormalUnique() => AssertRemoveAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_NormalUniqueShared() => AssertRemoveAllComponents(
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveAllComponents_Shared() => AssertRemoveAllComponents(
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveAllComponents_SharedNormal() => AssertRemoveAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_SharedNormalUnique() => AssertRemoveAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveAllComponents_SharedUnique() => AssertRemoveAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_SharedUniqueNormal() => AssertRemoveAllComponents(
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveAllComponents_Unique() => AssertRemoveAllComponents(
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveAllComponents_UniqueNormal() => AssertRemoveAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_UniqueNormalShared() => AssertRemoveAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveAllComponents_UniqueShared() => AssertRemoveAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveAllComponents_UniqueSharedNormal() => AssertRemoveAllComponents(
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_Normal() => AssertRemoveComponent(1, false,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_Normal_Large() => AssertRemoveComponent(UnitTestConsts.LargeCount, false,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_Normal_Large_Reverse() => AssertRemoveComponent(UnitTestConsts.LargeCount, true,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_NormalShared() => AssertRemoveComponent(1, false,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_NormalShared_Large() => AssertRemoveComponent(UnitTestConsts.LargeCount, false,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_NormalShared_Large_Reverse() => AssertRemoveComponent(UnitTestConsts.LargeCount, true,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_NormalSharedUnique() => AssertRemoveComponent(1, false,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_NormalUnique() => AssertRemoveComponent(1, false,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_NormalUniqueShared() => AssertRemoveComponent(1, false,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_Shared() => AssertRemoveComponent(1, false,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_Shared_Large() => AssertRemoveComponent(UnitTestConsts.LargeCount, false,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_Shared_Large_Reverse() => AssertRemoveComponent(UnitTestConsts.LargeCount, true,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_SharedNormal() => AssertRemoveComponent(1, false,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_SharedNormal_Large() => AssertRemoveComponent(UnitTestConsts.LargeCount, false,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_SharedNormal_Large_Reverse() => AssertRemoveComponent(UnitTestConsts.LargeCount, true,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_SharedNormalUnique() => AssertRemoveComponent(1, false,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_SharedUnique() => AssertRemoveComponent(1, false,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_SharedUniqueNormal() => AssertRemoveComponent(1, false,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_Unique() => AssertRemoveComponent(1, false,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void RemoveComponent_UniqueNormal() => AssertRemoveComponent(1, false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_UniqueNormalShared() => AssertRemoveComponent(1, false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void RemoveComponent_UniqueShared() => AssertRemoveComponent(1, false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void RemoveComponent_UniqueSharedNormal() => AssertRemoveComponent(1, false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_Normal_Add() => AssertReplaceComponent(false,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_Normal_Replace() => AssertReplaceComponent(true,
                new TestComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_NormalShared_Add() => AssertReplaceComponent(false,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_NormalShared_Replace() => AssertReplaceComponent(true,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_NormalSharedUnique_Add() => AssertReplaceComponent(false,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_NormalSharedUnique_Replace() => AssertReplaceComponent(true,
                new TestComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_NormalUnique_Add() => AssertReplaceComponent(false,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_NormalUnique_Replace() => AssertReplaceComponent(true,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_NormalUniqueShared_Add() => AssertReplaceComponent(false,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_NormalUniqueShared_Replace() => AssertReplaceComponent(true,
                new TestComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_Shared_Add() => AssertReplaceComponent(false,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_Shared_Replace() => AssertReplaceComponent(true,
                new TestSharedComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_SharedNormal_Add() => AssertReplaceComponent(false,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_SharedNormal_Replace() => AssertReplaceComponent(true,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_SharedNormalUnique_Add() => AssertReplaceComponent(false,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_SharedNormalUnique_Replace() => AssertReplaceComponent(true,
                new TestSharedComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestUniqueComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_SharedUnique_Add() => AssertReplaceComponent(false,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_SharedUnique_Replace() => AssertReplaceComponent(true,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_SharedUniqueNormal_Add() => AssertReplaceComponent(false,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_SharedUniqueNormal_Replace() => AssertReplaceComponent(true,
                new TestSharedComponent1 { Prop = 1 },
                new TestUniqueComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_Unique_Add() => AssertReplaceComponent(false,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_Unique_Replace() => AssertReplaceComponent(true,
                new TestUniqueComponent1 { Prop = 1 });

        [TestMethod]
        public void ReplaceComponent_UniqueNormal_Add() => AssertReplaceComponent(false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_UniqueNormal_Replace() => AssertReplaceComponent(true,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_UniqueNormalShared_Add() => AssertReplaceComponent(false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_UniqueNormalShared_Replace() => AssertReplaceComponent(true,
                new TestUniqueComponent1 { Prop = 1 },
                new TestComponent1 { Prop = 2 },
                new TestSharedComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_UniqueShared_Add() => AssertReplaceComponent(false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_UniqueShared_Replace() => AssertReplaceComponent(true,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 });

        [TestMethod]
        public void ReplaceComponent_UniqueSharedNormal_Add() => AssertReplaceComponent(false,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void ReplaceComponent_UniqueSharedNormal_Replace() => AssertReplaceComponent(true,
                new TestUniqueComponent1 { Prop = 1 },
                new TestSharedComponent1 { Prop = 2 },
                new TestComponent1 { Prop = 3 });

        [TestMethod]
        public void Unique_GetUniqueComponent()
        {
            Context.AddUniqueComponent(new TestUniqueComponent1 { Prop = 1 });

            Assert.IsTrue(Context.GetUniqueComponent<TestUniqueComponent1>().Prop == 1);
        }

        [TestMethod]
        public void Unique_GetUniqueEntity()
        {
            var entity = Context.AddUniqueComponent(new TestUniqueComponent1());

            Assert.IsTrue(entity == Context.GetUniqueEntity<TestUniqueComponent1>());
        }

        [TestMethod]
        public void Unique_HasUniqueComponent()
        {
            Context.AddUniqueComponent(new TestUniqueComponent1 { Prop = 1 });

            Assert.IsTrue(Context.HasUniqueComponent<TestUniqueComponent1>());
        }

        private void AssertGetAllComponents<T1>(T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 1);
            Assert.IsTrue(components[0] is T1);
            Assert.IsTrue(((T1)components[0]).Prop == component1.Prop);
        }

        private void AssertGetAllComponents<T1, T2>(T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);
            Context.AddComponent(entity, component2);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 2);
            var isOk = new int[2];
            for (var i = 0; i < isOk.Length; i++)
            {
                if (components[i] is T1)
                    isOk[0] += ((T1)components[i]).Prop == component1.Prop ? 1 : 0;
                if (components[i] is T2)
                    isOk[1] += ((T2)components[i]).Prop == component2.Prop ? 1 : 0;
            }
            for (var i = 0; i < isOk.Length; i++)
                Assert.IsTrue(isOk[i] == 1);
        }

        private void AssertGetAllComponents<T1, T2, T3>(T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);
            Context.AddComponent(entity, component2);
            Context.AddComponent(entity, component3);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 3);
            var isOk = new int[3];
            for (var i = 0; i < isOk.Length; i++)
            {
                if (components[i] is T1)
                    isOk[0] += ((T1)components[i]).Prop == component1.Prop ? 1 : 0;
                if (components[i] is T2)
                    isOk[1] += ((T2)components[i]).Prop == component2.Prop ? 1 : 0;
                if (components[i] is T3)
                    isOk[2] += ((T3)components[i]).Prop == component3.Prop ? 1 : 0;
            }
            for (var i = 0; i < isOk.Length; i++)
                Assert.IsTrue(isOk[i] == 1);
        }

        private void AssertBlueprintGetComponent<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var blueprint = GetBlueprint();
            blueprint.AddComponent(component1);

            var entities = Context.CreateEntities(entityCount, blueprint);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertBlueprintGetComponent<T1, T2>(int entityCount, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var blueprint = GetBlueprint();
            blueprint.AddComponent(component1);
            blueprint.AddComponent(component2);

            var entities = Context.CreateEntities(entityCount, blueprint);

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.GetComponent<T1>(entities[i]).Prop == component1.Prop,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[i]).Prop == component2.Prop,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        private void AssertBlueprintGetComponent<T1, T2, T3>(int entityCount, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var blueprint = GetBlueprint();
            blueprint.AddComponent(component1);
            blueprint.AddComponent(component2);
            blueprint.AddComponent(component3);

            var entities = Context.CreateEntities(entityCount, blueprint);

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

        private void AssertGetComponent<T1>(int entityCount, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
            }

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
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
                Context.AddComponent(entities[i], component2);
            }

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
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
                Context.AddComponent(entities[i], component2);
                Context.AddComponent(entities[i], component3);
            }

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

        private void AssertRemoveAllComponents<T1>(T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);

            Context.RemoveAllComponents(entity);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 0);
        }

        private void AssertRemoveAllComponents<T1, T2>(T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);
            Context.AddComponent(entity, component2);

            Context.RemoveAllComponents(entity);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 0);
        }

        private void AssertRemoveAllComponents<T1, T2, T3>(T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            Context.AddComponent(entity, component1);
            Context.AddComponent(entity, component2);
            Context.AddComponent(entity, component3);

            Context.RemoveAllComponents(entity);

            var components = Context.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 0);
        }

        private void AssertRemoveComponent<T1>(int entityCount, bool reverse, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
            }

            var id = reverse ? entities.Length - 1 : 0;
            var changeId = reverse ? -1 : 1;
            for (var i = 0; i < entities.Length; i++, id += changeId)
            {
                Context.RemoveComponent<T1>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T1>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
            }
        }

        private void AssertRemoveComponent<T1, T2>(int entityCount, bool reverse, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
                Context.AddComponent(entities[i], component2);
            }

            var id = reverse ? entities.Length - 1 : 0;
            var changeId = reverse ? -1 : 1;
            for (var i = 0; i < entities.Length; i++, id += changeId)
            {
                Context.RemoveComponent<T1>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T1>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[id]).Prop == component2.Prop,
                    $"Enity.Id {entities[id].Id}");

                Context.RemoveComponent<T2>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T2>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
            }
        }

        private void AssertRemoveComponent<T1, T2, T3>(int entityCount, bool reverse, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entities = Context.CreateEntities(entityCount);
            for (var i = 0; i < entities.Length; i++)
            {
                Context.AddComponent(entities[i], component1);
                Context.AddComponent(entities[i], component2);
                Context.AddComponent(entities[i], component3);
            }

            var id = reverse ? entities.Length - 1 : 0;
            var changeId = reverse ? -1 : 1;
            for (var i = 0; i < entities.Length; i++, id += changeId)
            {
                Context.RemoveComponent<T1>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T1>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
                Assert.IsTrue(Context.GetComponent<T2>(entities[id]).Prop == component2.Prop,
                    $"Enity.Id {entities[id].Id}");
                Assert.IsTrue(Context.GetComponent<T3>(entities[id]).Prop == component3.Prop,
                    $"Enity.Id {entities[id].Id}");

                Context.RemoveComponent<T2>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T2>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
                Assert.IsTrue(Context.GetComponent<T3>(entities[id]).Prop == component3.Prop,
                    $"Enity.Id {entities[id].Id}");

                Context.RemoveComponent<T3>(entities[id]);
                Assert.IsFalse(Context.HasComponent<T3>(entities[id]),
                    $"Enity.Id {entities[id].Id}");
            }
        }

        private void AssertReplaceComponent<T1>(bool replace, T1 component1)
            where T1 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            if (replace)
                Context.AddComponent(entity, new T1());
            Context.ReplaceComponent(entity, component1);

            Assert.IsTrue(Context.GetComponent<T1>(entity).Prop == component1.Prop);
        }

        private void AssertReplaceComponent<T1, T2>(bool replace, T1 component1, T2 component2)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            if (replace)
            {
                Context.AddComponent(entity, new T1());
                Context.AddComponent(entity, new T2());
            }
            Context.ReplaceComponent(entity, component1);
            Context.ReplaceComponent(entity, component2);

            Assert.IsTrue(Context.GetComponent<T1>(entity).Prop == component1.Prop);
            Assert.IsTrue(Context.GetComponent<T2>(entity).Prop == component2.Prop);
        }

        private void AssertReplaceComponent<T1, T2, T3>(bool replace, T1 component1, T2 component2, T3 component3)
            where T1 : unmanaged, IComponent, ITestComponent
            where T2 : unmanaged, IComponent, ITestComponent
            where T3 : unmanaged, IComponent, ITestComponent
        {
            var entity = Context.CreateEntity();
            if (replace)
            {
                Context.AddComponent(entity, new T1());
                Context.AddComponent(entity, new T2());
                Context.AddComponent(entity, new T3());
            }
            Context.ReplaceComponent(entity, component1);
            Context.ReplaceComponent(entity, component2);
            Context.ReplaceComponent(entity, component3);

            Assert.IsTrue(Context.GetComponent<T1>(entity).Prop == component1.Prop);
            Assert.IsTrue(Context.GetComponent<T2>(entity).Prop == component2.Prop);
            Assert.IsTrue(Context.GetComponent<T3>(entity).Prop == component3.Prop);
        }
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed
{

    [TestClass]
    public class EcsContext_Managed_EntityComponentGetLifeTest : BaseEscContext_EntityComponentGetLifeTest<EcsContextType_Managed>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Managed.ArcheType
{

    [TestClass]
    public class EcsContext_Managed_ArcheType_EntityComponentGetLifeTest : BaseEscContext_EntityComponentGetLifeTest<EcsContextType_Managed_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native
{

    [TestClass]
    public class EcsContext_Native_EntityComponentGetLifeTest : BaseEscContext_EntityComponentGetLifeTest<EcsContextType_Native>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType
{

    [TestClass]
    public class EcsContext_Native_ArcheType_EntityComponentGetLifeTest : BaseEscContext_EntityComponentGetLifeTest<EcsContextType_Native_ArcheType>
    {
    }
}

namespace EcsLte.UnitTest.EcsContextTests.Native.ArcheType.Continuous
{

    [TestClass]
    public class EcsContext_Native_ArcheType_Continuous_EntityComponentGetLifeTest : BaseEscContext_EntityComponentGetLifeTest<EcsContextType_Native_ArcheType_Continuous>
    {
    }
}
