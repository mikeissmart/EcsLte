using System;

namespace EcsLte.Exceptions
{
	public class KeyGroupComponentNotInAllOrAnyException : EcsLteException
	{
		public KeyGroupComponentNotInAllOrAnyException(BaseKey key, Type componentType)
			: base($"Key '{key}' group '{key.Group}' does not have component '{componentType.Name}' in all of or any of filter.",
				  "Add component to group filter all of or any of.")
		{ }
	}
}