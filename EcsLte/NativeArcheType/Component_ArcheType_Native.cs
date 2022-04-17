using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;

namespace EcsLte.NativeArcheType
{
    public struct Component_ArcheType_Native : IEquatable<Component_ArcheType_Native>, IDisposable
    {
        public unsafe ComponentConfig* ComponentConfigs;
        public unsafe SharedComponentDataIndex* SharedComponentDataIndexes;
        public int ComponentConfigLength { get; set; }
        public int SharedComponentDataLength { get; set; }

        public unsafe bool HasComponentConfig(ComponentConfig config)
        {
            for (var i = 0; i < ComponentConfigLength; i++)
            {
                if (ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public unsafe bool HasSharedIndex(ComponentConfig config, int sharedDataIndex)
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

        public static unsafe Component_ArcheType_Native AppendComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs == null)
            {
                var configs = MemoryHelper.Alloc<ComponentConfig>(1);
                configs[0] = config;

                return new Component_ArcheType_Native
                {
                    ComponentConfigs = configs,
                    SharedComponentDataIndexes = null,
                    ComponentConfigLength = 1,
                    SharedComponentDataLength = 0
                };
            }

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength + 1),
                SharedComponentDataIndexes = archeType.SharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(archeType.SharedComponentDataLength)
                    : null,
                ComponentConfigLength = archeType.ComponentConfigLength + 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength
            };

            ComponentConfigCopyInsertSort(
                archeType.ComponentConfigs,
                archeType.ComponentConfigLength,
                newArcheType.ComponentConfigs,
                config);

            if (newArcheType.SharedComponentDataLength > 0)
                CopySharedDataIndexes(ref archeType, ref newArcheType);

            return newArcheType;
        }

