using EcsLte.Data;
using EcsLte.Data.Unmanaged;
using EcsLte.Exceptions;
using EcsLte.NativeArcheType;
using EcsLte.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte.NativeArcheTypeContinous
{
    public class EntityBlueprint_ArcheType_Native_Continuous : IEntityBlueprint
    {
        private readonly DataCache<Dictionary<ComponentConfig, IEntityBlueprintComponentData_ArcheType_Native>, EntityBlueprintData_ArcheType_Native> _components;
        private ArcheTypeFactory_ArcheType_Native_Continuous _archeTypeFactory;
        private ArcheTypeIndex_ArcheType_Native _archeTypeIndex;
        private bool _isArcheTypeIndexDirty;

        public EntityBlueprint_ArcheType_Native_Continuous() => _components = new DataCache<Dictionary<ComponentConfig, IEntityBlueprintComponentData_ArcheType_Native>, EntityBlueprintData_ArcheType_Native>(
                UpdateCachedBlueprintData,
                new Dictionary<ComponentConfig, IEntityBlueprintComponentData_ArcheType_Native>(),
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

            _components.UncachedData.Add(config, new EntityBlueprintComponentData_ArcheType_Native<TComponent> { Component = component });
            _components.SetDirty();
            _isArcheTypeIndexDirty = true;
        }

        public void ReplaceComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
            {
                _components.UncachedData.Add(config, new EntityBlueprintComponentData_ArcheType_Native<TComponent> { Component = component });
                _components.SetDirty();
            }
            else
            {
                _components.UncachedData[config] = new EntityBlueprintComponentData_ArcheType_Native<TComponent> { Component = component };
            }

            _isArcheTypeIndexDirty = true;
        }

        public void RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            if (!_components.UncachedData.ContainsKey(config))
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            _components.UncachedData.Remove(config);
            _components.SetDirty();
            _isArcheTypeIndexDirty = true;
        }

        internal unsafe ArcheTypeIndex_ArcheType_Native GetArcheTypeIndex(
            ArcheTypeFactory_ArcheType_Native_Continuous archeTypeFactory,
            ComponentConfigIndex_ArcheType_Native* uniqueConfigs,
            DataChunkCache_ArcheType_Native_Continuous* dataChunkCache,
            IIndexDictionary[] sharedComponentIndexes,
            out EntityBlueprintData_ArcheType_Native data)
        {
            data = _components.CachedData;

            if (_isArcheTypeIndexDirty || _archeTypeIndex.IsNull || _archeTypeFactory != archeTypeFactory)
            {
                var ordered = _components.UncachedData.OrderBy(x => x.Key.ComponentIndex).ToArray();
                var archeType = new Component_ArcheType_Native
                {
                    ComponentConfigLength = ordered.Select(x => x.Key).Count()
                };
                archeType.ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength);
                if (data.Configs.Length > 0)
                {
                    fixed (ComponentConfig* configPtr = &data.Configs[0])
                    {
                        MemoryHelper.Copy(
                            configPtr,
                            archeType.ComponentConfigs,
                            data.Configs.Length * TypeCache<ComponentConfig>.SizeInBytes);
                    }
                }
                if (data.UniqueConfigs.Length > 0)
                {
                    fixed (ComponentConfig* configPtr = &data.UniqueConfigs[0])
                    {
                        MemoryHelper.Copy(
                            configPtr,
                            archeType.ComponentConfigs + data.Configs.Length,
                            data.UniqueConfigs.Length * TypeCache<ComponentConfig>.SizeInBytes);
                    }
                }

                var shareIndexes = ordered.Where(x => x.Key.IsShared)
                    .Select(x => new ShareComponentDataIndex
                    {
                        SharedIndex = x.Key.SharedIndex,
                        SharedDataIndex = sharedComponentIndexes[x.Key.SharedIndex].GetIndexObj(x.Value.GetComponent()),
                    })
                    .ToArray();
                archeType.SharedComponentDataLength = shareIndexes.Length;

                if (shareIndexes.Length > 0)
                {
                    archeType.SharedComponentDataLength = shareIndexes.Length;
                    archeType.ShareComponentDataIndexes = MemoryHelper.Alloc<ShareComponentDataIndex>(shareIndexes.Length);
                    fixed (ShareComponentDataIndex* shareIndexesPtr = &shareIndexes[0])
                    {
                        MemoryHelper.Copy(
                            shareIndexesPtr,
                            archeType.ShareComponentDataIndexes,
                            shareIndexes.Length * TypeCache<ShareComponentDataIndex>.SizeInBytes);
                    }
                }

                if (!archeTypeFactory.GetArcheTypeData(archeType, uniqueConfigs, dataChunkCache, out _, out _archeTypeIndex))
                    archeType.Dispose();

                _archeTypeFactory = archeTypeFactory;
                _isArcheTypeIndexDirty = false;
            }

            return _archeTypeIndex;
        }

        private EntityBlueprintData_ArcheType_Native UpdateCachedBlueprintData(Dictionary<ComponentConfig, IEntityBlueprintComponentData_ArcheType_Native> uncachedData)
        {
            var ordered = uncachedData.OrderBy(x => x.Key.ComponentIndex).ToArray();
            var componentPairs = ordered.Where(x => !x.Key.IsUnique);
            var uniquePairs = ordered.Where(x => x.Key.IsUnique);

            return new EntityBlueprintData_ArcheType_Native
            {
                Configs = componentPairs.Select(x => x.Key).ToArray(),
                UniqueConfigs = uniquePairs.Select(x => x.Key).ToArray(),
                Components = componentPairs.Select(x => x.Value).ToArray(),
                UniqueComponents = uniquePairs.Select(x => x.Value).ToArray(),
                ComponentsLengthInBytes = componentPairs.Select(x => x.Key.UnmanagedInBytesSize).Sum(),
                UniqueComponentsLengthInBytes = uniquePairs.Select(x => x.Key.UnmanagedInBytesSize).Sum(),
            };
        }
    }
}
