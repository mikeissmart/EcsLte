using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityGroup : IEcsContext, IGetEntity, IGetWatcher
    {
        private EntityGroupData _data;
        private WatcherTable _watcherTable;
        private EcsContextData _ecsContextData;

        internal EntityGroup(EcsContext context, EcsContextData ecsContextData,
            ComponentArcheTypeData[] archeTypeDatas,
            ISharedComponent[] sharedComponents)
        {
            _ecsContextData = ecsContextData;
            _data = ObjectCache.Pop<EntityGroupData>();
            _data.Initialize(ecsContextData, archeTypeDatas, sharedComponents);

            ecsContextData.ArcheTypeDataAdded += OnComponentArcheTypeDataAdded;
            ecsContextData.ArcheTypeDataRemoved += OnCompoinentArcheTypeDataRemoved;

            for (int i = 0; i < archeTypeDatas.Length; i++)
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
        }

        #region EcsContext

        public EcsContext CurrentContext { get; private set; }

        #endregion

        #region EntityKey

        internal ISharedComponent[] SharedKeys { get => _data.SharedComponents; }

        public Entity GetFirstOrDefault()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            return _data.Entities.GetEntities()
                .Where(x => x != Entity.Null)
                .FirstOrDefault();
        }

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

        private void OnComponentArcheTypeDataAdded(ComponentArcheTypeData archeTypeData)
        {
            if (_data.SharedComponents != null && archeTypeData.ArcheType.SharedComponents != null &&
                archeTypeData.ArcheType.SharedComponents.SequenceEqual(_data.SharedComponents))
            {
                AddComponentArcheTypeData(archeTypeData);
            }
        }

        private void AddComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
        {
            _data.AddComponentArcheTypeData(archeTypeData);
            archeTypeData.EntityAdded += OnEntityComponentAdded;
            archeTypeData.EntityRemoved += OnEntityComponentRemoved;
            archeTypeData.EntityUpdated += OnEntityComponentUpdated;

            // TODO probably dont need this. new archeTypes should be empty
            if (archeTypeData.GetEntities().Length > 0)
                throw new Exception();
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