using System;

namespace EcsLte
{
	public interface IPrimaryKey
	{
		Type ComponentType { get; }
		Group Group { get; }
		bool IsDestroyed { get; }

		Entity GetEntity(IComponent component);
	}
}