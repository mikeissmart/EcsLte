using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityArcheType : IEquatable<EntityArcheType>
    {
        private Dictionary<EcsContext, ArcheTypeData> _contextDatas;
        private int _hashCode;
        private readonly object _lockObj;

        public Type[] ComponentTypes { get; private set; }
        public Type[] UniqueComponentTypes { get; private set; }
        public ISharedComponent[] SharedComponents { get; private set; }
        internal ComponentConfig[] AllComponentConfigs { get; private set; }
        internal ComponentConfig[] GeneralComponentConfigs { get; private set; }
        internal ComponentConfig[] UniqueComponentConfigs { get; private set; }
        internal IComponentData[] SharedComponentDatas { get; private set; }

        public EntityArcheType()
        {
            _contextDatas = new Dictionary<EcsContext, ArcheTypeData>();
            _lockObj = new object();

            GeneralComponentConfigs = new ComponentConfig[0];
            UniqueComponentConfigs = new ComponentConfig[0];
            SharedComponentDatas = new IComponentData[0];

            AllComponentConfigs = new ComponentConfig[0];
            ComponentTypes = new Type[0];
            UniqueComponentTypes = new Type[0];
            SharedComponents = new ISharedComponent[0];
        }

        private EntityArcheType(bool dontGenerateTypes)
        {
            _contextDatas = new Dictionary<EcsContext, ArcheTypeData>();
            _lockObj = new object();
        }

        internal unsafe EntityArcheType(EcsContext context, ArcheTypeData archeTypeData)
            : this(false)
        {
            _contextDatas.Add(context, archeTypeData);

            GeneralComponentConfigs = new ComponentConfig[archeTypeData.GeneralConfigs.Length];
            Array.Copy(archeTypeData.GeneralConfigs, GeneralComponentConfigs, archeTypeData.GeneralConfigs.Length);

            UniqueComponentConfigs = new ComponentConfig[archeTypeData.UniqueConfigs.Length];
            Array.Copy(archeTypeData.UniqueConfigs, UniqueComponentConfigs, archeTypeData.UniqueConfigs.Length);

            SharedComponentDatas = new IComponentData[archeTypeData.SharedConfigs.Length];
            for (var i = 0; i < SharedComponentDatas.Length; i++)
            {
                var sharedDataIndex = archeTypeData.ArcheType.SharedComponentDataIndexes[i];
                SharedComponentDatas[i] = context.SharedIndexDics
                    .GetSharedIndexDic(archeTypeData.SharedConfigs[i])
                    .GetComponentData(sharedDataIndex);
            }

            GenerateConfigs();
        }

        internal EntityArcheType(ComponentConfig[] generalConfigs, ComponentConfig[] uniqueConfigs, IComponentData[] sharedComponentDatas)
            : this(false)
        {
            GeneralComponentConfigs = new ComponentConfig[generalConfigs.Length];
            Array.Copy(generalConfigs, GeneralComponentConfigs, generalConfigs.Length);

            UniqueComponentConfigs = new ComponentConfig[uniqueConfigs.Length];
            Array.Copy(uniqueConfigs, UniqueComponentConfigs, uniqueConfigs.Length);

            SharedComponentDatas = new IComponentData[sharedComponentDatas.Length];
            Array.Copy(sharedComponentDatas, SharedComponentDatas, sharedComponentDatas.Length);

            GenerateConfigs();
        }

        internal EntityArcheType(EntityArcheType clone)
        {
            _contextDatas = new Dictionary<EcsContext, ArcheTypeData>(clone._contextDatas);
            _hashCode = clone._hashCode;
            _lockObj = new object();

            GeneralComponentConfigs = clone.GeneralComponentConfigs;
            UniqueComponentConfigs = clone.UniqueComponentConfigs;
            SharedComponentDatas = clone.SharedComponentDatas;

            AllComponentConfigs = clone.AllComponentConfigs;
            ComponentTypes = clone.ComponentTypes;
            UniqueComponentTypes = clone.UniqueComponentTypes;
            SharedComponents = clone.SharedComponents;
        }

        public bool HasComponentType<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < GeneralComponentConfigs.Length; i++)
            {
                if (GeneralComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityArcheType AddComponentType<TComponent>()
            where TComponent : unmanaged, IGeneralComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHasComponent(GeneralComponentConfigs, config);

            AppendConfig(config);

            return this;
        }

        public bool HasSharedComponentType<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < SharedComponentDatas.Length; i++)
            {
                if (SharedComponentDatas[i].Config == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedComponent<TSharedComponent>(TSharedComponent component)
            where TSharedComponent : unmanaged, ISharedComponent
        {
            for (var i = 0; i < SharedComponentDatas.Length; i++)
            {
                if (SharedComponentDatas[i].ComponentEquals(component))
                    return true;
            }

            return false;
        }

        public EntityArcheType AddSharedComponent<TSharedComponent>(TSharedComponent component)
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            AssertAlreadyHasComponent(SharedComponentDatas, config);

            AppendSharedConfig(config, new ComponentData<TSharedComponent>(component));

            return this;
        }

        public TSharedComponent GetSharedComponent<TSharedComponent>()
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            AssertNotHaveSharedComponent(SharedComponentDatas, config);

            return (TSharedComponent)SharedComponentDatas.First(x => x.Config == config).Component;
        }

        public EntityArcheType UpdateSharedComponent<TSharedComponent>(TSharedComponent component)
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            AssertNotHaveSharedComponent(SharedComponentDatas, config);

            AppendSharedConfig(config, new ComponentData<TSharedComponent>(component));

            return this;
        }

        public bool HasUniqueComponentType<TComponent>()
            where TComponent : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < UniqueComponentConfigs.Length; i++)
            {
                if (UniqueComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityArcheType AddUniqueComponentType<TComponent>()
            where TComponent : unmanaged, IUniqueComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertAlreadyHasComponent(UniqueComponentConfigs, config);

            AppendConfig(config);

            return this;
        }

        internal static void AssertEntityArcheType(EntityArcheType archeType)
        {
            if (archeType == null)
                throw new ArgumentNullException(nameof(archeType));
            if (archeType.AllComponentConfigs.Length == 0)
                throw new ComponentsNoneException();
        }

        internal bool TryGetArcheTypeData(EcsContext context, out ArcheTypeData archeTypeData)
        {
            lock (_lockObj)
            {
                return _contextDatas.TryGetValue(context, out archeTypeData);
            }
        }

        internal void SetArcheTypeData(EcsContext context, ArcheTypeData archeTypeData)
        {
            lock (_lockObj)
            {
                if (!_contextDatas.ContainsKey(context))
                    _contextDatas.Add(context, archeTypeData);
                else
                    _contextDatas[context] = archeTypeData;
            }
        }

        public static bool operator !=(EntityArcheType lhs, EntityArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityArcheType lhs, EntityArcheType rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.AllComponentConfigs.Length != rhs.AllComponentConfigs.Length ||
                lhs.SharedComponentDatas.Length != rhs.SharedComponentDatas.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.SharedComponentDatas.Length; i++)
            {
                if (!lhs.SharedComponentDatas[i].Equals(rhs.SharedComponentDatas[i]))
                    return false;
            }
            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.AllComponentConfigs[i] != rhs.AllComponentConfigs[i])
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityArcheType obj && this == obj;

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(ComponentTypes.Length)
                    .AppendHashCode(SharedComponents.Length);
                foreach (var type in ComponentTypes)
                    hashCode = hashCode.AppendHashCode(type);
                foreach (var component in SharedComponents)
                    hashCode = hashCode.AppendHashCode(component);
                _hashCode = hashCode.HashCode;
            }

            return _hashCode;
        }

        private void AppendConfig(ComponentConfig config)
        {
            lock (_lockObj)
            {
                _contextDatas = new Dictionary<EcsContext, ArcheTypeData>();
                _hashCode = 0;

                GeneralComponentConfigs = config.IsGeneral
                    ? Helper.CopyInsertSort(GeneralComponentConfigs, config)
                    : GeneralComponentConfigs;
                UniqueComponentConfigs = config.IsUnique
                    ? Helper.CopyInsertSort(UniqueComponentConfigs, config)
                    : UniqueComponentConfigs;
                SharedComponentDatas = SharedComponentDatas;

                GenerateConfigs();
            }
        }

        private void AppendSharedConfig(ComponentConfig config, IComponentData componentData)
        {
            lock (_lockObj)
            {
                _contextDatas = new Dictionary<EcsContext, ArcheTypeData>();
                _hashCode = 0;

                GeneralComponentConfigs = GeneralComponentConfigs;
                UniqueComponentConfigs = UniqueComponentConfigs;
                SharedComponentDatas = Helper.CopyAddOrReplaceSort(SharedComponentDatas, componentData);

                GenerateConfigs();
            }
        }

        private void GenerateConfigs()
        {
            AllComponentConfigs = new ComponentConfig[GeneralComponentConfigs.Length +
                UniqueComponentConfigs.Length +
                SharedComponentDatas.Length];
            if (GeneralComponentConfigs.Length > 0)
                Array.Copy(GeneralComponentConfigs, AllComponentConfigs, GeneralComponentConfigs.Length);
            if (UniqueComponentConfigs.Length > 0)
            {
                Array.Copy(UniqueComponentConfigs, 0,
                    AllComponentConfigs, GeneralComponentConfigs.Length, UniqueComponentConfigs.Length);
            }
            if (SharedComponentDatas.Length > 0)
            {
                SharedComponents = new ISharedComponent[SharedComponentDatas.Length];
                for (int i = 0, configIndex = GeneralComponentConfigs.Length + UniqueComponentConfigs.Length;
                    i < SharedComponentDatas.Length;
                    i++, configIndex++)
                {
                    AllComponentConfigs[configIndex] = SharedComponentDatas[i].Config;
                    SharedComponents[i] = (ISharedComponent)SharedComponentDatas[i].Component;
                }
            }
            else
                SharedComponents = new ISharedComponent[0];
            Array.Sort(AllComponentConfigs);

            ComponentTypes = GeneralComponentConfigs.Select(x => x.ComponentType).ToArray();
            UniqueComponentTypes = UniqueComponentConfigs.Select(x => x.ComponentType).ToArray();
        }

        private static void AssertAlreadyHasComponent(in ComponentConfig[] configs, ComponentConfig config)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                if (configs[i] == config)
                    throw new EntityArcheTypeAlreadyHasComponentException(config.ComponentType);
            }
        }

        private static void AssertNotHaveComponent(in ComponentConfig[] configs, ComponentConfig config)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                if (configs[i] == config)
                    return;
            }

            throw new EntityArcheTypeNotHaveComponentException(config.ComponentType);
        }

        private static void AssertAlreadyHasComponent(in IComponentData[] componentDatas, ComponentConfig config)
        {
            for (var i = 0; i < componentDatas.Length; i++)
            {
                if (componentDatas[i].Config == config)
                    throw new EntityArcheTypeAlreadyHasComponentException(config.ComponentType);
            }
        }

        private static void AssertNotHaveSharedComponent(in IComponentData[] componentDatas, ComponentConfig config)
        {
            for (var i = 0; i < componentDatas.Length; i++)
            {
                if (componentDatas[i].Config == config)
                    return;
            }

            throw new EntityArcheTypeNotHaveComponentException(config.ComponentType);
        }
    }
}