using System;

namespace EcsLte.Exceptions
{
	public class EntityNotHaveComponentException : EcsLteException
	{
		public EntityNotHaveComponentException(EcsContext context, Entity entity, Type componentType)
			: base($"EcsContext '{context}' entity '{entity}' does not have component '{componentType.Name}'.",
				"Check if entity has component before get or remove component.")
		{
		}
	}
}