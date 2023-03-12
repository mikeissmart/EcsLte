using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Reflection;

namespace EcsLte
{
    internal interface IComponentAdapter
    {
        ComponentConfig Config { get; }

        TComponent2 GetComponent<TComponent2>(EntityData entityData, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent;
        void SetComponent<TComponent2>(ChangeVersion changeVersion, EntityData entityData, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent;
        void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent;
        void RemoveConfig(ref ArcheType cachedArcheType);
        IDataCatalog CreateCatalog();
    }

    internal class ComponentGeneralAdapter<TComponent> : IComponentAdapter
        where TComponent : unmanaged, IGeneralComponent
    {
        public ComponentConfig Config { get; private set; }

        public ComponentGeneralAdapter() => Config = ComponentConfig<TComponent>.Config;

        public unsafe TComponent2 GetComponent<TComponent2>(EntityData entityData, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => (TComponent2)archeTypeData.GetComponentObj(entityData, Config);

        public unsafe void SetComponent<TComponent2>(ChangeVersion changeVersion, EntityData entityData, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => archeTypeData.SetComponentAdapter(changeVersion, entityData, Config, component);

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType, Config);

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfig(ref cachedArcheType, Config);

        public IDataCatalog CreateCatalog()
            => new DataCatalog<UnmanagedDataBook<TComponent>>();
    }

    internal class ComponentManagedAdapter<TComponent> : IComponentAdapter
        where TComponent : IManagedComponent
    {
        public ComponentConfig Config { get; private set; }

        public ComponentManagedAdapter() => Config = ComponentConfig<TComponent>.Config;

        public TComponent2 GetComponent<TComponent2>(EntityData entityData, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => (TComponent2)archeTypeData.GetManagedComponentObj(entityData, Config);

        public void SetComponent<TComponent2>(ChangeVersion changeVersion, EntityData entityData, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => archeTypeData.SetManagedComponentAdapter(changeVersion, entityData, Config, component);

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType, Config);

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfig(ref cachedArcheType, Config);

        public IDataCatalog CreateCatalog()
            => new DataCatalog<ManagedDataBook<TComponent>>();
    }

    internal class ComponentSharedAdapter<TComponent> : IComponentAdapter
        where TComponent : ISharedComponent
    {
        public ComponentConfig Config { get; private set; }

        public ComponentSharedAdapter() => Config = ComponentConfig<TComponent>.Config;

        public unsafe TComponent2 GetComponent<TComponent2>(EntityData entityData, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
            => (TComponent2)archeTypeData.GetSharedComponentData(Config);

        public void SetComponent<TComponent2>(ChangeVersion changeVersion, EntityData entityData, TComponent2 component, ArcheTypeData archeTypeData)
            where TComponent2 : IComponent
        {
        }

        public void AddConfig<TComponent2>(ref ArcheType cachedArcheType, TComponent2 component, SharedComponentDictionaries sharedDics)
            where TComponent2 : IComponent
            => ArcheType.AddConfig(ref cachedArcheType,
                Config, sharedDics.GetDic(Config).GetSharedDataIndex(component));

        public void RemoveConfig(ref ArcheType cachedArcheType)
            => ArcheType.RemoveConfigAndSharedDataIndex(ref cachedArcheType, Config);

        public IDataCatalog CreateCatalog()
            => throw new NotImplementedException();
    }
}
