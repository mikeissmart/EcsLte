using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
	[TestClass]
	public class EcsContext_GetEntity : BasePrePostTest, IGetEntityTest
	{
		[TestMethod]
		public void HasEntity()
		{
			var entity = _context.CreateEntity();

			// Has entity
			Assert.IsTrue(_context.HasEntity(entity));
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.HasEntity(entity));
		}

		[TestMethod]
		public void GetEntities()
		{
			var entity = _context.CreateEntity();

			// Has entities
			Assert.IsTrue(_context.GetEntities().Length == 1);
			Assert.IsTrue(_context.GetEntities()[0] == entity);
			// EcsContext is destroyed
			EcsContexts.DestroyContext(_context);
			Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
				_context.HasEntity(entity));
		}
	}
}