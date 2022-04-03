using System;
using System.Linq;

namespace EcsLte.ManagedArcheType
{
    public struct Component_ArcheType_Managed : IEquatable<Component_ArcheType_Managed>
    {
        public ComponentConfig[] ComponentConfigs { get; set; }
        public ShareComponentDataIndex[] ShareComponentDataIndexes { get; set; }

        public static Component_ArcheType_Managed AppendComponent(Component_ArcheType_Managed archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs == null)
            {
                return new Component_ArcheType_Managed
                {
                    ComponentConfigs = new ComponentConfig[] { config },
                    ShareComponentDataIndexes = null
                };
            }

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
                ShareComponentDataIndexes = archeType.ShareComponentDataIndexes != null
                    ? new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length]
                    : null
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            newArcheType.ComponentConfigs[newArcheType.ComponentConfigs.Length - 1] = config;
            Array.Sort(newArcheType.ComponentConfigs);

            if (newArcheType.ShareComponentDataIndexes != null)
                Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

            return newArcheType;
        }

        public static Component_ArcheType_Managed AppendComponent(Component_ArcheType_Managed archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new ShareComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            if (archeType.ComponentConfigs == null)
            {
                return new Component_ArcheType_Managed
                {
                    ComponentConfigs = new ComponentConfig[] { config },
                    ShareComponentDataIndexes = new ShareComponentDataIndex[] { sharedDataIndex }
                };
            }

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
                ShareComponentDataIndexes = archeType.ShareComponentDataIndexes != null
                    ? new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length + 1]
                    : new ShareComponentDataIndex[] { sharedDataIndex }
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            newArcheType.ComponentConfigs[newArcheType.ComponentConfigs.Length - 1] = config;
            Array.Sort(newArcheType.ComponentConfigs);

            if (newArcheType.ShareComponentDataIndexes.Length > 1)
            {
                Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);
                newArcheType.ShareComponentDataIndexes[newArcheType.ShareComponentDataIndexes.Length - 1] = sharedDataIndex;
                Array.Sort(newArcheType.ShareComponentDataIndexes);
            }

            return newArcheType;
        }

        public static Component_ArcheType_Managed ReplaceSharedComponent(Component_ArcheType_Managed archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new ShareComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length],
                ShareComponentDataIndexes = new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length]
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

            for (var i = 0; i < newArcheType.ShareComponentDataIndexes.Length; i++)
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

        public static Component_ArcheType_Managed RemoveComponent(Component_ArcheType_Managed archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs.Length == 1)
                return new Component_ArcheType_Managed();

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = archeType.ComponentConfigs
                    .Where(x => x.ComponentIndex != config.ComponentIndex)
                    .ToArray(),
                ShareComponentDataIndexes = archeType.ShareComponentDataIndexes != null
                    ? new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length]
                    : null
            };

            if (archeType.ShareComponentDataIndexes != null)
                Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

            return newArcheType;
        }

        public static Component_ArcheType_Managed RemoveSharedComponent(Component_ArcheType_Managed archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs.Length == 1)
                return new Component_ArcheType_Managed();

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = archeType.ComponentConfigs.Length > 1
                    ? archeType.ComponentConfigs
                        .Where(x => x.ComponentIndex != config.ComponentIndex)
                        .ToArray()
                    : null,
                ShareComponentDataIndexes = archeType.ShareComponentDataIndexes.Length > 1
                    ? archeType.ShareComponentDataIndexes
                        .Where(x => x.SharedIndex != config.SharedIndex)
                        .ToArray()
                    : null
            };

            return newArcheType;
        }

        public static bool operator !=(Component_ArcheType_Managed lhs, Component_ArcheType_Managed rhs) => !(lhs == rhs);

        public static bool operator ==(Component_ArcheType_Managed lhs, Component_ArcheType_Managed rhs)
        {
            if ((lhs.ComponentConfigs == null && rhs.ComponentConfigs != null ||
                lhs.ComponentConfigs != null && rhs.ComponentConfigs == null) &&
                lhs.ComponentConfigs.Length != rhs.ComponentConfigs.Length)
            {
                return false;
            }

            if ((lhs.ShareComponentDataIndexes == null && rhs.ShareComponentDataIndexes != null ||
                lhs.ShareComponentDataIndexes != null && rhs.ShareComponentDataIndexes == null) &&
                lhs.ShareComponentDataIndexes.Length != rhs.ShareComponentDataIndexes.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.ComponentConfigs.Length; i++)
            {
                if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                    return false;
            }
            if (lhs.ShareComponentDataIndexes != null)
            {
                for (var i = 0; i < lhs.ShareComponentDataIndexes.Length; i++)
                {
                    if (lhs.ShareComponentDataIndexes[i] != rhs.ShareComponentDataIndexes[i])
                        return false;
                }
            }

            return true;
        }

        public bool Equals(Component_ArcheType_Managed other)
            => this == other;

        public override bool Equals(object other)
            => other is Component_ArcheType_Managed obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = -612338121;
            if (ComponentConfigs != null)
            {
                for (var i = 0; i < ComponentConfigs.Length; i++)
                    hashCode = hashCode * -1521134295 + ComponentConfigs[i].GetHashCode();
                if (ShareComponentDataIndexes != null)
                {
                    for (var i = 0; i < ShareComponentDataIndexes.Length; i++)
                        hashCode = hashCode * -1521134295 + ShareComponentDataIndexes[i].GetHashCode();
                }
            }

            return hashCode;
        }
    }
}
