namespace EcsLte.PerformanceTest
{
	internal class EcsContext_EntityLife_DestroyEntities : BasePerformanceTest
	{
		private Entity[] _entities;

		public override void PreRun()
		{
			base.PreRun();

			_entities = _context.CreateEntities(TestConsts.EntityLoopCount);
		}

		public override void Run() => _context.DestroyEntities(_entities);
	}
}