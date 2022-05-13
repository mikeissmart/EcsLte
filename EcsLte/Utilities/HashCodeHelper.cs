namespace EcsLte.Utilities
{
    internal struct HashCodeHelper
    {
        private static readonly int _hashCodeStart = -612338121;
        private static readonly int _hashCodeAppend = -1521134295;

        internal int HashCode { get; private set; }

        internal static HashCodeHelper StartHashCode() => new HashCodeHelper
        {
            HashCode = _hashCodeStart
        };

        internal HashCodeHelper AppendHashCode<T>(T obj)
        {
            HashCode = HashCode * _hashCodeAppend + obj.GetHashCode();

            return this;
        }

        public override int GetHashCode() => HashCode;
    }
}
