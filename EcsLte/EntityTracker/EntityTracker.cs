using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityTracker : IDisposable
    {
        private readonly int[] _trackingStates;
        private readonly HashSet<ComponentConfig> _trackingConfigs;
        private Type[] _addedComponentTypes;
        private Type[] _updatedComponentTypes;
        private Entity[] _entities;
        private Entity[] _cachedEntities;
        private bool _isCachedEntitiesDirty;

        public EcsContext Context { get; private set; }
        public bool IsTracking { get; private set; }
        public Type[] AddedTrackComponentTypes
        {
            get
            {
                if (_addedComponentTypes == null)
                {
                    _addedComponentTypes = _trackingStates
                        .Select((x, i) => (x, i))
                        .Where(x => (x.x & (int)TrackingState.Added) == (int)TrackingState.Added)
                        .Select(x => ComponentConfigs.Instance.AllComponentTypes[x.i])
                        .ToArray();
                }
                return _addedComponentTypes;
            }
        }
        public Type[] UpdatedTrackComponentTypes
        {
            get
            {
                if (_updatedComponentTypes == null)
                {
                    _updatedComponentTypes = _trackingStates
                        .Select((x, i) => (x, i))
                        .Where(x => (x.x & (int)TrackingState.Updated) == (int)TrackingState.Updated)
                        .Select(x => ComponentConfigs.Instance.AllComponentTypes[x.i])
                        .ToArray();
                }
                return _updatedComponentTypes;
            }
        }
        internal int TrackerIndex { get; set; }
        internal Entity[] Entities => _entities;
        internal Entity[] CahcedEntities
        {
            get
            {
                if (!IsTracking)
                    return new Entity[0];

                if (_isCachedEntitiesDirty)
                {
                    _cachedEntities = _entities
                        .Where(x => x != Entity.Null)
                        .ToArray();
                    _isCachedEntitiesDirty = false;
                }

                return _cachedEntities;
            }
        }

        public EntityTracker(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);

            _trackingStates = new int[ComponentConfigs.Instance.AllComponentCount];
            _trackingConfigs = new HashSet<ComponentConfig>();
            _entities = new Entity[0];
            _isCachedEntitiesDirty = true;

            Context = context;

            context.TrackerManager.RegisterTracker(this);
        }

        public bool GetComponentTrackingState<TComponent>(TrackingState tracking)
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            return (_trackingStates[config.ComponentIndex] & (int)tracking) == (int)tracking;
        }

        public void SetComponentTrackingState<TComponent>(TrackingState tracking)
            where TComponent : IComponent
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var config = ComponentConfig<TComponent>.Config;
            if (_trackingStates[config.ComponentIndex] != (int)tracking)
            {
                _trackingStates[config.ComponentIndex] = (int)tracking;
                if (tracking == 0)
                    _trackingConfigs.Remove(config);
                else
                    _trackingConfigs.Add(config);
                _addedComponentTypes = null;
                _updatedComponentTypes = null;
                if (IsTracking)
                    Context.TrackerManager.RegisterComponentEvent(this, (int)tracking, config);
            }
        }

        public void StopComponentTracking<TComponent>()
            where TComponent : IComponent
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var config = ComponentConfig<TComponent>.Config;
            if (_trackingStates[config.ComponentIndex] != 0)
            {
                _trackingStates[config.ComponentIndex] = 0;
                _trackingConfigs.Remove(config);
                _addedComponentTypes = null;
                _updatedComponentTypes = null;
                if (IsTracking)
                    Context.TrackerManager.RegisterComponentEvent(this, 0, config);
            }
        }

        public void StartTracking()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (IsTracking)
                return;

            IsTracking = true;
            Context.TrackerManager.StartTraking(this);
            ResizeTracker(Context.EntityCapacity());
            foreach (var config in _trackingConfigs)
            {
                Context.TrackerManager.RegisterComponentEvent(this,
                    _trackingStates[config.ComponentIndex],
                    config);
            }
        }

        public void StopTracking()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (!IsTracking)
                return;

            IsTracking = false;
            Context.TrackerManager.StartTraking(this);
            foreach (var config in _trackingConfigs)
                Context.TrackerManager.RegisterComponentEvent(this, 0, config);
        }

        public void ResetTracking()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (!IsTracking)
                return;

            _isCachedEntitiesDirty = true;
            Array.Clear(Entities, 0, Entities.Length);
        }

        internal void ResizeTracker(int entityCapacity)
        {
            if (_entities.Length != entityCapacity)
            {
                Array.Resize(ref _entities, entityCapacity);
                _isCachedEntitiesDirty = true;
            }
        }

        internal void AddTracked(Entity entity)
        {
            _entities[entity.Id] = entity;
            _isCachedEntitiesDirty = true;
        }

        internal void UpdateTracked(Entity entity)
        {
            _entities[entity.Id] = entity;
            _isCachedEntitiesDirty = true;
        }

        internal void RemoveTracked(Entity entity)
        {
            _entities[entity.Id] = Entity.Null;
            _isCachedEntitiesDirty = true;
        }

        public enum TrackingState
        {
            Added = 1,
            Updated = 2,
            AddedOrUpdated = 3
        }

        public void Dispose()
        {
            if (!Context.IsDestroyed)
                Context.TrackerManager.UnregisterTracker(this);
        }
    }
}
