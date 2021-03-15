using System;
using System.Collections;
using System.Collections.Generic;

namespace EcsLte
{
	internal struct EntityKeyInfo
	{
		private int _hashCode;

		public Type ComponentType { get; set; }
		public IKeyMember[] Members { get; set; }

		public override int GetHashCode()
		{
			if (_hashCode == 0)
			{
				_hashCode = (
					ComponentType,
					StructuralComparisons.StructuralEqualityComparer.GetHashCode(Members)
				).GetHashCode();
			}

			return _hashCode;
		}
	}

	internal class EntityInfoComparer<TComponent> : IEqualityComparer<TComponent>
		where TComponent : IComponent
	{
		public EntityKeyInfo KeyInfo { get; set; }

		public bool Equals(TComponent x, TComponent y)
		{
			foreach (var member in KeyInfo.Members)
			{
				if (!member.MemberEquals(x, y))
					return false;
			}

			return true;
		}

		public int GetHashCode(TComponent obj)
			=> obj.GetHashCode();
	}
}