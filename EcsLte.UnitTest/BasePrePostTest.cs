using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest
{
	public abstract class BasePrePostTest
	{
		protected EcsContext _context;

		[TestInitialize]
		public void PreTest() => _context = EcsContexts.CreateContext("Test");

		[TestCleanup]
		public void PostTest()
		{
			if (!_context.IsDestroyed)
				EcsContexts.DestroyContext(_context);
		}
	}
}