using System;
using System.Collections.Generic;

namespace EcsLte
{
	public interface ISharedKey
	{
		Type ComponentType { get; }
		Group Group { get; }
		bool IsDestroyed { get; }

		HashSet<Entity> GetEntities(IComponent component);

		Entity GetFirstOrSingleEntity(IComponent component);
	}
}