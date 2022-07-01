namespace EcsLte.Data
{
    internal unsafe struct MemoryBook
    {
        internal int Index;
        internal int PageCount;
        internal byte* Buffer;

        internal MemoryPage GetPage(int pageIndex) => new MemoryPage
        {
            Buffer = Buffer + (pageIndex * MemoryPage.PageBufferSizeInBytes)
        };
    }
}
