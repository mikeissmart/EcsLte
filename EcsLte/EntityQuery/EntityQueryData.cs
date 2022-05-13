using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal class EntityQueryData : IEquatable<EntityQueryData>
    {
        private readonly int _hashCode;

        internal ComponentConfig[] AllComponentConfigs { get; set; } = new ComponentConfig[0];
        internal ComponentConfig[] AnyComponentConfigs { get; set; } = new ComponentConfig[0];
        internal ComponentConfig[] NoneComponentConfigs { get; set; } = new ComponentConfig[0];
        internal int ConfigCount => AllComponentConfigs.Length + AnyComponentConfigs.Length + NoneComponentConfigs.Length;
        internal IEntityQueryFilterComponent[] FilterComponents { get; set; } = new IEntityQueryFilterComponent[0];
        internal EcsContext Context { get; set; }
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        internal List<PtrWrapper> ArcheTypeDatas { get; private set; } = new List<PtrWrapper>();
        internal int? ArcheTypeChangeVersion { get; set; }
        internal int? EntityQueryDataIndex { get; set; }

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
            if (lhs.AllComponentConfigs.Length != rhs.AllComponentConfigs.Length ||
                lhs.AnyComponentConfigs.Length != rhs.AnyComponentConfigs.Length ||
                lhs.NoneComponentConfigs.Length != rhs.NoneComponentConfigs.Length ||
                lhs.FilterComponents.Length != rhs.FilterComponents.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.AllComponentConfigs[i] != rhs.AllComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.AnyComponentConfigs[i] != rhs.AnyComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.NoneComponentConfigs[i] != rhs.NoneComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.AllComponentConfigs.Length; i++)
            {
                if (lhs.FilterComponents[i].Config != rhs.FilterComponents[i].Config)
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
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(AllComponentConfigs.Length)
                    .AppendHashCode(AnyComponentConfigs.Length)
                    .AppendHashCode(NoneComponentConfigs.Length)
                    .AppendHashCode(FilterComponents.Length);
                foreach (var config in AllComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var config in AnyComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var config in NoneComponentConfigs)
                    hashCode = hashCode.AppendHashCode(config);
                foreach (var sharedComponent in FilterComponents)
                    hashCode = hashCode.AppendHashCode(sharedComponent.Config);
            }

            return _hashCode;
        }
    }
}
