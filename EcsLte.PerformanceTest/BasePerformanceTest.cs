namespace EcsLte.PerformanceTest
{
	internal abstract class BasePerformanceTest
	{
		protected EcsContext _context;

		public virtual void PreRun() => _context = EcsContexts.CreateContext("Test");

		public abstract void Run();

		public virtual bool CanRunParallel() => false;

		public virtual void RunParallel()
		{
		}

		public virtual void PostRun() => EcsContexts.DestroyContext(_context);
	}
}