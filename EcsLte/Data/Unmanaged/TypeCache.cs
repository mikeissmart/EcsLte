using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte.Data.Unmanaged
{
    public static class TypeCache<T>
    {
        public static int SizeInBytes { get; private set; } = Marshal.SizeOf(typeof(T));
        public static int HashCode { get; private set; } = typeof(T).GetHashCode();
    }
}
