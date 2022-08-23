using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;

namespace EcsLte
{
    public class EntityArcheType : IEquatable<EntityArcheType>
    {
        private Data _data;

        public EcsContext Context => _data.Context;
        public Type[] ComponentTypes => _data.ComponentTypes;
        public Type[] ManagedComponentTypes => _data.ManagedComponentTypes;
        public Type[] SharedComponentTypes => _data.SharedComponentTypes;
        public ISharedComponent[] SharedComponents => _data.SharedComponents;
        internal SharedDataIndex[] SharedDataIndexes => _data.SharedDataIndexes;
        internal ISharedComponentData[] SharedComponentDatas => _data.SharedComponentDatas;
        internal ComponentConfig[] AllConfigs => _data.AllConfigs;
        internal ArcheTypeData ArcheTypeData
        {
            get => _data.ArcheTypeData;
            set => _data.ArcheTypeData = value;
        }

        internal EntityArcheType(EcsContext context) => _data = new Data
        {
            //ArcheTypeData
            HashCode = 0,
            LockObj = new object(),
            Context = context,
            AllConfigs = new ComponentConfig[0],
            Configs = new ComponentConfig[0],
            ManagedConfigs = new ComponentConfig[0],
            SharedConfigs = new ComponentConfig[0],
            SharedDataIndexes = new SharedDataIndex[0],
            SharedComponents = new ISharedComponent[0],
            SharedComponentDatas = new ISharedComponentData[0]
        };

        internal EntityArcheType(EcsContext context, EntityBlueprint blueprint)
        {
            _data = new Data
            {
                //ArcheTypeData
                HashCode = 0,
                LockObj = new object(),
                Context = context,
                AllConfigs = new ComponentConfig[blueprint.Components.Length +
                    blueprint.ManagedComponents.Length +
                    blueprint.SharedComponents.Length],
                Configs = new ComponentConfig[blueprint.GeneralComponentDatas.Length],
                ManagedConfigs = new ComponentConfig[blueprint.ManagedComponentDatas.Length],
                SharedConfigs = new ComponentConfig[blueprint.SharedComponentDatas.Length],
                SharedDataIndexes = new SharedDataIndex[blueprint.SharedComponentDatas.Length],
                SharedComponents = new ISharedComponent[blueprint.SharedComponentDatas.Length],
                SharedComponentDatas = new ISharedComponentData[blueprint.SharedComponentDatas.Length]
            };

            var allConfigIndex = 0;
            for (var i = 0; i < blueprint.GeneralComponentDatas.Length; i++, allConfigIndex++)
            {
                _data.Configs[i] = blueprint.GeneralComponentDatas[i].Config;
                _data.AllConfigs[allConfigIndex] = blueprint.GeneralComponentDatas[i].Config;
            }
            for (var i = 0; i < blueprint.ManagedComponentDatas.Length; i++, allConfigIndex++)
            {
                _data.ManagedConfigs[i] = blueprint.ManagedComponentDatas[i].Config;
                _data.AllConfigs[allConfigIndex] = blueprint.ManagedComponentDatas[i].Config;
            }
            for (var i = 0; i < blueprint.SharedComponentDatas.Length; i++, allConfigIndex++)
            {
                _data.SharedConfigs[i] = blueprint.SharedComponentDatas[i].Config;
                _data.SharedDataIndexes[i] = blueprint.SharedComponentDatas[i]
                    .GetSharedComponentDataIndex(context.SharedComponentDics);
                _data.AllConfigs[allConfigIndex] = blueprint.SharedComponentDatas[i].Config;
            }
            Helper.ArrayCopy(blueprint.SharedComponents, _data.SharedComponents, _data.SharedComponents.Length);
            Helper.ArrayCopy(blueprint.SharedComponentDatas, _data.SharedComponentDatas, _data.SharedComponentDatas.Length);
        }

