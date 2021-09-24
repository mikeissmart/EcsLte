using System.Runtime.CompilerServices;
using EcsLte.Utilities;

[assembly: InternalsVisibleTo("EcsLte.PerformanceTest")]

namespace EcsLte
{
    public partial struct Filter
    {
        internal static Filter AllOfComponentIndexes()
        {
            var filter = AllOfNoHashCode(
                ComponentIndexes.Instance.AllComponentIndexes
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AllOf<TComponent>()
            where TComponent : IComponent
        {
            var filter = AllOfNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AllOf<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var filter = AllOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AllOf<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var filter = AllOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AllOf<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var filter = AllOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AnyOf<TComponent>()
            where TComponent : IComponent
        {
            var filter = AnyOfNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AnyOf<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var filter = AnyOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AnyOf<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var filter = AnyOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var filter = AnyOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter NoneOf<TComponent>()
            where TComponent : IComponent
        {
            var filter = NoneOfNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter NoneOf<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var filter = NoneOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter NoneOf<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var filter = NoneOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        public static Filter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var filter = NoneOfNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            filter.CalculateHashCode();

            return filter;
        }

        internal static Filter AllOfNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var filter = new Filter
            {
                Indexes = hashed,
                AllOfIndexes = hashed,
                AnyOfIndexes = new int[0],
                NoneOfIndexes = new int[0]
            };

            return filter;
        }

        internal static Filter AnyOfNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var filter = new Filter
            {
                Indexes = hashed,
                AllOfIndexes = new int[0],
                AnyOfIndexes = hashed,
                NoneOfIndexes = new int[0]
            };

            return filter;
        }

        internal static Filter NoneOfNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var filter = new Filter
            {
                Indexes = hashed,
                AllOfIndexes = new int[0],
                AnyOfIndexes = new int[0],
                NoneOfIndexes = hashed
            };

            return filter;
        }
    }
}