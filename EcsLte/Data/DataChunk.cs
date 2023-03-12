using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EcsLte.Data
{
    internal unsafe class DataChunk
    {
        internal ArcheTypeData ArcheTypeData;
        internal int ChunkIndex;
        internal int EntityCount;
        internal int EntityAvailableCount => DataManager.PageCapacity - EntityCount;
        internal UnmanagedDataPage<Entity> EntityPage;
        internal IDataPage[] GeneralComponentPages;
        internal IDataPage[] ManagedComponentPages;
        internal ChangeVersion[] GeneralVersions;
        internal ChangeVersion[] ManagedVersions;
        internal bool IsFull => EntityCount == DataManager.PageCapacity;
        internal bool IsEmpty => EntityCount == 0;

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
                EntityPage.GetRange(ref entities, startingIndex, EntityCount);

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
            EntityPage.Set(entity, EntityCount);

            if (clearComponents)
            {
                for (var i = 0; i < GeneralComponentPages.Length; i++)
                    GeneralComponentPages[i].Clear(EntityCount);
                for (var i = 0; i < ManagedComponentPages.Length; i++)
                    ManagedComponentPages[i].Clear(EntityCount);
            }

            return new EntityData
            {
                ArcheTypeIndex = ArcheTypeData.ArcheTypeIndex,
                ChunkIndex = ChunkIndex,
                EntityIndex = EntityCount++
            };
        }

        internal int AddAvailableEntities(ChangeVersion changeVersion, in Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            UpdateComponentVersions(changeVersion);

            return AddAvailableEntitiesNoChangeVersion(entities, startingIndex, count, allEntityDatas, clearComponents);
        }

        internal int AddAvailableEntitiesNoChangeVersion(in Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            var addCount = Math.Min(EntityAvailableCount, count);
            EntityPage.Copy(entities, startingIndex, EntityCount, addCount);

            if (clearComponents)
            {
                for (var i = 0; i < GeneralComponentPages.Length; i++)
                    GeneralComponentPages[i].Clear(EntityCount, addCount);
                for (var i = 0; i < ManagedComponentPages.Length; i++)
                    ManagedComponentPages[i].Clear(EntityCount, addCount);
            }

            for (var i = 0; i < addCount; i++)
            {
                allEntityDatas[entities[startingIndex + i].Id] = new EntityData
                {
                    ArcheTypeIndex = ArcheTypeData.ArcheTypeIndex,
                    ChunkIndex = ChunkIndex,
                    EntityIndex = EntityCount++
                };
            }

            return addCount;
        }

        #endregion

        #region RemoveEntity

        internal void RemoveEntity(Entity removeEntity, EntityData* allEntityDatas)
        {
            var removeEntityData = allEntityDatas[removeEntity.Id];
            if (EntityCount > 1)
            {
                var lastIndex = EntityCount - 1;
                var lastEntity = EntityPage.Get(lastIndex);
                if (removeEntity != lastEntity)
                {
                    for (var i = 0; i < GeneralComponentPages.Length; i++)
                        GeneralComponentPages[i].Move(lastIndex, removeEntityData.EntityIndex);
                    for (var i = 0; i < ManagedComponentPages.Length; i++)
                        ManagedComponentPages[i].Move(lastIndex, removeEntityData.EntityIndex);
                }

                EntityPage.Set(lastEntity, removeEntityData.EntityIndex);
                allEntityDatas[lastEntity.Id] = removeEntityData;
            }
            else
            {
                for (var i = 0; i < ManagedComponentPages.Length; i++)
                    ManagedComponentPages[i].Clear(removeEntityData.EntityIndex);
            }

            EntityCount--;
        }

        internal void RemoveEntityAndFill(Entity removeEntity, DataChunk copyChunk, EntityData* allEntityDatas)
        {
            var copyEntity = copyChunk.EntityPage.Get(copyChunk.EntityCount - 1);
            var removeEntityData = allEntityDatas[removeEntity.Id];
            var copyEntityData = allEntityDatas[copyEntity.Id];

            for (var i = 0; i < GeneralComponentPages.Length; i++)
            {
                GeneralComponentPages[i].Move(copyChunk.GeneralComponentPages[i],
                    copyEntityData.EntityIndex, removeEntityData.EntityIndex);
            }
            for (var i = 0; i < ManagedComponentPages.Length; i++)
            {
                ManagedComponentPages[i].Move(copyChunk.ManagedComponentPages[i],
                    copyEntityData.EntityIndex, removeEntityData.EntityIndex);
            }

            EntityPage.Set(copyEntity, removeEntityData.EntityIndex);
            allEntityDatas[copyEntity.Id] = removeEntityData;

            copyChunk.EntityCount--;
        }

        internal void RemoveAllEntities()
        {
            for (var i = 0; i < ManagedComponentPages.Length; i++)
                ManagedComponentPages[i].Clear(0, EntityCount);

            EntityCount = 0;
        }

        #endregion

        #region GetComponents

        internal int GetAllEntityComponents(int entityIndex, ref IComponent[] components, int startingIndex)
        {
            var componentIndex = startingIndex;

            for (var i = 0; i < GeneralVersions.Length; i++)
                components[componentIndex++] = (IComponent)GeneralComponentPages[i].GetObj(entityIndex);
            for (var i = 0; i < ManagedVersions.Length; i++)
                components[componentIndex++] = (IComponent)ManagedComponentPages[i].GetObj(entityIndex);

            return componentIndex - startingIndex;
        }

        internal IDataPage GetComponentPage(ComponentConfigOffset configOffset)
            => GeneralComponentPages[configOffset.ConfigIndex];

        internal IDataPage GetManagedComponentPage(ComponentConfigOffset configOffset)
            => ManagedComponentPages[configOffset.ConfigIndex];

        #endregion

        #region GetGeneral

        internal IComponent GetComponentObj(int entityIndex, ComponentConfigOffset configOffset)
            => (IComponent)GeneralComponentPages[configOffset.ConfigIndex].GetObj(entityIndex);

        internal ref TComponent GetComponentRef<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => ref ((UnmanagedDataPage<TComponent>)GeneralComponentPages[configOffset.ConfigIndex]).GetRef(entityIndex);

        internal TComponent GetComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => ((UnmanagedDataPage<TComponent>)GeneralComponentPages[configOffset.ConfigIndex]).Get(entityIndex);

        internal int GetAllComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
        {
            ((UnmanagedDataPage<TComponent>)GeneralComponentPages[configOffset.ConfigIndex])
                .GetRange(ref components, startingIndex, EntityCount);

            return EntityCount;
        }

        #endregion

        #region GetManaged

        internal IComponent GetManagedComponentObj(int entityIndex, ComponentConfigOffset configOffset)
            => (IComponent)ManagedComponentPages[configOffset.ConfigIndex].GetObj(entityIndex);

        internal ref TComponent GetManagedComponentRef<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ref ((ManagedDataPage<TComponent>)ManagedComponentPages[configOffset.ConfigIndex]).GetRef(entityIndex);

        internal TComponent GetManagedComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ((ManagedDataPage<TComponent>)ManagedComponentPages[configOffset.ConfigIndex]).Get(entityIndex);

        internal int GetAllManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
        {
            ((ManagedDataPage<TComponent>)ManagedComponentPages[configOffset.ConfigIndex])
                .GetRange(ref components, startingIndex, EntityCount);

            return EntityCount;
        }

        #endregion

        #region SetGeneral

        internal void SetComponent<TComponent>(ChangeVersion changeVersion, int entityIndex, ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetComponent(changeVersion, entityIndex, ArcheTypeData.GetConfigOffset(config), component);

        internal void SetComponent<TComponent>(ChangeVersion changeVersion, int entityIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            GeneralVersions[configOffset.ConfigIndex] = changeVersion;

            ((UnmanagedDataPage<TComponent>)GeneralComponentPages[configOffset.ConfigIndex]).Set(component, entityIndex);
        }

        internal void SetComponentAdapter(ChangeVersion changeVersion, int entityIndex, ComponentConfigOffset configOffset, in IComponent component)
        {
            GeneralVersions[configOffset.ConfigIndex] = changeVersion;

            GeneralComponentPages[configOffset.ConfigIndex].SetObj(component, entityIndex);
        }

        internal void SetComponents<TComponent>(ChangeVersion changeVersion, int startingIndex, int count, ComponentConfigOffset configOffset,
            in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            GeneralVersions[configOffset.ConfigIndex] = changeVersion;

            ((UnmanagedDataPage<TComponent>)GeneralComponentPages[configOffset.ConfigIndex])
                .SetRange(component, startingIndex, count);
        }

        #endregion

        #region SetManaged

        internal void SetManagedComponent<TComponent>(ChangeVersion changeVersion, int entityIndex, ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetManagedComponent(changeVersion, entityIndex, ArcheTypeData.GetConfigOffset(config), component);

        internal void SetManagedComponent<TComponent>(ChangeVersion changeVersion, int entityIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
        {
            ManagedVersions[configOffset.ConfigIndex] = changeVersion;

            ((ManagedDataPage<TComponent>)ManagedComponentPages[configOffset.ConfigIndex]).Set(component, entityIndex);
        }

        internal void SetManagedComponentAdapter(ChangeVersion changeVersion, int entityIndex, ComponentConfigOffset configOffset, in IComponent component)
        {
            ManagedVersions[configOffset.ConfigIndex] = changeVersion;

            ManagedComponentPages[configOffset.ConfigIndex].SetObj(component, entityIndex);
        }

        internal void SetManagedComponents<TComponent>(ChangeVersion changeVersion, int startingIndex, int count, ComponentConfigOffset configOffset,
            in TComponent component)
            where TComponent : IManagedComponent
        {
            ManagedVersions[configOffset.ConfigIndex] = changeVersion;

            ((ManagedDataPage<TComponent>)ManagedComponentPages[configOffset.ConfigIndex])
                .SetRange(component, startingIndex, count);
        }

        #endregion

        #region CopyEntity

        internal void CopyComponentsFromSameArcheTypeData(ChangeVersion changeVersion, int srcEntityIndex, int destEntityIndex)
        {
            UpdateComponentVersions(changeVersion);

            for (var i = 0; i < GeneralVersions.Length; i++)
                GeneralComponentPages[i].Copy(srcEntityIndex, destEntityIndex);
            for (var i = 0; i < ManagedVersions.Length; i++)
                ManagedComponentPages[i].Copy(srcEntityIndex, destEntityIndex);
        }

        internal void CopyComponentsFromDifferentArcheTypeDataSameComponents(ChangeVersion changeVersion, DataChunk srcCunk,
            int srcEntityIndex, int destEntityIndex)
        {
            UpdateComponentVersions(changeVersion);

            for (var i = 0; i < GeneralVersions.Length; i++)
                GeneralComponentPages[i].Copy(srcCunk.GeneralComponentPages[i], srcEntityIndex, destEntityIndex);
            for (var i = 0; i < ManagedVersions.Length; i++)
                ManagedComponentPages[i].Copy(srcCunk.ManagedComponentPages[i], srcEntityIndex, destEntityIndex);
        }

        internal void CopyComponentsFromDifferentDataChunkSameComponents(ChangeVersion changeVersion, DataChunk srcCunk,
            int srcEntityIndex, int destEntityIndex, int count)
        {
            UpdateComponentVersions(changeVersion);

            for (var i = 0; i < GeneralVersions.Length; i++)
                GeneralComponentPages[i].Copy(srcCunk.GeneralComponentPages[i], srcEntityIndex, destEntityIndex, count);
            for (var i = 0; i < ManagedVersions.Length; i++)
                ManagedComponentPages[i].Copy(srcCunk.ManagedComponentPages[i], srcEntityIndex, destEntityIndex, count);
        }

        #endregion

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
                ArcheTypeData.SharedVersions[configOffset.ConfigIndex] = changeVersion;
        }
    }
}
