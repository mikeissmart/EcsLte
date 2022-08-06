using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public Entity DuplicateEntity(Entity srcEntity)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(srcEntity, out var srcEntityData, out var archeTypeData);

            CheckAndAllocEntity(archeTypeData, false,
                out var dupEntity, out var dupEntityData);

            archeTypeData.CopyComponentsSameArcheTypeData(
                srcEntityData.EntityIndex,
                dupEntityData.EntityIndex,
                1);

            return dupEntity;
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref entities, 0);

            return entities;
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref entities, 0);

            return entities;
        }

        public Entity[] DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount)
        {
            var entities = new Entity[0];
            DuplicateEntities(srcEntities, srcStartingIndex, srcCount,
                ref entities, 0);

            return entities;
        }

        public int DuplicateEntities(in Entity[] srcEntities,
            ref Entity[] destEntities)
        {
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, 0);

            return srcEntities.Length;
        }

        public int DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities)
        {
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, 0);

            return srcEntities.Length - srcStartingIndex;
        }

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities)
            => DuplicateEntities(srcEntities, srcStartingIndex, srcCount,
                ref destEntities, 0);

        public int DuplicateEntities(in Entity[] srcEntities,
            ref Entity[] destEntities, int destStartingIndex)
        {
            DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, destStartingIndex);

            return srcEntities.Length;
        }

        public int DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities, int destStartingIndex)
        {
            DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, destStartingIndex);

            return srcEntities.Length - srcStartingIndex;
        }

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities, int destStartingIndex)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            Helper.AssertEntities(srcEntities, srcStartingIndex, srcCount);
            Helper.AssertAndResizeEntities(ref destEntities, destStartingIndex, srcCount);

            for (var i = 0; i < srcCount; i++, srcStartingIndex++, destStartingIndex++)
            {
                AssertNotExistEntity(srcEntities[srcStartingIndex],
                    out var srcEntityData, out var archeTypeData);

                CheckAndAllocEntity(archeTypeData, false,
                    out destEntities[destStartingIndex], out var dupEntityData);

                archeTypeData.CopyComponentsSameArcheTypeData(
                    srcEntityData.EntityIndex,
                    dupEntityData.EntityIndex,
                    1);
            }
        }

        public Entity[] DuplicateEntities(EntityArcheType archeType)
        {
            var entities = new Entity[0];
            DuplicateEntities(archeType, ref entities, 0);

            return entities;
        }

        public int DuplicateEntities(EntityArcheType archeType,
            ref Entity[] destEntities)
            => DuplicateEntities(archeType, ref destEntities, 0);

        public int DuplicateEntities(EntityArcheType archeType,
            ref Entity[] destEntities, int destStartingIndex)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);
            Helper.AssertEntities(destEntities, destStartingIndex);

            return InternalDuplicateArcheTypeData(Context.ArcheTypes.GetArcheTypeData(archeType),
                ref destEntities, destStartingIndex);
        }

        public Entity[] DuplicateEntities(EntityFilter filter)
        {
            var entities = new Entity[0];
            DuplicateEntities(filter, ref entities, 0);

            return entities;
        }

        public int DuplicateEntities(EntityFilter filter,
            ref Entity[] destEntities)
            => DuplicateEntities(filter, ref destEntities, 0);

        public int DuplicateEntities(EntityFilter filter,
            ref Entity[] destEntities, int destStartingIndex)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);
            Helper.AssertEntities(destEntities, destStartingIndex);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var entityIndex = destStartingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                entityIndex += InternalDuplicateArcheTypeData(filteredArcheTypeDatas[i],
                    ref destEntities, entityIndex);
            }

            return entityIndex - destStartingIndex;
        }

        public Entity[] DuplicateEntities(EntityTracker tracker)
        {
            var entities = new Entity[0];
            DuplicateEntities(tracker, ref entities, 0);

            return entities;
        }

        public int DuplicateEntities(EntityTracker tracker,
            ref Entity[] destEntities)
            => DuplicateEntities(tracker, ref destEntities, 0);

        public int DuplicateEntities(EntityTracker tracker,
            ref Entity[] destEntities, int destStartingIndex)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityTracker.AssertEntityTracker(tracker, Context);
            Helper.AssertEntities(destEntities, destStartingIndex);

            var trackedArcheTypeDatas = tracker.CachedArcheTypeDatas;
            var entityIndex = destStartingIndex;
            for (var i = 0; i < trackedArcheTypeDatas.Length; i++)
            {
                entityIndex += InternalDuplicateTracker(tracker, trackedArcheTypeDatas[i],
                    ref destEntities, entityIndex);
            }

            return entityIndex - destStartingIndex;
        }

        public Entity[] DuplicateEntities(EntityQuery query)
        {
            var entities = new Entity[0];
            DuplicateEntities(query, ref entities, 0);

            return entities;
        }

        public int DuplicateEntities(EntityQuery query,
            ref Entity[] destEntities)
            => DuplicateEntities(query, ref destEntities, 0);

        public int DuplicateEntities(EntityQuery query,
            ref Entity[] destEntities, int destStartingIndex)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityQuery.AssertEntityQuery(query, Context);
            Helper.AssertEntities(destEntities, destStartingIndex);

            if (query.Filter != null && query.Tracker == null)
                return DuplicateEntities(query.Filter, ref destEntities, destStartingIndex);
            else if (query.Filter == null && query.Tracker != null)
                return DuplicateEntities(query.Tracker, ref destEntities, destStartingIndex);
            else if (query.Filter != null && query.Tracker != null)
            {
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                var entityIndex = destStartingIndex;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    entityIndex += InternalDuplicateTracker(query.Tracker, filteredArcheTypeDatas[i],
                        ref destEntities, entityIndex);
                }

                return entityIndex - destStartingIndex;
            }

            return 0;
        }
    }
}
