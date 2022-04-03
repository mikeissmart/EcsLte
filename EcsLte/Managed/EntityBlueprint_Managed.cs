using EcsLte.Data;
using EcsLte.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Managed
{
    public class EntityBlueprint_Managed : IEntityBlueprint
    {
        private readonly DataCache<Dictionary<ComponentConfig, IComponent>, KeyValuePair<ComponentConfig, IComponent>[]> _components;

        public EntityBlueprint_Managed() => _components = new DataCache<Dictionary<ComponentConfig, IComponent>, KeyValuePair<ComponentConfig, IComponent>[]>(
                UpdateCachedComponents,
                new Dictionary<ComponentConfig, IComponent>(),
                null);

        public bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent => _components.UncachedData.ContainsKey(ComponentConfig<TComponent>.Config);

        public TComponent GetComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            if (!_components.UncachedData.TryGetValue(ComponentConfig<TComponent>.Config, out var component))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)component;
        }

        public void AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (_components.UncachedData.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.UncachedData.Add(config, component);
            _components.SetDirty();
        }

        public void ReplaceComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
            {
                _components.UncachedData.Add(config, component);
                _components.SetDirty();
            }

            _components.UncachedData[config] = component;
        }

        public void RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.UncachedData.Remove(config);
            _components.SetDirty();
        }

        internal KeyValuePair<ComponentConfig, IComponent>[] GetComponentConfigsAndDataOrdered() => _components.CachedData;

        private KeyValuePair<ComponentConfig, IComponent>[] UpdateCachedComponents(Dictionary<ComponentConfig, IComponent> uncachedData)
            => uncachedData.OrderBy(x => x.Key.ComponentIndex)
                .ToArray();
    }
}
