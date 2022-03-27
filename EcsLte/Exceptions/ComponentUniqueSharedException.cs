using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
	public class ComponentUniqueSharedException : EcsLteException
	{
		public ComponentUniqueSharedException(Type[] componentTypes)
			: base($"Components cannot be IUniqueComponent and ISharedComponent : ({TypesToString(componentTypes)}).")
		{
		}
	}
}
