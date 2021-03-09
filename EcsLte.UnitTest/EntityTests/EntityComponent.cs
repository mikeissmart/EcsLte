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
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();

			Assert.IsTrue(entity.HasComponent<TestComponent1>());
		}

		[TestMethod]
		public void AddComponentDeadEntity()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.Destroy();

			Assert.ThrowsException<EntityIsNotAliveException>(() => entity.AddComponent<TestComponent1>());
		}

		[TestMethod]
		public void GetComponent()
		{
			var entity = World.CreateWorld().CreateEntity();
			var component = entity.AddComponent<TestComponent1>();

			Assert.IsTrue(component.Equals(entity.GetComponent<TestComponent1>()));
		}

		[TestMethod]
		public void GetComponentDeadEntity()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();
			entity.Destroy();

			Assert.ThrowsException<EntityIsNotAliveException>(() => entity.GetComponent<TestComponent1>());
		}

		[TestMethod]
		public void RemoveComponent()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();
			entity.RemoveComponent<TestComponent1>();

			Assert.IsFalse(entity.HasComponent<TestComponent1>());
		}

		[TestMethod]
		public void RemoveComponentDeadEntity()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();
			entity.Destroy();

			Assert.ThrowsException<EntityIsNotAliveException>(() => entity.RemoveComponent<TestComponent1>());
		}

		[TestMethod]
		public void RemoveAllComponents()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();
			entity.RemoveComponents();

			Assert.IsFalse(entity.HasComponent<TestComponent1>());
		}

		[TestMethod]
		public void RemoveAllComponentsDeadEntity()
		{
			var entity = World.CreateWorld().CreateEntity();
			entity.AddComponent<TestComponent1>();
			entity.Destroy();

			Assert.ThrowsException<EntityIsNotAliveException>(() => entity.RemoveComponents());
		}

		[TestMethod]
		public void ReplaceComponent()
		{
			var entity = World.CreateWorld().CreateEntity();
			var component = entity.AddComponent<TestComponent1>();

			component.Prop = 1;
			entity.ReplaceComponent(component);

			Assert.IsTrue(entity.GetComponent<TestComponent1>().Prop == 1);
		}

		[TestMethod]
		public void ReplaceComponentDeadEntity()
		{
			var entity = World.CreateWorld().CreateEntity();
			var component = entity.AddComponent<TestComponent1>();
			entity.Destroy();

			component.Prop = 1;

			Assert.ThrowsException<EntityIsNotAliveException>(() => entity.ReplaceComponent(component));
		}
	}
}