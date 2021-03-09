using EcsLte.Exceptions;
using System;

namespace EcsLte
{
	public struct Entity : IEquatable<Entity>, IComparable<Entity>
	{
		public static readonly Entity Null = new Entity { Info = new EntityInfo() };

		public bool IsNotNull { get => this != Null; }
		public bool IsNull { get => this == Null; }

		public int Id { get => Info?.Id ?? 0; }
		public int Generation { get => Info?.Generation ?? 0; }
		public int WorldId { get => Info?.WorldOwner.WorldId ?? 0; }
		public bool IsAlive { get => Info?.IsAlive ?? false; }

		internal EntityInfo Info { get; set; }

		public static bool operator !=(Entity lhs, Entity rhs)
			=> !(lhs == rhs);

		public static bool operator ==(Entity lhs, Entity rhs)
			=> lhs.Id == rhs.Id && lhs.Generation == rhs.Generation;

		public TComponent AddComponent<TComponent>()
			where TComponent : IComponent
		{
			if (!IsAlive)
				throw new EntityIsNotAliveException(this);

			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentIndex = Info.ComponentIndexes[componentPoolIndex];
			if (componentIndex != 0)
				// TODO: throw proper exception
				throw new Exception();

			componentIndex = Info.WorldOwner.ComponentPools[componentPoolIndex].AddComponent();

			Info.ComponentIndexes[componentPoolIndex] = componentIndex;
			Info.GetComponents.IsDirty = true;

			return (TComponent)Info.WorldOwner.ComponentPools[componentPoolIndex].GetComponent(componentIndex);
		}

		public TComponent AddComponent<TComponent>(TComponent component = default)
			where TComponent : IComponent
		{
			if (HasComponent<TComponent>())
				throw new EntityAlreadyHasComponentException(this, typeof(TComponent));

			var componentPoolIndex = ComponentIndex<TComponent>.Index;

			Info.ComponentIndexes[componentPoolIndex] =
				Info.WorldOwner.ComponentPools[componentPoolIndex].AddComponent(component);
			Info.GetComponents.IsDirty = true;

			return component;
		}

		public bool HasComponent<TComponent>()
			where TComponent : IComponent
		{
			if (!IsAlive)
				throw new EntityIsNotAliveException(this);

			return Info.ComponentIndexes[ComponentIndex<TComponent>.Index] != 0;
		}

		public TComponent GetComponent<TComponent>()
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>())
				throw new EntityNotHaveComponentException(this, typeof(TComponent));

			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentIndex = Info.ComponentIndexes[componentPoolIndex];

			return (TComponent)Info.WorldOwner.ComponentPools[componentPoolIndex].GetComponent(componentIndex);
		}

		public IComponent[] GetComponents()
		{
			if (!IsAlive)
				throw new EntityIsNotAliveException(this);
			return Info.GetComponents.Data;
		}

		public void ReplaceComponent<TComponent>(TComponent newComponent)
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>())
				AddComponent(newComponent);
			else
			{
				var componentPoolIndex = ComponentIndex<TComponent>.Index;
				var componentIndex = Info.ComponentIndexes[componentPoolIndex];

				Info.WorldOwner.ComponentPools[componentPoolIndex].SetComponent(componentIndex, newComponent);
				Info.GetComponents.IsDirty = true;
			}
		}

		public void RemoveComponent<TComponent>()
			where TComponent : IComponent
		{
			if (!HasComponent<TComponent>())
				throw new EntityNotHaveComponentException(this, typeof(TComponent));

			var componentPoolIndex = ComponentIndex<TComponent>.Index;
			var componentIndex = Info.ComponentIndexes[componentPoolIndex];

			Info.WorldOwner.ComponentPools[componentPoolIndex].ClearComponent(componentIndex);
			Info.ComponentIndexes[componentPoolIndex] = 0;
			Info.GetComponents.IsDirty = true;
		}

		public void RemoveComponents()
		{
			if (!IsAlive)
				throw new EntityIsNotAliveException(this);

			var componentPools = Info.WorldOwner.ComponentPools;
			for (int i = 0; i < Info.ComponentIndexes.Length; i++)
			{
				if (Info.ComponentIndexes[i] != 0)
				{
					componentPools[i].ClearComponent(Info.ComponentIndexes[i]);
					Info.ComponentIndexes[i] = 0;
				}
			}
			Info.GetComponents.IsDirty = true;
		}

		public void Destroy()
		{
			if (!IsAlive)
				throw new EntityIsNotAliveException(this);

			World.Worlds[WorldId].DestroyEntity(this);
		}

		public int CompareTo(Entity other)
		{
			int compare = Generation.CompareTo(other.Generation);
			if (compare == 0)
				compare = Id.CompareTo(Id);
			return compare;
		}

		public bool Equals(Entity other)
			=> this == other;

		public override bool Equals(object obj)
			=> obj is Entity other && (this == other);

		public override int GetHashCode()
			=> (Id, Generation).GetHashCode();

		public override string ToString()
			=> $"{Id}, {Generation}";
	}
}