using EcsLte.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.Native
{
    public class EntityBlueprint_Native : IEntityBlueprint
    {
        private readonly Dictionary<ComponentConfig, IEntityBlueprintComponentData_Native> _components;

        public EntityBlueprint_Native() => _components = new Dictionary<ComponentConfig, IEntityBlueprintComponentData_Native>();

        public bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent => _components.ContainsKey(ComponentConfig<TComponent>.Config);

        public TComponent GetComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            if (!_components.TryGetValue(ComponentConfig<TComponent>.Config, out var component))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)component;
        }

        public void AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (_components.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.Add(config, new EntityBlueprintComponentData_Native<TComponent>() { Component = component });
        }

        public void ReplaceComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.ContainsKey(config))
                _components.Add(config, new EntityBlueprintComponentData_Native<TComponent>() { Component = component });
            else
                _components[config] = new EntityBlueprintComponentData_Native<TComponent>() { Component = component };
        }

        public void RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.Remove(config);
        }

        internal KeyValuePair<ComponentConfig, IEntityBlueprintComponentData_Native>[] GetComponentConfigsAndDataOrdered() => _components.OrderBy(x => x.Key.ComponentIndex)
                .ToArray();
    }
}
