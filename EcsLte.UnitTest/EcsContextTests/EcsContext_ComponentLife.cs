using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
	[TestClass]
	public class EcsContext_ComponentLife : BasePrePostTest, IComponentLifeTest
	{
		[TestMethod]
		public void AddUniqueComponent()
		{
			var entity = _context.CreateEntity();
			var component = new TestUniqueComponent1 { Prop = 1 };

			_context.AddUniqueComponent(entity, component);

			// Correct component
			Assert.IsTrue(
				_context.GetComponent<TestUniqueComponent1>(entity).Prop == component.Prop);
			// Already has component
			Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
				_context.AddUniqueComponent(entity, component));
			// Unable to add same unique component to another entity
			var entity2 = _context.CreateEntity();
			Assert.ThrowsException<EntityAlreadyHasUniqueComponentException>(() =>
				_context.AddUniqueComponent(entity2, component));
			// Can add to another entity once exisitng unique component is removed
			_context.RemoveUniqueComponent<TestUniqueComponent1>(entity);
			_context.AddUniqueComponent(entity2, component);
			Assert.IsTrue(_context.HasComponent<TestUniqueComponent1>(entity2));
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.AddUniqueComponent(Entity.Null, new TestUniqueComponent1()));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.AddUniqueComponent(entity, new TestUniqueComponent1()));
		}

		[TestMethod]
		public void ReplaceUniqueComponent()
		{
			var entity = _context.CreateEntity();
			var component1 = new TestUniqueComponent1 { Prop = 1 };
			_context.AddUniqueComponent(entity, new TestUniqueComponent1());

			_context.ReplaceUniqueComponent(entity, component1);

			// Correct component
			Assert.IsTrue(
				_context.GetComponent<TestUniqueComponent1>(entity).Prop == component1.Prop);
			// Also adds component
			var component2 = new TestUniqueComponent2 { Prop = 2 };
			_context.ReplaceUniqueComponent(entity, component2);
			Assert.IsTrue(
				_context.GetComponent<TestUniqueComponent2>(entity).Prop == component2.Prop);
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.ReplaceUniqueComponent(Entity.Null, new TestUniqueComponent1()));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.ReplaceUniqueComponent(entity, new TestUniqueComponent1()));
		}

		[TestMethod]
		public void RemoveUniqueComponent()
		{
			var entity = _context.CreateEntity();
			_context.AddUniqueComponent(entity, new TestUniqueComponent1());

			_context.RemoveUniqueComponent<TestUniqueComponent1>(entity);

			// Correctly removes component
			Assert.IsFalse(_context.HasComponent<TestUniqueComponent1>(entity));
			// Cannot remove component entiy doesnt have
			Assert.ThrowsException<EntityNotHaveComponentException>(() =>
				_context.RemoveComponent<TestUniqueComponent1>(entity));
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.RemoveComponent<TestUniqueComponent1>(Entity.Null));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.RemoveComponent<TestUniqueComponent1>(entity));
		}

		[TestMethod]
		public void AddComponent()
		{
			var entity = _context.CreateEntity();
			var component = new TestComponent1 { Prop = 1 };

			_context.AddComponent(entity, component);

			// Correct component
			Assert.IsTrue(
				_context.GetComponent<TestComponent1>(entity).Prop == component.Prop);
			// Already has component
			Assert.ThrowsException<EntityAlreadyHasComponentException>(() =>
				_context.AddComponent(entity, component));
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.AddComponent(Entity.Null, component));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.AddComponent(entity, component));
		}

		[TestMethod]
		public void ReplaceComponent()
		{
			var entity = _context.CreateEntity();
			var component1 = new TestComponent1 { Prop = 1 };
			_context.AddComponent(entity, new TestComponent1());

			_context.ReplaceComponent(entity, component1);

			// Correct component
			Assert.IsTrue(
				_context.GetComponent<TestComponent1>(entity).Prop == component1.Prop);
			// Also adds component
			var component2 = new TestComponent2 { Prop = 2 };
			_context.ReplaceComponent(entity, component2);
			Assert.IsTrue(
				_context.GetComponent<TestComponent2>(entity).Prop == component2.Prop);
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.ReplaceComponent(Entity.Null, new TestComponent1()));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.ReplaceComponent(entity, new TestComponent1()));
		}

		[TestMethod]
		public void RemoveComponent()
		{
			var entity = _context.CreateEntity();
			_context.AddComponent(entity, new TestComponent1());

			_context.RemoveComponent<TestComponent1>(entity);

			// Correctly removes component
			Assert.IsFalse(_context.HasComponent<TestComponent1>(entity));
			// Cannot remove component entiy doesnt have
			Assert.ThrowsException<EntityNotHaveComponentException>(() =>
				_context.RemoveComponent<TestComponent1>(entity));
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.RemoveComponent<TestComponent1>(Entity.Null));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.RemoveComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void RemoveAllComponents()
		{
			var entity = _context.CreateEntity();
			_context.AddComponent(entity, new TestComponent1());

			_context.RemoveAllComponents(entity);

			// Correctly removes components
			Assert.IsTrue(_context.GetAllComponents(entity).Length == 0);
			Assert.IsFalse(_context.HasComponent<TestComponent1>(entity));
			// Entity does not exist
			Assert.ThrowsException<EntityDoesNotExistException>(() =>
				_context.RemoveAllComponents(Entity.Null));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.RemoveAllComponents(entity));
		}
	}
}