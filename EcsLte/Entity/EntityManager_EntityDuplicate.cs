using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public Entity DuplicateEntity(Entity srcEntity)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(srcEntity, out var srcEntityData, out var archeTypeData);
            ChangeVersion.IncVersion(ref _globalVersion);

            CheckAndAllocEntity(archeTypeData, false,
                out var dupEntity, out var dupEntityData);

            archeTypeData.CopyComponentsFrom(
                GlobalVersion,
                srcEntityData,
                dupEntityData);

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
            Helper.AssertArray(srcEntities, srcStartingIndex, srcCount);
            Helper.AssertAndResizeArray(ref destEntities, destStartingIndex, srcCount);

            if (srcCount > 0)
            {
                for (var i = 0; i < srcCount; i++, srcStartingIndex++, destStartingIndex++)
                {
                    AssertNotExistEntity(srcEntities[srcStartingIndex],
                        out var srcEntityData, out var archeTypeData);

                    CheckAndAllocEntity(archeTypeData, false,
                        out destEntities[destStartingIndex], out var dupEntityData);

                    archeTypeData.CopyComponentsFrom(
                        GlobalVersion,
                        srcEntityData,
                        dupEntityData);
                }

                ChangeVersion.IncVersion(ref _globalVersion);
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
            Helper.AssertArray(destEntities, destStartingIndex);

            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            if (archeTypeData.EntityCount > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);

                return InternalDuplicateArcheTypeData(archeTypeData,
                    ref destEntities, destStartingIndex);
            }

            return 0;
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
            Helper.AssertArray(destEntities, destStartingIndex);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                if (filteredArcheTypeDatas[i].EntityCount > 0)
                {
                    ChangeVersion.IncVersion(ref _globalVersion);
                    break;
                }
            }

            var entityIndex = destStartingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                entityIndex += InternalDuplicateArcheTypeData(filteredArcheTypeDatas[i],
                    ref destEntities, entityIndex);
            }

            return entityIndex - destStartingIndex;
        }
    }
}
