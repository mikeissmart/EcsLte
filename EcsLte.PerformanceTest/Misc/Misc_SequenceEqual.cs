using System.Linq;

namespace EcsLte.PerformanceTest
{
	internal class Misc_SequenceEqual : BasePerformanceTest
	{
		private ISharedComponent[][] _sharedComponents;

		public override void PreRun()
		{
			base.PreRun();

			_sharedComponents = new ISharedComponent[TestConsts.EntityLoopCount][];
			for (var i = 0; i < _sharedComponents.Length; i++)
				_sharedComponents[i] = new ISharedComponent[] { new TestSharedComponent1() };
		}

		public override void Run()
		{
			bool code;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				code = _sharedComponents[i].SequenceEqual(_sharedComponents[i]);
		}
	}
}