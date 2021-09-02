using System;

namespace EcsLte.PerformanceTest
{
    public struct BitMask : IEquatable<BitMask>
    {
        public static readonly BitMask Zero = new();
        public static readonly BitMask All = new() {Mask = -1};

        public int Mask { get; set; }

        public BitMask(int mask)
        {
            Mask = mask;
        }

        public void SetBit(int position, bool isTrue)
        {
            if (position < 0 || position > 32)
                throw new ArgumentOutOfRangeException();
            Mask &= (int) Math.Pow(isTrue ? 1 : 0, position);
        }

        public static implicit operator int(BitMask obj)
        {
            return obj.Mask;
        }

        public static bool operator !=(BitMask lhs, BitMask rhs)
        {
            return lhs.Mask != rhs.Mask;
        }

        public static bool operator ==(BitMask lhs, BitMask rhs)
        {
            return lhs.Mask == rhs.Mask;
        }

        public static BitMask operator &(BitMask lhs, BitMask rhs)
        {
            return lhs.And(lhs);
        }

        public static BitMask operator |(BitMask lhs, BitMask rhs)
        {
            return lhs.Or(lhs);
        }

        public static BitMask operator ^(BitMask lhs, BitMask rhs)
        {
            return lhs.Xor(lhs);
        }

        public bool Equals(int other)
        {
            return this == other;
        }

        public bool Equals(BitMask other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is BitMask otherMask && this == otherMask ||
                   obj is int otherInt && Mask == otherInt;
        }

        public override int GetHashCode()
        {
            return Mask.GetHashCode();
        }

        public override string ToString()
        {
            return Mask.ToString("x8");
        }

        public BitMask And(int other)
        {
            return new BitMask
            {
                Mask = Mask & other
            };
        }

        public BitMask Or(int other)
        {
            return new BitMask
            {
                Mask = Mask | other
            };
        }

        public BitMask Xor(int other)
        {
            return new BitMask
            {
                Mask = Mask ^ other
            };
        }

        public BitMask Invert()
        {
            return new BitMask
            {
                Mask = ~Mask
            };
        }
    }
}