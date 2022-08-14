using EcsLte.Utilities;
using System;

namespace EcsLte
{
    internal unsafe struct ArcheType : IEquatable<ArcheType>, IDisposable
    {
        internal ComponentConfig* Configs;
        internal int ConfigsLength;
        internal SharedDataIndex* SharedDataIndexes;
        internal int SharedDataIndexesLength;

        internal static ArcheType Alloc(int configsLength, int sharedDataIndexesLength)
            => new ArcheType
            {
                Configs = MemoryHelper.Alloc<ComponentConfig>(configsLength),
                SharedDataIndexes = sharedDataIndexesLength > 0
                        ? MemoryHelper.Alloc<SharedDataIndex>(sharedDataIndexesLength)
                        : null,
                ConfigsLength = configsLength,
                SharedDataIndexesLength = sharedDataIndexesLength
            };

        internal static ArcheType AllocClone(ArcheType src)
        {
            if (src.ConfigsLength == 0)
                return new ArcheType();

            var archeType = Alloc(src.ConfigsLength, src.SharedDataIndexesLength);

            MemoryHelper.Copy(
                src.Configs,
                archeType.Configs,
                src.ConfigsLength);
            if (src.SharedDataIndexesLength > 0)
            {
                MemoryHelper.Copy(
                    src.SharedDataIndexes,
                    archeType.SharedDataIndexes,
                    src.SharedDataIndexesLength);
            }

            return archeType;
        }

        internal static void CopyToCached(in ArcheType srcArcheType, ref ArcheType cachedArcheType)
        {
            cachedArcheType.ConfigsLength = srcArcheType.ConfigsLength;
            cachedArcheType.SharedDataIndexesLength = srcArcheType.SharedDataIndexesLength;

            if (srcArcheType.ConfigsLength > 0)
            {
                MemoryHelper.Copy(
                    srcArcheType.Configs,
                    cachedArcheType.Configs,
                    srcArcheType.ConfigsLength);
            }
            if (srcArcheType.SharedDataIndexesLength > 0)
            {
                MemoryHelper.Copy(
                    srcArcheType.SharedDataIndexes,
                    cachedArcheType.SharedDataIndexes,
                    srcArcheType.SharedDataIndexesLength);
            }
        }

        internal static void AddConfig(ref ArcheType cachedArcheType, ComponentConfig config)
        {
            var index = -1;
            if (cachedArcheType.ConfigsLength > 0)
            {
                index = cachedArcheType.ConfigsLength - 1;
                for (; index >= 0; index--)
                {
                    if (cachedArcheType.Configs[index].ComponentIndex < config.ComponentIndex)
                        break;
                }

                MemoryHelper.Copy(
                    cachedArcheType.Configs + index,
                    cachedArcheType.Configs + index + 1,
                    cachedArcheType.ConfigsLength - index);
            }

            cachedArcheType.Configs[index + 1] = config;
            cachedArcheType.ConfigsLength++;
        }

        internal static void AddConfig(ref ArcheType cachedArcheType,
            ComponentConfig config, SharedDataIndex sharedDataIndex)
        {
            var index = -1;
            if (cachedArcheType.ConfigsLength > 0)
            {
                index = cachedArcheType.ConfigsLength - 1;
                for (; index >= 0; index--)
                {
                    var check = cachedArcheType.Configs[index];
                    if (cachedArcheType.Configs[index].ComponentIndex < config.ComponentIndex)
                        break;
                }

                MemoryHelper.Copy(
                    cachedArcheType.Configs + index,
                    cachedArcheType.Configs + index + 1,
                    cachedArcheType.ConfigsLength - index);
            }
            cachedArcheType.Configs[index + 1] = config;
            cachedArcheType.ConfigsLength++;

            index = -1;
            if (cachedArcheType.SharedDataIndexesLength > 0)
            {
                index = cachedArcheType.SharedDataIndexesLength - 1;
                for (; index >= 0; index--)
                {
                    var check = cachedArcheType.SharedDataIndexes[index];
                    if (cachedArcheType.SharedDataIndexes[index].SharedIndex < config.SharedIndex)
                        break;
                }

                MemoryHelper.Copy(
                    cachedArcheType.SharedDataIndexes + index,
                    cachedArcheType.SharedDataIndexes + index + 1,
                    cachedArcheType.SharedDataIndexesLength - index);
            }
            cachedArcheType.SharedDataIndexes[index + 1] = sharedDataIndex;
            cachedArcheType.SharedDataIndexesLength++;
        }

