using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.EntityBlueprintTests
{
    [TestClass]
    public class EntityBlueprint_ComponentTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
        }

        [TestMethod]
        public void HasComponent_Never()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            Assert.IsFalse(blueprint.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void GetComponent_Never()
        {
            Assert.ThrowsException<EntityBlueprintNotHaveComponentException>(() =>
                new EntityBlueprint().GetComponent<TestComponent1>());
        }

        [TestMethod]
        public void GetComponent_Single()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void GetComponent_Multiple()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var component3 = new TestSharedComponent1 { Prop = 3 };
            var component4 = new TestSharedComponent2 { Prop = 4 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3)
                .AddComponent(component4);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestComponent2>().Prop == component2.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent1>().Prop == component3.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent2>().Prop == component4.Prop);
        }

        [TestMethod]
        public void AddComponent_Duplicate()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            Assert.ThrowsException<EntityBlueprintAlreadyHasComponentException>(() =>
                blueprint.AddComponent(new TestComponent1()));
        }

        [TestMethod]
        public void AddComponent_Single()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
        }

        [TestMethod]
        public void AddComponent_Multiple()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var component3 = new TestSharedComponent1 { Prop = 3 };
            var component4 = new TestSharedComponent2 { Prop = 4 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3)
                .AddComponent(component4);

            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
            Assert.IsTrue(blueprint.HasComponent<TestComponent2>());
            Assert.IsTrue(blueprint.HasComponent<TestSharedComponent1>());
            Assert.IsTrue(blueprint.HasComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void RemoveComponent_Never()
        {
            Assert.ThrowsException<EntityBlueprintNotHaveComponentException>(() =>
                new EntityBlueprint().RemoveComponent<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponent_Single()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1())
                .RemoveComponent<TestComponent1>();

            Assert.IsFalse(blueprint.HasComponent<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponent_Multiple()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var component3 = new TestSharedComponent1 { Prop = 3 };
            var component4 = new TestSharedComponent2 { Prop = 4 };
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3)
                .AddComponent(component4)
                .RemoveComponent<TestComponent1>()
                .RemoveComponent<TestComponent2>()
                .RemoveComponent<TestSharedComponent1>()
                .RemoveComponent<TestSharedComponent2>();

            Assert.IsFalse(blueprint.HasComponent<TestComponent1>());
            Assert.IsFalse(blueprint.HasComponent<TestComponent2>());
            Assert.IsFalse(blueprint.HasComponent<TestSharedComponent1>());
            Assert.IsFalse(blueprint.HasComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void UpdateComponent_Single_Never()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var blueprint = new EntityBlueprint()
                .UpdateComponent(component1);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void UpdateComponent_Single_Has()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1())
                .UpdateComponent(component1);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
        }

        [TestMethod]
        public void UpdateComponent_Multiple_Never()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var component3 = new TestSharedComponent1 { Prop = 3 };
            var component4 = new TestSharedComponent2 { Prop = 4 };
            var blueprint = new EntityBlueprint()
                .UpdateComponent(component1)
                .UpdateComponent(component2)
                .UpdateComponent(component3)
                .UpdateComponent(component4);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestComponent2>().Prop == component2.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent1>().Prop == component3.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent2>().Prop == component4.Prop);
        }

        [TestMethod]
        public void UpdateComponent_Multiple_Has()
        {
            var component1 = new TestComponent1 { Prop = 1 };
            var component2 = new TestComponent2 { Prop = 2 };
            var component3 = new TestSharedComponent1 { Prop = 3 };
            var component4 = new TestSharedComponent2 { Prop = 4 };
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1())
                .AddComponent(new TestComponent2())
                .AddComponent(new TestSharedComponent1())
                .AddComponent(new TestSharedComponent2())
                .UpdateComponent(component1)
                .UpdateComponent(component2)
                .UpdateComponent(component3)
                .UpdateComponent(component4);

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component1.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestComponent2>().Prop == component2.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent1>().Prop == component3.Prop);
            Assert.IsTrue(blueprint.GetComponent<TestSharedComponent2>().Prop == component4.Prop);
        }
    }
}
