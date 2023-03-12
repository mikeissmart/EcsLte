using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data
{
    internal interface IDataCatalog : IDisposable
    {
        void CheckUnusedPages(int pageCount);
        IDataPage DequeuePage();
        void EnqueuePage(IDataPage page);
    }

    internal abstract class BaseDataCatalog<TBook> : IDataCatalog
        where TBook : BaseDataBook, new()
    {
        protected List<TBook> _allBooks = new List<TBook>();
        protected Queue<IDataPage> _unusedPages = new Queue<IDataPage>();

        public void CheckUnusedPages(int pageCount)
        {
            if (_unusedPages.Count < pageCount)
            {
                var book = new TBook();
                book.InitPages(Math.Max(pageCount, DataManager.MinBookPageCount));
                _allBooks.Add(book);
                for (var i = 0; i < book.Pages.Length; i++)
                    _unusedPages.Enqueue(book.Pages[i]);
            }
        }

        public IDataPage DequeuePage()
        {
            var page = _unusedPages.Dequeue();
            page.Book.DequeuePage();

            return page;
        }

        public void EnqueuePage(IDataPage page)
        {
            page.Book.EnqueuePage();
            _unusedPages.Enqueue(page);
        }

        public void Dispose()
        {
            for (var i = 0; i < _allBooks.Count; i++)
                _allBooks[i].Dispose();
            _allBooks = null;
            _unusedPages = null;
        }
    }

    internal class DataCatalog<TBook> : BaseDataCatalog<TBook>
        where TBook : BaseDataBook, new()
    {
    }
}
