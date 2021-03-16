using System;

namespace EcsLte.Exceptions
{
	public class ComponentDoesNotHaveSharedKey : EcsLteException
	{
		public ComponentDoesNotHaveSharedKey(Type componentType)
			: base($"Component '{componentType.Name}' does not have shared key attribute.",
				  "Add shared key attribute to component.")
		{ }
	}
}