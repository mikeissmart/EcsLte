using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityBluprintTests
{
    [TestClass]
    public class EntityBlueprint_EntityBlueprint
    {
        [TestMethod]
        public void Clone()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            var clone = blueprint.Clone();
            clone.AddComponent(new TestComponent2());

            // Correct clone
            Assert.IsFalse(blueprint.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void HasComponent()
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            // Correct component
            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
        }

        [TestMethod]
        public void GetComponent()
        {
            var component = new TestComponent1 {Prop = 1};
            var blueprint = new EntityBlueprint()
                .AddComponent(component);

            // Correct component
            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component.Prop);
        }

        [TestMethod]
        public void AddComponent()
        {
            var component = new TestComponent1 {Prop = 1};

            var blueprint = new EntityBlueprint()
                .AddComponent(component);

            // Correct component
            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component.Prop);
            // Cannot add same component again
            Assert.ThrowsException<BlueprintAlreadyHasComponentException>(() =>
                blueprint.AddComponent(component));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var component = new TestComponent1 {Prop = 1};
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            blueprint.ReplaceComponent(component);

            // Correct component
            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component.Prop);
            // Replace also adds component
            var component2 = new TestComponent1 {Prop = 2};
            blueprint.ReplaceComponent(component2);
            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == component2.Prop);
        }

        [TestMethod]
        public void RemoveAllComponents()
        {
            var component = new TestComponent1 {Prop = 1};
            var blueprint = new EntityBlueprint()
                .AddComponent(new TestComponent1());

            blueprint.RemoveAllComponents();

            // Correct component
            Assert.IsFalse(blueprint.HasComponent<TestComponent1>());
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var component = new TestComponent1 {Prop = 1};
            var blueprint = new EntityBlueprint()
                .AddComponent(component);

            blueprint.RemoveComponent<TestComponent1>();

            // Correct component
            Assert.IsFalse(blueprint.HasComponent<TestComponent1>());
            // Cannot remove same component again
            Assert.ThrowsException<BlueprintNotHaveComponentException>(() =>
                blueprint.RemoveComponent<TestComponent1>());
        }
    }
}