using System;

namespace EcsLte
{
    public struct Entity : IEquatable<Entity>, IComparable<Entity>
    {
        public static readonly Entity Null = new Entity();

        public bool IsNotNull => this != Null;
        public bool IsNull => this == Null;

        public int Id { get; internal set; }
        public int Version { get; internal set; }

        public static bool operator !=(Entity lhs, Entity rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Entity lhs, Entity rhs)
        {
            return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
        }

        public int CompareTo(Entity other)
        {
            var compare = Version.CompareTo(other.Version);
            if (compare == 0)
                compare = Id.CompareTo(Id);
            return compare;
        }

        public bool Equals(Entity other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && this == other;
        }

        public override int GetHashCode()
        {
            return (Id, Version).GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}, {Version}";
        }

        internal static Entity CreateFromInfo(EntityInfo entityInfo)
        {
            return new Entity { Id = entityInfo.Id, Version = entityInfo.Version };
        }
    }
}