using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EcsLte.Utilities
{
    // https://stackoverflow.com/questions/24864233/marshal-structuretoptr-without-boxing
    public static unsafe class InteropTools
    {
        private static readonly Type SafeBufferType = typeof(SafeBuffer);

        public delegate void PtrToStructureNativeDelegate(byte* ptr, TypedReference structure, uint sizeofT);

        public delegate void StructureToPtrNativeDelegate(TypedReference structure, byte* ptr, uint sizeofT);

        private const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly MethodInfo PtrToStructureNativeMethod = SafeBufferType.GetMethod("PtrToStructureNative", flags);
        private static readonly MethodInfo StructureToPtrNativeMethod = SafeBufferType.GetMethod("StructureToPtrNative", flags);
        public static readonly PtrToStructureNativeDelegate PtrToStructureNative = (PtrToStructureNativeDelegate)Delegate.CreateDelegate(typeof(PtrToStructureNativeDelegate), PtrToStructureNativeMethod);
        public static readonly StructureToPtrNativeDelegate StructureToPtrNative = (StructureToPtrNativeDelegate)Delegate.CreateDelegate(typeof(StructureToPtrNativeDelegate), StructureToPtrNativeMethod);

        private static readonly Func<Type, bool, int> SizeOfHelper_f = (Func<Type, bool, int>)Delegate.CreateDelegate(typeof(Func<Type, bool, int>), typeof(Marshal).GetMethod("SizeOfHelper", flags));

        public static void StructureToPtrDirect(TypedReference structure, IntPtr ptr, int size) => StructureToPtrNative(structure, (byte*)ptr, unchecked((uint)size));

        public static void StructureToPtrDirect(TypedReference structure, IntPtr ptr) => StructureToPtrDirect(structure, ptr, SizeOf(__reftype(structure)));

        public static void PtrToStructureDirect(IntPtr ptr, TypedReference structure, int size) => PtrToStructureNative((byte*)ptr, structure, unchecked((uint)size));

        public static void PtrToStructureDirect(IntPtr ptr, TypedReference structure) => PtrToStructureDirect(ptr, structure, SizeOf(__reftype(structure)));

        public static void StructureToPtr<T>(ref T structure, IntPtr ptr) => StructureToPtrDirect(__makeref(structure), ptr);

        public static void PtrToStructure<T>(IntPtr ptr, out T structure)
        {
            structure = default(T);
            PtrToStructureDirect(ptr, __makeref(structure));
        }

        public static T PtrToStructure<T>(IntPtr ptr)
        {
            PtrToStructure(ptr, out T obj);
            return obj;
        }

        public static int SizeOf<T>(T structure) => SizeOf<T>();

        public static int SizeOf<T>() => SizeOf(typeof(T));

        public static int SizeOf(Type t) => SizeOfHelper_f(t, true);
    }
}