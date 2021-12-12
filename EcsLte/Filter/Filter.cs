using System;
using System.Linq;
using System.Runtime.CompilerServices;
using EcsLte.Utilities;

[assembly: InternalsVisibleTo("EcsLte.UnitTest")]

namespace EcsLte
{
	public partial struct Filter : IEquatable<Filter>
	{
		private int _hashCode;

		internal int[] Indexes { get; private set; }
		internal int[] AllOfIndexes { get; private set; }
		internal int[] AnyOfIndexes { get; private set; }
		internal int[] NoneOfIndexes { get; private set; }

		public static Filter Combine(params Filter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.Indexes = IndexHelpers.MergeDistinctIndexes(filter.Indexes, f.Indexes);
			foreach (var f in filters)
				filter.AllOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.AllOfIndexes, f.AllOfIndexes);
			foreach (var f in filters)
				filter.AnyOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.AnyOfIndexes, f.AnyOfIndexes);
			foreach (var f in filters)
				filter.NoneOfIndexes = IndexHelpers.MergeDistinctIndexes(filter.NoneOfIndexes, f.NoneOfIndexes);
			filter.CalculateHashCode();

			return filter;
		}

		public static bool operator !=(Filter lhs, Filter rhs) => !(lhs == rhs);

		public static bool operator ==(Filter lhs, Filter rhs) => lhs._hashCode == rhs._hashCode;

		public bool Equals(Filter other) => this == other;

		public override bool Equals(object obj) => obj is Filter other && this == other;

		public override int GetHashCode() => _hashCode;

		public override string ToString() => string.Join(", ", Indexes);

		internal bool IsFiltered(ComponentArcheType archeType) => FilteredAllOf(archeType.PoolIndexes) &&
				   FilteredAnyOf(archeType.PoolIndexes) &&
				   FilteredNoneOf(archeType.PoolIndexes);

		private bool FilteredAllOf(int[] componentIndexes)
		{
			if (AllOfIndexes == null || AllOfIndexes.Length == 0)
				return true;

			foreach (var index in AllOfIndexes)
				if (!componentIndexes.Contains(index))
					return false;

			return true;
		}

		private bool FilteredAnyOf(int[] componentIndexes)
		{
			if (AnyOfIndexes == null || AnyOfIndexes.Length == 0)
				return true;

			foreach (var index in AnyOfIndexes)
				if (componentIndexes.Contains(index))
					return true;

			return false;
		}

		private bool FilteredNoneOf(int[] componentIndexes)
		{
			if (NoneOfIndexes == null || NoneOfIndexes.Length == 0)
				return true;

			foreach (var index in NoneOfIndexes)
				if (componentIndexes.Contains(index))
					return false;

			return true;
		}

		private void CalculateHashCode()
		{
			_hashCode = -1663471673;
			_hashCode = _hashCode * -1521134295 + AllOfIndexes.Length;
			_hashCode = _hashCode * -1521134295 + AnyOfIndexes.Length;
			_hashCode = _hashCode * -1521134295 + NoneOfIndexes.Length;
			foreach (var index in AllOfIndexes)
				_hashCode = _hashCode * -1521134295 + index.GetHashCode();
			foreach (var index in AnyOfIndexes)
				_hashCode = _hashCode * -1521134295 + index.GetHashCode();
			foreach (var index in NoneOfIndexes)
				_hashCode = _hashCode * -1521134295 + index.GetHashCode();
		}
	}
}