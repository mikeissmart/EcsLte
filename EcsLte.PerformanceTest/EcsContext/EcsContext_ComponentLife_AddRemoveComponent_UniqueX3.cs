namespace EcsLte.PerformanceTest
{
	internal class EcsContext_ComponentLife_AddRemoveComponent_UniqueX3 : BasePerformanceTest
	{
		private Entity _entity;

		public override void PreRun()
		{
			base.PreRun();

			_entity = _context.CreateEntity();
		}

		public override void Run()
		{
			var component1 = new TestUniqueComponent1();
			var component2 = new TestUniqueComponent2();
			var component3 = new TestUniqueComponent3();
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_context.AddUniqueComponent(_entity, component1);
				_context.AddUniqueComponent(_entity, component2);
				_context.AddUniqueComponent(_entity, component3);
				_context.RemoveUniqueComponent<TestUniqueComponent1>(_entity);
				_context.RemoveUniqueComponent<TestUniqueComponent2>(_entity);
				_context.RemoveUniqueComponent<TestUniqueComponent3>(_entity);
			}
		}
	}
}