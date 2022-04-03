using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.ManagedArcheType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte.ManagedArcheType
{
    public class EntityBlueprint_ArcheType_Managed : IEntityBlueprint
    {
        private readonly DataCache<Dictionary<ComponentConfig, IComponent>, EntityBlueprintData_ArcheType_Managed> _components;
        private IComponentEntityFactory _lastComponentEntityFactory;
        private Component_ArcheType_Managed _archeType;
        private bool _isArcheTypeDirty;

        public EntityBlueprint_ArcheType_Managed()
        {
            _components = new DataCache<Dictionary<ComponentConfig, IComponent>, EntityBlueprintData_ArcheType_Managed>(
                UpdateCachedBlueprintData,
                new Dictionary<ComponentConfig, IComponent>(),
                null);
        }

        public bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            return _components.UncachedData.ContainsKey(ComponentConfig<TComponent>.Config);
        }

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
            _isArcheTypeDirty = true;
        }

        public void ReplaceComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
            {
                _components.UncachedData.Add(config, component);
                _components.SetDirty();
            }

            _isArcheTypeDirty = true;
            _components.UncachedData[config] = component;
        }

        public void RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.UncachedData.Remove(config);
            _components.SetDirty();
            _isArcheTypeDirty = true;
        }

        internal Component_ArcheType_Managed GetArcheType(IComponentEntityFactory componentEntityFactory, IIndexDictionary[] sharedComponentIndexes, out EntityBlueprintData_ArcheType_Managed data)
        {
            if (_isArcheTypeDirty || componentEntityFactory != _lastComponentEntityFactory)
            {
                var ordered = _components.UncachedData.OrderBy(x => x.Key.ComponentIndex).ToArray();
                _archeType = new Component_ArcheType_Managed
                {
                    ComponentConfigs = ordered.Select(x => x.Key).ToArray(),
                };

                var shareIndexes = ordered.Where(x => x.Key.IsShared)
                    .Select(x => new ShareComponentDataIndex
                    {
                        SharedIndex = x.Key.SharedIndex,
                        SharedDataIndex = sharedComponentIndexes[x.Key.SharedIndex].GetIndexObj(x.Value),
                    });

                if (shareIndexes.Count() > 0)
                    _archeType.ShareComponentDataIndexes = shareIndexes.ToArray();

                _isArcheTypeDirty = false;
                _lastComponentEntityFactory = componentEntityFactory;
            }

            data = _components.CachedData;
            return _archeType;
        }

        private EntityBlueprintData_ArcheType_Managed UpdateCachedBlueprintData(Dictionary<ComponentConfig, IComponent> uncachedData)
        {
            var ordered = uncachedData.OrderBy(x => x.Key.ComponentIndex).ToArray();
            var componentPairs = ordered.Where(x => !x.Key.IsUnique);
            var uniquePairs = ordered.Where(x => x.Key.IsUnique);

            return new EntityBlueprintData_ArcheType_Managed
            {
                Configs = componentPairs.Select(x => x.Key).ToArray(),
                UniqueConfigs = uniquePairs.Select(x => x.Key).ToArray(),
                Components = componentPairs.Select(x => x.Value).ToArray(),
                UniqueComponents = uniquePairs.Select(x => x.Value).ToArray(),
            };
        }
    }
}
