using System;

namespace EcsLte.Data
{
    internal unsafe struct PtrWrapper : IEquatable<PtrWrapper>, IComparable<PtrWrapper>
    {
        public static readonly PtrWrapper Null = new PtrWrapper();

        private void* _ptr;

        public bool IsNotNull => this != Null;
        public bool IsNull => this == Null;

        public void* Ptr
        {
            get => _ptr;
            set => _ptr = value;
        }

        public static bool operator !=(PtrWrapper lhs, PtrWrapper rhs) => !(lhs == rhs);

        public static unsafe bool operator ==(PtrWrapper lhs, PtrWrapper rhs) => lhs._ptr == rhs._ptr;

        public int CompareTo(PtrWrapper other) => GetHashCode().CompareTo(other.GetHashCode());

        public bool Equals(PtrWrapper other) => this == other;

        public override bool Equals(object other) => other is PtrWrapper obj && this == obj;

        public override int GetHashCode()
        {
            fixed (void** ptr = &_ptr)
            {
                return *(int*)ptr;
            }
        }
    }
}
