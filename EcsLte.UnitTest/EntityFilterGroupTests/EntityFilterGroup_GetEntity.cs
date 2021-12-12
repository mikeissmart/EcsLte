using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterGroupTests
{
	[TestClass]
	public class EntityFilterGroup_GetEntity : BasePrePostTest, IGetEntityTest
	{
		[TestMethod]
		public void HasEntity()
		{
			var component = new TestSharedComponent1 { Prop = 1 };
			var filterGroup = _context.FilterByGroupWith(Filter.AllOf<TestSharedComponent1>(), component);
			var entity = _context.CreateEntity();
			_context.AddComponent(entity, component);

			// Has entity
			Assert.IsTrue(filterGroup.HasEntity(entity));
			// Removes entity when not filtered
			_context.RemoveComponent<TestSharedComponent1>(entity);
			Assert.IsFalse(filterGroup.HasEntity(entity));
			// Destroy entity removes from filter
			_context.AddComponent(entity, new TestSharedComponent1());
			_context.DestroyEntity(entity);
			Assert.IsFalse(filterGroup.HasEntity(entity));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				filterGroup.HasEntity(entity));
		}

		[TestMethod]
		public void GetEntities()
		{
			var component = new TestSharedComponent1 { Prop = 1 };
			var filterGroup = _context.FilterByGroupWith(Filter.AllOf<TestSharedComponent1>(), component);
			var entity = _context.CreateEntity();
			_context.AddComponent(entity, component);

			// Has entities
			Assert.IsTrue(filterGroup.GetEntities().Length == 1);
			Assert.IsTrue(filterGroup.GetEntities()[0] == entity);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				filterGroup.HasEntity(entity));
		}
	}
}