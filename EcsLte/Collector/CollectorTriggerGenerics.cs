using EcsLte.Utilities;

namespace EcsLte
{
    public partial struct CollectorTrigger
    {
        public static CollectorTrigger Added<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new[] { ComponentIndex<TComponent>.Index },
                RemovedIndexes = new int[0],
                UpdatedIndexes = new int[0]
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = Added<TComponent1>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = Added<TComponent1, TComponent2>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Added<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = Added<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new int[0],
                RemovedIndexes = new[] { ComponentIndex<TComponent>.Index },
                UpdatedIndexes = new int[0]
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = Removed<TComponent1>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = Removed<TComponent1, TComponent2>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Removed<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = Removed<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Updated<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new int[0],
                RemovedIndexes = new int[0],
                UpdatedIndexes = new[] { ComponentIndex<TComponent>.Index }
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Updated<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = Updated<TComponent1>();

            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Updated<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = Updated<TComponent1, TComponent2>();

            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger Updated<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = Updated<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrUpdated<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new[] { ComponentIndex<TComponent>.Index },
                RemovedIndexes = new int[0],
                UpdatedIndexes = new[] { ComponentIndex<TComponent>.Index }
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrUpdated<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = AddedOrUpdated<TComponent1>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrUpdated<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = AddedOrUpdated<TComponent1, TComponent2>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrUpdated<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = AddedOrUpdated<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemoved<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new[] { ComponentIndex<TComponent>.Index },
                RemovedIndexes = new[] { ComponentIndex<TComponent>.Index },
                UpdatedIndexes = new int[0]
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemoved<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = AddedOrRemoved<TComponent1>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemoved<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = AddedOrRemoved<TComponent1, TComponent2>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemoved<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = AddedOrRemoved<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger RemovedOrUpdated<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new int[0],
                RemovedIndexes = new[] { ComponentIndex<TComponent>.Index },
                UpdatedIndexes = new[] { ComponentIndex<TComponent>.Index }
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger RemovedOrUpdated<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = RemovedOrUpdated<TComponent1>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger RemovedOrUpdated<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = RemovedOrUpdated<TComponent1, TComponent2>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger RemovedOrUpdated<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = RemovedOrUpdated<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }
        //

        public static CollectorTrigger AddedOrRemovedOrUpdated<TComponent>()
            where TComponent : IComponent
        {
            var collectorTrigger = new CollectorTrigger
            {
                AddedIndexes = new[] { ComponentIndex<TComponent>.Index },
                RemovedIndexes = new[] { ComponentIndex<TComponent>.Index },
                UpdatedIndexes = new[] { ComponentIndex<TComponent>.Index }
            };
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemovedOrUpdated<TComponent1, TComponent2>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
        {
            var collectorTrigger = AddedOrRemovedOrUpdated<TComponent1>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent2>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemovedOrUpdated<TComponent1, TComponent2, TComponent3>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
        {
            var collectorTrigger = AddedOrRemovedOrUpdated<TComponent1, TComponent2>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent3>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }

        public static CollectorTrigger AddedOrRemovedOrUpdated<TComponent1, TComponent2, TComponent3, TComponent4>()
            where TComponent1 : IComponent
            where TComponent2 : IComponent
            where TComponent3 : IComponent
            where TComponent4 : IComponent
        {
            var collectorTrigger = AddedOrRemovedOrUpdated<TComponent1, TComponent2, TComponent3>();

            collectorTrigger.AddedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.AddedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.RemovedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.RemovedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.UpdatedIndexes = Helpers.MergeDistinctIndex(collectorTrigger.UpdatedIndexes, ComponentIndex<TComponent4>.Index);
            collectorTrigger.GenerateHasCode();

            return collectorTrigger;
        }
    }
}