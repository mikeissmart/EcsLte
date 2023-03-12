using System.Diagnostics;

namespace EcsLte
{
    public partial class EntityManager
    {
        public int EntityCount()
        {
            Context.AssertContext();

            return _entitiesCount;
        }

        public int EntityCount(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            return Context.ArcheTypes.GetArcheTypeData(archeType).EntityCount();
        }

        public int EntityCount(EntityFilter filter)
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);

            var entityCount = 0;
            var archeTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < archeTypeDatas.Length; i++)
                entityCount += archeTypeDatas[i].EntityCount();

            return entityCount;
        }

        public int EntityCount(EntityTracker tracker)
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(tracker.TrackingFilter());
            var entityCount = 0;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                if (tracker.GetDataChunks(filteredArcheTypeDatas[i], out var chunks))
                {
                    for (var j = 0; j < chunks.Count; j++)
                        entityCount += chunks[j].EntityCount;
                }
            }

            return entityCount;
        }

        public int EntityCount(EntityQuery query)
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            var entityCount = 0;
            if (query.Filter != null && query.Tracker == null)
                entityCount = EntityCount(query.Filter);
            else if (query.Filter == null && query.Tracker != null)
                entityCount = EntityCount(query.Tracker);
            else if (query.Filter != null && query.Tracker != null)
            {
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    if (query.Tracker.GetDataChunks(filteredArcheTypeDatas[i], out var chunks))
                    {
                        for (var j = 0; j < chunks.Count; j++)
                            entityCount += chunks[j].EntityCount;
                    }
                }
            }

            return entityCount;
        }
    }
}
