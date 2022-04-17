using EcsLte.Exceptions;
using EcsLte.HybridArcheType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextHybridTests
{
    [TestClass]
    public class EntityBlueprint_HybridTests
    {
        [TestMethod]
        public void HasComponent()
        {
            var blueprint = new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1());

            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
            Assert.IsFalse(blueprint.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void GetComponent()
        {
            var blueprint = new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == 1);
            Assert.ThrowsException<EntityBlueprintNotHaveComponentException>(() =>
                blueprint.GetComponent<TestComponent2>());
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var blueprint = new EntityBlueprint_Hybrid()
                .AddComponent(new TestComponent1 { Prop = 1 })
                .RemoveComponent<TestComponent1>();

            Assert.IsFalse(blueprint.HasComponent<TestComponent1>());
            Assert.ThrowsException<EntityBlueprintNotHaveComponentException>(() =>
                blueprint.RemoveComponent<TestComponent1>());
        }
    }
}
