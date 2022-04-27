using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal unsafe struct ArcheType : IEquatable<ArcheType>, IDisposable
    {
        internal ComponentConfig* ComponentConfigs { get; set; }
        internal SharedComponentDataIndex* SharedComponentDataIndexes { get; set; }
        internal int ComponentConfigLength { get; set; }
        internal int SharedComponentDataLength { get; set; }

        internal bool HasComponentConfig(ComponentConfig config)
        {
            for (var i = 0; i < ComponentConfigLength; i++)
            {
                if (ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        internal bool HasSharedComponentDataIndex(SharedComponentDataIndex sharedComponentDataindex)
        {
            for (var i = 0; i < SharedComponentDataLength; i++)
            {
                var check = SharedComponentDataIndexes[i];
                if (check.SharedIndex == sharedComponentDataindex.SharedDataIndex &&
                    check.SharedDataIndex == sharedComponentDataindex.SharedDataIndex)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool ReplaceSharedComponentDataIndex(SharedComponentDataIndex sharedDataIndex)
        {
            for (var i = 0; i < SharedComponentDataLength; i++)
            {
                if (SharedComponentDataIndexes[i].SharedIndex == sharedDataIndex.SharedIndex)
                {
                    SharedComponentDataIndexes[i] = sharedDataIndex;
                    return true;
                }
            }

            return false;
        }

        internal ArcheType Clone()
        {
            var clone = new ArcheType
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(ComponentConfigLength),
                ComponentConfigLength = ComponentConfigLength,
                SharedComponentDataIndexes = SharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(SharedComponentDataLength)
                    : null,
                SharedComponentDataLength = SharedComponentDataLength
            };

            MemoryHelper.Copy(
                ComponentConfigs,
                clone.ComponentConfigs,
                ComponentConfigLength * TypeCache<ComponentConfig>.SizeInBytes);
            if (SharedComponentDataLength > 0)
            {
                MemoryHelper.Copy(
                    SharedComponentDataIndexes,
                    clone.SharedComponentDataIndexes,
                    SharedComponentDataLength * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }

            return clone;
        }

        public void Dispose()
        {
            if (ComponentConfigLength != 0)
                MemoryHelper.Free(ComponentConfigs);
            if (SharedComponentDataLength != 0)
                MemoryHelper.Free(SharedComponentDataIndexes);

            ComponentConfigs = null;
            SharedComponentDataIndexes = null;
            ComponentConfigLength = 0;
            SharedComponentDataLength = 0;
        }

        public static bool operator !=(ArcheType lhs, ArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheType lhs, ArcheType rhs)
        {
            if (lhs.ComponentConfigLength != rhs.ComponentConfigLength)
                return false;
            if (lhs.SharedComponentDataLength != rhs.SharedComponentDataLength)
                return false;
            unsafe
            {
                for (var i = 0; i < lhs.ComponentConfigLength; i++)
                {
                    if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                        return false;
                }
                for (var i = 0; i < lhs.SharedComponentDataLength; i++)
                {
                    if (lhs.SharedComponentDataIndexes[i] != rhs.SharedComponentDataIndexes[i])
                        return false;
                }
            }

            return true;
        }

        public bool Equals(ArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is ArcheType obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = HashCodeHelper.StartHashCode();
            for (var i = 0; i < ComponentConfigLength; i++)
                hashCode = hashCode.AppendHashCode(ComponentConfigs[i]);
            for (var i = 0; i < SharedComponentDataLength; i++)
                hashCode = hashCode.AppendHashCode(SharedComponentDataIndexes[i]);

            return hashCode.HashCode;
        }
    }
}
