using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityBlueprintTests
{
    [TestClass]
    public class EntityBlueprintTest : BasePrePostTest
    {
        [TestMethod]
        public void HasComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1());

            Assert.IsTrue(blueprint.HasComponent<TestComponent1>());
            Assert.IsFalse(blueprint.HasComponent<TestComponent2>());
        }

        [TestMethod]
        public void HasSharedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1());

            Assert.IsTrue(blueprint.HasSharedComponent<TestSharedComponent1>());
            Assert.IsFalse(blueprint.HasSharedComponent<TestSharedComponent2>());
        }

        [TestMethod]
        public void HasManagedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetManagedComponent(new TestManagedComponent1());

            Assert.IsTrue(blueprint.HasManagedComponent<TestManagedComponent1>());
            Assert.IsFalse(blueprint.HasManagedComponent<TestManagedComponent2>());
        }

        [TestMethod]
        public void GetComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.GetComponent<TestComponent1>().Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                new EntityBlueprint().GetComponent<TestComponent2>());
        }

        [TestMethod]
        public void SetComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.Components.Length == 1);
            Assert.IsTrue(blueprint.SharedComponents.Length == 0);
            Assert.IsTrue(blueprint.ManagedComponents.Length == 0);
            Assert.IsTrue(blueprint.Components[0].GetType() == typeof(TestComponent1));
        }

        [TestMethod]
        public void GetSharedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.GetSharedComponent<TestSharedComponent1>().Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                new EntityBlueprint().GetComponent<TestComponent2>());
        }

        [TestMethod]
        public void SetSharedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.Components.Length == 0);
            Assert.IsTrue(blueprint.SharedComponents.Length == 1);
            Assert.IsTrue(blueprint.ManagedComponents.Length == 0);
            Assert.IsTrue(blueprint.SharedComponents[0].GetType() == typeof(TestSharedComponent1));
        }

        [TestMethod]
        public void GetManagedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetManagedComponent(new TestManagedComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.GetManagedComponent<TestManagedComponent1>().Prop == 1);

            Assert.ThrowsException<ComponentNotHaveException>(() =>
                new EntityBlueprint().GetComponent<TestComponent2>());
        }

        [TestMethod]
        public void AddManagedComponent()
        {
            var blueprint = new EntityBlueprint()
                .SetManagedComponent(new TestManagedComponent1 { Prop = 1 });

            Assert.IsTrue(blueprint.Components.Length == 0);
            Assert.IsTrue(blueprint.SharedComponents.Length == 0);
            Assert.IsTrue(blueprint.ManagedComponents.Length == 1);
            Assert.IsTrue(blueprint.ManagedComponents[0].GetType() == typeof(TestManagedComponent1));
        }

        [TestMethod]
        public void GetEntityArcheType()
        {
            var sharedComponent = new TestSharedComponent1 { Prop = 1 };
            var entityArcheType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(sharedComponent)
                .AddManagedComponentType<TestManagedComponent1>();
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1())
                .SetSharedComponent(new TestSharedComponent1 { Prop = 1 })
                .SetManagedComponent(new TestManagedComponent1());

            var blueprintArcheType = blueprint.GetArcheType(Context);

            Assert.IsTrue(blueprintArcheType != null);
            Assert.IsTrue(entityArcheType == blueprintArcheType);

            blueprintArcheType.AddComponentType<TestComponent2>();

            var blueprintArcheType2 = blueprint.GetArcheType(Context);
            Assert.IsTrue(blueprintArcheType != blueprintArcheType2);
        }
    }
}