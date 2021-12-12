namespace EcsLte.PerformanceTest
{
	internal class EntityCommandQueue_ComponentLife_ReplaceComponent_UniqueX2 : BasePerformanceTest
	{
		private Entity _entity;
		private TestUniqueComponent1 _component1;
		private TestUniqueComponent1 _component2;

		public override void PreRun()
		{
			base.PreRun();

			_entity = _context.CreateEntity();
			_component1 = new TestUniqueComponent1 { Prop = 1 };
			_component2 = new TestUniqueComponent1 { Prop = 2 };
			_context.AddUniqueComponent(_entity, _component1);
		}

		public override void Run()
		{
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.DefaultCommand.ReplaceUniqueComponent(_entity, _component1);
				_context.DefaultCommand.ReplaceUniqueComponent(_entity, _component2);
			}
			_context.DefaultCommand.RunCommands();
		}
	}
}