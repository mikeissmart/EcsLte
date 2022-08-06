using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public bool HasEntity(Entity entity)
        {
            Context.AssertContext();

            return InternalHasEntity(entity, out var _, out var _);
        }

        public bool HasEntity(Entity entity, EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            return InternalHasEntity(entity, out var _, out var archeTypeData) &&
                Context.ArcheTypes.GetArcheTypeData(archeType) == archeTypeData;
        }

        public bool HasEntity(Entity entity, EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            if (!InternalHasEntity(entity, out var _, out var archeTypeData))
                return false;

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                if (archeTypeData == filteredArcheTypeDatas[i])
                    return true;
            }

            return false;
        }

        public bool HasEntity(Entity entity, EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            if (!InternalHasEntity(entity, out var _, out var archeTypeData))
                return false;

            var trackedCount = tracker.GetArcheTypeDataEntities(archeTypeData,
                ref _cachedInternalEntities, 0);
            for (var i = 0; i < trackedCount; i++)
            {
                if (_cachedInternalEntities[i] == entity)
                    return true;
            }

            return false;
        }

        public bool HasEntity(Entity entity, EntityQuery query)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                return HasEntity(entity, query.Filter);
            else if (query.Filter == null && query.Tracker != null)
                return HasEntity(entity, query.Tracker);
            else if (query.Filter != null && query.Tracker != null)
            {
                if (!InternalHasEntity(entity, out var _, out var archeTypeData))
                    return false;

                var hasArcheTypeData = false;
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    if (archeTypeData == filteredArcheTypeDatas[i])
                    {
                        hasArcheTypeData = true;
                        break;
                    }
                }
                if (hasArcheTypeData)
                {
                    var trackedCount = query.Tracker.GetArcheTypeDataEntities(archeTypeData,
                        ref _cachedInternalEntities, 0);
                    for (var i = 0; i < trackedCount; i++)
                    {
                        if (_cachedInternalEntities[i] == entity)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
