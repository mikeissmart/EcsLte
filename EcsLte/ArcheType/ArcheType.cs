using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal unsafe struct ArcheType : IEquatable<ArcheType>, IDisposable
    {
        internal static ArcheType Alloc(int componentConfigLength, int sharedComponentDataLength)
        {
            if (componentConfigLength < 0)
                throw new ArgumentOutOfRangeException(nameof(componentConfigLength));
            if (sharedComponentDataLength < 0 || sharedComponentDataLength > componentConfigLength)
                throw new ArgumentOutOfRangeException(nameof(sharedComponentDataLength));

            return new ArcheType
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(componentConfigLength),
                SharedComponentDataIndexes = sharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(sharedComponentDataLength)
                    : null,
                ComponentConfigLength = componentConfigLength,
                SharedComponentDataLength = sharedComponentDataLength
            };
        }

        internal static ArcheType AllocClone(ArcheType source)
        {
            var archeType = Alloc(source.ComponentConfigLength, source.SharedComponentDataLength);

            MemoryHelper.Copy(
                source.ComponentConfigs,
                archeType.ComponentConfigs,
                archeType.ComponentConfigLength * TypeCache<ComponentConfig>.SizeInBytes);
            if (source.SharedComponentDataLength > 0)
            {
                MemoryHelper.Copy(
                    source.SharedComponentDataIndexes,
                    archeType.SharedComponentDataIndexes,
                    source.SharedComponentDataLength * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }

            return archeType;
        }

        internal ComponentConfig* ComponentConfigs { get; private set; }
        internal SharedComponentDataIndex* SharedComponentDataIndexes { get; private set; }
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

        internal bool ReplaceSharedComponentDataIndex(SharedComponentDataIndex nextSharedDataIndex)
        {
            for (var i = 0; i < SharedComponentDataLength; i++)
            {
                var check = SharedComponentDataIndexes[i];
                if (check.SharedIndex == nextSharedDataIndex.SharedIndex)
                {
                    if (check.SharedDataIndex != nextSharedDataIndex.SharedDataIndex)
                    {
                        SharedComponentDataIndexes[i] = nextSharedDataIndex;
                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
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
            if (lhs.SharedComponentDataLength != rhs.SharedComponentDataLength)
                return false;
            if (lhs.ComponentConfigLength != rhs.ComponentConfigLength)
                return false;

            for (var i = 0; i < lhs.SharedComponentDataLength; i++)
            {
                if (lhs.SharedComponentDataIndexes[i].SharedIndex != rhs.SharedComponentDataIndexes[i].SharedIndex)
                    return false;
            }
            for (var i = 0; i < lhs.ComponentConfigLength; i++)
            {
                if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                    return false;
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