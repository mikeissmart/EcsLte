using EcsLte.Data;
using System;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal interface IComponentAdapter
    {
        ComponentConfig Config { get; }
        ComponentConfigOffset ConfigOffset { get; }
        SharedComponentDataIndex SharedDataIndex { get; }
        bool IsUpdated { get; }

        unsafe void SetComponentsPtr(byte* componentsPtr);

        void SetComponentConfigOffset(ComponentConfigOffset configOffset);

        T GetComponent<T>() where T : IComponent;

        void SetComponent<T>(T component) where T : IComponent;
    }

    internal class EntityQueryComponentAdapter
    {
        internal static IComponentAdapter Create(ComponentConfig config,
            SharedComponentIndexDictionaries sharedComponentIndexes, ManagedComponentPools managePools)
        {
            IComponentAdapter adapter;
            if (config.IsBlittable)
            {
                adapter = config.IsShared
                    ? new BlittableSharedComponentAdapter(config, sharedComponentIndexes)
                    : new BlittableComponentAdapter(config);
            }
            else
            {
                adapter = config.IsShared
                    ? new ManageSharedComponentAdapter(config, managePools, sharedComponentIndexes)
                    : new ManageComponentAdapter(config, managePools);
            }

            return adapter;
        }

        private abstract class ComponentAdapter : IComponentAdapter
        {
            protected unsafe byte* ComponentPtr { get; private set; }
            public ComponentConfig Config { get; private set; }
            public ComponentConfigOffset ConfigOffset { get; private set; }
            public SharedComponentDataIndex SharedDataIndex { get; protected set; }
            public bool IsUpdated { get; protected set; }

            protected ComponentAdapter(ComponentConfig config) => Config = config;

            public virtual unsafe void SetComponentsPtr(byte* componentsPtr) => ComponentPtr = componentsPtr + ConfigOffset.OffsetInBytes;

            public void SetComponentConfigOffset(ComponentConfigOffset configOffset) => ConfigOffset = configOffset;

            public abstract T GetComponent<T>() where T : IComponent;

            public abstract void SetComponent<T>(T component) where T : IComponent;
        }

        private class BlittableComponentAdapter : ComponentAdapter
        {
            public BlittableComponentAdapter(ComponentConfig config)
                : base(config)
            {
            }

            public override unsafe T GetComponent<T>() => Marshal.PtrToStructure<T>((IntPtr)ComponentPtr);

            public override unsafe void SetComponent<T>(T component)
            {
                Marshal.StructureToPtr(component, (IntPtr)ComponentPtr, false);
                IsUpdated = true;
            }
        }

        private class BlittableSharedComponentAdapter : BlittableComponentAdapter
        {
            private readonly IIndexDictionary _sharedComponentIndexDic;

            public BlittableSharedComponentAdapter(ComponentConfig config, SharedComponentIndexDictionaries sharedIndexDics)
                : base(config) => _sharedComponentIndexDic = sharedIndexDics.GetSharedIndexDic(config);

            public override unsafe void SetComponent<T>(T component)
            {
                base.SetComponent(component);
                SharedDataIndex = new SharedComponentDataIndex
                {
                    SharedIndex = Config.SharedIndex,
                    SharedDataIndex = ((IndexDictionary<T>)_sharedComponentIndexDic)
                        .GetOrAdd(component)
                };
            }
        }

        private class ManageComponentAdapter : ComponentAdapter
        {
            private readonly IManagedComponentPool _managePool;
            private int _componentIndex;

            internal ManageComponentAdapter(ComponentConfig config, ManagedComponentPools managePools)
                : base(config) => _managePool = managePools.GetPool(config);

            public override unsafe void SetComponentsPtr(byte* componentsPtr)
            {
                base.SetComponentsPtr(componentsPtr);
                _componentIndex = *(int*)ComponentPtr;
            }

            public override unsafe T GetComponent<T>() => ((ManagedComponentPool<T>)_managePool).GetComponent(_componentIndex);

            public override void SetComponent<T>(T component)
            {
                ((ManagedComponentPool<T>)_managePool).SetComponent(_componentIndex, component);
                IsUpdated = true;
            }
        }

        private class ManageSharedComponentAdapter : ManageComponentAdapter
        {
            private readonly IIndexDictionary _sharedComponentIndexDic;

            internal ManageSharedComponentAdapter(ComponentConfig config, ManagedComponentPools managePools,
                SharedComponentIndexDictionaries sharedIndexDics)
                : base(config, managePools) => _sharedComponentIndexDic = sharedIndexDics.GetSharedIndexDic(config);

            public override void SetComponent<T>(T component)
            {
                base.SetComponent(component);
                SharedDataIndex = new SharedComponentDataIndex
                {
                    SharedIndex = Config.SharedIndex,
                    SharedDataIndex = ((IndexDictionary<T>)_sharedComponentIndexDic)
                        .GetOrAdd(component)
                };
            }
        }
    }
}