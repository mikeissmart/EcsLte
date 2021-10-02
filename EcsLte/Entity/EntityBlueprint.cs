using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    internal class BlueprintComponent
    {
        internal ComponentPoolConfig Config { get; set; }
        internal IComponent Component { get; set; }

        internal BlueprintComponent(ComponentPoolConfig config, IComponent component)
        {
            Config = config;
            Component = component;
        }

        internal BlueprintComponent Clone()
        {
            return new BlueprintComponent(Config, Component);
        }
    }

    public class EntityBlueprint
    {
        private Dictionary<int, BlueprintComponent> _components;
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
                var _archeType = new ComponentArcheType();
                foreach (var bpComponent in _components.Values)
                {
                    if (bpComponent.Config.IsShared)
                        _archeType = ComponentArcheType.AppendSharedComponent(
                            _archeType,
                            (ISharedComponent)bpComponent.Component,
                            bpComponent.Config.Index);
                    else
                        _archeType = ComponentArcheType.AppendComponentPoolIndex(
                            _archeType, bpComponent.Config.Index);
                }
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
            return _components.ContainsKey(ComponentPoolIndex<TComponent>.Config.Index);
        }

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            if (!HasComponent<TComponent>())
                throw new BlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)_components[ComponentPoolIndex<TComponent>.Config.Index].Component;
        }

        public EntityBlueprint AddComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            if (HasComponent<TComponent>())
                throw new BlueprintAlreadyHasComponentException(typeof(TComponent));

            var config = ComponentPoolIndex<TComponent>.Config;
            _components.Add(config.Index,
                new BlueprintComponent(config, component));
            _createArcheType = true;

            return this;
        }

        public EntityBlueprint ReplaceComponent<TComponent>(TComponent newComponent) where TComponent : IComponent
        {
            if (HasComponent<TComponent>())
            {
                _components[ComponentPoolIndex<TComponent>.Config.Index].Component = newComponent;
                _createArcheType = true;
            }
            else
                AddComponent(newComponent);

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

            _components.Remove(ComponentPoolIndex<TComponent>.Config.Index);
            _createArcheType = true;

            return this;
        }
    }
}