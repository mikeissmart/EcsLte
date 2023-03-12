using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public Entity CopyEntityTo(EntityManager srcEntityManager,
            Entity srcEntity)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopyToSameContextException();

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            srcEntityManager.Context.AssertContext();
            srcEntityManager.Context.AssertStructualChangeAvailable();
            srcEntityManager.AssertNotExistEntity(srcEntity,
                out var srcEntityData, out var srcArcheTypeData);

            ChangeVersion.IncVersion(ref _globalVersion);
            InternalCacheDiffContextArcheType(srcArcheTypeData);

            var copyArcheTypeData = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);
            CheckAndAllocEntity(copyArcheTypeData, false,
                out var copyEntity, out var copyEntityData);

            copyArcheTypeData.CopyComponentsFromDifferentArcheTypeDataSameComponents(
                GlobalVersion,
                srcArcheTypeData,
                srcEntityData,
                copyEntityData);

            return copyEntity;
        }

        public Entity[] CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities)
        {
            var entities = new Entity[0];
            CopyEntitiesTo(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref entities, 0);

            return entities;
        }

        public Entity[] CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex)
        {
            var entities = new Entity[0];
            CopyEntitiesTo(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref entities, 0);

            return entities;
        }

        public Entity[] CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount)
        {
            var entities = new Entity[0];
            CopyEntitiesTo(srcEntityManager,
                srcEntities, srcStartingIndex, srcCount,
                ref entities, 0);

            return entities;
        }

        public int CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            ref Entity[] destEntities)
        {
            CopyEntitiesTo(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, 0);

            return srcEntities.Length;
        }

        public int CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities)
        {
            CopyEntitiesTo(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, 0);

            return srcEntities.Length - srcStartingIndex;
        }

        public void CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities)
            => CopyEntitiesTo(srcEntityManager,
                srcEntities, srcStartingIndex, srcCount,
                ref destEntities, 0);

        public int CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            ref Entity[] destEntities, int destStartingIndex)
        {
            CopyEntitiesTo(srcEntityManager,
                srcEntities, 0, srcEntities?.Length ?? 0,
                ref destEntities, destStartingIndex);

            return srcEntities.Length;
        }

        public int CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            ref Entity[] destEntities, int destStartingIndex)
        {
            CopyEntitiesTo(srcEntityManager,
                srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex,
                ref destEntities, destStartingIndex);

            return srcEntities.Length - srcStartingIndex;
        }

        public void CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopyToSameContextException();

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            srcEntityManager.Context.AssertContext();
            Helper.AssertArray(srcEntities, srcStartingIndex, srcCount);
            Helper.AssertAndResizeArray(ref destEntities, destStartingIndex, srcCount);

            if (srcCount > 0)
            {
                var cachedArcheTypes = new Dictionary<ArcheTypeIndex, (ArcheTypeData, ArcheTypeData)>();
                for (var i = 0; i < srcCount; i++, srcStartingIndex++, destStartingIndex++)
                {
                    var srcEntity = srcEntities[srcStartingIndex];
                    srcEntityManager.AssertNotExistEntity(srcEntity,
                        out var srcEntityData, out var srcArcheTypeData);

                    if (!cachedArcheTypes.TryGetValue(srcEntityData.ArcheTypeIndex, out var archeTypeDatas))
                    {
                        archeTypeDatas.Item1 = srcArcheTypeData;
                        InternalCacheDiffContextArcheType(srcArcheTypeData);
                        archeTypeDatas.Item2 = Context.ArcheTypes.GetArcheTypeData(_cachedArcheType);

                        cachedArcheTypes.Add(srcEntityData.ArcheTypeIndex, archeTypeDatas);
                    }
                    CheckAndAllocEntity(archeTypeDatas.Item2, false,
                        out destEntities[destStartingIndex], out var copyEntityData);

                    archeTypeDatas.Item2.CopyComponentsFromDifferentArcheTypeDataSameComponents(
                        GlobalVersion,
                        srcArcheTypeData,
                        srcEntityData,
                        copyEntityData);
                }

                ChangeVersion.IncVersion(ref _globalVersion);
            }
        }

        public Entity[] CopyEntitiesTo(EntityArcheType srcArcheType)
        {
            var entities = new Entity[0];
            CopyEntitiesTo(srcArcheType, ref entities, 0);

            return entities;
        }

        public int CopyEntitiesTo(EntityArcheType srcArcheType,
            ref Entity[] destEntities)
            => CopyEntitiesTo(srcArcheType, ref destEntities, 0);

        public int CopyEntitiesTo(EntityArcheType srcArcheType,
            ref Entity[] destEntities, int destStartingIndex)
        {
            EntityArcheType.AssertEntityArcheType(srcArcheType, srcArcheType?.Context);
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            srcArcheType.Context.AssertContext();
            if (Context == srcArcheType.Context)
                throw new EntityCopyToSameContextException();
            Helper.AssertArray(destEntities, destStartingIndex);

            var srcArcheTypeData = srcArcheType.Context.ArcheTypes.GetArcheTypeData(srcArcheType);
            if (srcArcheTypeData.EntityCount() > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);

                return InternalCopyToArcheTypeData(srcArcheTypeData,
                    ref destEntities, destStartingIndex);
            }

            return 0;
        }

        public Entity[] CopyEntitiesTo(EntityFilter srcFilter)
        {
            var entities = new Entity[0];
            CopyEntitiesTo(srcFilter, ref entities, 0);

            return entities;
        }

        public int CopyEntitiesTo(EntityFilter srcFilter,
            ref Entity[] destEntities)
            => CopyEntitiesTo(srcFilter, ref destEntities, 0);

        public int CopyEntitiesTo(EntityFilter srcFilter,
            ref Entity[] destEntities, int destStartingIndex)
        {
            EntityFilter.AssertEntityFilter(srcFilter, srcFilter?.Context);
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            srcFilter.Context.AssertContext();
            if (Context == srcFilter.Context)
                throw new EntityCopyToSameContextException();
            Helper.AssertArray(destEntities, destStartingIndex);

            var filteredArcheTypeDatas = srcFilter.Context.ArcheTypes.GetArcheTypeDatas(srcFilter);
            var entityIndex = destStartingIndex;
            var incVersion = false;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.EntityCount() > 0)
                {
                    incVersion = true;
                    entityIndex += InternalCopyToArcheTypeData(archeTypeData,
                        ref destEntities, entityIndex);
                }
            }
            if (incVersion)
                ChangeVersion.IncVersion(ref _globalVersion);

            return entityIndex - destStartingIndex;
        }

        public Entity[] CopyAllEntitiesTo(EntityManager srcEntityManager)
        {
            var entities = new Entity[0];
            CopyAllEntitiesTo(srcEntityManager,
                ref entities, 0);

            return entities;
        }

        public int CopyAllEntitiesTo(EntityManager srcEntityManager,
            ref Entity[] destEntities) => CopyAllEntitiesTo(srcEntityManager,
                ref destEntities, 0);

        public int CopyAllEntitiesTo(EntityManager srcEntityManager,
            ref Entity[] destEntities, int destStartingIndex)
        {
            if (srcEntityManager == null)
                throw new ArgumentNullException(nameof(srcEntityManager));
            if (this == srcEntityManager)
                throw new EntityCopyToSameContextException();

            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            srcEntityManager.Context.AssertContext();
            Helper.AssertArray(destEntities, destStartingIndex);

            if (srcEntityManager._entitiesCount > 0)
            {
                var entityIndex = destStartingIndex;
                foreach (var archeTypeDatas in srcEntityManager.Context.ArcheTypes.ArcheTypeDatas)
                {
                    for (var i = 1; i < archeTypeDatas.Count; i++)
                    {
                        entityIndex += InternalCopyToArcheTypeData(archeTypeDatas[i],
                            ref destEntities, entityIndex);
                    }
                }
                ChangeVersion.IncVersion(ref _globalVersion);

                return entityIndex - destStartingIndex;
            }

            return 0;
        }
    }
}
