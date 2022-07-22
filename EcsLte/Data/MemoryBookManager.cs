using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte.Data
{
    public unsafe class MemoryBookManager
    {
        private static readonly int _minPageCount = 10;

        private readonly List<MemoryBook> _books;
        private readonly Queue<MemoryPage> _unusedPages;
        private readonly object _lockObj;

        internal MemoryBookManager()
        {
            _books = new List<MemoryBook>();
            _unusedPages = new Queue<MemoryPage>();
            _lockObj = new object();
        }

        internal MemoryPage CheckoutPage()
        {
            lock (_lockObj)
            {
                AllocatePages(1);

                return _unusedPages.Dequeue();
            }
        }

        internal void CheckoutPages1(MemoryPage* pages, int startingIndex, int count)
        {
            lock (_lockObj)
            {
                AllocatePages(count);

                for (var i = 0; i < count; i++)
                    pages[i + startingIndex] = _unusedPages.Dequeue();
            }
        }

        internal void CheckoutPages1(ref MemoryPage[] pages, int startingIndex, int count)
        {
            lock (_lockObj)
            {
                AllocatePages(count);

                for (var i = 0; i < count; i++)
                    pages[i + startingIndex] = _unusedPages.Dequeue();
            }
        }

        internal void ReturnPage1(MemoryPage page)
        {
            lock (_lockObj)
            {
                _unusedPages.Enqueue(page);
            }
        }

        internal void ReturnPages1(MemoryPage* pages, int startingIndex, int count)
        {
            lock (_lockObj)
            {
                for (var i = 0; i < count; i++)
                    _unusedPages.Enqueue(pages[i + startingIndex]);
            }
        }

        internal void ReturnPages1(MemoryPage[] pages, int startingIndex, int count)
        {
            lock (_lockObj)
            {
                for (var i = 0; i < count; i++)
                    _unusedPages.Enqueue(pages[i + startingIndex]);
            }
        }

        internal void AllocatePages(int count)
        {
            lock (_lockObj)
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
        }

        internal void InternalDestroy()
        {
            foreach (var book in _books)
                MemoryHelper.Free(book.Buffer);
            _books.Clear();
            _unusedPages.Clear();
        }
    }
}
