using System;

namespace EcsLte
{
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
	public class SharedKeyAttribute : BaseKeyAttribute
	{
		public SharedKeyAttribute(params string[] memberNames)
			: base(memberNames) { }
	}
}