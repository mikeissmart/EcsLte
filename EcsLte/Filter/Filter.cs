using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public partial class Filter : IAllOfFilter
	{
		private static HashSet<int> _distinctIndicesBuffer;

		private DataCache<int> _hashCache;

		public Filter()
		{
			AllOfIndices = new int[0];
			AnyOfIndices = new int[0];
			NoneOfIndices = new int[0];

			_hashCache = new DataCache<int>(UpdateHashCache);
		}

		public int[] AllOfIndices { get; private set; }
		public int[] AnyOfIndices { get; private set; }
		public int[] NoneOfIndices { get; private set; }

		public static Filter Combine(params IFilter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.AllOfIndices = MergeDistinctIndices(filter.AllOfIndices, f.AllOfIndices);
			foreach (var f in filters)
				filter.AnyOfIndices = MergeDistinctIndices(filter.AnyOfIndices, f.AnyOfIndices);
			foreach (var f in filters)
				filter.NoneOfIndices = MergeDistinctIndices(filter.NoneOfIndices, f.NoneOfIndices);

			return filter;
		}

		public static IAllOfFilter AllOf(params IFilter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.AllOfIndices = MergeDistinctIndices(filter.AllOfIndices, f.AllOfIndices);

			return filter;
		}

		public static IAnyOfFilter AnyOf(params IFilter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.AnyOfIndices = MergeDistinctIndices(filter.AnyOfIndices, f.AnyOfIndices);

			return filter;
		}

		public static INoneOfFilter NoneOf(params IFilter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.NoneOfIndices = MergeDistinctIndices(filter.NoneOfIndices, f.NoneOfIndices);

			return filter;
		}

		IAnyOfFilter IAllOfFilter.AnyOf(params IFilter[] filters)
		{
			foreach (var f in filters)
				AnyOfIndices = MergeDistinctIndices(AnyOfIndices, f.AnyOfIndices);

			return this;
		}

		INoneOfFilter IAnyOfFilter.NoneOf(params IFilter[] filters)
		{
			foreach (var f in filters)
				NoneOfIndices = MergeDistinctIndices(NoneOfIndices, f.NoneOfIndices);

			return this;
		}

		public bool Filtered(Entity entity)
		{
			var indexes = entity.Info.ComponentIndexes;

			return FilteredAllOf(indexes) &&
				FilteredAnyOf(indexes) &&
				FilteredNoneOf(indexes);
		}

		public override int GetHashCode()
			=> _hashCache.Data;

		public override bool Equals(object obj)
		{
			if (!(obj is Filter objFilter) || obj == null)
				return false;
			return GetHashCode() == objFilter.GetHashCode();
		}

		private static int[] MergeDistinctIndex(int[] indices, int index)
		{
			if (indices == null)
				indices = new int[1] { index };
			else
			{
				if (!indices.Any(x => x == index))
				{
					Array.Resize(ref indices, indices.Length + 1);
					indices[indices.Length - 1] = index;
					Array.Sort(indices);
				}
			}

			return indices;
		}

		private static int[] MergeDistinctIndices(int[] indices1, int[] indices2)
		{
			if (indices1 == null && indices2 == null)
				return new int[0];
			else if (indices1 == null)
				return indices2;
			else if (indices2 == null)
				return indices1;

			if (_distinctIndicesBuffer == null)
				_distinctIndicesBuffer = new HashSet<int>();
			else
				_distinctIndicesBuffer.Clear();

			foreach (int indx in indices1)
				_distinctIndicesBuffer.Add(indx);
			foreach (int indx in indices2)
				_distinctIndicesBuffer.Add(indx);

			var indices = new int[_distinctIndicesBuffer.Count];
			_distinctIndicesBuffer.CopyTo(indices);
			Array.Sort(indices);

			return indices;
		}

		private bool FilteredAllOf(int[] componentIndexes)
		{
			bool isOk = true;

			foreach (var index in AllOfIndices)
			{
				if (componentIndexes[index] == 0)
				{
					isOk = false;
					break;
				}
			}

			return isOk;
		}

		private bool FilteredAnyOf(int[] componentIndexes)
		{
			bool isOk = false;

			foreach (var index in AnyOfIndices)
			{
				if (componentIndexes[index] != 0)
				{
					isOk = true;
					break;
				}
			}

			return isOk;
		}

		private bool FilteredNoneOf(int[] componentIndexes)
		{
			bool isOk = true;

			foreach (var index in NoneOfIndices)
			{
				if (componentIndexes[index] != 0)
				{
					isOk = false;
					break;
				}
			}

			return isOk;
		}

		private int UpdateHashCache()
		{
			return (
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(AllOfIndices),
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(AnyOfIndices),
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(NoneOfIndices)
			).GetHashCode();
		}
	}
}