        internal unsafe EntityArcheType(EcsContext context, ArcheTypeData archeTypeData)
        {
            _data = new Data
            {
                ArcheTypeData = archeTypeData,
                HashCode = 0,
                LockObj = new object(),
                Context = context,
                AllConfigs = new ComponentConfig[archeTypeData.ArcheType.ConfigsLength],
                Configs = new ComponentConfig[archeTypeData.GeneralConfigs.Length],
                ManagedConfigs = new ComponentConfig[archeTypeData.ManagedConfigs.Length],
                SharedConfigs = new ComponentConfig[archeTypeData.SharedConfigs.Length],
                SharedDataIndexes = new SharedDataIndex[archeTypeData.SharedConfigs.Length],
                SharedComponents = new ISharedComponent[archeTypeData.SharedConfigs.Length],
                SharedComponentDatas = new ISharedComponentData[archeTypeData.SharedConfigs.Length]
            };

            fixed (ComponentConfig* configsPtr = &_data.AllConfigs[0])
            {
                MemoryHelper.Copy(
                    archeTypeData.ArcheType.Configs,
                    configsPtr,
                    archeTypeData.ArcheType.ConfigsLength);
            }
            for (var i = 0; i < archeTypeData.GeneralConfigs.Length; i++, i++)
                _data.Configs[i] = archeTypeData.GeneralConfigs[i].Config;
            for (var i = 0; i < archeTypeData.ManagedConfigs.Length; i++, i++)
                _data.ManagedConfigs[i] = archeTypeData.ManagedConfigs[i].Config;
            for (var i = 0; i < archeTypeData.SharedConfigs.Length; i++, i++)
                _data.SharedConfigs[i] = archeTypeData.SharedConfigs[i].Config;
            if (_data.SharedDataIndexes.Length > 0)
            {
                fixed (SharedDataIndex* sharedDataIndexPtr = &_data.SharedDataIndexes[0])
                {
                    MemoryHelper.Copy(
                        archeTypeData.ArcheType.SharedDataIndexes,
                        sharedDataIndexPtr,
                        archeTypeData.ArcheType.SharedDataIndexesLength);
                }
            }
            Helper.ArrayCopy(archeTypeData.SharedComponentDatas, _data.SharedComponentDatas, archeTypeData.SharedComponentDatas.Length);

            for (var i = 0; i < archeTypeData.SharedConfigs.Length; i++)
                _data.SharedComponents[i] = archeTypeData.SharedComponentDatas[i].Component;
        }

        internal EntityArcheType(EntityArcheType archeType) => _data = archeType._data;

        public bool HasComponent<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.Configs.Length; i++)
            {
                if (_data.Configs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasManagedComponent<TComponent>()
            where TComponent : IManagedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.ManagedConfigs.Length; i++)
            {
                if (_data.ManagedConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedComponent<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.SharedConfigs.Length; i++)
            {
                if (_data.SharedConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public TComponent GetSharedComponent<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.SharedComponentDatas.Length; i++)
            {
                if (_data.SharedComponentDatas[i].Config == config)
                    return (TComponent)_data.SharedComponentDatas[i].Component;
            }

            throw new ComponentNotHaveException(config.ComponentType);
        }

        public EntityArcheType AddComponentType<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHaveComponent(_data.Configs, config);

            AppendConfig(config);

            return this;
        }

        public EntityArcheType AddManagedComponentType<TComponent>()
            where TComponent : IManagedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHaveComponent(_data.ManagedConfigs, config);

            AppendConfig(config);

            return this;
        }

        public EntityArcheType AddSharedComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHaveComponent(_data.SharedConfigs, config);

            AppendSharedConfig(config, HasSharedComponent<TComponent>(), new SharedComponentData<TComponent>(component));

