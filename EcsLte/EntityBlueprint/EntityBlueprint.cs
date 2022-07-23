using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Linq;

namespace EcsLte
{
    public class EntityBlueprint
    {
        private EntityArcheType _blueprintArcheType;
        private IComponentData[] _generalComponentDatas;
        private IComponentData[] _sharedComponentDatas;
        private IComponentData[] _uniqueComponentDatas;
        private readonly object _lockObj;

        public IGeneralComponent[] Components { get; private set; }
        public ISharedComponent[] SharedComponents { get; private set; }
        public IUniqueComponent[] UniqueComponents { get; private set; }
        internal IComponentData[] GeneralComponentDatas => _generalComponentDatas;
        internal IComponentData[] SharedComponentDatas => _sharedComponentDatas;
        internal IComponentData[] UniqueComponentDatas => _uniqueComponentDatas;

        public EntityBlueprint()
        {
            _generalComponentDatas = new IComponentData[0];
            _sharedComponentDatas = new IComponentData[0];
            _uniqueComponentDatas = new IComponentData[0];
            _lockObj = new object();

            Components = new IGeneralComponent[0];
            SharedComponents = new ISharedComponent[0];
            UniqueComponents = new IUniqueComponent[0];
        }

        internal EntityBlueprint(EntityBlueprint clone)
        {
            _lockObj = new object();

            _blueprintArcheType = clone._blueprintArcheType;

            _generalComponentDatas = clone._generalComponentDatas;
            _sharedComponentDatas = clone._sharedComponentDatas;
            _uniqueComponentDatas = clone._uniqueComponentDatas;

            Components = clone.Components;
            SharedComponents = clone.SharedComponents;
            UniqueComponents = clone.UniqueComponents;
        }

        public bool HasComponent<TComponent>()
            where TComponent : IGeneralComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _generalComponentDatas.Length; i++)
            {
                if (_generalComponentDatas[i].Config == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedComponent<TComponent>()
            where TComponent : ISharedComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _sharedComponentDatas.Length; i++)
            {
                if (_sharedComponentDatas[i].Config == config)
                    return true;
            }

            return false;
        }

        public bool HasUniqueComponent<TComponent>()
            where TComponent : IUniqueComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _uniqueComponentDatas.Length; i++)
            {
                if (_uniqueComponentDatas[i].Config == config)
                    return true;
            }

            return false;
        }

        public TComponent GetComponent<TComponent>()
            where TComponent : IGeneralComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(GeneralComponentDatas, config);

            return (TComponent)Components[IndexOfComponent(config)];
        }

        public TComponent GetSharedComponent<TComponent>()
            where TComponent : ISharedComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(SharedComponentDatas, config);

            return (TComponent)SharedComponents[IndexOfSharedComponent(config)];
        }

        public TComponent GetUniqueComponent<TComponent>()
            where TComponent : IUniqueComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertNotHaveComponent(UniqueComponentDatas, config);

            return (TComponent)UniqueComponents[IndexOfUniqueComponent(config)];
        }

        public EntityBlueprint SetComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            GenerateComponents(HasComponent<TComponent>() ? _blueprintArcheType : null,
                new ComponentData<TComponent>(component));

            return this;
        }

        public EntityBlueprint SetSharedComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            GenerateComponents(null, new ComponentData<TComponent>(component));

            return this;
        }

        public EntityBlueprint SetUniqueComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IUniqueComponent
        {
            GenerateComponents(HasUniqueComponent<TComponent>() ? _blueprintArcheType : null,
                new ComponentData<TComponent>(component));

            return this;
        }

        public EntityArcheType GetArcheType() => new EntityArcheType(GetBlueprintArcheType());

        internal static void AssertEntityBlueprint(EntityBlueprint blueprint)
        {
            if (blueprint == null)
                throw new ArgumentNullException(nameof(blueprint));
            if (blueprint._generalComponentDatas.Length == 0 &&
                blueprint._sharedComponentDatas.Length == 0 &&
                blueprint._uniqueComponentDatas.Length == 0)
            {
                throw new ComponentsNoneException();
            }
        }

        internal EntityArcheType GetBlueprintArcheType()
        {
            lock (_lockObj)
            {
                if (_blueprintArcheType == null)
                {
                    _blueprintArcheType = new EntityArcheType(
                        _generalComponentDatas.Select(x => x.Config).ToArray(),
                        _uniqueComponentDatas.Select(x => x.Config).ToArray(),
                        _sharedComponentDatas);
                }

                return _blueprintArcheType;
            }
        }

        private void GenerateComponents(EntityArcheType blueprintArcheType, IComponentData componentData)
        {
            lock (_lockObj)
            {
                _blueprintArcheType = blueprintArcheType;

                _generalComponentDatas = componentData.Config.IsGeneral
                    ? Helper.CopyAddOrReplaceSort(_generalComponentDatas, componentData)
                    : _generalComponentDatas;
                _sharedComponentDatas = componentData.Config.IsShared
                    ? Helper.CopyAddOrReplaceSort(_sharedComponentDatas, componentData)
                    : _sharedComponentDatas;
                _uniqueComponentDatas = componentData.Config.IsUnique
                    ? Helper.CopyAddOrReplaceSort(_uniqueComponentDatas, componentData)
                    : _uniqueComponentDatas;

                Components = GeneralComponentDatas.Select(x => (IGeneralComponent)x.Component).ToArray();
                SharedComponents = SharedComponentDatas.Select(x => (ISharedComponent)x.Component).ToArray();
                UniqueComponents = UniqueComponentDatas.Select(x => (IUniqueComponent)x.Component).ToArray();
            }
        }

        private int IndexOfComponent(ComponentConfig config)
        {
            if (_generalComponentDatas != null)
            {
                for (var i = 0; i < _generalComponentDatas.Length; i++)
                {
                    if (_generalComponentDatas[i].Config == config)
                        return i;
                }
            }

            return -1;
        }

        private int IndexOfSharedComponent(ComponentConfig config)
        {
            if (_sharedComponentDatas != null)
            {
                for (var i = 0; i < _sharedComponentDatas.Length; i++)
                {
                    if (_sharedComponentDatas[i].Config == config)
                        return i;
                }
            }

            return -1;
        }

        private int IndexOfUniqueComponent(ComponentConfig config)
        {
            if (_uniqueComponentDatas != null)
            {
                for (var i = 0; i < _uniqueComponentDatas.Length; i++)
                {
                    if (_uniqueComponentDatas[i].Config == config)
                        return i;
                }
            }

            return -1;
        }

        private void AssertAlreadyHasComponent(in IComponentData[] componentDatas, ComponentConfig config)
        {
            for (var i = 0; i < componentDatas.Length; i++)
            {
                if (componentDatas[i].Config == config)
                    throw new EntityBlueprintAlreadyHasComponentException(config.ComponentType);
            }
        }

        private void AssertNotHaveComponent(in IComponentData[] componentDatas, ComponentConfig config)
        {
            for (var i = 0; i < componentDatas.Length; i++)
            {
                if (componentDatas[i].Config == config)
                    return;
            }

            throw new EntityBlueprintNotHaveComponentException(config.ComponentType);
        }
    }
}