using EcsLte.Utilities;
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

        public static bool operator !=(Entity lhs, Entity rhs) => !(lhs == rhs);

        public static bool operator ==(Entity lhs, Entity rhs) => lhs.Id == rhs.Id && lhs.Version == rhs.Version;

        public bool Equals(Entity other) => this == other;

        public override bool Equals(object other) => other is Entity obj && this == obj;

        public override string ToString() => $"({Id}, {Version})";

        public override int GetHashCode() => HashCodeHelper.StartHashCode()
                .AppendHashCode(Id)
                .AppendHashCode(Version)
                .HashCode;
        public int CompareTo(Entity other)
        {
            var compare = Version.CompareTo(other.Version);
            if (compare == 0)
                compare = Id.CompareTo(Id);
            return compare;
        }
    }
}
