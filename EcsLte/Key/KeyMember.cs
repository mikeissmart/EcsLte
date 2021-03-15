using System.Reflection;

namespace EcsLte
{
	internal interface IKeyMember
	{
		bool MemberEquals(object lhs, object rhs);
	}

	internal class FieldKeyMember : IKeyMember
	{
		public FieldInfo Field { get; set; }

		public bool MemberEquals(object lhs, object rhs)
			=> Field.GetValue(lhs).Equals(Field.GetValue(rhs));
	}

	internal class PropertyKeyMember : IKeyMember
	{
		public PropertyInfo Property { get; set; }

		public bool MemberEquals(object lhs, object rhs)
			=> Property.GetValue(lhs).Equals(Property.GetValue(rhs));
	}
}