using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_ComponentGetAll : BasePrePostTest
    {
        [TestMethod]
        public void GetAllComponents()
        {
            var entity = Context.Entities.CreateEntity(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 1 })
                    .SetComponent(new TestComponent2 { Prop = 2 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 3 })
                    .SetSharedComponent(new TestSharedComponent2 { Prop = 4 })
                    .SetManagedComponent(new TestManagedComponent1 { Prop = 5 })
                    .SetManagedComponent(new TestManagedComponent2 { Prop = 6 }));

            var components = Context.Entities.GetAllComponents(entity);
            Assert.IsTrue(components.Length == 6);
            AssertComponent<TestComponent1>(components, 0, 1);
            AssertComponent<TestComponent2>(components, 0, 2);
            AssertComponent<TestSharedComponent1>(components, 0, 3);
            AssertComponent<TestSharedComponent2>(components, 0, 4);
            AssertComponent<TestManagedComponent1>(components, 0, 5);
            AssertComponent<TestManagedComponent2>(components, 0, 6);

            components = new IComponent[0];
            var componentCount = Context.Entities.GetAllComponents(entity, ref components);
            Assert.IsTrue(components.Length == 6);
            Assert.IsTrue(componentCount == 6);
            AssertComponent<TestComponent1>(components, 0, 1);
            AssertComponent<TestComponent2>(components, 0, 2);
            AssertComponent<TestSharedComponent1>(components, 0, 3);
            AssertComponent<TestSharedComponent2>(components, 0, 4);
            AssertComponent<TestManagedComponent1>(components, 0, 5);
            AssertComponent<TestManagedComponent2>(components, 0, 6);

            components = new IComponent[5];
            componentCount = Context.Entities.GetAllComponents(entity, ref components, 5);
            Assert.IsTrue(components.Length == 11);
            Assert.IsTrue(componentCount == 6);
            AssertComponent<TestComponent1>(components, 5, 1);
            AssertComponent<TestComponent2>(components, 5, 2);
            AssertComponent<TestSharedComponent1>(components, 5, 3);
            AssertComponent<TestSharedComponent2>(components, 5, 4);
            AssertComponent<TestManagedComponent1>(components, 5, 5);
            AssertComponent<TestManagedComponent2>(components, 5, 6);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.GetAllComponents(Entity.Null));

            components = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Entities.GetAllComponents(entity, ref components));
            Assert.ThrowsException<ArgumentNullException>(() =>
                Context.Entities.GetAllComponents(entity, ref components, 0));

            components = new IComponent[0];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Context.Entities.GetAllComponents(entity, ref components, 1));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.GetAllComponents(entity));
        }

        private void AssertComponent<TComponent>(IComponent[] components, int startingIndex,
            int propNum)
            where TComponent : IComponent, ITestComponent
        {
            var component = (ITestComponent)components
                .Skip(startingIndex)
                .Where(x => x.GetType() == typeof(TComponent))
                .FirstOrDefault();

            Assert.IsTrue(component != null,
                $"No Component: {nameof(TComponent)}");
            Assert.IsTrue(component.Prop == propNum,
                $"Wrong Component Value: {typeof(TComponent).Name}, {component.Prop}");
        }
    }
}
