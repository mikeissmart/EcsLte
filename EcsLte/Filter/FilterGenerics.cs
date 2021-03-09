namespace EcsLte
{
	public partial class Filter
	{
		public static IAllOfFilter AllOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter();
			filter.AllOfIndices = new int[] { ComponentIndex<TComponent>.Index };

			return filter;
		}

		public static IAllOfFilter AllOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = (Filter)AllOf<TComponent1>();

			filter.AllOfIndices = MergeDistinctIndex(filter.AllOfIndices, ComponentIndex<TComponent2>.Index);

			return filter;
		}

		public static IAllOfFilter AllOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = (Filter)AllOf<TComponent1, TComponent2>();

			filter.AllOfIndices = MergeDistinctIndex(filter.AllOfIndices, ComponentIndex<TComponent3>.Index);

			return filter;
		}

		public static IAllOfFilter AllOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = (Filter)AllOf<TComponent1, TComponent2, TComponent3>();

			filter.AllOfIndices = MergeDistinctIndex(filter.AllOfIndices, ComponentIndex<TComponent4>.Index);

			return filter;
		}

		public static IAllOfFilter AllOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = (Filter)AllOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.AllOfIndices = MergeDistinctIndex(filter.AllOfIndices, ComponentIndex<TComponent5>.Index);

			return filter;
		}

		public static IAnyOfFilter AnyOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter();
			filter.AnyOfIndices = new int[] { ComponentIndex<TComponent>.Index };

			return filter;
		}

		public static IAnyOfFilter AnyOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = (Filter)AnyOf<TComponent1>();

			filter.AnyOfIndices = MergeDistinctIndex(filter.AnyOfIndices, ComponentIndex<TComponent2>.Index);

			return filter;
		}

		public static IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = (Filter)AnyOf<TComponent1, TComponent2>();

			filter.AnyOfIndices = MergeDistinctIndex(filter.AnyOfIndices, ComponentIndex<TComponent3>.Index);

			return filter;
		}

		public static IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = (Filter)AnyOf<TComponent1, TComponent2, TComponent3>();

			filter.AnyOfIndices = MergeDistinctIndex(filter.AnyOfIndices, ComponentIndex<TComponent4>.Index);

			return filter;
		}

		public static IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = (Filter)AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.AnyOfIndices = MergeDistinctIndex(filter.AnyOfIndices, ComponentIndex<TComponent5>.Index);

			return filter;
		}

		public static INoneOfFilter NoneOf<TComponent>()
			where TComponent : IComponent
		{
			var filter = new Filter();
			filter.NoneOfIndices = new int[] { ComponentIndex<TComponent>.Index };

			return filter;
		}

		public static INoneOfFilter NoneOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
		{
			var filter = (Filter)NoneOf<TComponent1>();

			filter.NoneOfIndices = MergeDistinctIndex(filter.NoneOfIndices, ComponentIndex<TComponent2>.Index);

			return filter;
		}

		public static INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
		{
			var filter = (Filter)NoneOf<TComponent1, TComponent2>();

			filter.NoneOfIndices = MergeDistinctIndex(filter.NoneOfIndices, ComponentIndex<TComponent3>.Index);

			return filter;
		}

		public static INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
		{
			var filter = (Filter)NoneOf<TComponent1, TComponent2, TComponent3>();

			filter.NoneOfIndices = MergeDistinctIndex(filter.NoneOfIndices, ComponentIndex<TComponent4>.Index);

			return filter;
		}

		public static INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent
		{
			var filter = (Filter)NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			filter.NoneOfIndices = MergeDistinctIndex(filter.NoneOfIndices, ComponentIndex<TComponent5>.Index);

			return filter;
		}

		IAnyOfFilter IAllOfFilter.AnyOf<TComponent>()
		{
			AnyOfIndices = MergeDistinctIndex(AnyOfIndices, ComponentIndex<TComponent>.Index);

			return this;
		}

		IAnyOfFilter IAllOfFilter.AnyOf<TComponent1, TComponent2>()
		{
			AnyOf<TComponent1>();

			AnyOfIndices = MergeDistinctIndex(AnyOfIndices, ComponentIndex<TComponent2>.Index);

			return this;
		}

		IAnyOfFilter IAllOfFilter.AnyOf<TComponent1, TComponent2, TComponent3>()
		{
			AnyOf<TComponent1, TComponent2>();

			AnyOfIndices = MergeDistinctIndex(AnyOfIndices, ComponentIndex<TComponent3>.Index);

			return this;
		}

		IAnyOfFilter IAllOfFilter.AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>()
		{
			AnyOf<TComponent1, TComponent2, TComponent3>();

			AnyOfIndices = MergeDistinctIndex(AnyOfIndices, ComponentIndex<TComponent4>.Index);

			return this;
		}

		IAnyOfFilter IAllOfFilter.AnyOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
		{
			AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			AnyOfIndices = MergeDistinctIndex(AnyOfIndices, ComponentIndex<TComponent5>.Index);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf<TComponent>()
		{
			NoneOfIndices = MergeDistinctIndex(NoneOfIndices, ComponentIndex<TComponent>.Index);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf<TComponent1, TComponent2>()
		{
			NoneOf<TComponent1>();

			NoneOfIndices = MergeDistinctIndex(NoneOfIndices, ComponentIndex<TComponent2>.Index);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf<TComponent1, TComponent2, TComponent3>()
		{
			NoneOf<TComponent1, TComponent2>();

			NoneOfIndices = MergeDistinctIndex(NoneOfIndices, ComponentIndex<TComponent3>.Index);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>()
		{
			NoneOf<TComponent1, TComponent2, TComponent3>();

			NoneOfIndices = MergeDistinctIndex(NoneOfIndices, ComponentIndex<TComponent4>.Index);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
		{
			NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>();

			NoneOfIndices = MergeDistinctIndex(NoneOfIndices, ComponentIndex<TComponent5>.Index);

			return this;
		}
	}
}