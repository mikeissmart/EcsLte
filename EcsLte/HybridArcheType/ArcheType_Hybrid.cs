using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;

namespace EcsLte.HybridArcheType
{
    public unsafe struct ArcheType_Hybrid : IEquatable<ArcheType_Hybrid>, IDisposable
    {
        public ComponentConfig* ComponentConfigs { get; set; }
        public SharedComponentDataIndex* SharedComponentDataIndexes { get; set; }
        public int ComponentConfigLength { get; set; }
        public int SharedComponentDataLength { get; set; }

        public bool HasComponentConfig(ComponentConfig config)
        {
            for (var i = 0; i < ComponentConfigLength; i++)
            {
                if (ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedIndex(ComponentConfig config, int sharedDataIndex)
        {
            for (var i = 0; i < SharedComponentDataLength; i++)
            {
                var check = SharedComponentDataIndexes[i];
                if (check.SharedIndex == config.ComponentIndex &&
                    check.SharedDataIndex == sharedDataIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ReplaceSharedComponentDataIndex(SharedComponentDataIndex sharedDataIndex)
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

        public ArcheType_Hybrid Clone()
        {
            var clone = new ArcheType_Hybrid
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

        public static bool operator !=(ArcheType_Hybrid lhs, ArcheType_Hybrid rhs) => !(lhs == rhs);

        public static bool operator ==(ArcheType_Hybrid lhs, ArcheType_Hybrid rhs)
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

        public bool Equals(ArcheType_Hybrid other)
            => this == other;

        public override bool Equals(object other)
            => other is ArcheType_Hybrid obj && this == obj;

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
