using System.Collections.Generic;
using EcsLte.Exceptions;

namespace EcsLte
{
	public class EntityCommandQueue : IEcsContext, IEntityLife, IComponentLife
	{
		private readonly EntityCommandQueueData _data;

		internal EntityCommandQueue(EcsContext context, EntityCommandQueueData data)
		{
			_data = data;

			CurrentContext = context;
		}

		#region EscContext

		public EcsContext CurrentContext { get; }

		#endregion

		#region EntityCommandQueue

		public string Name
		{
			get
			{
				if (CurrentContext.IsDestroyed)
					throw new EcsContextIsDestroyedException(CurrentContext);

				return _data.Name;
			}
		}

		public void RunCommands()
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			// TODO
			// lock (_data.ContextData.AllWatchers)
			// {
			//     for (int i = 0; i < _data.ContextData.AllWatchers.Count; i++)
			//         _data.ContextData.AllWatchers[i].ClearEntities();
			// }

			lock (_data.Commands)
			{
				for (var i = 0; i < _data.Commands.Count; i++)
					_data.Commands[i].ExecuteCommand(CurrentContext);
				_data.Commands.Clear();
			}
		}

		public void ClearCommands()
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			lock (_data.Commands)
			{
				_data.Commands.Clear();
			}
		}

		private void AppendCommand(Entity entity, EntityCommand entityCommand)
		{
			lock (_data.Commands)
			{
				_data.Commands.Add(entityCommand);
			}
		}

		public static bool operator !=(EntityCommandQueue lhs, EntityCommandQueue rhs) => !(lhs == rhs);

		public static bool operator ==(EntityCommandQueue lhs, EntityCommandQueue rhs)
		{
			if (lhs is null || rhs is null)
				return false;
			return lhs._data.Equals(rhs._data);
		}

		public bool Equals(EntityCommandQueue other) => this == other;

		public override bool Equals(object obj) => obj is EntityCommandQueue other && _data.Equals(other._data);

		public override int GetHashCode() => _data.GetHashCode();

		#endregion

		#region EntityLife

		public Entity CreateEntity()
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			var entity = _data.ContextData.CreateEntityPrep();
			AppendCommand(entity, new CreateEntityCommand
			{
				QueuedEntity = entity,
				ContextData = _data.ContextData
			});

			return entity;
		}

		public Entity CreateEntity(EntityBlueprint blueprint)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			var entity = _data.ContextData.CreateEntityPrep();
			AppendCommand(entity, new CreateEntityCommand
			{
				QueuedEntity = entity,
				Blueprint = blueprint,
				ContextData = _data.ContextData
			});

			return entity;
		}

		public Entity[] CreateEntities(int count)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			var entities = _data.ContextData.CreateEntitiesPrep(count);
			for (var i = 0; i < entities.Length; i++)
			{
				var entity = entities[i];
				AppendCommand(entity, new CreateEntityCommand
				{
					QueuedEntity = entity,
					ContextData = _data.ContextData
				});
			}

			return entities;
		}

		public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			var entities = _data.ContextData.CreateEntitiesPrep(count);
			for (var i = 0; i < entities.Length; i++)
			{
				var entity = entities[i];
				AppendCommand(entity, new CreateEntityCommand
				{
					QueuedEntity = entity,
					Blueprint = blueprint,
					ContextData = _data.ContextData
				});
			}

			return entities;
		}

		public void DestroyEntity(Entity entity)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AppendCommand(entity, new DestroyEntityCommand
			{
				QueuedEntity = entity
			});
		}

		public void DestroyEntities(IEnumerable<Entity> entities)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			foreach (var entity in entities)
				AppendCommand(entity, new DestroyEntityCommand
				{
					QueuedEntity = entity
				});
		}

		#endregion

		#region ComponentLife

		public void AddUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique componentUnique)
			where TComponentUnique : IUniqueComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AddComponent(entity, componentUnique);
		}

		public void ReplaceUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique newComponentUnique)
			where TComponentUnique : IUniqueComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			ReplaceComponent(entity, newComponentUnique);
		}

		public void RemoveUniqueComponent<TComponentUnique>(Entity entity)
			where TComponentUnique : IUniqueComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			RemoveComponent<TComponentUnique>(entity);
		}

		public void AddComponent<TComponent>(Entity entity, TComponent component)
			where TComponent : IComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AppendCommand(entity, new AddComponentEntityCommand<TComponent>
			{
				QueuedEntity = entity,
				Component = component
			});
		}

		public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
			where TComponent : IComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AppendCommand(entity, new ReplaceComponentEntityCommand<TComponent>
			{
				QueuedEntity = entity,
				Component = newComponent
			});
		}

		public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AppendCommand(entity, new RemoveComponentEntityCommand<TComponent>(entity));
		}

		public void RemoveAllComponents(Entity entity)
		{
			if (CurrentContext.IsDestroyed)
				throw new EcsContextIsDestroyedException(CurrentContext);

			AppendCommand(entity, new RemoveAllComponentsEntityCommand(entity));
		}

		#endregion
	}
}