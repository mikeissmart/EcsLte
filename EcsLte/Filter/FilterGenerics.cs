namespace EcsLte
{
	public partial struct Filter
	{
		public static Filter AllOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter
			{
				AllOfIndexes = new int[] { ComponentIndex<TComponent>.Index },
				AnyOfIndexes = new int[0],
				NoneOfIndexes = new int[0]
			};
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AllOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = AllOf<TComponent1>();

			filter.AllOfIndexes = MergeDistinctIndex(filter.AllOfIndexes, ComponentIndex<TComponent2>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AllOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = AllOf<TComponent1, TComponent2>();

			filter.AllOfIndexes = MergeDistinctIndex(filter.AllOfIndexes, ComponentIndex<TComponent3>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AllOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = AllOf<TComponent1, TComponent2, TComponent3>();

			filter.AllOfIndexes = MergeDistinctIndex(filter.AllOfIndexes, ComponentIndex<TComponent4>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AllOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = AllOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.AllOfIndexes = MergeDistinctIndex(filter.AllOfIndexes, ComponentIndex<TComponent5>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AnyOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter
			{
				AllOfIndexes = new int[0],
				AnyOfIndexes = new int[] { ComponentIndex<TComponent>.Index },
				NoneOfIndexes = new int[0]
			};
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AnyOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = AnyOf<TComponent1>();

			filter.AnyOfIndexes = MergeDistinctIndex(filter.AnyOfIndexes, ComponentIndex<TComponent2>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AnyOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = AnyOf<TComponent1, TComponent2>();

			filter.AnyOfIndexes = MergeDistinctIndex(filter.AnyOfIndexes, ComponentIndex<TComponent3>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = AnyOf<TComponent1, TComponent2, TComponent3>();

			filter.AnyOfIndexes = MergeDistinctIndex(filter.AnyOfIndexes, ComponentIndex<TComponent4>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.AnyOfIndexes = MergeDistinctIndex(filter.AnyOfIndexes, ComponentIndex<TComponent5>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter NoneOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter
			{
				AllOfIndexes = new int[0],
				AnyOfIndexes = new int[0],
				NoneOfIndexes = new int[] { ComponentIndex<TComponent>.Index }
			};
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter NoneOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = NoneOf<TComponent1>();

			filter.NoneOfIndexes = MergeDistinctIndex(filter.NoneOfIndexes, ComponentIndex<TComponent2>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter NoneOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = NoneOf<TComponent1, TComponent2>();

			filter.NoneOfIndexes = MergeDistinctIndex(filter.NoneOfIndexes, ComponentIndex<TComponent3>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = NoneOf<TComponent1, TComponent2, TComponent3>();

			filter.NoneOfIndexes = MergeDistinctIndex(filter.NoneOfIndexes, ComponentIndex<TComponent4>.Index);
			filter.GenerateHasCode();

			return filter;
		}

		public static Filter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.NoneOfIndexes = MergeDistinctIndex(filter.NoneOfIndexes, ComponentIndex<TComponent5>.Index);
			filter.GenerateHasCode();

			return filter;
		}
	}
}