        public static unsafe Component_ArcheType_Native AppendComponent(Component_ArcheType_Native archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            if (archeType.ComponentConfigs == null)
            {
                var configs = MemoryHelper.Alloc<ComponentConfig>(1);
                configs[0] = config;
                var sharedIndexes = MemoryHelper.Alloc<SharedComponentDataIndex>(1);
                sharedIndexes[0] = sharedDataIndex;

                return new Component_ArcheType_Native
                {
                    ComponentConfigs = configs,
                    SharedComponentDataIndexes = sharedIndexes,
                    ComponentConfigLength = 1,
                    SharedComponentDataLength = 1
                };
            }

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength + 1),
                SharedComponentDataIndexes = MemoryHelper.Alloc<SharedComponentDataIndex>(archeType.SharedComponentDataLength + 1),
                ComponentConfigLength = archeType.ComponentConfigLength + 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength + 1
            };

            ComponentConfigCopyInsertSort(
                archeType.ComponentConfigs,
                archeType.ComponentConfigLength,
                newArcheType.ComponentConfigs,
                config);

            if (archeType.SharedComponentDataLength > 0)
            {
                ShareComponentDataIndexCopyInsertSort(
                    archeType.SharedComponentDataIndexes,
                    archeType.SharedComponentDataLength,
                    newArcheType.SharedComponentDataIndexes,
                    sharedDataIndex);
            }
            else
            {
                newArcheType.SharedComponentDataIndexes[0] = sharedDataIndex;
            }

            return newArcheType;
        }

        public static unsafe Component_ArcheType_Native ReplaceSharedComponent(Component_ArcheType_Native archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };
            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength),
                SharedComponentDataIndexes = MemoryHelper.Alloc<SharedComponentDataIndex>(archeType.SharedComponentDataLength),
                ComponentConfigLength = archeType.ComponentConfigLength,
                SharedComponentDataLength = archeType.SharedComponentDataLength
            };

            CopyComponentConfigs(ref archeType, ref newArcheType);
            CopySharedDataIndexes(ref archeType, ref newArcheType);

            for (var i = 0; i < newArcheType.SharedComponentDataLength; i++)
            {
                var check = newArcheType.SharedComponentDataIndexes[i];
                if (check.SharedIndex == sharedDataIndex.SharedIndex)
                {
                    newArcheType.SharedComponentDataIndexes[i] = sharedDataIndex;
                    break;
                }
            }

            return newArcheType;
        }

        public static unsafe Component_ArcheType_Native RemoveComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigLength == 1)
                return new Component_ArcheType_Native();

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength - 1),
                SharedComponentDataIndexes = archeType.SharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(archeType.SharedComponentDataLength)
                    : null,
                ComponentConfigLength = archeType.ComponentConfigLength - 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength
            };

            if (archeType.SharedComponentDataLength > 0)
                CopySharedDataIndexes(ref archeType, ref newArcheType);

            for (int i = 0, j = 0; i < archeType.ComponentConfigLength; i++)
            {
                var check = archeType.ComponentConfigs[i];
                if (check.ComponentIndex != config.ComponentIndex)
                    newArcheType.ComponentConfigs[j++] = check;
            }

            return newArcheType;
        }

        public static unsafe Component_ArcheType_Native RemoveSharedComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigLength == 1)
                return new Component_ArcheType_Native();

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength - 1),
                SharedComponentDataIndexes = archeType.SharedComponentDataLength > 1
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(archeType.SharedComponentDataLength - 1)
                    : null,
                ComponentConfigLength = archeType.ComponentConfigLength - 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength - 1
            };

            for (int i = 0, j = 0; i < archeType.ComponentConfigLength; i++)
            {
                var check = archeType.ComponentConfigs[i];
                if (check.ComponentIndex != config.ComponentIndex)
                    newArcheType.ComponentConfigs[j++] = check;
            }
            if (archeType.SharedComponentDataLength > 1)
            {
                for (int i = 0, j = 0; i < archeType.SharedComponentDataLength; i++)
                {
                    var check = archeType.SharedComponentDataIndexes[i];
                    if (check.SharedIndex != config.SharedIndex)
                        newArcheType.SharedComponentDataIndexes[j++] = check;
                }
            }

            return newArcheType;
        }

        public unsafe void Dispose()
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

        public static bool operator !=(Component_ArcheType_Native lhs, Component_ArcheType_Native rhs) => !(lhs == rhs);

        public static bool operator ==(Component_ArcheType_Native lhs, Component_ArcheType_Native rhs)
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

        public bool Equals(Component_ArcheType_Native other)
            => this == other;

        public override bool Equals(object other)
            => other is Component_ArcheType_Native obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = -612338121;
            unsafe
            {
                for (var i = 0; i < ComponentConfigLength; i++)
                    hashCode = hashCode * -1521134295 + ComponentConfigs[i].GetHashCode();
                for (var i = 0; i < SharedComponentDataLength; i++)
                    hashCode = hashCode * -1521134295 + SharedComponentDataIndexes[i].GetHashCode();
            }

            return hashCode;
        }

        /*private static unsafe void ComponentConfigInsertSort(ComponentConfig* configs, int length, ComponentConfig value)
        {
            for (length--; length > 0 && value.CompareTo(configs[length - 1]) < 0; length--)
                configs[length] = configs[length - 1];
            configs[length] = value;
        }*/

        private static unsafe void ComponentConfigCopyInsertSort(ComponentConfig* source, int sourceLength, ComponentConfig* destination, ComponentConfig insert)
        {
            var index = sourceLength - 1;
            for (; index >= 0; index--)
            {
                if (source[index].ComponentIndex < insert.ComponentIndex)
                    break;
            }

            // Copy before insert
            if (index >= 0)
            {
                MemoryHelper.Copy(
                    source,
                    destination,
                    (index + 1) * TypeCache<ComponentConfig>.SizeInBytes);
            }

            // insert
            destination[index + 1] = insert;

            // Copy after insert
            if (sourceLength - 1 != index)
            {
                MemoryHelper.Copy(
                    &source[index + 1],
                    &destination[index + 2],
                    (sourceLength - (index + 1)) * TypeCache<ComponentConfig>.SizeInBytes);
            }
        }

        private static unsafe void ShareComponentDataIndexCopyInsertSort(SharedComponentDataIndex* source, int sourceLength, SharedComponentDataIndex* destination, SharedComponentDataIndex insert)
        {
            var index = sourceLength - 1;
            for (; index >= 0; index--)
            {
                if (source[index].SharedIndex < insert.SharedIndex)
                    break;
            }

            // Copy before insert
            if (index >= 0)
            {
                MemoryHelper.Copy(
                    source,
                    destination,
                    //1 - 0 = 1
                    (index + 1) * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }

            // insert
            destination[index + 1] = insert;

            // Copy after insert
            if (sourceLength - 1 != index)
            {
                MemoryHelper.Copy(
                    &source[index + 1],
                    &destination[index + 2],
                    (sourceLength - (index + 1)) * TypeCache<SharedComponentDataIndex>.SizeInBytes);
            }
        }

        /*private static unsafe void ShareComponentDataIndexInsertSort(SharedComponentDataIndex* sharedIndexes, int length, SharedComponentDataIndex value)
        {
            for (length--; length > 0 && value.CompareTo(sharedIndexes[length - 1]) < 0; length--)
                sharedIndexes[length] = sharedIndexes[length - 1];
            sharedIndexes[length] = value;
        }*/

        private static unsafe void CopyComponentConfigs(ref Component_ArcheType_Native source, ref Component_ArcheType_Native destination) => MemoryHelper.Copy(source.ComponentConfigs, destination.ComponentConfigs, TypeCache<ComponentConfig>.SizeInBytes * source.ComponentConfigLength);

        private static unsafe void CopySharedDataIndexes(ref Component_ArcheType_Native source, ref Component_ArcheType_Native destination) => MemoryHelper.Copy(source.SharedComponentDataIndexes, destination.SharedComponentDataIndexes, TypeCache<SharedComponentDataIndex>.SizeInBytes * source.SharedComponentDataLength);
    }
}
