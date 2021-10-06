using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    internal class BlueprintComponent
    {
        internal BlueprintComponent(ComponentPoolConfig config, IComponent component)
        {
            Config = config;
            Component = component;
        }

        internal ComponentPoolConfig Config { get; set; }
        internal IComponent Component { get; set; }

        internal BlueprintComponent Clone()
        {
            return new BlueprintComponent(Config, Component);
        }
    }

    public class EntityBlueprint
    {
        private readonly Dictionary<int, BlueprintComponent> _components;
        private ComponentArcheType _archeType;
        private bool _createArcheType;

        public EntityBlueprint()
        {
            _components = new Dictionary<int, BlueprintComponent>();
            _archeType = new ComponentArcheType();
            _createArcheType = true;
        }

        internal ComponentArcheType CreateArcheType()
        {
            if (_createArcheType)
            {
                _archeType = new ComponentArcheType();
                foreach (var bpComponent in _components.Values)
                    _archeType = ComponentArcheType.AppendComponent(
                        _archeType, bpComponent.Component,
                        bpComponent.Config);
                _createArcheType = false;
            }

            return _archeType;
        }

        internal int[] GetComponentPoolInexes()
        {
            return _components.Keys.ToArray();
        }

        internal BlueprintComponent[] GetBlueprintComponents()
        {
            return _components.Values.ToArray();
        }

        public EntityBlueprint Clone()
        {
            var clone = new EntityBlueprint();
            foreach (var pair in _components)
                clone._components.Add(pair.Key, pair.Value.Clone());

            return clone;
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            return _components.ContainsKey(ComponentPoolIndex<TComponent>.Config.PoolIndex);
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            if (!HasComponent<TComponent>())
                throw new BlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)_components[ComponentPoolIndex<TComponent>.Config.PoolIndex].Component;
        }

        public EntityBlueprint AddComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            if (HasComponent<TComponent>())
                throw new BlueprintAlreadyHasComponentException(typeof(TComponent));

            var config = ComponentPoolIndex<TComponent>.Config;
            _components.Add(config.PoolIndex,
                new BlueprintComponent(config, component));
            _createArcheType = true;

            return this;
        }

        public EntityBlueprint ReplaceComponent<TComponent>(TComponent newComponent) where TComponent : IComponent
        {
            if (HasComponent<TComponent>())
            {
                _components[ComponentPoolIndex<TComponent>.Config.PoolIndex].Component = newComponent;
                _createArcheType = true;
            }
            else
            {
                AddComponent(newComponent);
            }

            return this;
        }

        public EntityBlueprint RemoveAllComponents()
        {
            _components.Clear();
            _createArcheType = true;

            return this;
        }

        public EntityBlueprint RemoveComponent<TComponent>() where TComponent : IComponent
        {
            if (!HasComponent<TComponent>())
                throw new BlueprintNotHaveComponentException(typeof(TComponent));

            _components.Remove(ComponentPoolIndex<TComponent>.Config.PoolIndex);
            _createArcheType = true;

            return this;
        }
    }
}