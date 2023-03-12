using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public enum EntityTrackerMode
    {
        AnyChanges,
        AllChanges
    }

    public class EntityTracker
    {
        private Dictionary<ArcheTypeIndex, CacheData> _archeTypeDatas;
        private EntityFilter _filter;
        private EntityTrackerMode? _filterMode;
        private ChangeVersion _componentVersion;
        private ChangeVersion _cacheVersion;

        public EcsContext Context { get; private set; }
        public ChangeVersion TrackChangeVersion { get; private set; }
        public EntityTrackerMode Mode { get; private set; }
        internal HashSet<ComponentConfig> TrackingGeneralConfigs { get; private set; }
        internal HashSet<ComponentConfig> TrackingManagedConfigs { get; private set; }
        internal HashSet<ComponentConfig> TrackingSharedConfigs { get; private set; }

        internal EntityTracker(EcsContext context)
        {
            _archeTypeDatas = new Dictionary<ArcheTypeIndex, CacheData>();
            _filter = context.Filters.CreateFilter();

            Context = context;
            TrackingGeneralConfigs = new HashSet<ComponentConfig>();
            TrackingManagedConfigs = new HashSet<ComponentConfig>();
            TrackingSharedConfigs = new HashSet<ComponentConfig>();
        }

        public bool IsTrackingComponent<TComponent>()
            where TComponent : IComponent
            => IsTrackingComponent(ComponentConfig<TComponent>.Config);

        public bool IsTrackingComponent(ComponentConfig config)
        {
            Context.AssertContext();

            if (config.IsGeneral)
                return TrackingGeneralConfigs.Contains(config);
            if (config.IsManaged)
                return TrackingManagedConfigs.Contains(config);
            //if (config.IsShared)
            return TrackingSharedConfigs.Contains(config);
        }

        public EntityTracker SetTrackingComponent<TComponent>(bool tracking)
            where TComponent : IComponent
            => SetTrackingComponent(ComponentConfig<TComponent>.Config, tracking);

        public EntityTracker SetTrackingComponent(ComponentConfig config, bool tracking)
        {
            Context.AssertContext();

            if (config.IsGeneral)
            {
                if ((!tracking && TrackingGeneralConfigs.Remove(config)) ||
                    (tracking && TrackingGeneralConfigs.Add(config)))
                {
                    _componentVersion = Context.Entities.GlobalVersion;;
                    _cacheVersion = Context.Entities.GlobalVersion;
                }
            }
            else if (config.IsManaged)
            {
                if ((!tracking && TrackingManagedConfigs.Remove(config)) ||
                    (tracking && TrackingManagedConfigs.Add(config)))
                {
                    _componentVersion = Context.Entities.GlobalVersion;;
                    _cacheVersion = Context.Entities.GlobalVersion;
                }
            }
            else
            {
                if ((!tracking && TrackingSharedConfigs.Remove(config)) ||
                    (tracking && TrackingSharedConfigs.Add(config)))
                {
                    _componentVersion = Context.Entities.GlobalVersion;;
                    _cacheVersion = Context.Entities.GlobalVersion;
                }
            }

            return this;
        }

        public EntityTracker SetAllTrackingComponents(bool tracking)
        {
            Context.AssertContext();

            var clearCache = false;
            for (var i = 0; i < ComponentConfigs.Instance.AllGeneralCount; i++)
            {
                var config = ComponentConfigs.Instance.AllGeneralConfigs[i];
                if ((!tracking && TrackingGeneralConfigs.Remove(config)) ||
                    (tracking && TrackingGeneralConfigs.Add(config)))
                    clearCache = true;
            }
            for (var i = 0; i < ComponentConfigs.Instance.AllManagedCount; i++)
            {
                var config = ComponentConfigs.Instance.AllManagedConfigs[i];
                if ((!tracking && TrackingManagedConfigs.Remove(config)) ||
                    (tracking && TrackingManagedConfigs.Add(config)))
                    clearCache = true;
            }
            for (var i = 0; i < ComponentConfigs.Instance.AllSharedCount; i++)
            {
                var config = ComponentConfigs.Instance.AllSharedConfigs[i];
                if ((!tracking && TrackingSharedConfigs.Remove(config)) ||
                    (tracking && TrackingSharedConfigs.Add(config)))
                    clearCache = true;
            }

            if (clearCache)
            {
                _componentVersion = Context.Entities.GlobalVersion;;
                _cacheVersion = Context.Entities.GlobalVersion;
            }

            return this;
        }

        public EntityTracker SetTrackingMode(EntityTrackerMode mode)
        {
            Context.AssertContext();

            if (Mode != mode)
            {
                Mode = mode;
                _cacheVersion = Context.Entities.GlobalVersion;
            }

            return this;
        }

        public EntityTracker SetChangeVersion(ChangeVersion changeVersion)
        {
            Context.AssertContext();

            if (TrackChangeVersion != changeVersion)
            {
                TrackChangeVersion = changeVersion;
                _cacheVersion = Context.Entities.GlobalVersion;
            }

            return this;
        }

        internal EntityFilter TrackingFilter()
        {
            // TODO going to be too expensive, need to find way to reduce archeTypeData filtering?
            if (_filterMode != Mode)
            {
                if (Mode == EntityTrackerMode.AllChanges)
                    _filter.WhereAllOf(this);
                else if (Mode == EntityTrackerMode.AnyChanges)
                    _filter.WhereAnyOf(this);
                _filterMode = Mode;
            }

            return _filter;
        }

        internal int GetArcheTypeDataEntities(ArcheTypeData archeTypeData, ref Entity[] entities, int startingIndex)
        {
            if (archeTypeData.EntityCount() == 0)
                return 0;

            var cacheData = GetCacheData(archeTypeData);
            var entityIndex = startingIndex;
            for (var i = 0; i < cacheData.Chunks.Count; i++)
            {
                var chunk = cacheData.Chunks[i];
                Helper.ResizeRefArray(ref entities, entityIndex, chunk.EntityCount);
                entityIndex += chunk.GetEntities(ref entities, entityIndex);
            }

            return entityIndex - startingIndex;
        }

        internal bool GetDataChunks(ArcheTypeData archeTypeData, out List<DataChunk> chunks)
        {
            if (archeTypeData.EntityCount() == 0)
            {
                chunks = null;
                return false;
            }

            chunks = GetCacheData(archeTypeData).Chunks;
            return chunks.Count > 0;
        }

        internal void InternalDestroy()
        {
            _archeTypeDatas.Clear();
            _componentVersion = Context.Entities.GlobalVersion;;
            TrackingGeneralConfigs.Clear();
            TrackingManagedConfigs.Clear();
            TrackingSharedConfigs.Clear();
        }

        private CacheData GetCacheData(ArcheTypeData archeTypeData)
        {
            if (!_archeTypeDatas.TryGetValue(archeTypeData.ArcheTypeIndex, out var cacheData))
            {
                cacheData = new CacheData();
                _archeTypeDatas.Add(archeTypeData.ArcheTypeIndex, cacheData);
            }

            if (ChangeVersion.DidChange(_componentVersion, cacheData.ComponentVersion))
            {
                Helper.ResizeRefArray(ref cacheData.GeneralOffsets, 0, TrackingGeneralConfigs.Count);
                cacheData.GeneralOffsetsCount = 0;
                foreach (var config in TrackingGeneralConfigs)
                {
                    if (archeTypeData.HasConfigOffset(config, out var offset))
                        cacheData.GeneralOffsets[cacheData.GeneralOffsetsCount++] = offset;
                }

                Helper.ResizeRefArray(ref cacheData.ManagedOffsets, 0, TrackingManagedConfigs.Count);
                cacheData.ManagedOffsetsCount = 0;
                foreach (var config in TrackingManagedConfigs)
                {
                    if (archeTypeData.HasConfigOffset(config, out var offset))
                        cacheData.ManagedOffsets[cacheData.ManagedOffsetsCount++] = offset;
                }

                Helper.ResizeRefArray(ref cacheData.SharedOffsets, 0, TrackingSharedConfigs.Count);
                cacheData.SharedOffsetsCount = 0;
                foreach (var config in TrackingSharedConfigs)
                {
                    if (archeTypeData.HasConfigOffset(config, out var offset))
                        cacheData.SharedOffsets[cacheData.SharedOffsetsCount++] = offset;
                }
                cacheData.ComponentVersion = _componentVersion;
            }

            if (ChangeVersion.DidChange(_cacheVersion, cacheData.CacheVersion))
            {
                cacheData.Chunks.Clear();
                cacheData.CacheVersion = _cacheVersion;

                if (Mode == EntityTrackerMode.AnyChanges)
                    GetArcheTypeDataChunksAny(archeTypeData, cacheData);
                else
                    GetArcheTypeDataChunksAll(archeTypeData, cacheData);
            }

            return cacheData;
        }

        private void GetArcheTypeDataChunksAny(ArcheTypeData archeTypeData, CacheData cacheData)
        {
            var addAllChunks = false;
            for (var j = 0; j < cacheData.SharedOffsetsCount; j++)
            {
                if (ChangeVersion.DidChange(archeTypeData.SharedVersions[cacheData.SharedOffsets[j].ConfigIndex], TrackChangeVersion))
                {
                    addAllChunks = true;
                    break;
                }
            }
            if (addAllChunks)
            {
                for (var i = 0; i < archeTypeData.ChunksCount; i++)
                    cacheData.Chunks.Add(archeTypeData.Chunks[i]);
                return;
            }

            for (var i = 0; i < archeTypeData.ChunksCount; i++)
            {
                var addChunk = false;
                var chunk = archeTypeData.Chunks[i];
                for (var j = 0; j < cacheData.GeneralOffsetsCount; j++)
                {
                    if (ChangeVersion.DidChange(chunk.GeneralVersions[cacheData.GeneralOffsets[j].ConfigIndex], TrackChangeVersion))
                    {
                        addChunk = true;
                        break;
                    }
                }
                if (!addChunk)
                {
                    for (var j = 0; j < cacheData.ManagedOffsetsCount; j++)
                    {
                        if (ChangeVersion.DidChange(chunk.ManagedVersions[cacheData.ManagedOffsets[j].ConfigIndex], TrackChangeVersion))
                        {
                            addChunk = true;
                            break;
                        }
                    }
                }
                if (addChunk)
                    cacheData.Chunks.Add(chunk);
            }
        }

        private void GetArcheTypeDataChunksAll(ArcheTypeData archeTypeData, CacheData cacheData)
        {
            var isSharedOk = true;
            for (var j = 0; j < cacheData.SharedOffsetsCount; j++)
            {
                if (!ChangeVersion.DidChange(archeTypeData.SharedVersions[cacheData.SharedOffsets[j].ConfigIndex], TrackChangeVersion))
                {
                    isSharedOk = false;
                    break;
                }
            }
            if (!isSharedOk)
                return;

            for (var i = 0; i < archeTypeData.ChunksCount; i++)
            {
                var addChunk = true;
                var chunk = archeTypeData.Chunks[i];
                for (var j = 0; j < cacheData.GeneralOffsetsCount; j++)
                {
                    if (!ChangeVersion.DidChange(chunk.GeneralVersions[cacheData.GeneralOffsets[j].ConfigIndex], TrackChangeVersion))
                    {
                        addChunk = false;
                        break;
                    }
                }
                if (addChunk)
                {
                    for (var j = 0; j < cacheData.ManagedOffsetsCount; j++)
                    {
                        if (!ChangeVersion.DidChange(chunk.ManagedVersions[cacheData.ManagedOffsets[j].ConfigIndex], TrackChangeVersion))
                        {
                            addChunk = false;
                            break;
                        }
                    }
                }
                if (addChunk)
                    cacheData.Chunks.Add(chunk);
            }
        }

        #region Assert

        internal static void AssertEntityTracker(EntityTracker tracker, EcsContext context)
        {
            if (tracker == null)
                throw new ArgumentNullException(nameof(tracker));
            if (tracker.Context != context)
                throw new EcsContextNotSameException(tracker.Context, context);
        }

        #endregion

        private class CacheData
        {
            public List<DataChunk> Chunks = new List<DataChunk>();
            public ComponentConfigOffset[] GeneralOffsets = new ComponentConfigOffset[0];
            public ComponentConfigOffset[] ManagedOffsets = new ComponentConfigOffset[0];
            public ComponentConfigOffset[] SharedOffsets = new ComponentConfigOffset[0];
            public int GeneralOffsetsCount;
            public int ManagedOffsetsCount;
            public int SharedOffsetsCount;
            public ChangeVersion ComponentVersion;
            public ChangeVersion CacheVersion;
        }
    }
}
