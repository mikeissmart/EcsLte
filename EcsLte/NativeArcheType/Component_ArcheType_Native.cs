using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte.NativeArcheType
{
    public struct Component_ArcheType_Native : IEquatable<Component_ArcheType_Native>, IDisposable
    {
        public unsafe ComponentConfig* ComponentConfigs;
        public unsafe ShareComponentDataIndex* ShareComponentDataIndexes;
        public int ComponentConfigLength { get; set; }
        public int SharedComponentDataLength { get; set; }

        public unsafe bool HasComponentConfig(ComponentConfig config)
        {
            for (int i = 0; i < ComponentConfigLength; i++)
            {
                if (ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public unsafe bool HasSharedIndex(ComponentConfig config, int sharedDataIndex)
        {
            for (int i = 0; i < SharedComponentDataLength; i++)
            {
                var check = ShareComponentDataIndexes[i];
                if (check.SharedIndex == config.ComponentIndex &&
                    check.SharedDataIndex == sharedDataIndex)
                    return true;
            }

            return false;
        }

        public unsafe static Component_ArcheType_Native AppendComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs == null)
            {
                var configs = MemoryHelper.Alloc<ComponentConfig>(1);
                configs[0] = config;

                return new Component_ArcheType_Native
                {
                    ComponentConfigs = configs,
                    ShareComponentDataIndexes = null,
                    ComponentConfigLength = 1,
                    SharedComponentDataLength = 0
                };
            }

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength + 1),
                ShareComponentDataIndexes = archeType.SharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<ShareComponentDataIndex>(archeType.SharedComponentDataLength)
                    : null,
                ComponentConfigLength = archeType.ComponentConfigLength + 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength
            };

            CopyComponentConfigs(ref archeType, ref newArcheType);
            ComponentConfigInsertSort(newArcheType.ComponentConfigs, newArcheType.ComponentConfigLength, config);

            if (newArcheType.SharedComponentDataLength > 0)
                CopySharedDataIndexes(ref archeType, ref newArcheType);

            return newArcheType;
        }

        public unsafe static Component_ArcheType_Native AppendComponent(Component_ArcheType_Native archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new ShareComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            if (archeType.ComponentConfigs == null)
            {
                var configs = MemoryHelper.Alloc<ComponentConfig>(1);
                configs[0] = config;
                var sharedIndexes = MemoryHelper.Alloc<ShareComponentDataIndex>(1);
                sharedIndexes[0] = sharedDataIndex;

                return new Component_ArcheType_Native
                {
                    ComponentConfigs = configs,
                    ShareComponentDataIndexes = sharedIndexes,
                    ComponentConfigLength = 1,
                    SharedComponentDataLength = 1
                };
            }

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength + 1),
                ShareComponentDataIndexes = MemoryHelper.Alloc<ShareComponentDataIndex>(archeType.SharedComponentDataLength + 1),
                ComponentConfigLength = archeType.ComponentConfigLength + 1,
                SharedComponentDataLength = archeType.SharedComponentDataLength + 1
            };

            CopyComponentConfigs(ref archeType, ref newArcheType);
            ComponentConfigInsertSort(newArcheType.ComponentConfigs, newArcheType.ComponentConfigLength, config);

            if (archeType.SharedComponentDataLength > 0)
            {
                CopySharedDataIndexes(ref archeType, ref newArcheType);
                ShareComponentDataIndexInsertSort(newArcheType.ShareComponentDataIndexes, newArcheType.SharedComponentDataLength, sharedDataIndex);
            }
            else
                newArcheType.ShareComponentDataIndexes[0] = sharedDataIndex;

            return newArcheType;
        }

        public unsafe static Component_ArcheType_Native ReplaceSharedComponent(Component_ArcheType_Native archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new ShareComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };
            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength),
                ShareComponentDataIndexes = MemoryHelper.Alloc<ShareComponentDataIndex>(archeType.SharedComponentDataLength),
                ComponentConfigLength = archeType.ComponentConfigLength,
                SharedComponentDataLength = archeType.SharedComponentDataLength
            };

            CopyComponentConfigs(ref archeType, ref newArcheType);
            CopySharedDataIndexes(ref archeType, ref newArcheType);

            for (int i = 0; i < newArcheType.SharedComponentDataLength; i++)
            {
                var check = newArcheType.ShareComponentDataIndexes[i];
                if (check.SharedIndex == sharedDataIndex.SharedIndex)
                {
                    newArcheType.ShareComponentDataIndexes[i] = sharedDataIndex;
                    break;
                }
            }

            return newArcheType;
        }

        public unsafe static Component_ArcheType_Native RemoveComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigLength == 1)
                return new Component_ArcheType_Native();

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength - 1),
                ShareComponentDataIndexes = archeType.SharedComponentDataLength > 0
                    ? MemoryHelper.Alloc<ShareComponentDataIndex>(archeType.SharedComponentDataLength)
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

        public unsafe static Component_ArcheType_Native RemoveSharedComponent(Component_ArcheType_Native archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigLength == 1)
                return new Component_ArcheType_Native();

            var newArcheType = new Component_ArcheType_Native
            {
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(archeType.ComponentConfigLength - 1),
                ShareComponentDataIndexes = archeType.SharedComponentDataLength > 1
                    ? MemoryHelper.Alloc<ShareComponentDataIndex>(archeType.SharedComponentDataLength - 1)
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
                    var check = archeType.ShareComponentDataIndexes[i];
                    if (check.SharedIndex != config.SharedIndex)
                        newArcheType.ShareComponentDataIndexes[j++] = check;
                }
            }

            return newArcheType;
        }

        public unsafe void Dispose()
        {
            if (ComponentConfigLength != 0)
                MemoryHelper.Free(ComponentConfigs);
            if (SharedComponentDataLength != 0)
                MemoryHelper.Free(ShareComponentDataIndexes);

            ComponentConfigs = null;
            ShareComponentDataIndexes = null;
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
                for (int i = 0; i < lhs.ComponentConfigLength; i++)
                {
                    if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                        return false;
                }
                for (int i = 0; i < lhs.SharedComponentDataLength; i++)
                {
                    if (lhs.ShareComponentDataIndexes[i] != rhs.ShareComponentDataIndexes[i])
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
                for (int i = 0; i < ComponentConfigLength; i++)
                    hashCode = hashCode * -1521134295 + ComponentConfigs[i].GetHashCode();
                for (int i = 0; i < SharedComponentDataLength; i++)
                    hashCode = hashCode * -1521134295 + ShareComponentDataIndexes[i].GetHashCode();
            }

            return hashCode;
        }

        private unsafe static void ComponentConfigInsertSort(ComponentConfig* configs, int length, ComponentConfig value)
        {
            for (length--; length > 0 && value.CompareTo(configs[length - 1]) < 0; length--)
                configs[length] = configs[length - 1];
            configs[length] = value;
        }

        private unsafe static void ShareComponentDataIndexInsertSort(ShareComponentDataIndex* sharedIndexes, int length, ShareComponentDataIndex value)
        {
            for (length--; length > 0 && value.CompareTo(sharedIndexes[length - 1]) < 0; length--)
                sharedIndexes[length] = sharedIndexes[length - 1];
            sharedIndexes[length] = value;
        }

        private unsafe static void CopyComponentConfigs(ref Component_ArcheType_Native source, ref Component_ArcheType_Native destination)
        {
            MemoryHelper.Copy(source.ComponentConfigs, destination.ComponentConfigs, TypeCache<ComponentConfig>.SizeInBytes * source.ComponentConfigLength);
        }

        private unsafe static void CopySharedDataIndexes(ref Component_ArcheType_Native source, ref Component_ArcheType_Native destination)
        {
            MemoryHelper.Copy(source.ShareComponentDataIndexes, destination.ShareComponentDataIndexes, TypeCache<ShareComponentDataIndex>.SizeInBytes * source.SharedComponentDataLength);
        }
    }
}
