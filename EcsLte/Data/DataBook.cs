using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data
{
    internal interface IDataBook : IDisposable
    {
        void DequeuePage();
        void EnqueuePage();
    }

    internal abstract class BaseDataBook : IDataBook
    {
        public IDataPage[] Pages { get; protected set; }
        public int UnusedPagesCount { get; protected set; }

        public void DequeuePage() => UnusedPagesCount--;

        public void EnqueuePage() => UnusedPagesCount++;

        public abstract void InitPages(int pageCount);

        public abstract void Dispose();
    }

    internal class ManagedDataBook<T> : BaseDataBook
    {
        internal T[] Items;

        public override void InitPages(int pageCount)
        {
            Items = new T[pageCount * DataManager.PageCapacity];
            Pages = new ManagedDataPage<T>[pageCount];
            for (var i = 0; i < pageCount; i++)
                Pages[i] = new ManagedDataPage<T>(this, i);
            UnusedPagesCount = pageCount;
        }

        public override void Dispose()
        {
            Items = null;
        }
    }

    internal unsafe class UnmanagedDataBook<T> : BaseDataBook
        where T : unmanaged
    {
        internal T* Items;

        public override void InitPages(int pageCount)
        {
            Items = MemoryHelper.Alloc<T>(pageCount * DataManager.PageCapacity);
            Pages = new UnmanagedDataPage<T>[pageCount];
            for (var i = 0; i < pageCount; i++)
                Pages[i] = new UnmanagedDataPage<T>(this, i);
            UnusedPagesCount = pageCount;
        }

        public override void Dispose()
        {
            MemoryHelper.Free(Items);
            Items = null;
        }
    }
}
