using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EcsLte.Data
{
    internal interface IDataPage
    {
        IDataBook Book { get; }

        void SetObj(object item, int index);
        object GetObj(int index);
        void Move(int srcIndex, int destIndex);
        void Move(IDataPage srcPage, int srcIndex, int destIndex);
        void Copy(int srcIndex, int destIndex);
        void Copy(IDataPage srcPage, int srcIndex, int destIndex);
        void Copy(IDataPage srcPage, int srcIndex, int destIndex, int count);
        void Clear(int index);
        void Clear(int index, int count);
    }

    internal class ManagedDataPage<T> : IDataPage
    {
        private ManagedDataBook<T> _book;
        private int _itemIndex;

        public IDataBook Book => _book;

        internal ManagedDataPage(ManagedDataBook<T> book, int pageIndex)
        {
            _book = book;
            _itemIndex = pageIndex * DataManager.PageCapacity;
        }

        void IDataPage.SetObj(object item, int index)
            => Set((T)item, index);

        object IDataPage.GetObj(int index)
            => Get(index);

        public void Move(int srcIndex, int destIndex)
        {
            _book.Items[_itemIndex + destIndex] = _book.Items[_itemIndex + srcIndex];
            _book.Items[_itemIndex + srcIndex] = default;
        }

        public void Move(IDataPage srcPage, int srcIndex, int destIndex)
        {
            var page = (ManagedDataPage<T>)srcPage;

            _book.Items[_itemIndex + destIndex] = page._book.Items[page._itemIndex + srcIndex];
            page._book.Items[page._itemIndex + srcIndex] = default;
        }

        public void Copy(int srcIndex, int destIndex)
            => _book.Items[_itemIndex + destIndex] = _book.Items[_itemIndex + srcIndex];

        public void Copy(IDataPage srcPage, int srcIndex, int destIndex)
        {
            var page = ((ManagedDataPage<T>)srcPage);

            _book.Items[_itemIndex + destIndex] = page._book.Items[page._itemIndex + srcIndex];
        }

        public void Copy(IDataPage srcPage, int srcIndex, int destIndex, int count)
        {
            var page = ((ManagedDataPage<T>)srcPage);

            Helper.ArrayCopy(
                page._book.Items,
                page._itemIndex + srcIndex,
                _book.Items,
                _itemIndex + destIndex,
                count);
        }

        public void Copy(in T[] src, int srcIndex, int destIndex, int count)
            => Helper.ArrayCopy(
                src,
                srcIndex,
                _book.Items,
                _itemIndex + destIndex,
                count);

        public void Clear(int index)
            => _book.Items[_itemIndex + index] = default;

        public void Clear(int index, int count)
            => Array.Clear(_book.Items, _itemIndex + index, count);

        public ref T GetRef(int index)
            => ref _book.Items[_itemIndex + index];

        public T Get(int index)
            => _book.Items[_itemIndex + index];

        public void GetRange(ref T[] dest, int startingIndex, int count)
            => Helper.ArrayCopy(
                _book.Items,
                _itemIndex,
                dest,
                startingIndex,
                count);

        public void Set(T item, int index)
            => _book.Items[_itemIndex + index] = item;

        public void SetRange(T item, int startingIndex, int count)
        {
            for (var i = 0; i < count; i++)
                _book.Items[_itemIndex + startingIndex + i] = item;
        }
    }

    internal unsafe class UnmanagedDataPage<T> : IDataPage
        where T : unmanaged
    {
        UnmanagedDataBook<T> _book;
        private T* _items;

        public IDataBook Book => _book;

        internal UnmanagedDataPage(UnmanagedDataBook<T> book, int pageIndex)
        {
            _book = book;
            _items = book.Items + (pageIndex * DataManager.PageCapacity);
        }

        void IDataPage.SetObj(object item, int index)
            => Set((T)item, index);

        object IDataPage.GetObj(int index)
            => Get(index);

        public void Move(int srcIndex, int destIndex)
            => _items[destIndex] = _items[srcIndex];
        // No need for clear since theres no gc

        public void Move(IDataPage srcPage, int srcIndex, int destIndex)
        {
            var page = (UnmanagedDataPage<T>)srcPage;

            _items[destIndex] = page._items[srcIndex];
            // No need for clear since theres no gc
        }

        public void Copy(int srcIndex, int destIndex)
            => _items[destIndex] = _items[srcIndex];

        public void Copy(IDataPage srcPage, int srcIndex, int destIndex)
        {
            var page = ((UnmanagedDataPage<T>)srcPage);

            _items[destIndex] = page._items[srcIndex];
        }

        public void Copy(IDataPage srcPage, int srcIndex, int destIndex, int count)
        {
            var page = ((UnmanagedDataPage<T>)srcPage);

            MemoryHelper.Copy(
                page._items + srcIndex,
                _items + destIndex,
                count);
        }

        public void Copy(in T[] src, int srcIndex, int destIndex, int count)
        {
            fixed (T* ptr = &src[srcIndex])
            {
                MemoryHelper.Copy(
                    ptr,
                    _items + destIndex,
                    count);
            }
        }

        public void Clear(int index)
            => _items[index] = default;

        public void Clear(int index, int count)
            => MemoryHelper.Clear(_items + index, count);

        public ref T GetRef(int index)
            => ref *(_items + index);

        public T Get(int index)
            => *(_items + index);

        public void GetRange(ref T[] dest, int startingIndex, int count)
        {
            fixed (T* destPtr = &dest[startingIndex])
            {
                MemoryHelper.Copy(
                    _items,
                    destPtr,
                    count);
            }
        }

        public void Set(T item, int index)
            => *(_items + index) = item;

        public void SetRange(T item, int startingIndex, int count)
        {
            for (var i = 0; i < count; i++)
                *(_items + startingIndex + i) = item;
        }
    }
}
