using System;

namespace EcsLte.Exceptions
{
	public class ComponentDoesNotHavePrimaryKey : EcsLteException
	{
		public ComponentDoesNotHavePrimaryKey(Type componentType)
			: base($"Component '{componentType.Name}' does not have primary key attribute.",
				  "Add primary key attribute to component.")
		{ }
	}
}