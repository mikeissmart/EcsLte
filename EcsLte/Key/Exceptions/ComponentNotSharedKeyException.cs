using System;

namespace EcsLte.Exceptions
{
	public class ComponentNotSharedKeyException : EcsLteException
	{
		public ComponentNotSharedKeyException(Type componentType)
			: base($"Component '{componentType.Name}' does not have any shared entity keyes.",
				  "Add SharedEntityKeyAttribute to component.")
		{ }
	}
}