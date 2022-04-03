using System;

namespace EcsLte.Data.Unmanaged
{
    internal struct PtrWrapper : IEquatable<PtrWrapper>, IComparable<PtrWrapper>
    {
        public static readonly PtrWrapper Null = new PtrWrapper();

        public bool IsNotNull => this != Null;
        public bool IsNull => this == Null;

        public unsafe void* Ptr;

        public static bool operator !=(PtrWrapper lhs, PtrWrapper rhs) => !(lhs == rhs);

        public static unsafe bool operator ==(PtrWrapper lhs, PtrWrapper rhs) => lhs.Ptr == rhs.Ptr;

        public unsafe int CompareTo(PtrWrapper other) => GetHashCode().CompareTo(other.GetHashCode());

        public bool Equals(PtrWrapper other) => this == other;

        public override bool Equals(object other) => other is PtrWrapper obj && this == obj;

        public override unsafe int GetHashCode()
        {
            fixed (void** ptr = &Ptr)
            {
                return *(int*)ptr;
            }
        }
    }
}
