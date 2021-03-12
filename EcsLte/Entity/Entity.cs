using System;

namespace EcsLte
{
	public struct Entity : IEquatable<Entity>, IComparable<Entity>
	{
		public static readonly Entity Null = new Entity();

		public bool IsNotNull { get => this != Null; }
		public bool IsNull { get => this == Null; }

		public int Id { get => Info?.Id ?? 0; }
		public int Generation { get => Info?.Generation ?? 0; }
		public int WorldId { get => Info?.World.WorldId ?? 0; }

		internal EntityInfo Info { get; set; }

		public static bool operator !=(Entity lhs, Entity rhs)
			=> !(lhs == rhs);

		public static bool operator ==(Entity lhs, Entity rhs)
			=> lhs.Id == rhs.Id && lhs.Generation == rhs.Generation;

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