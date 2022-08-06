using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte.Data
{
    /*public unsafe class MemoryBookManager
    {
        private static readonly int _minPageCount = 10;

        // TODO change managed classes to unmanaged arrays
        private readonly List<MemoryBook> _books;
        private readonly Queue<MemoryPage> _unusedPages;

        internal MemoryBookManager()
        {
            _books = new List<MemoryBook>();
            _unusedPages = new Queue<MemoryPage>();
        }

        internal MemoryPage CheckoutPage()
        {
            AllocatePages(1);

            return _unusedPages.Dequeue();
        }

        internal void CheckoutPages(MemoryPage* pages, int startingIndex, int count)
        {
            AllocatePages(count);

            for (var i = 0; i < count; i++)
                pages[i + startingIndex] = _unusedPages.Dequeue();
        }

        internal void CheckoutPages(ref MemoryPage[] pages, int startingIndex, int count)
        {
            AllocatePages(count);

            for (var i = 0; i < count; i++)
                pages[i + startingIndex] = _unusedPages.Dequeue();
        }

        internal void ReturnPage(MemoryPage page)
        {
            _unusedPages.Enqueue(page);
        }

        internal void ReturnPages(MemoryPage* pages, int startingIndex, int count)
        {
            for (var i = 0; i < count; i++)
                _unusedPages.Enqueue(pages[i + startingIndex]);
        }

        internal void ReturnPages(MemoryPage[] pages, int startingIndex, int count)
        {
            for (var i = 0; i < count; i++)
                _unusedPages.Enqueue(pages[i + startingIndex]);
        }

        internal void AllocatePages(int count)
        {
            count -= _unusedPages.Count;
            if (count < 1)
                return;

            count = Math.Max(count, _minPageCount);
            var book = new MemoryBook
            {
                Index = _books.Count,
                PageCount = count,
                Buffer = MemoryHelper.Alloc<byte>(MemoryPage.PageBufferSizeInBytes * count)
            };
            _books.Add(book);

            for (var i = 0; i < count; i++)
                _unusedPages.Enqueue(book.GetPage(i));
        }

        internal void InternalDestroy()
        {
            foreach (var book in _books)
                MemoryHelper.Free(book.Buffer);
            _books.Clear();
            _unusedPages.Clear();
        }
    }*/
}
