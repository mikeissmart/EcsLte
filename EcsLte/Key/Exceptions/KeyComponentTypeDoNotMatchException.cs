using System;

namespace EcsLte.Exceptions
{
	public class KeyComponentTypeDoNotMatchException : EcsLteException
	{
		public KeyComponentTypeDoNotMatchException(Type componentType, Type otherType)
			: base($"Component '{componentType.Name}' does not match '{otherType.Name}'.",
				  "Component types must match.")
		{ }
	}
}