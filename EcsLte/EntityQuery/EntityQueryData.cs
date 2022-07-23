namespace EcsLte
{
    /*internal class EntityQueryData : IEquatable<EntityQueryData>
    {
        private int _hashCode;
        private Type[] _allComponentTypes;
        private Type[] _anyComponentTypes;
        private Type[] _noneComponentTypes;
        private ISharedComponent[] _filterComponents;

        internal Type[] AllComponentTypes
        {
            get
            {
                if (_allComponentTypes == null)
                    _allComponentTypes = Helper.GetComponentConfigTypes(AllComponentConfigs);
                return _allComponentTypes;
            }
        }

        internal Type[] AnyComponentTypes
        {
            get
            {
                if (_anyComponentTypes == null)
                    _anyComponentTypes = Helper.GetComponentConfigTypes(AnyComponentConfigs);
                return _anyComponentTypes;
            }
        }

        internal Type[] NoneComponentTypes
        {
            get
            {
                if (_noneComponentTypes == null)
                    _noneComponentTypes = Helper.GetComponentConfigTypes(NoneComponentConfigs);
                return _noneComponentTypes;
            }
        }

        internal ISharedComponent[] FilterComponents
        {
            get
            {
                if (_filterComponents == null)
                {
                    _filterComponents = new ISharedComponent[FilterComponentDatas.Length];
                    for (var i = 0; i < FilterComponentDatas.Length; i++)
                        _filterComponents[i] = (ISharedComponent)FilterComponentDatas[i].Component;
                }
                return _filterComponents;
            }
        }

        internal int ConfigCount => AllComponentConfigs.Length + AnyComponentConfigs.Length + NoneComponentConfigs.Length;
        internal ComponentConfig[] AllComponentConfigs { get; private set; }
        internal ComponentConfig[] AnyComponentConfigs { get; private set; }
        internal ComponentConfig[] NoneComponentConfigs { get; private set; }
        internal IComponentData[] FilterComponentDatas { get; private set; }
        internal Dictionary<EcsContext, EntityQueryEcsContextData> ContextQueryData { get; private set; }

        internal EntityQueryData()
        {
            AllComponentConfigs = new ComponentConfig[0];
            AnyComponentConfigs = new ComponentConfig[0];
            NoneComponentConfigs = new ComponentConfig[0];
            FilterComponentDatas = new IComponentData[0];
            ContextQueryData = new Dictionary<EcsContext, EntityQueryEcsContextData>();
        }

        internal EntityQueryData(ComponentConfig[] allComponentConfigs,
           ComponentConfig[] anyComponentConfigs,
           ComponentConfig[] noneComponentConfigs,
           IComponentData[] filterComponentDatas)
        {
            AllComponentConfigs = allComponentConfigs;
            AnyComponentConfigs = anyComponentConfigs;
            NoneComponentConfigs = noneComponentConfigs;
            FilterComponentDatas = filterComponentDatas;
            ContextQueryData = new Dictionary<EcsContext, EntityQueryEcsContextData>();
        }

        internal bool IsFiltered(ArcheType archeType)
            => IsFilteredAllComponentConfigs(archeType) &&
                IsFilteredAnyComponentConfigs(archeType) &&
                IsFilteredNoneComponentConfigs(archeType);

        private unsafe bool IsFilteredAllComponentConfigs(ArcheType archeType)
        {
            for (int i = 0, archeTypeIndex = 0; i < AllComponentConfigs.Length; i++)
            {
                var isOk = false;
                for (; archeTypeIndex < archeType.ComponentConfigLength; archeTypeIndex++)
                {
                    if (AllComponentConfigs[i] == archeType.ComponentConfigs[archeTypeIndex])
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
            for (var i = 0; i < AnyComponentConfigs.Length; i++)
            {
                for (var j = 0; j < archeType.ComponentConfigLength; j++)
                {
                    if (AnyComponentConfigs[i] == archeType.ComponentConfigs[j])
                        return true;
                }
            }

            return false || AnyComponentConfigs.Length == 0;
        }

        private unsafe bool IsFilteredNoneComponentConfigs(ArcheType archeType)
        {
            for (var i = 0; i < NoneComponentConfigs.Length; i++)
            {
                for (var j = 0; j < archeType.ComponentConfigLength; j++)
                {
                    if (NoneComponentConfigs[i] == archeType.ComponentConfigs[j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(EntityQueryData lhs, EntityQueryData rhs) => !(lhs == rhs);

        public static bool operator ==(EntityQueryData lhs, EntityQueryData rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.AllComponentConfigs.Length != rhs.AllComponentConfigs.Length ||
                lhs.AnyComponentConfigs.Length != rhs.AnyComponentConfigs.Length ||
                lhs.NoneComponentConfigs.Length != rhs.NoneComponentConfigs.Length ||
                lhs.FilterComponentDatas.Length != rhs.FilterComponentDatas.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.AllComponentConfigs[i] != rhs.AllComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.AnyComponentConfigs.Length; i++)
            {
                if (lhs.AnyComponentConfigs[i] != rhs.AnyComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.NoneComponentConfigs.Length; i++)
            {
                if (lhs.NoneComponentConfigs[i] != rhs.NoneComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.FilterComponentDatas.Length; i++)
            {
                if (lhs.FilterComponentDatas[i].Config != rhs.FilterComponentDatas[i].Config)
                    return false;
            }

            return true;
        }

        public bool Equals(EntityQueryData other) => this == other;

        public override bool Equals(object other) => other is EntityQueryData obj && this == obj;

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                var hasCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(AllComponentConfigs.Length)
                    .AppendHashCode(AnyComponentConfigs.Length)
                    .AppendHashCode(NoneComponentConfigs.Length)
                    .AppendHashCode(FilterComponentDatas.Length);
                foreach (var config in AllComponentConfigs)
                    hasCode = hasCode.AppendHashCode(config);
                foreach (var config in AnyComponentConfigs)
                    hasCode = hasCode.AppendHashCode(config);
                foreach (var config in NoneComponentConfigs)
                    hasCode = hasCode.AppendHashCode(config);
                foreach (var sharedComponent in FilterComponentDatas)
                    hasCode = hasCode.AppendHashCode(sharedComponent.Config);

                _hashCode = hasCode.HashCode;
            }

            return _hashCode;
        }
    }*/
}