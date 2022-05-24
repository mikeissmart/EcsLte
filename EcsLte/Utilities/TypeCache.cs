using System.Runtime.InteropServices;

namespace EcsLte.Utilities
{
    internal static class TypeCache<T>
    {
        public static int SizeInBytes { get; private set; } = Marshal.SizeOf(typeof(T));
        public static int HashCode { get; private set; } = typeof(T).GetHashCode();
    }
}