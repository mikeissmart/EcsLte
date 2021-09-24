using System;

namespace EcsLte.Utilities
{
    internal struct BitMask : IEquatable<BitMask>
    {
        private int _hashCode;
        private string _toString;
        private int[] _masks;

        public int[] Masks { get => _masks; }

        public static bool operator !=(BitMask lhs, BitMask rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(BitMask lhs, BitMask rhs)
        {
            if (lhs._masks == null && rhs._masks == null)
                return true;
            else if (lhs._masks == null && rhs._masks != null)
                return false;
            else if (lhs._masks != null && rhs._masks == null)
                return false;

            for (int i = 0; i < lhs._masks.Length; i++)
            {
                if (lhs._masks[i] != rhs._masks[i])
                    return false;
            }
            return true;
        }

        public static BitMask operator &(BitMask lhs, BitMask rhs)
        {
            var result = new BitMask();
            if (lhs._masks != null && rhs._masks != null)
            {
                var maxMasks = Math.Max(lhs._masks.Length, rhs._masks.Length);
                result._masks = new int[maxMasks];
                for (int i = 0; i < maxMasks; i++)
                    result._masks[i] = lhs._masks[i] & rhs._masks[i];
            }

            return result;
        }

        public static BitMask operator |(BitMask lhs, BitMask rhs)
        {
            var result = new BitMask();
            if (lhs._masks != null && rhs._masks != null)
            {
                var maxMasks = 0;
                var minMasks = 0;
                BitMask longerMask;
                if (lhs._masks.Length > rhs._masks.Length)
                {
                    minMasks = rhs._masks.Length;
                    maxMasks = lhs._masks.Length;
                    longerMask = lhs;
                }
                else
                {
                    minMasks = rhs._masks.Length;
                    maxMasks = lhs._masks.Length;
                    longerMask = rhs;
                }

                result._masks = new int[maxMasks];
                for (int i = 0; i < minMasks; i++)
                    result._masks[i] = lhs._masks[i] | rhs._masks[i];
                for (int i = minMasks; i < maxMasks; i++)
                    result._masks[i] = longerMask._masks[i];
            }

            return result;
        }

        public static BitMask operator ^(BitMask lhs, BitMask rhs)
        {
            var result = new BitMask();
            if (lhs._masks != null && rhs._masks != null)
            {
                var maxMasks = Math.Max(lhs._masks.Length, rhs._masks.Length);
                result._masks = new int[maxMasks];
                for (int i = 0; i < maxMasks; i++)
                    result._masks[i] = lhs._masks[i] ^ rhs._masks[i];
            }

            return result;
        }

        public static BitMask operator ~(BitMask other)
        {
            var result = new BitMask();
            if (other._masks != null)
            {
                result._masks = new int[other._masks.Length];
                for (int i = 0; i < other._masks.Length; i++)
                    result._masks[i] = ~other._masks[i];
            }

            return result;
        }

        public bool Equals(BitMask other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is BitMask otherMask && this == otherMask;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return _toString;
        }

        public bool IsAnySet()
        {
            if (_masks == null)
                return false;
            foreach (var mask in _masks)
            {
                if (mask > 0)
                    return true;
            }

            return false;
        }

        public bool IsAnyClear()
        {
            return !IsAnySet();
        }

        public bool IsSet(int bit)
        {
            if (_masks == null || _masks.Length == 0)
                return false;

            int maskIndex = bit / 32;
            int pos = (bit - (maskIndex * 32)) % 32;
            int flag = 1 << pos;

            if (_masks.Length <= maskIndex)
                return (_masks[maskIndex] & flag) == flag;
            return false;
        }

        public bool IsClear(int bit)
        {
            return !IsSet(bit);
        }

        public void Set(int bit)
        {
            if (IsSet(bit))
                // Already set
                return;

            int maskIndex = bit / 32;
            int pos = (bit - (maskIndex * 32)) % 32;
            int flag = 1 << pos;

            if (_masks == null)
                _masks = new int[maskIndex + 1];
            else if (_masks.Length <= maskIndex)
                Array.Resize(ref _masks, maskIndex + 1);

            _masks[maskIndex] |= flag;
            CalculateHashCode();
        }

        public void Clear(int bit)
        {
            if (!IsSet(bit))
                // Nothing to set
                return;

            int maskIndex = bit / 32;
            int pos = (bit - (maskIndex * 32)) % 32;
            int flag = 1 << pos;

            _masks[maskIndex] ^= flag;
            CalculateHashCode();
        }

        private void CalculateHashCode()
        {
            _hashCode = -1663471673;
            _hashCode = _hashCode * -1521134295 + _masks.Length;
            foreach (var index in _masks)
                _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            _toString = "";
            foreach (var mask in _masks)
                _toString += mask.ToString("x8");
        }
    }
}