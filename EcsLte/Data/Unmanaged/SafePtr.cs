namespace EcsLte.Data.Unmanaged
{
    public struct SafePtr
    {
        public unsafe void* Ptr;
        public int LengthInBytes;
    }
}
