using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace EcsLte
{
    internal unsafe class ArcheTypeDataChunk : IEquatable<ArcheTypeDataChunk>
    {
        internal static readonly int ChunkMaxCapacity = 10000;

        internal ArcheTypeData ParentArcheTypeData { get; private set; }

        internal int ChunkIndex { get; private set; }
        internal int EntityCount { get; private set; }
        internal int EntityCapacity => ChunkMaxCapacity;
        internal int EntityAvailable => EntityCapacity - EntityCount;
        internal bool IsFull => EntityCapacity == EntityCount;
        internal bool IsEmpty => EntityCount == 0;
        internal ChangeVersion[] GeneralVersions { get; private set; }
        internal ChangeVersion[] ManagedVersions { get; private set; }

        internal ArcheTypeDataChunk(ArcheTypeData parentArcheTypeData, int chunkIndex)
        {
            ParentArcheTypeData = parentArcheTypeData;
            ChunkIndex = chunkIndex;

            GeneralVersions = new ChangeVersion[parentArcheTypeData.GeneralConfigs.Length];
            ManagedVersions = new ChangeVersion[parentArcheTypeData.ManagedConfigs.Length];
        }

        #region TransferEntity

        internal static void TransferEntitySameArcheTypeDifferentChunk(
            Entity entity,
            ArcheTypeDataChunk prevChunk,
            ArcheTypeDataChunk nextChunk,
            EntityData* allEntityDatas)
        {
            var archeTypeData = prevChunk.ParentArcheTypeData;
            var prevEntityData = allEntityDatas[entity.Id];
            var nextEntityData = nextChunk.AddEntityNoChangeVersion(entity, false);

            for (var i = 0; i < archeTypeData.GeneralConfigs.Length; i++)
            {
                archeTypeData.GeneralBuffers[i].CopyFromSame(
                    prevChunk.ChunkIndex,
                    prevEntityData.EntityIndex,
                    nextEntityData.ChunkIndex,
                    nextEntityData.EntityIndex,
                    1);
            }
            for (var i = 0; i < archeTypeData.ManagedConfigs.Length; i++)
            {
                archeTypeData.ManagedPools[i].CopyFromSame(
                    prevChunk.ChunkIndex,
                    prevEntityData.EntityIndex,
                    nextEntityData.ChunkIndex,
                    nextEntityData.EntityIndex,
                    1);
            }

            prevChunk.RemoveEntityFromChunk(entity, allEntityDatas);
            allEntityDatas[entity.Id] = nextEntityData;
        }

        #endregion

        #region GetEntity

        /// <summary>
        /// Doesnt resize ref entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal int GetEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount > 0)
            {
                fixed (Entity* entitiesPtr = &entities[startingIndex])
                {
                    MemoryHelper.Copy(
                        ParentArcheTypeData.Entities + (ChunkIndex * ChunkMaxCapacity),
                        entitiesPtr,
                        EntityCount);
                }
            }

            return EntityCount;
        }

        #endregion

        #region AddEntity

        internal EntityData AddEntity(ChangeVersion changeVersion, Entity entity, bool clearComponents)
        {
            UpdateComponentVersions(changeVersion);

            return AddEntityNoChangeVersion(entity, clearComponents);
        }

        internal EntityData AddEntityNoChangeVersion(Entity entity, bool clearComponents)
        {
            ParentArcheTypeData.Entities[(ChunkIndex * ChunkMaxCapacity) + EntityCount] = entity;

            return new EntityData
            {
                ArcheTypeIndex = ParentArcheTypeData.ArcheTypeIndex,
                ChunkIndex = ChunkIndex,
                EntityIndex = EntityCount++
            };
        }

        internal int AddEntities(ChangeVersion changeVersion, Entity* entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            UpdateComponentVersions(changeVersion);

            return AddEntitiesNoChangeVersion(entities, startingIndex, count, allEntityDatas, clearComponents);
        }

        internal int AddEntitiesNoChangeVersion(Entity* entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            if (clearComponents)
            {
                for (var i = 0; i < ParentArcheTypeData.GeneralConfigs.Length; i++)
                    ParentArcheTypeData.GeneralBuffers[i].ClearComponents(ChunkIndex, EntityCount, count);
                for (var i = 0; i < ParentArcheTypeData.ManagedPools.Length; i++)
                    ParentArcheTypeData.ManagedPools[i].ClearComponents(ChunkIndex, EntityCount, count);
            }

            var addCount = Math.Min(EntityAvailable, count);
            for (var i = 0; i < addCount; i++)
            {
                allEntityDatas[entities[startingIndex + i].Id] = new EntityData
                {
                    ArcheTypeIndex = ParentArcheTypeData.ArcheTypeIndex,
                    ChunkIndex = ChunkIndex,
                    EntityIndex = EntityCount++
                };
            }

            return addCount;
        }

        #endregion

        #region RemoveEntity

        internal void RemoveEntityFromChunk(Entity entity, EntityData* allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                var lastIndex = EntityCount - 1;
                var lastEntity = ParentArcheTypeData.Entities[(ChunkIndex * ChunkMaxCapacity) + lastIndex];
                if (entity != lastEntity)
                {
                    // Move last entity to removed entity index
                    for (var i = 0; i < ParentArcheTypeData.GeneralConfigs.Length; i++)
                    {
                        ParentArcheTypeData.GeneralBuffers[i].CopyFromSame(ChunkIndex, lastIndex,
                            ChunkIndex, entityData.EntityIndex, 1);
                    }
                    for (var i = 0; i < ParentArcheTypeData.ManagedPools.Length; i++)
                    {
                        ParentArcheTypeData.ManagedPools[i].CopyFromSame(ChunkIndex, lastIndex,
                            ChunkIndex, entityData.EntityIndex, 1);
                    }

                    ParentArcheTypeData.Entities[(ChunkIndex * ChunkMaxCapacity) + entityData.EntityIndex] = lastEntity;
                    allEntityDatas[lastEntity.Id].EntityIndex = entityData.EntityIndex;
                }
            }

            EntityCount--;
        }

        internal void RemoveEntityAndCopyFromDifferentChunk(Entity removeEntity,
            Entity copyEntity,
            ArcheTypeDataChunk copyChunk,
            EntityData* allEntityDatas)
        {
            var removeEntityData = allEntityDatas[removeEntity.Id];
            var copyEntityData = allEntityDatas[copyEntity.Id];

            for (var i = 0; i < ParentArcheTypeData.GeneralConfigs.Length; i++)
            {
                ParentArcheTypeData.GeneralBuffers[i].CopyFromSame(
                    copyChunk.ChunkIndex,
                    copyEntityData.EntityIndex,
                    ChunkIndex,
                    removeEntityData.EntityIndex,
                    1);
            }
            for (var i = 0; i < ParentArcheTypeData.ManagedConfigs.Length; i++)
            {
                ParentArcheTypeData.ManagedPools[i].CopyFromSame(
                    copyChunk.ChunkIndex,
                    copyEntityData.EntityIndex,
                    ChunkIndex,
                    removeEntityData.EntityIndex,
                    1);
            }

            ParentArcheTypeData.Entities[(removeEntityData.ChunkIndex * ChunkMaxCapacity) + removeEntityData.EntityIndex] = copyEntity;
            allEntityDatas[copyEntity.Id] = removeEntityData;

            copyChunk.EntityCount--;
        }

        internal void RemoveAllEntities()
        {
            // Clear managed so gc can clean up too
            for (var i = 0; i < ParentArcheTypeData.ManagedConfigs.Length; i++)
            {
                ParentArcheTypeData.ManagedPools[i].ClearComponents(
                    ChunkIndex, 0, EntityCount);
            }

            EntityCount = 0;
        }

        #endregion

        #region GetGeneral

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponent<TComponent>(entityData, ParentArcheTypeData.GetConfigOffset(config));

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => *(TComponent*)ParentArcheTypeData.GeneralBuffers[configOffset.ConfigIndex].PtrComponent(entityData.ChunkIndex, entityData.EntityIndex);

        /// <summary>
        /// Doesnt resize ref
        /// </summary>
        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetAllComponents(ref components, startingIndex, ParentArcheTypeData.GetConfigOffset(config));

        /// <summary>
        /// Doesnt resize ref
        /// </summary>
        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
        {
            if (EntityCount > 0)
            {
                fixed (TComponent* componentPtr = &components[startingIndex])
                {
                    MemoryHelper.Copy(
                        (TComponent*)ParentArcheTypeData.GeneralBuffers[configOffset.ConfigIndex].Buffer,
                        componentPtr,
                        EntityCount);
                }
            }

            return EntityCount;
        }

        #endregion

        #region GetManaged

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetManagedComponent<TComponent>(entityData, ParentArcheTypeData.GetConfigOffset(config));

        internal TComponent GetManagedComponent<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ((ComponentPool<TComponent>)ParentArcheTypeData.ManagedPools[configOffset.ConfigIndex]).GetComponent(entityData.ChunkIndex, entityData.EntityIndex);

        internal ref TComponent GetManagedComponentRef<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IManagedComponent
            => ref GetManagedComponentRef<TComponent>(entityData, ParentArcheTypeData.GetConfigOffset(config));

        internal ref TComponent GetManagedComponentRef<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ref ((ComponentPool<TComponent>)ParentArcheTypeData.ManagedPools[configOffset.ConfigIndex]).GetComponentRef(entityData);

        /// <summary>
        /// Doesnt resize ref
        /// </summary>
        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetAllManagedComponents(ref components, startingIndex, ParentArcheTypeData.GetConfigOffset(config));

        /// <summary>
        /// Doesnt resize ref
        /// </summary>
        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
        {
            if (EntityCount > 0)
            {
                ((ComponentPool<TComponent>)ParentArcheTypeData.ManagedPools[configOffset.ConfigIndex])
                    .GetAllComponents(ref components, startingIndex, EntityCount);
            }

            return EntityCount;
        }

        #endregion

        #region SetGeneral

        internal void SetComponent<TComponent>(ChangeVersion changeVersion, int entityIndex,
            ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetComponent(changeVersion, entityIndex, ParentArcheTypeData.GetConfigOffset(config), component);

        internal void SetComponent<TComponent>(ChangeVersion changeVersion, int entityIndex,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            GeneralVersions[configOffset.ConfigIndex] = changeVersion;

            *(TComponent*)ParentArcheTypeData.GeneralBuffers[configOffset.ConfigIndex]
                .PtrComponent(ChunkIndex, entityIndex) = component;
        }

        internal void SetComponents<TComponent>(ChangeVersion changeVersion, ComponentConfigOffset configOffset,
            in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            GeneralVersions[configOffset.ConfigIndex] = changeVersion;

            var buffer = (TComponent*)ParentArcheTypeData.GeneralBuffers[configOffset.ConfigIndex]
                .PtrChunk(ChunkIndex);
            for (var i = 0; i < EntityCount; i++)
                buffer[i++] = component;
        }

        #endregion

        #region SetManaged

        internal void SetManagedComponent<TComponent>(ChangeVersion changeVersion, int entityIndex,
            ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetManagedComponent(changeVersion, entityIndex, ParentArcheTypeData.GetConfigOffset(config), component);

        internal void SetManagedComponent<TComponent>(ChangeVersion changeVersion, int entityIndex,
            ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
        {
            ManagedVersions[configOffset.ConfigIndex] = changeVersion;

            ((ComponentPool<TComponent>)ParentArcheTypeData.ManagedPools[configOffset.ConfigIndex])
                .SetComponent(ChunkIndex, entityIndex, component);
        }

        internal void SetManagedComponents<TComponent>(ChangeVersion changeVersion, ComponentConfigOffset configOffset,
            in TComponent component)
            where TComponent : IManagedComponent
        {
            ManagedVersions[configOffset.ConfigIndex] = changeVersion;

            ((ComponentPool<TComponent>)ParentArcheTypeData.ManagedPools[configOffset.ConfigIndex])
                .SetComponents(ChunkIndex, EntityCount, component);
        }

        #endregion

        internal void CopyComponentsFrom(ChangeVersion changeVersion, EntityData srcEntityData, EntityData destEntityData)
        {
            UpdateComponentVersions(changeVersion);

            for (var i = 0; i < GeneralVersions.Length; i++)
            {
                ParentArcheTypeData.GeneralBuffers[i].CopyFromSame(srcEntityData.ChunkIndex, srcEntityData.EntityIndex,
                    destEntityData.ChunkIndex, destEntityData.EntityIndex, 1);
            }
            for (var i = 0; i < ManagedVersions.Length; i++)
            {
                ParentArcheTypeData.ManagedPools[i].CopyFromSame(srcEntityData.ChunkIndex, srcEntityData.EntityIndex,
                    destEntityData.ChunkIndex, destEntityData.EntityIndex, 1);
            }
        }

        internal void UpdateComponentVersions(ChangeVersion changeVersion)
        {
            for (var i = 0; i < GeneralVersions.Length; i++)
                GeneralVersions[i] = changeVersion;
            for (var i = 0; i < ManagedVersions.Length; i++)
                ManagedVersions[i] = changeVersion;
        }

        internal void UpdateComponentVersion(ChangeVersion changeVersion, ComponentConfigOffset configOffset)
        {
            if (configOffset.Config.IsGeneral)
                GeneralVersions[configOffset.ConfigIndex] = changeVersion;
            else if (configOffset.Config.IsManaged)
                ManagedVersions[configOffset.ConfigIndex] = changeVersion;
            else
                ParentArcheTypeData.SharedVersions[configOffset.ConfigIndex] = changeVersion;
        }

        #region Equals

        public static bool operator !=(ArcheTypeDataChunk lhs, ArcheTypeDataChunk rhs)
            => !(lhs == rhs);

        public static bool operator ==(ArcheTypeDataChunk lhs, ArcheTypeDataChunk rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.ParentArcheTypeData.ArcheTypeIndex != rhs.ParentArcheTypeData.ArcheTypeIndex ||
                lhs.ChunkIndex != rhs.ChunkIndex)
            {
                return false;
            }

            return true;
        }

        public bool Equals(ArcheTypeDataChunk other)
            => this == other;

        public override bool Equals(object other)
            => other is ArcheTypeDataChunk obj && this == obj;

        public override int GetHashCode()
        {
            return HashCodeHelper.StartHashCode()
                .AppendHashCode(ParentArcheTypeData.ArcheTypeIndex)
                .AppendHashCode(ChunkIndex)
                .HashCode;
        }

        #endregion
    }
}
