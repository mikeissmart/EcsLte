using EcsLte.Data;
using System;

namespace EcsLte
{
    internal interface IComponentAdapter
    {
        ComponentConfig Config { get; }

        void SetComponentConfigOffset(ComponentConfigOffset configOffset);
        unsafe void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData);
        unsafe void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData);
        SharedComponentDataIndex GetSharedDataIndex();
    }

    internal interface IComponentAdapter<TComponent> : IComponentAdapter
             where TComponent : IComponent
    {
        ref TComponent GetComponentRef();
    }

    internal abstract class ComponentAdapter<TComponent> : IComponentAdapter<TComponent>
         where TComponent : IComponent
    {
        public ComponentConfig Config { get; private set; }

        protected ComponentConfigOffset ConfigOffset;
        protected TComponent Component;
        protected TComponent OrignalComponent;
        protected unsafe ArcheTypeData CurrentArcheTypeData;

        protected ComponentAdapter(ComponentConfig config) => Config = config;

        public ref TComponent GetComponentRef() => ref Component;
        public virtual SharedComponentDataIndex GetSharedDataIndex() => throw new NotImplementedException();
        public void SetComponentConfigOffset(ComponentConfigOffset configOffset) => ConfigOffset = configOffset;

        public abstract unsafe void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData);
        public abstract unsafe void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData);
    }

    internal class BlittableComponentAdapter<TComponent> : ComponentAdapter<TComponent>
         where TComponent : IComponent
    {
        public BlittableComponentAdapter(ComponentConfig config)
            : base(config)
        {
        }

        public override unsafe void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData)
        {
            CurrentArcheTypeData = archeTypeData;
            Component = archeTypeData.GetComponentOffset<TComponent>(entityData, ConfigOffset);
            OrignalComponent = Component;
        }

        public override unsafe void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData) => archeTypeData.SetComponentOffset(entityData, Component, ConfigOffset);
    }

    internal class BlittableSharedComponentAdapter<TComponent> : BlittableComponentAdapter<TComponent>
         where TComponent : IComponent
    {
        private readonly IndexDictionary<TComponent> _sharedComponentIndexDic;

        public BlittableSharedComponentAdapter(ComponentConfig config, SharedComponentIndexDictionaries sharedIndexDics)
            : base(config) =>
            _sharedComponentIndexDic = sharedIndexDics.GetSharedIndexDic<TComponent>();

        public override SharedComponentDataIndex GetSharedDataIndex() => new SharedComponentDataIndex
        {
            SharedIndex = Config.SharedIndex,
            SharedDataIndex = _sharedComponentIndexDic.GetOrAdd(Component)
        };
    }

    internal class ManageComponentAdapter<TComponent> : ComponentAdapter<TComponent>
         where TComponent : IComponent
    {
        private readonly ManagedComponentPool<TComponent> _managePool;

        public ManageComponentAdapter(ComponentConfig config, ManagedComponentPools managePools)
            : base(config) => _managePool = managePools.GetPool<TComponent>();

        public override unsafe void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData)
        {
            CurrentArcheTypeData = archeTypeData;
            Component = archeTypeData.GetComponentOffset(entityData, ConfigOffset, _managePool);
            OrignalComponent = Component;
        }

        public override unsafe void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData) => archeTypeData.SetComponentOffset(entityData, Component, ConfigOffset, _managePool);
    }

    internal class ManageSharedComponentAdapter<TComponent> : ManageComponentAdapter<TComponent>
         where TComponent : IComponent
    {
        private readonly IndexDictionary<TComponent> _sharedComponentIndexDic;

        public ManageSharedComponentAdapter(ComponentConfig config, ManagedComponentPools managePools,
            SharedComponentIndexDictionaries sharedIndexDics)
            : base(config, managePools) =>
            _sharedComponentIndexDic = sharedIndexDics.GetSharedIndexDic<TComponent>();

        public override SharedComponentDataIndex GetSharedDataIndex() => new SharedComponentDataIndex
        {
            SharedIndex = Config.SharedIndex,
            SharedDataIndex = _sharedComponentIndexDic.GetOrAdd(Component)
        };
    }

    internal static class EntityQueryComponentAdapters
    {
        internal static IComponentAdapter[] CreateAdapters(ComponentConfig[] configs,
            SharedComponentIndexDictionaries sharedComponentIndexes, ManagedComponentPools managePools)
        {
            var adapters = new IComponentAdapter[configs.Length];
            for (var i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                var componentType = ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex];
                IComponentAdapter adapter;
                if (config.IsBlittable)
                {
                    if (config.IsShared)
                    {
                        adapter = (IComponentAdapter)Activator.CreateInstance(
                            typeof(BlittableSharedComponentAdapter<>).MakeGenericType(componentType),
                            new object[] { config, sharedComponentIndexes });
                    }
                    else
                    {
                        adapter = (IComponentAdapter)Activator.CreateInstance(
                            typeof(BlittableComponentAdapter<>).MakeGenericType(componentType),
                            new object[] { config });
                    }
                }
                else
                {
                    if (config.IsShared)
                    {
                        adapter = (IComponentAdapter)Activator.CreateInstance(
                            typeof(ManageSharedComponentAdapter<>).MakeGenericType(componentType),
                            new object[] { config, managePools, sharedComponentIndexes });
                    }
                    else
                    {
                        adapter = (IComponentAdapter)Activator.CreateInstance(
                            typeof(ManageComponentAdapter<>).MakeGenericType(componentType),
                            new object[] { config, managePools });
                    }
                }
                adapters[i] = adapter;
            }

            return adapters;
        }
    }
}