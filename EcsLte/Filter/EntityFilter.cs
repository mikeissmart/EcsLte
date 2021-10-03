using System;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityFilter : IEcsContext, IGetEntity, IGetWatcher, IGroupWith
    {
        private readonly EntityFilterData _data;
        private EcsContextData _ecsContextData;
        private readonly WatcherTable _watcherTable;

        internal EntityFilter(EcsContext context, EcsContextData ecsContextData, Filter filter,
            ComponentArcheTypeData[] archeTypeDatas)
        {
            _data = ObjectCache.Pop<EntityFilterData>();
            _data.Initialize(ecsContextData, archeTypeDatas);

            ecsContextData.AnyArcheTypeDataAdded += OnAnyComponentArcheTypeDataAdded;

            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var archeData = archeTypeDatas[i];
                archeData.EntityAdded += OnEntityComponentAdded;
                archeData.EntityRemoved += OnEntityComponentRemoved;
                archeData.EntityUpdated += OnEntityComponentUpdated;
                archeData.ArcheTypeDataRemoved += OnComponentArcheTypeDataRemoved;

                ParallelRunner.RunParallelForEach(archeData.GetEntities(),
                    entity => _data.Entities[entity.Id] = entity);
            }

            _watcherTable = ObjectCache.Pop<WatcherTable>();
            _watcherTable.Initialize(context, ecsContextData);

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

        public Watcher WatchAdded(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Added(filter);
        }

        public Watcher WatchUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Updated(filter);
        }

        public Watcher WatchRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.Removed(filter);
        }

        public Watcher WatchAddedOrUpdated(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrUpdated(filter);
        }

        public Watcher WatchAddedOrRemoved(Filter filter)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _watcherTable.AddedOrRemoved(filter);
        }

        #endregion

        #region GroupWith

        public EntityGroup GroupWith(ISharedComponent sharedComponent)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);
            if (sharedComponent == null)
                throw new ArgumentNullException();

            return GroupWith(new[] { sharedComponent });
        }

        public EntityGroup GroupWith(params ISharedComponent[] sharedComponents)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);
            foreach (var sharedComponent in sharedComponents)
                if (sharedComponent == null)
                    throw new ArgumentNullException();

            var keyCollection = new GroupWithCollection(sharedComponents);
            lock (_ecsContextData.EntityGroups)
            {
                if (!_ecsContextData.EntityGroups.TryGetValue(keyCollection, out var entityKey))
                {
                    var archeTypeDatas = _ecsContextData.FilterComponentArcheTypeData(Filter, sharedComponents);
                    entityKey = new EntityGroup(CurrentContext, _ecsContextData,
                        Filter,
                        archeTypeDatas,
                        sharedComponents);
                    _ecsContextData.EntityGroups.Add(keyCollection, entityKey);
                }

                return entityKey;
            }
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

        private void OnAnyComponentArcheTypeDataAdded(ComponentArcheTypeData archeTypeData)
        {
            if (Filter.IsFiltered(archeTypeData.ArcheType))
            {
                _data.AddComponentArcheTypeData(archeTypeData);
                archeTypeData.EntityAdded += OnEntityComponentAdded;
                archeTypeData.EntityRemoved += OnEntityComponentRemoved;
                archeTypeData.EntityUpdated += OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved += OnComponentArcheTypeDataRemoved;
            }
        }

        private void OnComponentArcheTypeDataRemoved(ComponentArcheTypeData archeTypeData)
        {
            if (_data.RemoveComponentArcheTypeData(archeTypeData))
            {
                archeTypeData.EntityAdded -= OnEntityComponentAdded;
                archeTypeData.EntityRemoved -= OnEntityComponentRemoved;
                archeTypeData.EntityUpdated -= OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved -= OnComponentArcheTypeDataRemoved;
            }
        }

        #endregion
    }
}