            return this;
        }

        public EntityArcheType UpdateSharedComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(_data.SharedConfigs, config);

            AppendSharedConfig(config, true, new SharedComponentData<TComponent>(component));

            return this;
        }

        #region Equals

        public static bool operator !=(EntityArcheType lhs, EntityArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityArcheType lhs, EntityArcheType rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs._data.Context != rhs._data.Context ||
                lhs._data.AllConfigs.Length != rhs._data.AllConfigs.Length ||
                lhs._data.Configs.Length != rhs._data.Configs.Length ||
                lhs._data.ManagedConfigs.Length != rhs._data.ManagedConfigs.Length ||
                lhs._data.SharedConfigs.Length != rhs._data.SharedConfigs.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs._data.SharedDataIndexes.Length; i++)
            {
                if (lhs._data.SharedDataIndexes[i] != rhs._data.SharedDataIndexes[i])
                    return false;
            }
            for (var i = 0; i < lhs._data.AllConfigs.Length; i++)
            {
                if (lhs._data.AllConfigs[i] != rhs._data.AllConfigs[i])
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityArcheType obj && this == obj;

        #endregion

        public override int GetHashCode()
        {
            if (_data.HashCode == 0)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(_data.AllConfigs.Length)
                    .AppendHashCode(_data.SharedComponentDatas.Length);
                for (var i = 0; i < _data.AllConfigs.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.AllConfigs[i]);
                for (var i = 0; i < _data.SharedComponentDatas.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.SharedComponentDatas[i]);
                _data.HashCode = hashCode.HashCode;
            }

            return _data.HashCode;
        }

        #region Private

        private void AppendConfig(ComponentConfig config) => _data = new Data
        {
            //ArcheTypeData
            HashCode = 0,
            LockObj = new object(),
            Context = _data.Context,
            AllConfigs = Helper.CopyInsertSort(_data.AllConfigs, config),
            Configs = config.IsGeneral
                    ? Helper.CopyInsertSort(_data.Configs, config)
                    : _data.Configs,
            ManagedConfigs = config.IsManaged
                    ? Helper.CopyInsertSort(_data.ManagedConfigs, config)
                    : _data.ManagedConfigs,
            SharedConfigs = _data.SharedConfigs,
            SharedDataIndexes = _data.SharedDataIndexes,
            SharedComponents = _data.SharedComponents,
            SharedComponentDatas = _data.SharedComponentDatas
        };

        private void AppendSharedConfig(ComponentConfig config, bool hasComponent, ISharedComponentData sharedComponentData)
        {
            var data = new Data
            {
                //ArcheTypeData
                HashCode = 0,
                LockObj = new object(),
                Context = _data.Context,
                //Configs
                Configs = _data.Configs,
                ManagedConfigs = _data.ManagedConfigs,
                //SharedConfigs
                //SharedDataIndexes
                //SharedComponents
                //SharedComponentDatas
            };

            if (hasComponent)
            {
                data.AllConfigs = _data.AllConfigs;
                data.SharedConfigs = _data.SharedConfigs;

                data.SharedDataIndexes = new SharedDataIndex[_data.SharedDataIndexes.Length];
                Helper.ArrayCopy(_data.SharedDataIndexes, data.SharedDataIndexes, data.SharedDataIndexes.Length);

                data.SharedComponents = new ISharedComponent[_data.SharedComponents.Length];
                Helper.ArrayCopy(_data.SharedComponents, data.SharedComponents, data.SharedComponents.Length);

                data.SharedComponentDatas = new ISharedComponentData[_data.SharedComponentDatas.Length];
                Helper.ArrayCopy(_data.SharedComponentDatas, data.SharedComponentDatas, data.SharedComponentDatas.Length);

                for (var i = 0; i < data.SharedComponentDatas.Length; i++)
                {
                    if (data.SharedComponentDatas[i].Config == config)
                    {
                        data.SharedComponentDatas[i] = sharedComponentData;
                        data.SharedDataIndexes[i] = sharedComponentData
                            .GetSharedComponentDataIndex(Context.SharedComponentDics);
                        data.SharedComponents[i] = sharedComponentData.Component;
                        break;
                    }
                }
            }
            else
            {
                data.AllConfigs = Helper.CopyInsertSort(_data.AllConfigs, config);
                data.SharedConfigs = Helper.CopyInsertSort(_data.SharedConfigs, config);
                data.SharedComponentDatas = Helper.CopyInsertSort(_data.SharedComponentDatas, sharedComponentData);

                data.SharedDataIndexes = new SharedDataIndex[data.SharedComponentDatas.Length];
                data.SharedComponents = new ISharedComponent[data.SharedComponentDatas.Length];
                for (var i = 0; i < data.SharedComponentDatas.Length; i++)
                {
                    data.SharedDataIndexes[i] = data.SharedComponentDatas[i]
                        .GetSharedComponentDataIndex(Context.SharedComponentDics);
                    data.SharedComponents[i] = data.SharedComponentDatas[i].Component;
                }
            }

            _data = data;
        }

        #endregion

        #region Assert

        internal static void AssertEntityArcheType(EntityArcheType archeType, EcsContext context)
        {
            if (archeType == null)
                throw new ArgumentNullException(nameof(archeType));
            if (archeType.Context != context)
                throw new EcsContextNotSameException(archeType.Context, context);
        }

        private static void AssertAlreadyHaveComponent(in ComponentConfig[] configs, ComponentConfig config)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                if (configs[i] == config)
                    throw new ComponentAlreadyHaveException(config.ComponentType);
            }
        }

        private static void AssertNotHaveComponent(in ComponentConfig[] configs, ComponentConfig config)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                if (configs[i] == config)
                    return;
            }

            throw new ComponentNotHaveException(config.ComponentType);
        }

        #endregion

        private class Data
        {
            private Type[] _componentTypes;
            private Type[] _managedComponentTypes;
            private Type[] _sharedComponentTypes;

            public ArcheTypeData ArcheTypeData;
            public int HashCode;
            public object LockObj;
            public EcsContext Context;
            public ComponentConfig[] AllConfigs;
            public ComponentConfig[] Configs;
            public ComponentConfig[] ManagedConfigs;
            public ComponentConfig[] SharedConfigs;
            public SharedDataIndex[] SharedDataIndexes;
            public ISharedComponent[] SharedComponents;
            public ISharedComponentData[] SharedComponentDatas;

            public Type[] ComponentTypes
            {
                get
                {
                    if (_componentTypes == null)
                    {
                        _componentTypes = new Type[Configs.Length];
                        for (var i = 0; i < Configs.Length; i++)
                            _componentTypes[i] = Configs[i].ComponentType;
                    }

                    return _componentTypes;
                }
            }
            public Type[] ManagedComponentTypes
            {
                get
                {
                    if (_managedComponentTypes == null)
                    {
                        _managedComponentTypes = new Type[ManagedConfigs.Length];
                        for (var i = 0; i < ManagedConfigs.Length; i++)
                            _managedComponentTypes[i] = ManagedConfigs[i].ComponentType;
                    }

                    return _managedComponentTypes;
                }
            }
            public Type[] SharedComponentTypes
            {
                get
                {
                    if (_sharedComponentTypes == null)
                    {
                        _sharedComponentTypes = new Type[SharedConfigs.Length];
                        for (var i = 0; i < SharedConfigs.Length; i++)
                            _sharedComponentTypes[i] = SharedConfigs[i].ComponentType;
                    }

                    return _sharedComponentTypes;
                }
            }
        }
    }
}
