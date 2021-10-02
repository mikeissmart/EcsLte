using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal struct ComponentArcheType
    {
        public static bool IsEmpty(ComponentArcheType archeType)
        {
            return archeType.ComponentPoolIndexes?.Length == 0 &&
                archeType.SharedComponents?.Length == 0;
        }

        public static int CalculateHashCode(ComponentArcheType archeType)
        {
            var hashCode = -1663471673;
            if (archeType.ComponentPoolIndexes != null)
            {
                hashCode = hashCode * -1521134295 + archeType.ComponentPoolIndexes.Length;
                foreach (var index in archeType.ComponentPoolIndexes)
                    hashCode = hashCode * -1521134295 + index.GetHashCode();
            }
            if (archeType.SharedComponents != null)
            {
                hashCode = hashCode * -1521134295 + archeType.SharedComponents.Length;
                foreach (var key in archeType.SharedComponents)
                    hashCode = hashCode * -1521134295 + key.GetHashCode();
            }

            return hashCode;
        }

        internal static ComponentArcheType AppendComponentPoolIndex(int componentPoolIndex)
        {
            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[] { componentPoolIndex },
                SharedComponents = new ISharedComponent[0]
            };

            return result;
        }

        internal static ComponentArcheType AppendComponentPoolIndex(ComponentArcheType archeType, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes =
                archeType.ComponentPoolIndexes == null || archeType.ComponentPoolIndexes.Length == 0
                    ? new int[] { componentPoolIndex }
                    : IndexHelpers
                        .MergeDistinctIndex(archeType.ComponentPoolIndexes, componentPoolIndex);
            result.SharedComponents = archeType.SharedComponents;

            return result;
        }

        internal static ComponentArcheType RemoveComponentPoolIndex(ComponentArcheType archeType, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = archeType.ComponentPoolIndexes == null
                ? new int[0]
                : archeType.ComponentPoolIndexes
                    .Where(x => x != componentPoolIndex)
                    .ToArray();
            result.SharedComponents = archeType.SharedComponents;

            return result;
        }

        internal static ComponentArcheType AppendSharedComponent(ISharedComponent sharedComponent, int componentPoolIndex)
        {
            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[] { componentPoolIndex },
                SharedComponents = new[] { sharedComponent }
            };

            return result;
        }

        internal static ComponentArcheType AppendSharedComponent(
            ComponentArcheType archeType, ISharedComponent sharedComponent, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = IndexHelpers
                .MergeDistinctIndex(archeType.ComponentPoolIndexes, componentPoolIndex);
            HashSet<ISharedComponent> hashShared;
            if (archeType.SharedComponents == null)
                hashShared = new HashSet<ISharedComponent>();
            else
                hashShared = new HashSet<ISharedComponent>(archeType.SharedComponents);
            hashShared.Add(sharedComponent);
            result.SharedComponents = hashShared.OrderBy(x => x.GetHashCode()).ToArray();

            return result;
        }

        internal static ComponentArcheType RemoveSharedComponent(
            ComponentArcheType archeType, ISharedComponent sharedComponent, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = archeType.ComponentPoolIndexes
                .Where(x => x != componentPoolIndex)
                .ToArray();
            result.SharedComponents = archeType.SharedComponents
                .Where(x => !x.Equals(sharedComponent))
                .ToArray();

            return result;
        }

        internal int[] ComponentPoolIndexes { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }
    }
}