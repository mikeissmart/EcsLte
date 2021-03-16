using EcsLte.Exceptions;
using System.Collections.Generic;

namespace EcsLte
{
	public class PrimaryKey<TComponent> : BaseKey, IPrimaryKey
		where TComponent : IComponent
	{
		private readonly Dictionary<TComponent, Entity> _keyLookup;

		public PrimaryKey(EntityManager entityManager, Group group)
			: base(entityManager, group, ComponentIndex<TComponent>.Index, typeof(TComponent))
		{
			_keyLookup = new Dictionary<TComponent, Entity>();

			foreach (var entity in group.Entities)
			{
				var component = _entityManager.GetComponent(entity, _componentPoolIndex);
				if (component != null)
					GroupEntityAddedEvent(entity, _componentPoolIndex, component);
			}
		}

		public Entity GetEntity(IComponent component)
		{
			if (!(component is TComponent))
				throw new KeyComponentTypeDoNotMatchException(ComponentType, component.GetType());

			return GetEntity((TComponent)component);
		}

		public Entity GetEntity(TComponent component)
		{
			if (IsDestroyed)
				throw new KeyIsDestroyedException(this);

			if (_keyLookup.TryGetValue(component, out Entity entity))
				return entity;
			return Entity.Null;
		}

		protected override void GroupEntityAddedEvent(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (_keyLookup.ContainsKey((TComponent)component))
				throw new PrimaryKeyDuplicateKeyException(this, component);

			_keyLookup.Add((TComponent)component, entity);
		}

		protected override void GroupEntityRemovedEvent(Entity entity, int componentPoolIndex, IComponent component)
			=> _keyLookup.Remove((TComponent)component);
	}
}