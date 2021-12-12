using System.Linq;

namespace EcsLte.PerformanceTest
{
	internal class Misc_SequenceEqual_GetHashCode : BasePerformanceTest
	{
		private int[][] _sharedComponents;

		public override void PreRun()
		{
			base.PreRun();

			_sharedComponents = new int[TestConsts.EntityLoopCount][];
			for (var i = 0; i < _sharedComponents.Length; i++)
				_sharedComponents[i] = new int[] { new TestSharedComponent1().GetHashCode() };
		}

		public override void Run()
		{
			bool result;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
				result = _sharedComponents[i].SequenceEqual(_sharedComponents[i]);
		}
	}
}