using EcsLte.Utilities;

namespace EcsLte
{
    /*public partial struct CollectorTrigger
    {
        public static CollectorTrigger Added<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = AddedNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = AddedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = AddedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = AddedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = RemovedNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = RemovedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = RemovedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = RemovedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Replaced<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = ReplacedNoHashCode(
                ComponentIndex<TComponent>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Replaced<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = ReplacedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Replaced<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = ReplacedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Replaced<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = ReplacedNoHashCode(
                ComponentIndex<TComponent1>.Index,
                ComponentIndex<TComponent2>.Index,
                ComponentIndex<TComponent3>.Index,
                ComponentIndex<TComponent4>.Index
            );
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        internal static CollectorTrigger AddedNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var collectorTrigger = new CollectorTrigger
            {
                Indexes = hashed,
                AddedIndexes = hashed,
                RemovedIndexes = new int[0],
                ReplacedIndexes = new int[0]
            };

            return collectorTrigger;
        }

        internal static CollectorTrigger RemovedNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var collectorTrigger = new CollectorTrigger
            {
                Indexes = hashed,
                AddedIndexes = new int[0],
                RemovedIndexes = hashed,
                ReplacedIndexes = new int[0]
            };

            return collectorTrigger;
        }

        internal static CollectorTrigger ReplacedNoHashCode(params int[] indexes)
        {
            var hashed = IndexHelpers.HashIndexes(indexes);

            var collectorTrigger = new CollectorTrigger
            {
                Indexes = hashed,
                AddedIndexes = new int[0],
                RemovedIndexes = new int[0],
                ReplacedIndexes = hashed
            };

            return collectorTrigger;
        }
    }*/
}