        internal static void RemoveConfig(ref ArcheType cachedArcheType, ComponentConfig config)
        {
            var index = cachedArcheType.ConfigsLength - 1;
            for (; index >= 0; index--)
            {
                if (cachedArcheType.Configs[index].ComponentIndex == config.ComponentIndex)
                    break;
            }

            if (index != cachedArcheType.ConfigsLength - 1)
            {
                MemoryHelper.Copy(
                    cachedArcheType.Configs + index + 1,
                    cachedArcheType.Configs + index,
                    cachedArcheType.ConfigsLength - index);
            }
            cachedArcheType.ConfigsLength--;
        }

        internal static void RemoveConfigAndSharedDataIndex(ref ArcheType cachedArcheType, ComponentConfig config)
        {
            var index = cachedArcheType.ConfigsLength - 1;
            for (; index >= 0; index--)
            {
                if (cachedArcheType.Configs[index].ComponentIndex == config.ComponentIndex)
                    break;
            }

            if (index != cachedArcheType.ConfigsLength - 1)
            {
                MemoryHelper.Copy(
                    cachedArcheType.Configs + index + 1,
                    cachedArcheType.Configs + index,
                    cachedArcheType.ConfigsLength - index);
            }
            cachedArcheType.ConfigsLength--;

            index = cachedArcheType.SharedDataIndexesLength - 1;
            for (; index >= 0; index--)
            {
                if (cachedArcheType.SharedDataIndexes[index].SharedIndex == config.SharedIndex)
                    break;
            }

            if (index != cachedArcheType.SharedDataIndexesLength - 1)
            {
                MemoryHelper.Copy(
                    cachedArcheType.SharedDataIndexes + index + 1,
                    cachedArcheType.SharedDataIndexes + index,
                    cachedArcheType.SharedDataIndexesLength - index);
            }
            cachedArcheType.SharedDataIndexesLength--;
        }

        internal static bool ReplaceSharedDataIndex(ref ArcheType cachedArcheType,
            SharedDataIndex sharedDataIndex)
        {
            for (var i = 0; i < cachedArcheType.SharedDataIndexesLength; i++)
            {
                var check = cachedArcheType.SharedDataIndexes[i];
                if (check.SharedIndex == sharedDataIndex.SharedIndex)
                {
                    if (check.DataIndex != sharedDataIndex.DataIndex)
                    {
                        cachedArcheType.SharedDataIndexes[i] = sharedDataIndex;
                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        internal bool HasConfig(ComponentConfig config)
        {
            for (var i = 0; i < ConfigsLength; i++)
            {
                if (Configs[i] == config)
                    return true;
            }

            return false;
        }

        internal bool HasSharedDataIndex(SharedDataIndex sharedDataIndex)
        {
            for (var i = 0; i < SharedDataIndexesLength; i++)
            {
                if (SharedDataIndexes[i].SharedIndex == sharedDataIndex.SharedIndex)
                    return SharedDataIndexes[i].DataIndex == sharedDataIndex.DataIndex;
            }

            return false;
        }

        public void Dispose()
        {
            if (ConfigsLength != 0)
                MemoryHelper.Free(Configs);
            if (SharedDataIndexesLength != 0)
                MemoryHelper.Free(SharedDataIndexes);

            Configs = null;
            ConfigsLength = 0;
            SharedDataIndexes = null;
            SharedDataIndexesLength = 0;
        }

        #region Equals

        public static bool operator !=(ArcheType lhs, ArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheType lhs, ArcheType rhs)
        {
            if (lhs.SharedDataIndexesLength != rhs.SharedDataIndexesLength)
                return false;
            if (lhs.ConfigsLength != rhs.ConfigsLength)
                return false;

            for (var i = 0; i < lhs.SharedDataIndexesLength; i++)
            {
                if (lhs.SharedDataIndexes[i] != rhs.SharedDataIndexes[i])
                    return false;
            }
            for (var i = 0; i < lhs.ConfigsLength; i++)
            {
                if (lhs.Configs[i] != rhs.Configs[i])
                    return false;
            }

            return true;
        }

        public bool Equals(ArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is ArcheType obj && this == obj;

        #endregion

        public override int GetHashCode()
        {
            var hashCode = HashCodeHelper.StartHashCode();
            for (var i = 0; i < ConfigsLength; i++)
                hashCode = hashCode.AppendHashCode(Configs[i]);
            for (var i = 0; i < SharedDataIndexesLength; i++)
                hashCode = hashCode.AppendHashCode(SharedDataIndexes[i]);

            return hashCode.HashCode;
        }
    }
}
