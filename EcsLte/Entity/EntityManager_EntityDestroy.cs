using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void DestroyEntity(Entity entity)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity, out var _, out var archeTypeData);

            DeallocEntity(entity, archeTypeData);
        }

        public void DestroyEntities(in Entity[] entities)
            => DestroyEntities(entities, 0, entities?.Length ?? 0);

        public void DestroyEntities(in Entity[] entities, int startingIndex)
            => DestroyEntities(entities, startingIndex, (entities?.Length ?? 0) - startingIndex);

        public void DestroyEntities(in Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            Helper.AssertEntities(entities, startingIndex, count);

            for (var i = 0; i < count; i++, startingIndex++)
            {
                var entity = entities[startingIndex];
                AssertNotExistEntity(entity, out var _, out var archeTypeData);

                DeallocEntity(entity, archeTypeData);
            }
        }

        public void DestroyEntities(EntityArcheType archeType)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            DealloArcheTypeDataEntities(Context.ArcheTypes.GetArcheTypeData(archeType));
        }

        public void DestroyEntities(EntityFilter filter)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                DealloArcheTypeDataEntities(filteredArcheTypeDatas[i]);
        }

        public void DestroyEntities(EntityTracker tracker)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var trackedArcheTypeDatas = tracker.CachedArcheTypeDatas;
            for (var i = 0; i < trackedArcheTypeDatas.Length; i++)
                InternalDestroyTracker(tracker, trackedArcheTypeDatas[i]);
        }

        public void DestroyEntities(EntityQuery query)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                DestroyEntities(query.Filter);
            else if (query.Filter == null && query.Tracker != null)
                GetEntities(query.Tracker);
            else if (query.Filter != null && query.Tracker != null)
            {
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                    InternalDestroyTracker(query.Tracker, filteredArcheTypeDatas[i]);
            }
        }

        public void DestroyAllEntities()
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();

            _reusableEntitiesCount += Context.ArcheTypes.GetAndClearAllEntities(ref _reusableEntities, _reusableEntitiesCount);
            MemoryHelper.Clear(_entityDatas, _entitiesCount);
            Context.Tracking.AllEntitiesDestroyed();
        }
    }
}
