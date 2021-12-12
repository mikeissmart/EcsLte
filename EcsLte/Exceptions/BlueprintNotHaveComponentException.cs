using System;

namespace EcsLte.Exceptions
{
	public class BlueprintNotHaveComponentException : EcsLteException
	{
		public BlueprintNotHaveComponentException(Type componentType)
			: base($"Blueprint does not have component '{componentType.Name}'.",
				"Check if blueprint has component before get or remove component.")
		{
		}
	}
}