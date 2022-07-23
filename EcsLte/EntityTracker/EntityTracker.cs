using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityTracker
    {
        private int[] _trackingStates;
        private Dictionary<ArcheTypeData, int> _archeTypeDatas;
        private ArcheTypeData[] _cachedArcheTypes;
        private bool _isCachedArcheTypeDatasDirty;
        private HashSet<ComponentConfig> _trackingConfigs;
        private Entity[] _entities;
        private List<Entity> _cachedEntitiesList;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }
        public string Name { get; private set; }
        public bool IsTrackingEntityStateChange { get; private set; }
        public bool IsTracking { get; private set; }
        public bool IsDestroyed { get; private set; }
        internal Entity[] Entities => _entities;
        internal Entity[] CachedEntities
        {
            get
            {
                if (_isCachedEntitiesDirty)
                {
                    _cachedEntitiesList.Clear();
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        if (entity.IsNotNull)
                            _cachedEntitiesList.Add(entity);
                    }

                    _cachedEntities = _cachedEntitiesList.ToArray();
                    _isCachedEntitiesDirty = false;
                }

                return _cachedEntities;
            }
        }
        internal ArcheTypeData[] CachedArcheTypeDatas
        {
            get
            {
                if (_isCachedArcheTypeDatasDirty)
                {
                    _cachedArcheTypes = _archeTypeDatas.Keys.ToArray();
                    _isCachedArcheTypeDatasDirty = false;
                }

                return _cachedArcheTypes;
            }
        }

        internal EntityTracker(EcsContext context, string name)
        {
            _trackingStates = new int[ComponentConfigs.Instance.AllComponentCount];
            _archeTypeDatas = new Dictionary<ArcheTypeData, int>();
            _cachedArcheTypes = new ArcheTypeData[0];
            _isCachedArcheTypeDatasDirty = true;
            _trackingConfigs = new HashSet<ComponentConfig>();
            _entities = new Entity[0];
            _cachedEntitiesList = new List<Entity>();
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _lockObj = new object();

            Context = context;
            Name = name;
        }

        internal EntityTracker(EntityTracker clone)
        {
            AssertEntityTracker(clone, clone?.Context);

            _trackingStates = new int[ComponentConfigs.Instance.AllComponentCount];
            Array.Copy(clone._trackingStates, _trackingStates, _trackingStates.Length);
            _archeTypeDatas = new Dictionary<ArcheTypeData, int>(clone._archeTypeDatas);
            _isCachedArcheTypeDatasDirty = true;
            _trackingConfigs = new HashSet<ComponentConfig>(clone._trackingConfigs);
            _entities = new Entity[clone._entities.Length];
            Array.Copy(clone._entities, _entities, _entities.Length);
            _cachedEntitiesList = new List<Entity>();
            _cachedEntities = new Entity[0];
            _isCachedEntitiesDirty = true;
            _lockObj = new object();

            Context = clone.Context;
            Name = clone.Name;
            IsTrackingEntityStateChange = clone.IsTrackingEntityStateChange;
            if (clone.IsTracking)
            {
                IsTracking = true;
                StartTracking();
            }
        }

        public bool IsTrackingComponent<TComponent>()
            where TComponent : IComponent
        {
            lock (_lockObj)
            {
                AssertTracker();

                return _trackingStates[ComponentConfig<TComponent>.Config.ComponentIndex] != 0;
            }
        }

        public EntityTrackerState GetComponentState<TComponent>()
            where TComponent : IComponent
        {
            lock (_lockObj)
            {
                AssertTracker();

                var state = _trackingStates[ComponentConfig<TComponent>.Config.ComponentIndex];
                if (state == 0)
                    throw new EntityTrackerNotTrackingComponentException(this, ComponentConfig<TComponent>.Config.ComponentType);

                return (EntityTrackerState)state;
            }
        }

        public EntityTracker SetComponentState<TComponent>(EntityTrackerState state)
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertTracker();

                _trackingStates[config.ComponentIndex] = (int)state;
                _trackingConfigs.Add(config);

                if (IsTracking)
                    Context.Tracking.RegisterComponentEvent(this, (int)state, config);
            }

            return this;
        }

        public EntityTracker ClearComponentState<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;

            lock (_lockObj)
            {
                AssertTracker();

                if (_trackingStates[config.ComponentIndex] != 0)
                {
                    _trackingStates[config.ComponentIndex] = 0;
                    _trackingConfigs.Add(config);

                    if (IsTracking)
                        Context.Tracking.RegisterComponentEvent(this, 0, config);
                }
            }

            return this;
        }

        public EntityTracker ClearAllComponentStates()
        {
            lock (_lockObj)
            {
                AssertTracker();

                if (IsTracking)
                {
                    foreach (var config in _trackingConfigs)
                        Context.Tracking.RegisterComponentEvent(this, 0, config);
                }

                Array.Clear(_trackingStates, 0, _trackingStates.Length);
                _trackingConfigs.Clear();
            }

            return this;
        }

        public EntityTracker SetEntityStateChange(bool track)
        {
            lock (_lockObj)
            {
                AssertTracker();

                if (IsTrackingEntityStateChange != track)
                {
                    IsTrackingEntityStateChange = track;

                    if (IsTracking)
                        Context.Tracking.RegisterEntityStateEvent(this, track);
                }
            }

            return this;
        }

        public EntityTracker StartTracking()
        {
            lock (_lockObj)
            {
                AssertTracker();

                if (!IsTracking)
                {
                    IsTracking = true;

                    Context.Tracking.StartTraking(this);
                    foreach (var config in _trackingConfigs)
                    {
                        Context.Tracking.RegisterComponentEvent(this,
                            _trackingStates[config.ComponentIndex], config);
                    }
                    Context.Tracking.RegisterEntityStateEvent(this, IsTrackingEntityStateChange);
                }
            }

            return this;
        }

        public EntityTracker StopTracking()
        {
            lock (_lockObj)
            {
                AssertTracker();

                if (IsTracking)
                {
                    IsTracking = false;

                    Context.Tracking.StopTracking(this);
                    foreach (var config in _trackingConfigs)
                        Context.Tracking.RegisterComponentEvent(this, 0, config);
                    Context.Tracking.RegisterEntityStateEvent(this, false);
                }
            }

            return this;
        }

        public EntityTracker ClearEntities()
        {
            lock (_lockObj)
            {
                AssertTracker();

                _archeTypeDatas.Clear();
                _isCachedArcheTypeDatasDirty = true;

                Array.Clear(_entities, 0, _entities.Length);
                _isCachedEntitiesDirty = true;
            }

            return this;
        }

        internal static void AssertEntityTracker(EntityTracker tracker, EcsContext context)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));
            if (tracker.Context != context)
                throw new EcsContextDifferentException(tracker.Context, context);
            if (tracker.IsDestroyed)
                throw new EntityTrackerIsDestroyedException(tracker);
        }

        internal void AssertTracker()
        {
            if (IsDestroyed)
                throw new EntityTrackerIsDestroyedException(this);
        }

        internal bool HasArcheTypeData(ArcheTypeData archeTypeData) => _archeTypeDatas.ContainsKey(archeTypeData);

        internal bool HasEntity(Entity entity) => entity.Id < _entities.Length &&
                _entities[entity.Id] == entity;

        internal void InternalDestroy()
        {
            lock (_lockObj)
            {
                Context.Tracking.StopTracking(this);
                foreach (var config in _trackingConfigs)
                    Context.Tracking.RegisterComponentEvent(this, 0, config);
                Context.Tracking.RegisterEntityStateEvent(this, false);

                _trackingStates = null;
                _archeTypeDatas = null;
                _cachedArcheTypes = null;
                _isCachedArcheTypeDatasDirty = true;
                _trackingConfigs = null;
                _entities = null;
                _cachedEntitiesList = null;
                _cachedEntities = null;
                _isCachedEntitiesDirty = true;

                IsTrackingEntityStateChange = false;
                IsDestroyed = true;
            }
        }

        internal void AddTracked(Entity entity, ArcheTypeData archeTypeData) => IncArcheTypeData(entity, archeTypeData);

        internal void UpdateTracked(Entity entity, ArcheTypeData archeTypeData) => IncArcheTypeData(entity, archeTypeData);

        internal void ArcheTypeChangeTracked(Entity entity, ArcheTypeData prevArcheTypeData, ArcheTypeData nextArcheTypeData)
        {
            if (_entities[entity.Id] == entity)
            {
                _archeTypeDatas[prevArcheTypeData]--;
                if (_archeTypeDatas[prevArcheTypeData] == 0)
                {
                    _archeTypeDatas.Remove(prevArcheTypeData);
                    _isCachedArcheTypeDatasDirty = true;
                }

                if (!_archeTypeDatas.ContainsKey(nextArcheTypeData))
                {
                    _archeTypeDatas.Add(nextArcheTypeData, 1);
                    _isCachedArcheTypeDatasDirty = true;
                }
                else
                    _archeTypeDatas[nextArcheTypeData]++;
            }
        }

        internal void EntityStateTracked(Entity entity, ArcheTypeData archeTypeData) => IncArcheTypeData(entity, archeTypeData);

        internal void RemoveTracked(Entity entity, ArcheTypeData archeTypeData) => DecArcheTypeData(entity, archeTypeData);

        internal void ResizeTracked(int entityCapacity)
        {
            if (_entities.Length < entityCapacity)
            {
                Array.Resize(ref _entities, entityCapacity);
                _isCachedEntitiesDirty = true;
            }
        }

        private void IncArcheTypeData(Entity entity, ArcheTypeData archeTypeData)
        {
            if (_entities[entity.Id] != entity)
            {
                _entities[entity.Id] = entity;
                _isCachedEntitiesDirty = true;

                if (!_archeTypeDatas.ContainsKey(archeTypeData))
                {
                    _archeTypeDatas.Add(archeTypeData, 1);
                    _isCachedArcheTypeDatasDirty = true;
                }
                else
                    _archeTypeDatas[archeTypeData]++;
            }
        }

        private void DecArcheTypeData(Entity entity, ArcheTypeData archeTypeData)
        {
            if (_entities[entity.Id] == entity)
            {
                _entities[entity.Id] = Entity.Null;
                _isCachedEntitiesDirty = true;

                if (_archeTypeDatas.ContainsKey(archeTypeData))
                {
                    _archeTypeDatas[archeTypeData]--;
                    if (_archeTypeDatas[archeTypeData] == 0)
                    {
                        _archeTypeDatas.Remove(archeTypeData);
                        _isCachedArcheTypeDatasDirty = true;
                    }
                }
            }
        }
    }
}
