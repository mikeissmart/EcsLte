using System;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal struct ComponentArcheType : IEquatable<ComponentArcheType>
    {
        public static bool IsEmpty(ComponentArcheType archeType)
        {
            return archeType.PoolIndexes.Length == 0;
        }

        internal static ComponentArcheType AppendComponent(IComponent component, ComponentPoolConfig config)
        {
            return new ComponentArcheType
            {
                PoolIndexes = new[] { config.PoolIndex },
                SharedComponents = config.IsShared
                    ? new[] { (ISharedComponent)component }
                    : null
            };
        }

        internal static ComponentArcheType AppendComponent(ComponentArcheType archeType, IComponent component,
            ComponentPoolConfig config)
        {
            if (archeType.PoolIndexes == null)
                return AppendComponent(component, config);

            return new ComponentArcheType
            {
                PoolIndexes = IndexHelpers.MergeDistinctIndex(archeType.PoolIndexes, config.PoolIndex),
                SharedComponents = config.IsShared
                    ? MergeDistinctSharedComponents(archeType.SharedComponents, (ISharedComponent)component)
                    : null
            };
        }

        internal static ComponentArcheType RemoveComponent(ComponentArcheType archeType, IComponent component,
            ComponentPoolConfig config)
        {
            return new ComponentArcheType
            {
                PoolIndexes = archeType.PoolIndexes
                    .Where(x => x != config.PoolIndex)
                    .ToArray(),
                SharedComponents = config.IsShared
                    ? archeType.SharedComponents.Length > 1
                        ? archeType.SharedComponents
                            .Where(x => !x.Equals(component))
                            .ToArray()
                        : null
                    : null
            };
        }

        private static ISharedComponent[] MergeDistinctSharedComponents(ISharedComponent[] sharedComponents,
            ISharedComponent sharedComponent)
        {
            if (sharedComponents == null)
            {
                sharedComponents = new ISharedComponent[1] { sharedComponent };
            }
            else if (!sharedComponents.Any(x => x.Equals(sharedComponent)))
            {
                Array.Resize(ref sharedComponents, sharedComponents.Length + 1);
                sharedComponents[sharedComponents.Length - 1] = sharedComponent;
                Array.Sort(sharedComponents, (x, y) => x.GetHashCode().CompareTo(y.GetHashCode()));
            }

            return sharedComponents;
        }

        internal int[] PoolIndexes { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = -1663471673;
            if (PoolIndexes != null)
            {
                hashCode = hashCode * -1521134295 + PoolIndexes.Length;
                foreach (var index in PoolIndexes)
                    hashCode = hashCode * -1521134295 + index.GetHashCode();
            }

            if (SharedComponents != null)
            {
                hashCode = hashCode * -1521134295 + SharedComponents.Length;
                foreach (var component in SharedComponents)
                    hashCode = hashCode * -1521134295 + component.GetHashCode();
            }

            return hashCode;
        }

        public bool Equals(ComponentArcheType other)
        {
            if (SharedComponents == null && other.SharedComponents == null)
                return true;
            if ((SharedComponents == null || other.SharedComponents == null) ||
                (SharedComponents.Length != other.SharedComponents.Length))
                return false;
            if (PoolIndexes.Length != other.PoolIndexes.Length)
                return false;
            for (int i = 0; i < SharedComponents.Length; i++)
            {
                if (!SharedComponents[i].Equals(other.SharedComponents[i]))
                    return false;
            }
            for (int i = 0; i < PoolIndexes.Length; i++)
            {
                if (PoolIndexes[i] != other.PoolIndexes[i])
                    return false;
            }
            return true;
        }
    }
}