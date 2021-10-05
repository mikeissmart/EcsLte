using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityGroupData
    {
        internal static int CalculateSharedComponentHashCode(ISharedComponent[] components)
        {
            var componentHashes = components
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);
            var hashCode = -1663471673;
            hashCode = hashCode * -1521134295 + componentHashes.Count();
            foreach (var key in componentHashes)
                hashCode = hashCode * -1521134295 + key.GetHashCode();

            return hashCode;
        }

        internal static EntityGroupData Initialize(EcsContextData contextData, int hashCode,
            ISharedComponent[] sharedComponents,
            ComponentArcheTypeData[] initialArcheTypeDatas)
        {
            var data = ObjectCache<EntityGroupData>.Pop();

            // data._refCount;

            data.ArcheTypeCollection = ComponentArcheTypeDataCollection.Initialize(initialArcheTypeDatas);
            data.ContextData = contextData;
            data.Entities = contextData.CreateEntityCollection();
            data.SharedComponents = sharedComponents;
            data.Watchers = WatcherTable.Initialize();
            data.HashCode = hashCode;

            contextData.AnyArcheTypeDataAdded += data.OnAnyComponentArcheTypeDataAdded;

            for (var i = 0; i < initialArcheTypeDatas.Length; i++)
            {
                var archeTypeData = initialArcheTypeDatas[i];
                archeTypeData.EntityAdded += data.OnEntityComponentAdded;
                archeTypeData.EntityRemoved += data.OnEntityComponentRemoved;
                archeTypeData.EntityUpdated += data.OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved += data.OnComponentArcheTypeDataRemoved;

                archeTypeData.GetEntities()
                    .RunForEachParallel(x => data.Entities[x.Id] = x);
            }

            return data;
        }

        internal static void Uninitialize(EntityGroupData data)
        {
            data._refCount = 0;

            for (var i = 0; i < data.ArcheTypeCollection.ArcheTypeDatas.Length; i++)
            {
                var archeTypeData = data.ArcheTypeCollection.ArcheTypeDatas[i];
                archeTypeData.EntityAdded -= data.OnEntityComponentAdded;
                archeTypeData.EntityRemoved -= data.OnEntityComponentRemoved;
                archeTypeData.EntityUpdated -= data.OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved -= data.OnComponentArcheTypeDataRemoved;
            }
            ComponentArcheTypeDataCollection.Uninitialize(data.ArcheTypeCollection);
            data.ContextData.RemoveEntityCollection(data.Entities);
            WatcherTable.Uninitialize(data.Watchers);
            data.HashCode = 0;

            data.NoRef = null;

            ObjectCache<EntityGroupData>.Push(data);
        }

        private int _refCount;

        internal ComponentArcheTypeDataCollection ArcheTypeCollection { get; private set; }
        internal EcsContextData ContextData { get; private set; }
        internal IEntityCollection Entities { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }
        internal WatcherTable Watchers { get; private set; }
        internal int HashCode { get; private set; }

        internal event RefCountZeroEvent<EntityGroupData> NoRef;

        public void IncRefCount()
        {
            _refCount++;
        }

        public void DecRefCount()
        {
            _refCount--;
            if (_refCount == 0)
                NoRef.Invoke(this);
        }

        #region Events

        private void OnEntityComponentAdded(Entity entity)
        {
            Entities[entity.Id] = entity;
            Watchers.AddedEntity(entity);
        }

        private void OnEntityComponentUpdated(Entity entity)
        {
            if (Entities.HasEntity(entity))
                Watchers.UpdatedEntity(entity);
        }

        private void OnEntityComponentRemoved(Entity entity)
        {
            Entities[entity.Id] = Entity.Null;
            Watchers.RemovedEntity(entity);
        }

        private void OnAnyComponentArcheTypeDataAdded(ComponentArcheTypeData archeTypeData)
        {
            if (SharedComponents != null && archeTypeData.ArcheType.SharedComponents != null &&
                archeTypeData.ArcheType.SharedComponents.SequenceEqual(SharedComponents))
            {
                ArcheTypeCollection.AddComponentArcheTypeData(archeTypeData);
                archeTypeData.EntityAdded += OnEntityComponentAdded;
                archeTypeData.EntityRemoved += OnEntityComponentRemoved;
                archeTypeData.EntityUpdated += OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved += OnComponentArcheTypeDataRemoved;
            }
        }

        private void OnComponentArcheTypeDataRemoved(ComponentArcheTypeData archeTypeData)
        {
            if (ArcheTypeCollection.RemoveComponentArcheTypeData(archeTypeData))
            {
                archeTypeData.EntityAdded -= OnEntityComponentAdded;
                archeTypeData.EntityRemoved -= OnEntityComponentRemoved;
                archeTypeData.EntityUpdated -= OnEntityComponentUpdated;
                archeTypeData.ArcheTypeDataRemoved -= OnComponentArcheTypeDataRemoved;
            }
        }

        #endregion
    }
}