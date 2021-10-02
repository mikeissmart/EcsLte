using System;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityFilter : IEcsContext, IGetEntity, IGetWatcher
    {
        private readonly EntityFilterData _data;
        private EcsContextData _ecsContextData;
        private readonly WatcherTable _watcherTable;

        internal EntityFilter(EcsContext context, EcsContextData ecsContextData, Filter filter,
            ComponentArcheTypeData[] archeTypeDatas)
        {
            _data = ObjectCache.Pop<EntityFilterData>();
            _data.Initialize(ecsContextData, archeTypeDatas);

            ecsContextData.ArcheTypeDataAdded += OnCompoinentArcheTypeDataAdded;
            ecsContextData.ArcheTypeDataRemoved += OnCompoinentArcheTypeDataRemoved;

            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var archData = archeTypeDatas[i];
                archData.EntityAdded += OnEntityComponentAdded;
                archData.EntityRemoved += OnEntityComponentRemoved;
                archData.EntityUpdated += OnEntityComponentUpdated;

                ParallelRunner.RunParallelForEach(archData.GetEntities(),
                    entity => _data.Entities[entity.Id] = entity);
            }

            _watcherTable = ObjectCache.Pop<WatcherTable>();
            _watcherTable.Initialize(context);

            _ecsContextData = ecsContextData;

            CurrentContext = context;
            Filter = filter;
        }

        #region EcsContext

        public EcsContext CurrentContext { get; }

        #endregion

        #region EntityFilter

        public Filter Filter { get; }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);
            _watcherTable.Reset();
            ObjectCache.Push(_watcherTable);
        }

        #endregion

        #region GetEntity

        public bool HasEntity(Entity entity)
        {
            if (!CurrentContext.HasEntity(entity))
                return false;

            return _data.Entities[entity.Id] == entity;
        }

        public Entity[] GetEntities()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.GetEntities();
        }

        #endregion

        #region GetWatcher

        public Watcher Added(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Added(filter);
        }

        public Watcher Updated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Updated(filter);
        }

        public Watcher Removed(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Removed(filter);
        }

        public Watcher AddedOrUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrUpdated(filter);
        }

        public Watcher AddedOrRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrRemoved(filter);
        }

        #endregion

        #region Events

        private void OnEntityComponentAdded(Entity entity)
        {
            _data.Entities[entity.Id] = entity;
            _watcherTable.AddedEntity(entity);
        }

        private void OnEntityComponentUpdated(Entity entity)
        {
            if (HasEntity(entity))
                _watcherTable.UpdatedEntity(entity);
        }

        private void OnEntityComponentRemoved(Entity entity)
        {
            _data.Entities[entity.Id] = Entity.Null;
            _watcherTable.RemovedEntity(entity);
        }

        private void OnCompoinentArcheTypeDataAdded(ComponentArcheTypeData archeTypeData)
        {
            if (Filter.IsFiltered(archeTypeData.ArcheType))
            {
                _data.AddComponentArcheTypeData(archeTypeData);
                archeTypeData.EntityAdded += OnEntityComponentAdded;
                archeTypeData.EntityRemoved += OnEntityComponentRemoved;
                archeTypeData.EntityUpdated += OnEntityComponentUpdated;

                // TODO probably dont need this. new archeTypes should be empty
                if (archeTypeData.GetEntities().Length > 0)
                    throw new Exception();
            }
        }

        private void OnCompoinentArcheTypeDataRemoved(ComponentArcheTypeData archeTypeData)
        {
            if (_data.RemoveComponentArcheTypeData(archeTypeData))
            {
                archeTypeData.EntityAdded -= OnEntityComponentAdded;
                archeTypeData.EntityRemoved -= OnEntityComponentRemoved;
                archeTypeData.EntityUpdated -= OnEntityComponentUpdated;

                // TODO probably dont need this. removed archeTypes should be empty
                if (archeTypeData.GetEntities().Length > 0)
                    throw new Exception();
            }
        }

        #endregion
    }
}