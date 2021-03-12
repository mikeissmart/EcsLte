using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityTests
{
	[TestClass]
	public class EntityComponent
	{
		[TestMethod]
		public void AddComponent()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);

			Assert.IsTrue(world.EntityManager.HasComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void AddComponentDeadEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.DestroyEntity(entity);

			Assert.ThrowsException<WorldDoesNotHaveEntityException>(() => world.EntityManager
				.AddComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void GetComponent()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			var component = world.EntityManager.AddComponent<TestComponent1>(entity);

			Assert.IsTrue(component.Equals(world.EntityManager.GetComponent<TestComponent1>(entity)));
		}

		[TestMethod]
		public void GetComponentDeadEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.DestroyEntity(entity);

			Assert.ThrowsException<WorldDoesNotHaveEntityException>(() => world.EntityManager
				.GetComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void RemoveComponent()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.RemoveComponent<TestComponent1>(entity);

			Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void RemoveComponentDeadEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.DestroyEntity(entity);

			Assert.ThrowsException<WorldDoesNotHaveEntityException>(() => world.EntityManager
				.RemoveComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void RemoveAllComponents()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.RemoveAllComponents(entity);

			Assert.IsFalse(world.EntityManager.HasComponent<TestComponent1>(entity));
		}

		[TestMethod]
		public void RemoveAllComponentsDeadEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.DestroyEntity(entity);

			Assert.ThrowsException<WorldDoesNotHaveEntityException>(() => world.EntityManager
				.RemoveAllComponents(entity));
		}

		[TestMethod]
		public void ReplaceComponent()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			var component = world.EntityManager.AddComponent<TestComponent1>(entity);

			component.Prop = 1;
			world.EntityManager.ReplaceComponent(entity, component);

			Assert.IsTrue(world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
		}

		[TestMethod]
		public void ReplaceComponentDeadEntity()
		{
			var world = World.CreateWorld();
			var entity = world.EntityManager.CreateEntity();
			var component = world.EntityManager.AddComponent<TestComponent1>(entity);
			world.EntityManager.DestroyEntity(entity);

			component.Prop = 1;

			Assert.ThrowsException<WorldDoesNotHaveEntityException>(() => world.EntityManager.ReplaceComponent(entity, component));
		}
	}
}