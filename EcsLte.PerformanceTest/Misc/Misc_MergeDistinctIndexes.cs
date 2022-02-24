namespace EcsLte.PerformanceTest
{
	internal class Misc_MergeDistinctIndexes : BasePerformanceTest
	{
		/*private int[] _a;
		private int[] _b;
		private int[] _result;
		private int _aSize = TestConsts.EntityLoopCount;
		private int _bSize = 1;

		public override void PreRun()
		{
			base.PreRun();

			_a = new int[_aSize];
			_b = new int[_bSize];

			for (int i = 0; i < _aSize; i++)
				_a[i] = i;
			for (int i = 0; i < _bSize; i++)
				_b[i] = i + _aSize;
		}

		public override void Run()
		{
			_result = IndexHelpers.MergeDistinctIndexes(_a, _b);
		}*/

		/*public override void Run()
		{
			var comp1 = new TestSharedComponent1();
			var comp2 = new TestSharedComponent2();

			for (int i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				var config1 = ComponentPoolIndex<TestSharedComponent1>.Config;
				var config2 = ComponentPoolIndex<TestSharedComponent2>.Config;
				var a = ComponentArcheType.AppendComponent(comp1, config1);
				var b = ComponentArcheType.AppendComponent(a, comp2, config2);
			}
		}*/

		/*private Dictionary<int, Entity> _dic1 = new Dictionary<int, Entity>();
		private Dictionary<int, Entity> _dic2 = new Dictionary<int, Entity>();

		public override void Run()
		{
			var e = new Entity();
			for (int i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				_dic1.Add(i, e);
				_dic1.Remove(i);
				_dic2.Add(i, e);
			}
		}*/

		private ComponentArcheType _type1;
		private ComponentArcheType _type2;

		public override void PreRun()
		{
			base.PreRun();

			var comp1 = new TestSharedComponent1();
			var comp2 = new TestSharedComponent2();
			var config1 = ComponentPoolIndex<TestSharedComponent1>.Config;
			var config2 = ComponentPoolIndex<TestSharedComponent2>.Config;

			_type1 = ComponentArcheType.AppendComponent(comp1, config1);
			_type2 = ComponentArcheType.AppendComponent(_type1, comp2, config2);
		}

		public override void Run()
		{
			var a = 0;
			for (var i = 0; i < TestConsts.EntityLoopCount; i++)
			{
				a = _type1.GetHashCode();
				a = _type2.GetHashCode();
			}
		}
	}
}

/*

IndexHelpers.MergeDistinctIndexes		120
ComponentArcheType.AppendComponent		400
Dictionary.Add_Remove_Add				 40
Dictionary.TryGetValue					 15
GetHashCode								100
*/
