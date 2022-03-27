using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
	public struct ShareComponentDataIndex : IEquatable<ShareComponentDataIndex>, IComparable<ShareComponentDataIndex>
	{
		public int SharedIndex { get; set; }
		public int SharedDataIndex { get; set; }

		public static bool operator !=(ShareComponentDataIndex lhs, ShareComponentDataIndex rhs) => !(lhs == rhs);

		public static bool operator ==(ShareComponentDataIndex lhs, ShareComponentDataIndex rhs)
		{
			return lhs.SharedIndex == rhs.SharedIndex &&
				lhs.SharedDataIndex == rhs.SharedDataIndex;
		}

		public bool Equals(ShareComponentDataIndex other)
			=> this == other;

		public override bool Equals(object other)
			=> other is ShareComponentDataIndex obj && this == obj;

		public override int GetHashCode()
		{
			var hashCode = 1193469065;
			hashCode = hashCode * -1521134295 + SharedIndex.GetHashCode();
			hashCode = hashCode * -1521134295 + SharedDataIndex.GetHashCode();
			return hashCode;
		}

		public int CompareTo(ShareComponentDataIndex other)
		{
			var compare = SharedIndex.CompareTo(other.SharedIndex);
			if (compare == 0)
				compare = SharedDataIndex.CompareTo(other.SharedDataIndex);
			return compare;
		}
	}
}
