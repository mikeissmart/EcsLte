using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public Entity[] GetEntities()
        {
            var entities = new Entity[0];
            GetEntities(ref entities, 0);

            return entities;
        }

        public int GetEntities(ref Entity[] entities)
            => GetEntities(ref entities, 0);

        public int GetEntities(ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            Helper.AssertArray(entities, startingIndex);

            Context.ArcheTypes.GetAllEntities(ref entities, startingIndex);

            return _entitiesCount;
        }

        public Entity[] GetEntities(EntityArcheType archeType)
        {
            var entities = new Entity[0];
            GetEntities(archeType, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityArcheType archeType, ref Entity[] entities)
            => GetEntities(archeType, ref entities, 0);

        public int GetEntities(EntityArcheType archeType, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            if (archeTypeData.EntityCount() > 0)
            {
                Helper.AssertAndResizeArray(ref entities, startingIndex, archeTypeData.EntityCount());
                archeTypeData.GetAllEntities(ref entities, startingIndex);
            }

            return archeTypeData.EntityCount();
        }

        public Entity[] GetEntities(EntityFilter filter)
        {
            var entities = new Entity[0];
            GetEntities(filter, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityFilter filter, ref Entity[] entities)
            => GetEntities(filter, ref entities, 0);

        public int GetEntities(EntityFilter filter, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);
            Helper.AssertArray(entities, startingIndex);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var entityIndex = startingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.EntityCount() > 0)
                {
                    Helper.ResizeRefArray(ref entities, entityIndex, archeTypeData.EntityCount());
                    entityIndex += archeTypeData.GetAllEntities(ref entities, entityIndex);
                }
            }

            return entityIndex - startingIndex;
        }

        public Entity[] GetEntities(EntityTracker tracker)
        {
            var entities = new Entity[0];
            GetEntities(tracker, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityTracker tracker, ref Entity[] entities)
            => GetEntities(tracker, ref entities, 0);

        public int GetEntities(EntityTracker tracker, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            Helper.AssertArray(entities, startingIndex);

            var trackedArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(tracker.TrackingFilter());
            var entityIndex = startingIndex;
            for (var i = 0; i < trackedArcheTypeDatas.Length; i++)
            {
                entityIndex += InternalGetEntitiesTracker(tracker, trackedArcheTypeDatas[i],
                    ref entities, entityIndex);
            }

            return entityIndex - startingIndex;
        }

        public Entity[] GetEntities(EntityQuery query)
        {
            var entities = new Entity[0];
            GetEntities(query, ref entities, 0);

            return entities;
        }

        public int GetEntities(EntityQuery query, ref Entity[] entities)
            => GetEntities(query, ref entities, 0);

        public int GetEntities(EntityQuery query, ref Entity[] entities, int startingIndex)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                return GetEntities(query.Filter, ref entities, startingIndex);
            else if (query.Filter == null && query.Tracker != null)
                return GetEntities(query.Tracker, ref entities, startingIndex);
            else if (query.Filter != null && query.Tracker != null)
            {
                Helper.AssertArray(entities, startingIndex);

                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                var entityIndex = startingIndex;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    entityIndex += query.Tracker.GetArcheTypeDataEntities(filteredArcheTypeDatas[i],
                        ref entities, entityIndex);
                }

                return entityIndex - startingIndex;
            }

            return 0;
        }

        public EntityArcheType GetArcheType(Entity entity)
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var _, out var archeTypeData);

            return new EntityArcheType(Context, archeTypeData);
        }

        internal EntityData GetEntityData(Entity entity)
            => _entityDatas[entity.Id];
    }
}
