using System;

namespace EcsLte
{
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
	public class PrimaryKeyAttribute : Attribute
	{
		public PrimaryKeyAttribute()
		{
		}
	}
}