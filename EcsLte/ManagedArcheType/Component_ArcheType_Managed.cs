using System;
using System.Linq;

namespace EcsLte.ManagedArcheType
{
    public struct Component_ArcheType_Managed : IEquatable<Component_ArcheType_Managed>
    {
        public ComponentConfig[] ComponentConfigs { get; set; }
        public SharedComponentDataIndex[] SharedComponentDataIndexes { get; set; }

        public bool HasComponentConfig(ComponentConfig config)
        {
            if (ComponentConfigs != null)
            {
                for (var i = 0; i < ComponentConfigs.Length; i++)
                {
                    if (ComponentConfigs[i] == config)
                        return true;
                }
            }

            return false;
        }

        public static Component_ArcheType_Managed AppendComponent(Component_ArcheType_Managed archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs == null)
            {
                return new Component_ArcheType_Managed
                {
                    ComponentConfigs = new ComponentConfig[] { config },
                    SharedComponentDataIndexes = null
                };
            }

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
                SharedComponentDataIndexes = archeType.SharedComponentDataIndexes != null
                    ? new SharedComponentDataIndex[archeType.SharedComponentDataIndexes.Length]
                    : null
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            newArcheType.ComponentConfigs[newArcheType.ComponentConfigs.Length - 1] = config;
            Array.Sort(newArcheType.ComponentConfigs);

            if (newArcheType.SharedComponentDataIndexes != null)
                Array.Copy(archeType.SharedComponentDataIndexes, newArcheType.SharedComponentDataIndexes, archeType.SharedComponentDataIndexes.Length);

            return newArcheType;
        }

        public static Component_ArcheType_Managed AppendComponent(Component_ArcheType_Managed archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            if (archeType.ComponentConfigs == null)
            {
                return new Component_ArcheType_Managed
                {
                    ComponentConfigs = new ComponentConfig[] { config },
                    SharedComponentDataIndexes = new SharedComponentDataIndex[] { sharedDataIndex }
                };
            }

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
                SharedComponentDataIndexes = archeType.SharedComponentDataIndexes != null
                    ? new SharedComponentDataIndex[archeType.SharedComponentDataIndexes.Length + 1]
                    : new SharedComponentDataIndex[] { sharedDataIndex }
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            newArcheType.ComponentConfigs[newArcheType.ComponentConfigs.Length - 1] = config;
            Array.Sort(newArcheType.ComponentConfigs);

            if (newArcheType.SharedComponentDataIndexes.Length > 1)
            {
                Array.Copy(archeType.SharedComponentDataIndexes, newArcheType.SharedComponentDataIndexes, archeType.SharedComponentDataIndexes.Length);
                newArcheType.SharedComponentDataIndexes[newArcheType.SharedComponentDataIndexes.Length - 1] = sharedDataIndex;
                Array.Sort(newArcheType.SharedComponentDataIndexes);
            }

            return newArcheType;
        }

        public static Component_ArcheType_Managed ReplaceSharedComponent(Component_ArcheType_Managed archeType, ComponentConfig config, int sharedComponentDataIndex)
        {
            var sharedDataIndex = new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = sharedComponentDataIndex
            };

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length],
                SharedComponentDataIndexes = new SharedComponentDataIndex[archeType.SharedComponentDataIndexes.Length]
            };

            Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
            Array.Copy(archeType.SharedComponentDataIndexes, newArcheType.SharedComponentDataIndexes, archeType.SharedComponentDataIndexes.Length);

            for (var i = 0; i < newArcheType.SharedComponentDataIndexes.Length; i++)
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

        public static Component_ArcheType_Managed RemoveComponent(Component_ArcheType_Managed archeType, ComponentConfig config)
        {
            if (archeType.ComponentConfigs.Length == 1)
                return new Component_ArcheType_Managed();

            var newArcheType = new Component_ArcheType_Managed
            {
                ComponentConfigs = archeType.ComponentConfigs
                    .Where(x => x.ComponentIndex != config.ComponentIndex)
                    .ToArray(),
                SharedComponentDataIndexes = archeType.SharedComponentDataIndexes != null
                    ? new SharedComponentDataIndex[archeType.SharedComponentDataIndexes.Length]
                    : null
            };

            if (archeType.SharedComponentDataIndexes != null)
                Array.Copy(archeType.SharedComponentDataIndexes, newArcheType.SharedComponentDataIndexes, archeType.SharedComponentDataIndexes.Length);

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
                SharedComponentDataIndexes = archeType.SharedComponentDataIndexes.Length > 1
                    ? archeType.SharedComponentDataIndexes
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

            if ((lhs.SharedComponentDataIndexes == null && rhs.SharedComponentDataIndexes != null ||
                lhs.SharedComponentDataIndexes != null && rhs.SharedComponentDataIndexes == null) &&
                lhs.SharedComponentDataIndexes.Length != rhs.SharedComponentDataIndexes.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.ComponentConfigs.Length; i++)
            {
                if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                    return false;
            }
            if (lhs.SharedComponentDataIndexes != null)
            {
                for (var i = 0; i < lhs.SharedComponentDataIndexes.Length; i++)
                {
                    if (lhs.SharedComponentDataIndexes[i] != rhs.SharedComponentDataIndexes[i])
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
                if (SharedComponentDataIndexes != null)
                {
                    for (var i = 0; i < SharedComponentDataIndexes.Length; i++)
                        hashCode = hashCode * -1521134295 + SharedComponentDataIndexes[i].GetHashCode();
                }
            }

            return hashCode;
        }
    }
}
