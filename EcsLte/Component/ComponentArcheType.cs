using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal struct ComponentArcheType : IEquatable<ComponentArcheType>
    {
        internal static ComponentArcheType AppendComponentPoolIndex(int componentPoolIndex)
        {
            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[] { componentPoolIndex },
                PrimaryComponent = null,
                SharedComponents = new ISharedComponent[0]
            };
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType AppendComponentPoolIndex(ComponentArcheType archeType, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = IndexHelpers
                .MergeDistinctIndexes(archeType.ComponentPoolIndexes, new[] { componentPoolIndex });
            result.PrimaryComponent = archeType.PrimaryComponent;
            result.SharedComponents = archeType.SharedComponents;
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType RemoveComponentPoolIndex(ComponentArcheType archeType, int componentPoolIndex)
        {
            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = archeType.ComponentPoolIndexes
                .Where(x => x != componentPoolIndex)
                .ToArray();
            result.PrimaryComponent = archeType.PrimaryComponent;
            result.SharedComponents = archeType.SharedComponents;
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType SetPrimaryComponent(IPrimaryComponent primaryComponent)
        {
            var primaryComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                .GetComponentPoolIndex(primaryComponent.GetType());

            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[] { primaryComponentPoolIndex },
                PrimaryComponent = primaryComponent,
                SharedComponents = new ISharedComponent[0]
            };
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType SetPrimaryComponent(
            ComponentArcheType archeType, IPrimaryComponent primaryComponent)
        {
            if (archeType.PrimaryComponent != null)
                archeType = RemovePrimaryComponent(archeType);

            var primaryComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                .GetComponentPoolIndex(primaryComponent.GetType());

            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = IndexHelpers
                .MergeDistinctIndexes(archeType.ComponentPoolIndexes, new[] { primaryComponentPoolIndex });
            result.PrimaryComponent = primaryComponent;
            result.SharedComponents = archeType.SharedComponents;
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType RemovePrimaryComponent(ComponentArcheType archeType)
        {
            var result = new ComponentArcheType();
            if (archeType.PrimaryComponent != null)
            {
                var primaryComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                    .GetComponentPoolIndex(archeType.PrimaryComponent.GetType());
                result.ComponentPoolIndexes = archeType.ComponentPoolIndexes
                    .Where(x => x != primaryComponentPoolIndex)
                    .ToArray();
            }
            else
                result.ComponentPoolIndexes = archeType.ComponentPoolIndexes;
            result.PrimaryComponent = null;
            result.SharedComponents = archeType.SharedComponents;
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType AppendSharedComponent(ISharedComponent sharedComponent)
        {
            var sharedComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                .GetComponentPoolIndex(sharedComponent.GetType());

            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[] { sharedComponentPoolIndex },
                PrimaryComponent = null,
                SharedComponents = new[] { sharedComponent }
            };
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType AppendSharedComponent(
            ComponentArcheType archeType, ISharedComponent sharedComponent)
        {
            var sharedComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                .GetComponentPoolIndex(sharedComponent.GetType());

            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = IndexHelpers
                .MergeDistinctIndexes(archeType.ComponentPoolIndexes, new[] { sharedComponentPoolIndex });
            result.PrimaryComponent = archeType.PrimaryComponent;
            var hashShared = new HashSet<ISharedComponent>(archeType.SharedComponents);
            hashShared.Add(sharedComponent);
            result.SharedComponents = hashShared.OrderBy(x => x.GetHashCode()).ToArray();
            result.CalculateHashCode();

            return result;
        }

        internal static ComponentArcheType RemoveSharedComponent(
            ComponentArcheType archeType, ISharedComponent sharedComponent)
        {
            var sharedComponentPoolIndex = EcsLte.ComponentPoolIndexes.Instance
                .GetComponentPoolIndex(sharedComponent.GetType());

            var result = new ComponentArcheType();
            result.ComponentPoolIndexes = archeType.ComponentPoolIndexes
                .Where(x => x != sharedComponentPoolIndex)
                .ToArray();
            result.PrimaryComponent = archeType.PrimaryComponent;
            result.SharedComponents = archeType.SharedComponents
                .Where(x => !x.Equals(sharedComponent))
                .ToArray();
            result.CalculateHashCode();

            return result;
        }

        internal static readonly ComponentArcheType Null = CreateNull();

        private int _hashCode;

        internal int[] ComponentPoolIndexes { get; private set; }
        internal IPrimaryComponent PrimaryComponent { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }

        public static bool operator !=(ComponentArcheType lhs, ComponentArcheType rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(ComponentArcheType lhs, ComponentArcheType rhs)
        {
            return lhs._hashCode == rhs._hashCode;
        }

        public bool Equals(ComponentArcheType other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentArcheType other && this == other;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private static ComponentArcheType CreateNull()
        {
            var result = new ComponentArcheType
            {
                ComponentPoolIndexes = new int[0],
                PrimaryComponent = null,
                SharedComponents = new ISharedComponent[0]
            };
            result.CalculateHashCode();

            return result;
        }

        private void CalculateHashCode()
        {
            _hashCode = -1663471673;
            if (ComponentPoolIndexes != null)
            {
                _hashCode = _hashCode * -1521134295 + ComponentPoolIndexes.Length;
                foreach (var index in ComponentPoolIndexes)
                    _hashCode = _hashCode * -1521134295 + index.GetHashCode();
            }
            if (PrimaryComponent != null)
                _hashCode = _hashCode * -1521134295 + PrimaryComponent.GetHashCode();
            if (SharedComponents != null)
            {
                _hashCode = _hashCode * -1521134295 + SharedComponents.Length;
                foreach (var key in SharedComponents)
                    _hashCode = _hashCode * -1521134295 + key.GetHashCode();
            }
        }
    }
}