namespace EcsLte.PerformanceTest
{
	internal class EntityCommandQueue_EntityLife_CreateEntities : BasePerformanceTest
	{
		public override void Run()
		{
			var entities = _context.DefaultCommand.CreateEntities(TestConsts.EntityLoopCount);
			_context.DefaultCommand.RunCommands();
		}
	}
}