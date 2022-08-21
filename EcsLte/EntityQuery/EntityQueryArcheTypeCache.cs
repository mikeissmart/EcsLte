using System.Collections.Generic;

namespace EcsLte
{
    internal class EntityQueryArcheTypeCacheRoot
    {
        private readonly Dictionary<ArcheTypeIndex, EntityQuerySharedCache> _archeTypeDic;

        public List<EntityQuerySharedCache> SharedCaches { get; private set; }
        public Queue<List<Entity>> EntitiesCache { get; private set; }

        public EntityQueryArcheTypeCacheRoot()
        {
            _archeTypeDic = new Dictionary<ArcheTypeIndex, EntityQuerySharedCache>();

            SharedCaches = new List<EntityQuerySharedCache>();
            EntitiesCache = new Queue<List<Entity>>();
        }

        public EntityQuerySharedCache GetCache(ArcheTypeIndex archeTypeIndex)
        {
            if (!_archeTypeDic.TryGetValue(archeTypeIndex, out var cache))
            {
                cache = new EntityQuerySharedCache(archeTypeIndex)
                {
                    Components = new List<(IComponent, ComponentConfig)>()
                };
                _archeTypeDic.Add(archeTypeIndex, cache);
            }

            return cache;
        }

        public void Clear()
        {
            _archeTypeDic.Clear();
            SharedCaches.Clear();
        }
    }

    internal class EntityQuerySharedCache
    {
        private System.Collections.IDictionary _cacheDic;

        public List<Entity> Entities { get; private set; }
        public List<(IComponent, ComponentConfig)> Components { get; set; }
        public ArcheTypeIndex ArcheTypeIndex { get; private set; }

        public EntityQuerySharedCache(ArcheTypeIndex archeTypeIndex) => ArcheTypeIndex = archeTypeIndex;

        public EntityQuerySharedCache GetCache<TComponent>(IEntityQueryAdapter adapter)
            where TComponent : IComponent
        {
            if (!ComponentConfig<TComponent>.Config.IsShared)
                return this;

            var component = ((IEntityQueryAdapter<TComponent>)adapter).GetUpdatedComponent();
            Dictionary<TComponent, EntityQuerySharedCache> dic;
            if (_cacheDic == null)
            {
                dic = new Dictionary<TComponent, EntityQuerySharedCache>();
                _cacheDic = dic;
                Components.Add((component, ComponentConfig<TComponent>.Config));
            }
            else
                dic = (Dictionary<TComponent, EntityQuerySharedCache>)_cacheDic;

            if (!dic.TryGetValue(component, out var cache))
            {
                cache = new EntityQuerySharedCache(ArcheTypeIndex)
                {
                    Components = new List<(IComponent, ComponentConfig)>()
                };
                cache.Components.AddRange(Components);
                dic.Add(component, cache);
            }

            return cache;
        }

        public EntityQuerySharedCache GetCache<TComponent>(IEntityQueryAdapter adapter, EntityQueryArcheTypeCacheRoot cacheRoot)
            where TComponent : IComponent
        {
            if (!ComponentConfig<TComponent>.Config.IsShared)
                return this;

            var component = ((IEntityQueryAdapter<TComponent>)adapter).GetUpdatedComponent();
            Dictionary<TComponent, EntityQuerySharedCache> dic;
            if (_cacheDic == null)
            {
                dic = new Dictionary<TComponent, EntityQuerySharedCache>();
                _cacheDic = dic;
                Components.Add((component, ComponentConfig<TComponent>.Config));
            }
            else
                dic = (Dictionary<TComponent, EntityQuerySharedCache>)_cacheDic;

            if (!dic.TryGetValue(component, out var cache))
            {
                cache = new EntityQuerySharedCache(ArcheTypeIndex)
                {
                    Components = new List<(IComponent, ComponentConfig)>()
                };
                cache.Components.AddRange(Components);
                cache.Entities = cacheRoot.EntitiesCache.Count > 0
                    ? cacheRoot.EntitiesCache.Dequeue()
                    : new List<Entity>();
                dic.Add(component, cache);

                cacheRoot.SharedCaches.Add(cache);
            }

            return cache;
        }
    }
}
