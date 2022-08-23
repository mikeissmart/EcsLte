using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityBlueprint
    {
        private Data _data;

        public IGeneralComponent[] Components => _data.Components;
        public IManagedComponent[] ManagedComponents => _data.ManagedComponents;
        public ISharedComponent[] SharedComponents => _data.SharedComponents;
        internal IGeneralComponentData[] GeneralComponentDatas => _data.GeneralComponentDatas;
        internal IManagedComponentData[] ManagedComponentDatas => _data.ManagedComponentDatas;
        internal ISharedComponentData[] SharedComponentDatas => _data.SharedComponentDatas;

        public EntityBlueprint() => _data = new Data
        {
            ContextArcheTypes = new Dictionary<EcsContext, EntityArcheType>(),
            HashCode = 0,
            Components = new IGeneralComponent[0],
            ManagedComponents = new IManagedComponent[0],
            SharedComponents = new ISharedComponent[0],
            GeneralComponentDatas = new IGeneralComponentData[0],
            ManagedComponentDatas = new IManagedComponentData[0],
            SharedComponentDatas = new ISharedComponentData[0],
        };

        internal EntityBlueprint(EntityBlueprint blueprint) => _data = blueprint._data;

        public bool HasComponent<TComponent>()
            where TComponent : unmanaged, IGeneralComponent => IndexOfComponent(ComponentConfig<TComponent>.Config) != -1;

        public bool HasManagedComponent<TComponent>()
            where TComponent : IManagedComponent => IndexOfManagedComponent(ComponentConfig<TComponent>.Config) != -1;

        public bool HasSharedComponent<TComponent>()
            where TComponent : unmanaged, ISharedComponent => IndexOfSharedComponent(ComponentConfig<TComponent>.Config) != -1;

        public TComponent GetComponent<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfComponent(config);

            if (index != -1)
                return (TComponent)_data.Components[index];

            throw new ComponentNotHaveException(config.ComponentType);
        }

        public TComponent GetManagedComponent<TComponent>()
            where TComponent : IManagedComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfManagedComponent(config);

            if (index != -1)
                return (TComponent)_data.ManagedComponents[index];

            throw new ComponentNotHaveException(config.ComponentType);
        }

        public TComponent GetSharedComponent<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfSharedComponent(config);

            if (index != -1)
                return (TComponent)_data.SharedComponents[index];

            throw new ComponentNotHaveException(config.ComponentType);
        }

        public EntityBlueprint SetComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            var index = IndexOfComponent(ComponentConfig<TComponent>.Config);
            var componentData = new GeneralComponentData<TComponent>(component);
            var data = new Data
            {
                ContextArcheTypes = index != -1
                    ? _data.ContextArcheTypes
                    : new Dictionary<EcsContext, EntityArcheType>(),
                HashCode = index != -1
                    ? _data.HashCode
                    : 0,
                Components = index != -1
                    ? new IGeneralComponent[_data.Components.Length]
                    : new IGeneralComponent[_data.Components.Length + 1],
                ManagedComponents = _data.ManagedComponents,
                SharedComponents = _data.SharedComponents,
                GeneralComponentDatas = index != -1
                    ? new IGeneralComponentData[_data.GeneralComponentDatas.Length]
                    : Helper.CopyInsertSort(_data.GeneralComponentDatas, componentData),
                ManagedComponentDatas = _data.ManagedComponentDatas,
                SharedComponentDatas = _data.SharedComponentDatas
            };

            if (index != -1)
            {
                Helper.ArrayCopy(_data.Components, data.Components, data.Components.Length);
                Helper.ArrayCopy(_data.GeneralComponentDatas, data.GeneralComponentDatas, data.GeneralComponentDatas.Length);

                data.Components[index] = component;
                data.GeneralComponentDatas[index] = componentData;
            }
            else
            {
                for (var i = 0; i < data.GeneralComponentDatas.Length; i++)
                    data.Components[i] = data.GeneralComponentDatas[i].Component;
            }

            _data = data;

            return this;
        }

        public EntityBlueprint SetManagedComponent<TComponent>(TComponent component)
            where TComponent : IManagedComponent
        {
            var index = IndexOfManagedComponent(ComponentConfig<TComponent>.Config);
            var componentData = new ManagedComponentData<TComponent>(component);
            var data = new Data
            {
                ContextArcheTypes = index != -1
                    ? _data.ContextArcheTypes
                    : new Dictionary<EcsContext, EntityArcheType>(),
                HashCode = index != -1
                    ? _data.HashCode
                    : 0,
                Components = _data.Components,
                ManagedComponents = index != -1
                    ? new IManagedComponent[_data.ManagedComponents.Length]
                    : new IManagedComponent[_data.ManagedComponents.Length + 1],
                SharedComponents = _data.SharedComponents,
                GeneralComponentDatas = _data.GeneralComponentDatas,
                ManagedComponentDatas = index != -1
                    ? new IManagedComponentData[_data.ManagedComponentDatas.Length]
                    : Helper.CopyInsertSort(_data.ManagedComponentDatas, componentData),
                SharedComponentDatas = _data.SharedComponentDatas
            };

            if (index != -1)
            {
                Helper.ArrayCopy(_data.ManagedComponents, data.ManagedComponents, data.ManagedComponents.Length);
                Helper.ArrayCopy(_data.ManagedComponentDatas, data.ManagedComponentDatas, data.ManagedComponentDatas.Length);

                data.ManagedComponents[index] = component;
                data.ManagedComponentDatas[index] = componentData;
            }
            else
            {
                for (var i = 0; i < data.ManagedComponentDatas.Length; i++)
                    data.ManagedComponents[i] = data.ManagedComponentDatas[i].Component;
            }

            _data = data;

            return this;
        }

        public EntityBlueprint SetSharedComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            var index = IndexOfSharedComponent(ComponentConfig<TComponent>.Config);
            var componentData = new SharedComponentData<TComponent>(component);
            var data = new Data
            {
                ContextArcheTypes = new Dictionary<EcsContext, EntityArcheType>(),
                HashCode = 0,
                Components = _data.Components,
                ManagedComponents = _data.ManagedComponents,
                SharedComponents = index != -1
                    ? new ISharedComponent[_data.SharedComponents.Length]
                    : new ISharedComponent[_data.SharedComponents.Length + 1],
                GeneralComponentDatas = _data.GeneralComponentDatas,
                ManagedComponentDatas = _data.ManagedComponentDatas,
                SharedComponentDatas = index != -1
                    ? new ISharedComponentData[_data.SharedComponentDatas.Length]
                    : Helper.CopyInsertSort(_data.SharedComponentDatas, componentData)
            };

            if (index != -1)
            {
                Helper.ArrayCopy(_data.SharedComponents, data.SharedComponents, data.SharedComponents.Length);
                Helper.ArrayCopy(_data.SharedComponentDatas, data.SharedComponentDatas, data.SharedComponentDatas.Length);

                data.SharedComponents[index] = component;
                data.SharedComponentDatas[index] = componentData;
            }
            else
            {
                for (var i = 0; i < data.SharedComponentDatas.Length; i++)
                    data.SharedComponents[i] = data.SharedComponentDatas[i].Component;
            }

            _data = data;

            return this;
        }

        public EntityArcheType GetArcheType(EcsContext context)
        {
            EcsContext.AssertContext(context);
            if (!_data.ContextArcheTypes.TryGetValue(context, out var archeType))
            {
                archeType = new EntityArcheType(context, this);
                _data.ContextArcheTypes.Add(context, archeType);
            }

            return new EntityArcheType(archeType);
        }

        private int IndexOfComponent(ComponentConfig config)
        {
            for (var i = 0; i < _data.GeneralComponentDatas.Length; i++)
            {
                if (_data.GeneralComponentDatas[i].Config == config)
                    return i;
            }

            return -1;
        }

        private int IndexOfManagedComponent(ComponentConfig config)
        {
            for (var i = 0; i < _data.ManagedComponentDatas.Length; i++)
            {
                if (_data.ManagedComponentDatas[i].Config == config)
                    return i;
            }

            return -1;
        }

        private int IndexOfSharedComponent(ComponentConfig config)
        {
            for (var i = 0; i < _data.SharedComponentDatas.Length; i++)
            {
                if (_data.SharedComponentDatas[i].Config == config)
                    return i;
            }

            return -1;
        }

        internal static void AssertEntityBlueprint(EntityBlueprint blueprint)
        {
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
        }

        private class Data
        {
            public Dictionary<EcsContext, EntityArcheType> ContextArcheTypes;
            public int HashCode;
            public IGeneralComponent[] Components;
            public IManagedComponent[] ManagedComponents;
            public ISharedComponent[] SharedComponents;
            public IGeneralComponentData[] GeneralComponentDatas;
            public IManagedComponentData[] ManagedComponentDatas;
            public ISharedComponentData[] SharedComponentDatas;
        }
    }
}
