using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityFilter : IEquatable<EntityFilter>
    {
        private readonly Dictionary<EcsContext, EntityFilterContextData> _contextDatas;
        private int _hashCode;
        private readonly object _lockObj;

        public Type[] AllOfComponentTypes { get; private set; }
        public Type[] AnyOfComponentTypes { get; private set; }
        public Type[] NoneOfComponentTypes { get; private set; }
        public ISharedComponent[] FilterByComponents { get; private set; }
        internal int ConfigCount { get; private set; }
        internal ComponentConfig[] AllOfComponentConfigs { get; private set; }
        internal ComponentConfig[] AnyOfComponentConfigs { get; private set; }
        internal ComponentConfig[] NoneOfComponentConfigs { get; private set; }
        internal IComponentData[] FilterByComponentDatas { get; private set; }

        public EntityFilter()
        {
            _contextDatas = new Dictionary<EcsContext, EntityFilterContextData>();
            _lockObj = new object();

            AllOfComponentConfigs = new ComponentConfig[0];
            AnyOfComponentConfigs = new ComponentConfig[0];
            NoneOfComponentConfigs = new ComponentConfig[0];
            FilterByComponentDatas = new IComponentData[0];

            GenerateTypes();
        }

        internal EntityFilter(EntityFilter clone)
        {
            _contextDatas = new Dictionary<EcsContext, EntityFilterContextData>(clone._contextDatas);
            _lockObj = new object();

            AllOfComponentConfigs = clone.AllOfComponentConfigs;
            AnyOfComponentConfigs = clone.AnyOfComponentConfigs;
            NoneOfComponentConfigs = clone.NoneOfComponentConfigs;
            FilterByComponentDatas = clone.FilterByComponentDatas;

            AllOfComponentTypes = clone.AllOfComponentTypes;
            AnyOfComponentTypes = clone.AnyOfComponentTypes;
            NoneOfComponentTypes = clone.NoneOfComponentTypes;
            FilterByComponents = clone.FilterByComponents;
            ConfigCount = clone.ConfigCount;
        }

        public bool HasWhereOf<TComponent>()
            where TComponent : IComponent
            => HasWhereAllOf<TComponent>() ||
                HasWhereAnyOf<TComponent>() ||
                HasWhereNoneOf<TComponent>();

        public bool HasWhereAllOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < AllOfComponentConfigs.Length; i++)
            {
                if (AllOfComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityFilter WhereAllOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertHasWhereAnyOf(config);
            AssertHasWhereNoneOf(config);

            GenerateConfigs(config, FilterCategory.All);

            return this;
        }

        public EntityFilter WhereAllOf(EntityArcheType archeType)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            for (var i = 0; i < archeType.AllComponentConfigs.Length; i++)
            {
                var config = archeType.AllComponentConfigs[i];
                AssertHasWhereAnyOf(config);
                AssertHasWhereNoneOf(config);
            }

            GenerateConfigs(archeType.AllComponentConfigs, FilterCategory.All);

            return this;
        }

        public bool HasWhereAnyOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < AnyOfComponentConfigs.Length; i++)
            {
                if (AnyOfComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityFilter WhereAnyOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertHasWhereAllOf(config);
            AssertHasWhereNoneOf(config);

            GenerateConfigs(config, FilterCategory.Any);

            return this;
        }

        public EntityFilter WhereAnyOf(EntityArcheType archeType)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            for (var i = 0; i < archeType.AllComponentConfigs.Length; i++)
            {
                var config = archeType.AllComponentConfigs[i];
                AssertHasWhereAllOf(config);
                AssertHasWhereNoneOf(config);
            }

            GenerateConfigs(archeType.AllComponentConfigs, FilterCategory.Any);

            return this;
        }

        public bool HasWhereNoneOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < NoneOfComponentConfigs.Length; i++)
            {
                if (NoneOfComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public EntityFilter WhereNoneOf<TComponent>()
            where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            AssertHasWhereAllOf(config);
            AssertHasWhereAnyOf(config);

            GenerateConfigs(config, FilterCategory.None);

            return this;
        }

        public EntityFilter WhereNoneOf(EntityArcheType archeType)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            for (var i = 0; i < archeType.AllComponentConfigs.Length; i++)
            {
                var config = archeType.AllComponentConfigs[i];
                AssertHasWhereAllOf(config);
                AssertHasWhereAnyOf(config);
            }

            GenerateConfigs(archeType.AllComponentConfigs, FilterCategory.None);

            return this;
        }

        public bool HasFilterBy<TSharedComponent>()
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;

            return FilterByComponentDatas.Any(x => x.Config == config);
        }

        public TSharedComponent GetFilterBy<TSharedComponent>()
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            for (var i = 0; i < FilterByComponentDatas.Length; i++)
            {
                if (FilterByComponentDatas[i].Config == config)
                    return (TSharedComponent)FilterByComponentDatas[i].Component;
            }

            throw new EntityFilterNotFilterByException(config.ComponentType);
        }

        public EntityFilter FilterBy<TSharedComponent>(TSharedComponent component)
            where TSharedComponent : unmanaged, ISharedComponent
        {
            var config = ComponentConfig<TSharedComponent>.Config;
            AssertHasWhereAnyOf(config);
            AssertHasWhereNoneOf(config);

            GenerateConfigs(new ComponentData<TSharedComponent>(component));

            return this;
        }

        public EntityFilter FilterBy(EntityArcheType archeType)
        {
            EntityArcheType.AssertEntityArcheType(archeType);
            if (archeType.SharedComponentDatas.Length == 0)
                throw new EntityArcheTypeNoSharedComponentException();

            for (var i = 0; i < archeType.SharedComponentDatas.Length; i++)
            {
                var config = archeType.SharedComponentDatas[i].Config;
                AssertHasWhereAnyOf(config);
                AssertHasWhereNoneOf(config);
            }

            GenerateConfigs(archeType.SharedComponentDatas);

            return this;
        }

        internal static void AssertEntityFilter(EntityFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
        }

        internal EntityFilterContextData GetContextData(EcsContext context)
        {
            lock (_lockObj)
            {
                if (!_contextDatas.TryGetValue(context, out var contextData))
                {
                    contextData = new EntityFilterContextData();
                    _contextDatas.Add(context, contextData);
                }

                context.ArcheTypeManager.UpdateEntityFilter(this, contextData);

                return contextData;
            }
        }

        internal bool IsFiltered(ArcheType archeType)
            => IsFilteredAllComponentConfigs(archeType) &&
                IsFilteredAnyComponentConfigs(archeType) &&
                IsFilteredNoneComponentConfigs(archeType);

        private unsafe bool IsFilteredAllComponentConfigs(ArcheType archeType)
        {
            for (var i = 0; i < AllOfComponentConfigs.Length; i++)
            {
                var isOk = false;
                for (var j = 0; j < archeType.ComponentConfigLength; j++)
                {
                    if (AllOfComponentConfigs[i] == archeType.ComponentConfigs[j])
                    {
                        isOk = true;
                        break;
                    }
                }

                if (!isOk)
                    return false;
            }

            return true;
        }

        private unsafe bool IsFilteredAnyComponentConfigs(ArcheType archeType)
        {
            for (var i = 0; i < AnyOfComponentConfigs.Length; i++)
            {
                for (var j = 0; j < archeType.ComponentConfigLength; j++)
                {
                    if (AnyOfComponentConfigs[i] == archeType.ComponentConfigs[j])
                        return true;
                }
            }

            return false || AnyOfComponentConfigs.Length == 0;
        }

        private unsafe bool IsFilteredNoneComponentConfigs(ArcheType archeType)
        {
            for (var i = 0; i < NoneOfComponentConfigs.Length; i++)
            {
                for (var j = 0; j < archeType.ComponentConfigLength; j++)
                {
                    if (NoneOfComponentConfigs[i] == archeType.ComponentConfigs[j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(EntityFilter lhs, EntityFilter rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityFilter lhs, EntityFilter rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.AllOfComponentConfigs.Length != rhs.AllOfComponentConfigs.Length ||
                lhs.AnyOfComponentConfigs.Length != rhs.AnyOfComponentConfigs.Length ||
                lhs.NoneOfComponentConfigs.Length != rhs.NoneOfComponentConfigs.Length ||
                lhs.FilterByComponentDatas.Length != rhs.FilterByComponentDatas.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.AllOfComponentConfigs.Length; i++)
            {
                if (lhs.AllOfComponentConfigs[i] != rhs.AllOfComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.AnyOfComponentConfigs.Length; i++)
            {
                if (lhs.AnyOfComponentConfigs[i] != rhs.AnyOfComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.NoneOfComponentConfigs.Length; i++)
            {
                if (lhs.NoneOfComponentConfigs[i] != rhs.NoneOfComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.FilterByComponentDatas.Length; i++)
            {
                if (!lhs.FilterByComponentDatas[i].Equals(rhs.FilterByComponentDatas[i].Config))
                    return false;
            }

            return true;
        }

        public bool Equals(EntityFilter other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityFilter obj && this == obj;

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(AllOfComponentConfigs.Length)
                    .AppendHashCode(AnyOfComponentConfigs.Length)
                    .AppendHashCode(NoneOfComponentConfigs.Length)
                    .AppendHashCode(FilterByComponents.Length);
                foreach (var config in AllOfComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var config in AnyOfComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var config in NoneOfComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var componentData in FilterByComponentDatas)
                    hashCode = hashCode.AppendHashCode(componentData.Config);
                _hashCode = hashCode.HashCode;
            }

            return _hashCode;
        }

        private void GenerateConfigs(ComponentConfig config, FilterCategory category)
        {
            lock (_lockObj)
            {
                AllOfComponentConfigs = category == FilterCategory.All
                    ? AppendConfig(AllOfComponentConfigs, config)
                    : AllOfComponentConfigs;
                AnyOfComponentConfigs = category == FilterCategory.Any
                    ? AppendConfig(AnyOfComponentConfigs, config)
                    : AnyOfComponentConfigs;
                NoneOfComponentConfigs = category == FilterCategory.None
                    ? AppendConfig(NoneOfComponentConfigs, config)
                    : NoneOfComponentConfigs;
                FilterByComponentDatas = FilterByComponentDatas;

                GenerateTypes();
            }
        }

        private void GenerateConfigs(IComponentData componentData)
        {
            lock (_lockObj)
            {
                AllOfComponentConfigs = !AllOfComponentConfigs.Any(x => x == componentData.Config)
                    ? Helper.CopyInsertSort(AllOfComponentConfigs, componentData.Config)
                    : AllOfComponentConfigs;
                AnyOfComponentConfigs = AnyOfComponentConfigs;
                NoneOfComponentConfigs = NoneOfComponentConfigs;
                FilterByComponentDatas = Helper.CopyAddOrReplaceSort(FilterByComponentDatas, componentData);

                GenerateTypes();
            }
        }

        private void GenerateConfigs(IComponentData[] componentDatas)
        {
            lock (_lockObj)
            {
                var configs = componentDatas
                    .Select(x => x.Config)
                    .ToArray();
                AllOfComponentConfigs = AppendConfigs(AllOfComponentConfigs, configs);
                AnyOfComponentConfigs = AnyOfComponentConfigs;
                NoneOfComponentConfigs = NoneOfComponentConfigs;
                FilterByComponentDatas = Helper.CopyAddOrReplaceSorts(FilterByComponentDatas, componentDatas);

                GenerateTypes();
            }
        }

        private void GenerateConfigs(ComponentConfig[] configs, FilterCategory category)
        {
            lock (_lockObj)
            {
                AllOfComponentConfigs = category == FilterCategory.All
                    ? AppendConfigs(AllOfComponentConfigs, configs)
                    : AllOfComponentConfigs;
                AnyOfComponentConfigs = category == FilterCategory.Any
                    ? AppendConfigs(AnyOfComponentConfigs, configs)
                    : AnyOfComponentConfigs;
                NoneOfComponentConfigs = category == FilterCategory.None
                    ? AppendConfigs(NoneOfComponentConfigs, configs)
                    : NoneOfComponentConfigs;
                FilterByComponentDatas = FilterByComponentDatas;

                GenerateTypes();
            }
        }

        private void GenerateTypes()
        {
            AllOfComponentTypes = AllOfComponentConfigs
                .Select(x => x.ComponentType)
                .ToArray();
            AnyOfComponentTypes = AnyOfComponentConfigs
                .Select(x => x.ComponentType)
                .ToArray();
            NoneOfComponentTypes = NoneOfComponentConfigs
                .Select(x => x.ComponentType)
                .ToArray();
            FilterByComponents = FilterByComponentDatas
                .Select(x => (ISharedComponent)x.Component)
                .ToArray();
            ConfigCount = AllOfComponentConfigs.Length + AnyOfComponentConfigs.Length + NoneOfComponentConfigs.Length;
        }

        private ComponentConfig[] AppendConfig(in ComponentConfig[] src, ComponentConfig config) => !src.Any(x => x == config)
                ? Helper.CopyInsertSort(src, config)
                : src;

        private ComponentConfig[] AppendConfigs(in ComponentConfig[] src, ComponentConfig[] configs)
        {
            var destConfigs = new List<ComponentConfig>(src);
            foreach (var config in configs)
            {
                if (!destConfigs.Contains(config))
                    destConfigs.Add(config);
            }
            destConfigs.Sort();
            return destConfigs.ToArray();
        }

        private void AssertHasWhereAllOf(ComponentConfig config)
        {
            for (var i = 0; i < AllOfComponentConfigs.Length; i++)
            {
                if (AllOfComponentConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereAllOfException(config.ComponentType);
            }
        }

        private void AssertHasWhereAnyOf(ComponentConfig config)
        {
            for (var i = 0; i < AnyOfComponentConfigs.Length; i++)
            {
                if (AnyOfComponentConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereAnyOfException(config.ComponentType);
            }
        }

        private void AssertHasWhereNoneOf(ComponentConfig config)
        {
            for (var i = 0; i < NoneOfComponentConfigs.Length; i++)
            {
                if (NoneOfComponentConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereNoneOfException(config.ComponentType);
            }
        }

        private enum FilterCategory
        {
            All,
            Any,
            None
        }
    }
}
