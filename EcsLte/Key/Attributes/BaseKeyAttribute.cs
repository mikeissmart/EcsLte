using System;

namespace EcsLte
{
	public abstract class BaseKeyAttribute : Attribute
	{
		public BaseKeyAttribute(params string[] memberNames)
			=> MemberNames = memberNames;

		public string[] MemberNames { get; set; }
	}
}