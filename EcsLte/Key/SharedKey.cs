using EcsLte.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public class SharedKey<TComponent> : BaseKey, ISharedKey
		where TComponent : IComponent
	{
		private readonly Dictionary<TComponent, HashSet<Entity>> _keyLookup;

		internal SharedKey(EntityManager entityManager, Group group)
			: base(entityManager, group, ComponentIndex<TComponent>.Index, typeof(TComponent))
		{
			_keyLookup = new Dictionary<TComponent, HashSet<Entity>>();

			foreach (var entity in group.Entities)
			{
				var component = _entityManager.GetComponent(entity, _componentPoolIndex);
				if (component != null)
					GroupEntityAddedEvent(entity, _componentPoolIndex, component);
			}
		}

		public HashSet<Entity> GetEntities(IComponent component)
		{
			if (!(component is TComponent))
				throw new KeyComponentTypeDoNotMatchException(ComponentType, component.GetType());

			return GetEntities((TComponent)component);
		}

		public HashSet<Entity> GetEntities(TComponent component)
		{
			if (IsDestroyed)
				throw new KeyIsDestroyedException(this);

			if (_keyLookup.TryGetValue(component, out HashSet<Entity> entities))
				return entities;
			return new HashSet<Entity>();
		}

		public Entity GetFirstOrSingleEntity(IComponent component)
		{
			var entities = GetEntities(component);
			if (entities.Count > 0)
				return entities.ElementAt(0);
			return Entity.Null;
		}

		public Entity GetFirstOrSingleEntity(TComponent component)
		{
			var entities = GetEntities(component);
			if (entities.Count > 0)
				return entities.ElementAt(0);
			return Entity.Null;
		}

		protected override void GroupEntityAddedEvent(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (!_keyLookup.TryGetValue((TComponent)component, out HashSet<Entity> entities))
			{
				entities = new HashSet<Entity>();
				_keyLookup.Add((TComponent)component, entities);
			}

			entities.Add(entity);
		}

		protected override void GroupEntityRemovedEvent(Entity entity, int componentPoolIndex, IComponent component)
		{
			if (_keyLookup.TryGetValue((TComponent)component, out HashSet<Entity> entities))
				entities.Remove(entity);
		}
	}
}