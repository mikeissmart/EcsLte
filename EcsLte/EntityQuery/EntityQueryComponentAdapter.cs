using EcsLte.Data;
using System;

namespace EcsLte
{
    internal interface IComponentAdapter
    {
        ComponentConfig Config { get; }

        void SetComponentConfigOffset(ComponentConfigOffset configOffset);
        void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData);
        void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData);
        SharedComponentDataIndex GetSharedDataIndex();
    }

    internal interface IComponentAdapter<TComponent> : IComponentAdapter
             where TComponent : IComponent
    {
        ref TComponent GetComponentRef();
    }

    internal class ComponentAdapter<TComponent> : IComponentAdapter<TComponent>
         where TComponent : unmanaged, IComponent
    {
        public ComponentConfig Config { get; private set; }

        protected ComponentConfigOffset ConfigOffset;
        protected TComponent Component;
        protected ArcheTypeData CurrentArcheTypeData;

        public ComponentAdapter(ComponentConfig config) => Config = config;

        public ref TComponent GetComponentRef() => ref Component;
        public virtual SharedComponentDataIndex GetSharedDataIndex() => throw new NotImplementedException();
        public void SetComponentConfigOffset(ComponentConfigOffset configOffset) => ConfigOffset = configOffset;

        public void StoreComponent(EntityData entityData, ArcheTypeData archeTypeData)
        {
            CurrentArcheTypeData = archeTypeData;
            Component = archeTypeData.GetComponentOffset<TComponent>(entityData, ConfigOffset);
        }

        public void UpdateComponent(EntityData entityData, ArcheTypeData archeTypeData) =>
            archeTypeData.SetComponentOffset(entityData, Component, ConfigOffset);
    }

    internal class SharedComponentAdapter<TComponent> : ComponentAdapter<TComponent>
         where TComponent : unmanaged, IComponent
    {
        private readonly IndexDictionary<TComponent> _sharedComponentIndexDic;

        public SharedComponentAdapter(ComponentConfig config, SharedComponentIndexDictionaries sharedIndexDics)
            : base(config) =>
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
            SharedComponentIndexDictionaries sharedComponentIndexes)
        {
            var adapters = new IComponentAdapter[configs.Length];
            for (var i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                var componentType = ComponentConfigs.Instance.AllComponentTypes[config.ComponentIndex];
                IComponentAdapter adapter;
                if (config.IsShared)
                {
                    adapter = (IComponentAdapter)Activator.CreateInstance(
                        typeof(SharedComponentAdapter<>).MakeGenericType(componentType),
                        new object[] { config, sharedComponentIndexes });
                }
                else
                {
                    adapter = (IComponentAdapter)Activator.CreateInstance(
                        typeof(ComponentAdapter<>).MakeGenericType(componentType),
                        new object[] { config });
                }
                adapters[i] = adapter;
            }

            return adapters;
        }
    }
}