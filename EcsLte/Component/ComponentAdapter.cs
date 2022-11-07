using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal interface IComponentAdapter
    {
        ComponentConfig Config { get; }

        TComponent2 GetComponent<TComponent2>(int entityIndex, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent;
        void SetComponent<TComponent2>(int entityIndex, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent;
        void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent;
        void RemoveConfig(ref ArcheType cachedArcheType);
    }

    internal class ComponentGeneralAdapter<TComponent> : IComponentAdapter
        where TComponent : unmanaged, IGeneralComponent
    {
        private ComponentConfig _config;

        public ComponentConfig Config => _config;

        public ComponentGeneralAdapter() => _config = ComponentConfig<TComponent>.Config;

        public unsafe TComponent2 GetComponent<TComponent2>(int entityIndex, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
        {
            InteropTools.PtrToStructure<TComponent2>(
                (IntPtr)archeTypeData.GetComponentPtr(entityIndex, _config),
                out var component);

            return component;
        }

        public unsafe TComponent2 GetComponent<TComponent2>(ArcheTypeData archeTypeData, int entityIndex, ComponentConfigOffset configOffset)
            where TComponent2 : IComponent
        {
            InteropTools.PtrToStructure<TComponent2>(
                (IntPtr)archeTypeData.GetComponentPtr(entityIndex, configOffset),
                out var component);

            return component;
        }

        public unsafe void SetComponent<TComponent2>(int entityIndex, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => InteropTools.StructureToPtr(ref component, (IntPtr)archeTypeData.GetComponentPtr(entityIndex, _config));

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType, _config);

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfig(ref cachedArcheType, Config);
    }

    internal class ComponentManagedAdapter<TComponent> : IComponentAdapter
        where TComponent : IManagedComponent
    {
        private ComponentConfig _config;

        public ComponentConfig Config => _config;

        public ComponentManagedAdapter() => _config = ComponentConfig<TComponent>.Config;

        public TComponent2 GetComponent<TComponent2>(int entityIndex, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => (TComponent2)archeTypeData.GetManagedComponentPool(_config)
                .GetComponent(entityIndex);

        public void SetComponent<TComponent2>(int entityIndex, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => archeTypeData.GetManagedComponentPool(_config)
                .SetComponent(entityIndex, component);

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType, _config);

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfig(ref cachedArcheType, Config);
    }

    internal class ComponentSharedAdapter<TComponent> : IComponentAdapter
        where TComponent : ISharedComponent
    {
        private ComponentConfig _config;

        public ComponentConfig Config => _config;

        public ComponentSharedAdapter() => _config = ComponentConfig<TComponent>.Config;

        public unsafe TComponent2 GetComponent<TComponent2>(int entityIndex, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => (TComponent2)archeTypeData.GetSharedComponentData(_config);

        public void SetComponent<TComponent2>(int entityIndex, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
        {
        }

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType,
                _config, sharedDics.GetDic(_config).GetSharedDataIndex(component));

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfigAndSharedDataIndex(ref cachedArcheType, Config);
    }
}
