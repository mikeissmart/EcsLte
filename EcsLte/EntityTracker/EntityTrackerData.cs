using EcsLte.Data;
using EcsLte.Utilities;
using System;

namespace EcsLte
{
    /*internal unsafe class EntityTrackerData
    {
        private static readonly int _entitiesPerPage = MemoryPage.PageBufferSizeInBytes / TypeCache<Entity>.SizeInBytes;

        private MemoryPage[] _addPages = new MemoryPage[0];
        private MemoryPage[] _updatePages = new MemoryPage[0];
        private int _addPagesCount;
        private int _updatePagesCount;

        internal EntityTracker.TrackingState Tracking { get; private set; }

        public EntityTrackerData(EntityTracker.TrackingState tracking) => Tracking = tracking;

        internal void ChangeTracking(EntityTracker.TrackingState tracking, MemoryBookManager bookManager)
        {
            if (Tracking == tracking)
                return;

            if (tracking == EntityTracker.TrackingState.Added)
            {
                if (Tracking == EntityTracker.TrackingState.Updated ||
                    Tracking == EntityTracker.TrackingState.AddedOrUpdated)
                {
                    bookManager.ReturnPages(_updatePages, _updatePagesCount, 0);
                    _updatePagesCount = 0;
                }
            }
            else if (tracking == EntityTracker.TrackingState.Updated)
            {
                if (Tracking == EntityTracker.TrackingState.Added ||
                    Tracking == EntityTracker.TrackingState.AddedOrUpdated)
                {
                    bookManager.ReturnPages(_addPages, _addPagesCount, 0);
                    _addPagesCount = 0;
                }
            }
            Tracking = tracking;
        }

        internal void ResizeEntityCapacity(int entityCapacity, MemoryBookManager bookManager)
        {
            var newPageCount = entityCapacity / _entitiesPerPage +
                (entityCapacity % _entitiesPerPage > 0 ? 1 : 0);
            if (_addPagesCount != newPageCount &&
                (Tracking == EntityTracker.TrackingState.Added || Tracking == EntityTracker.TrackingState.AddedOrUpdated))
            {
                CheckPages(ref _addPages, newPageCount, _addPagesCount);
                bookManager.CheckoutPages(ref _addPages, newPageCount - _addPagesCount, _addPagesCount);
                _addPagesCount = newPageCount;
            }
            if (_updatePagesCount != newPageCount &&
                (Tracking == EntityTracker.TrackingState.Updated || Tracking == EntityTracker.TrackingState.AddedOrUpdated))
            {
                CheckPages(ref _updatePages, newPageCount, _updatePagesCount);
                bookManager.CheckoutPages(ref _updatePages, newPageCount - _updatePagesCount, _updatePagesCount);
                _updatePagesCount = newPageCount;
            }
        }

        internal void ReturnAllPages(MemoryBookManager bookManager)
        {
            if (_addPagesCount > 0)
            {
                bookManager.ReturnPages(_addPages, _addPagesCount, 0);
                _addPagesCount = 0;
            }
            if (_updatePagesCount > 0)
            {
                bookManager.ReturnPages(_updatePages, _updatePagesCount, 0);
                _updatePagesCount = 0;
            }
        }

        internal void SetAddEntity(Entity entity) => SetEntity(entity, _addPages);

        internal void SetUpdateEntity(Entity entity) => SetEntity(entity, _updatePages);

        internal void ClearPages()
        {
            for (var i = 0; i < _addPagesCount; i++)
                MemoryHelper.Clear(_addPages[i].Buffer, MemoryPage.PageBufferSizeInBytes);
            for (var i = 0; i < _updatePagesCount; i++)
                MemoryHelper.Clear(_updatePages[i].Buffer, MemoryPage.PageBufferSizeInBytes);
        }

        internal bool HasEntity(Entity entity)
        {
            switch (Tracking)
            {
                case EntityTracker.TrackingState.Added:
                    return GetEntity(entity, _addPages) == entity;
                case EntityTracker.TrackingState.Updated:
                    return GetEntity(entity, _updatePages) == entity;
                case EntityTracker.TrackingState.AddedOrUpdated:
                    return GetEntity(entity, _addPages) == entity ||
                        GetEntity(entity, _updatePages) == entity;
                default:
                    throw new ArgumentException();
            }
        }

        private void CheckPages(ref MemoryPage[] pages, int newPageCount, int currentPageCount)
        {
            if (newPageCount > (pages.Length - currentPageCount))
            {
                Array.Resize(ref pages, newPageCount);
            }
        }

        private Entity GetEntity(Entity entity, MemoryPage[] pages)
        {
            var pageIndex = entity.Id / _entitiesPerPage;
            var slotOffset = (entity.Id % _entitiesPerPage) * TypeCache<Entity>.SizeInBytes;
            return *(Entity*)(pages[pageIndex].Buffer + slotOffset);
        }

        private void SetEntity(Entity entity, MemoryPage[] pages)
        {
            var pageIndex = entity.Id / _entitiesPerPage;
            var slotOffset = (entity.Id % _entitiesPerPage) * TypeCache<Entity>.SizeInBytes;
            *(Entity*)(pages[pageIndex].Buffer + slotOffset) = entity;
        }
    }*/
}
