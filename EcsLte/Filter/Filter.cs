using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public partial struct Filter : IEquatable<Filter>
	{
		private static HashSet<int> _distinctIndicesBuffer;

		private int _hashCode;
		private int[] _indexes;

		public int[] Indexes
		{
			get
			{
				if (_indexes == null)
					_indexes = MergeDistinctIndices(AllOfIndexes, AnyOfIndexes, NoneOfIndexes);
				return _indexes;
			}
		}

		public int[] AllOfIndexes { get; private set; }
		public int[] AnyOfIndexes { get; private set; }
		public int[] NoneOfIndexes { get; private set; }

		public static Filter Combine(params Filter[] filters)
		{
			var filter = new Filter();
			foreach (var f in filters)
				filter.AllOfIndexes = MergeDistinctIndices(filter.AllOfIndexes, f.AllOfIndexes);
			foreach (var f in filters)
				filter.AnyOfIndexes = MergeDistinctIndices(filter.AnyOfIndexes, f.AnyOfIndexes);
			foreach (var f in filters)
				filter.NoneOfIndexes = MergeDistinctIndices(filter.NoneOfIndexes, f.NoneOfIndexes);

			return filter;
		}

		public static bool operator !=(Filter lhs, Filter rhs)
			=> !(lhs == rhs);

		public static bool operator ==(Filter lhs, Filter rhs)
			=> lhs.GetHashCode() == rhs.GetHashCode();

		public bool Equals(Filter other)
			=> this == other;

		public override bool Equals(object obj)
			=> obj is Filter other && (this == other);

		public bool Filtered(Entity entity)
			=> FilteredAllOf(entity) &&
				FilteredAnyOf(entity) &&
				FilteredNoneOf(entity);

		public override int GetHashCode()
			=> _hashCode == 0
				? GenerateHasCode()
				: _hashCode;

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

		private static int[] MergeDistinctIndices(params int[][] allIndices)
		{
			if (_distinctIndicesBuffer == null)
				_distinctIndicesBuffer = new HashSet<int>();
			else
				_distinctIndicesBuffer.Clear();

			foreach (var indices in allIndices)
			{
				if (indices != null)
				{
					foreach (int index in indices)
						_distinctIndicesBuffer.Add(index);
				}
			}

			var mergedIndices = new int[_distinctIndicesBuffer.Count];
			_distinctIndicesBuffer.CopyTo(mergedIndices);
			Array.Sort(mergedIndices);

			return mergedIndices;
		}

		private bool FilteredAllOf(Entity entity)
		{
			if (AllOfIndexes == null || AllOfIndexes.Length == 0)
				return true;

			bool isOk = true;
			foreach (var index in AllOfIndexes)
			{
				if (entity.Info[index] == null)
				{
					isOk = false;
					break;
				}
			}

			return isOk;
		}

		private bool FilteredAnyOf(Entity entity)
		{
			if (AnyOfIndexes == null || AnyOfIndexes.Length == 0)
				return true;

			bool isOk = false;
			foreach (var index in AnyOfIndexes)
			{
				if (entity.Info[index] != null)
				{
					isOk = true;
					break;
				}
			}

			return isOk;
		}

		private bool FilteredNoneOf(Entity entity)
		{
			if (NoneOfIndexes == null || NoneOfIndexes.Length == 0)
				return true;

			bool isOk = true;
			foreach (var index in NoneOfIndexes)
			{
				if (entity.Info[index] != null)
				{
					isOk = false;
					break;
				}
			}

			return isOk;
		}

		private int GenerateHasCode()
		{
			_hashCode = (
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(AllOfIndexes),
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(AnyOfIndexes),
				StructuralComparisons.StructuralEqualityComparer.GetHashCode(NoneOfIndexes)
			).GetHashCode();
			return _hashCode;
		}
	}
}