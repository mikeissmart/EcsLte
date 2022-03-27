using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
	public class EntityBlueprintNotHaveComponentException : EcsLteException
	{
		public EntityBlueprintNotHaveComponentException(Type componentType)
			: base($"EntityBlueprint does not have component '{componentType.Name}'.")
		{
		}
	}
}
