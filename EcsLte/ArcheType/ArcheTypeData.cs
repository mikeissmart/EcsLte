using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EcsLte
{
    public unsafe class ArcheTypeData
    {
        private ComponentConfigOffset* _configOffsets;

        private Entity* _entities;
        private int _entitiesLength;
        private MemoryPage* _pages;
        private int _pagesCount;
        private int _pagesLength;
        private int _slotsPerPage;
        private readonly byte* _sharedComponentsBuffer;
        private readonly int _sharedComponentsSizeInBytes;

        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal ArcheType ArcheType { get; private set; }
        internal int EntityCount { get; private set; }
        internal int ComponentsSizeInBytes { get; private set; }
        internal ComponentConfig[] GeneralConfigs { get; private set; }
        internal ComponentConfig[] SharedConfigs { get; private set; }
        internal ComponentConfig[] UniqueConfigs { get; private set; }

        internal ArcheTypeData(ArcheType archeType, ArcheTypeIndex archeIndex, SharedComponentIndexDictionaries sharedDics)
        {
            var sharedConfigs = new List<ComponentConfigOffset>();
            var generalConfigs = new List<ComponentConfig>();
            var uniqueConfigs = new List<ComponentConfig>();

            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(ComponentConfigs.Instance.AllComponentCount);
            var sharedOffset = 0;
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                var configOffset = new ComponentConfigOffset
                {
                    Config = config,
                    ConfigIndex = i,
                };

                if (config.IsShared)
                {
                    configOffset.OffsetInBytes = sharedOffset;
                    sharedOffset += config.UnmanagedSizeInBytes == 0
                        ? 1
                        : config.UnmanagedSizeInBytes;
                    sharedConfigs.Add(configOffset);
                }
                else
                {
                    configOffset.OffsetInBytes = ComponentsSizeInBytes;
                    ComponentsSizeInBytes += config.UnmanagedSizeInBytes == 0
                        ? 1
                        : config.UnmanagedSizeInBytes;

                    if (config.IsGeneral)
                        generalConfigs.Add(config);
                    else if (config.IsUnique)
                        uniqueConfigs.Add(config);
                    else
                        throw new ArgumentException();
                }

                _configOffsets[config.ComponentIndex] = configOffset;
            }

            if (generalConfigs.Count > 0 || uniqueConfigs.Count > 0)
                _slotsPerPage = MemoryPage.PageBufferSizeInBytes / ComponentsSizeInBytes;
            ArcheTypeIndex = archeIndex;
            ArcheType = archeType;

            SharedConfigs = new ComponentConfig[sharedConfigs.Count];
            if (sharedConfigs.Count > 0)
            {
                _sharedComponentsBuffer = MemoryHelper.Alloc<byte>(sharedOffset);
                _sharedComponentsSizeInBytes = sharedOffset;
                ;
                for (var i = 0; i < SharedConfigs.Length; i++)
                {
                    var configOffset = sharedConfigs[i];
                    SharedConfigs[i] = configOffset.Config;
                    sharedDics.GetSharedIndexDic(configOffset.Config)
                        .GetComponentData(archeType.SharedComponentDataIndexes[i])
                        .CopyComponentData(GetSharedComponentOffsetPtr(configOffset));
                }
            }
            GeneralConfigs = generalConfigs.ToArray();
            UniqueConfigs = uniqueConfigs.ToArray();
        }

        internal static EntityData TransferEntity(
            Entity entity,
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData,
            EntityData* allEntityDatas,
            MemoryBookManager bookManager)
        {
            var prevEntityData = allEntityDatas[entity.Id];
            var nextEntityData = nextArcheTypeData.AddEntity(entity, bookManager);

            MemoryHelper.Copy(
                prevEntityData.ComponentsBuffer,
                nextEntityData.ComponentsBuffer,
                nextArcheTypeData.ComponentsSizeInBytes);

            prevArcheTypeData.RemoveEntityReorder(entity, allEntityDatas, bookManager);
            allEntityDatas[entity.Id] = nextEntityData;

            return nextEntityData;
        }

        internal static void TransferAllEntities(
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData,
            EntityData* allEntityDatas,
            MemoryBookManager bookManager)
        {
            if (prevArcheTypeData.EntityCount == 0)
                return;

            nextArcheTypeData.CheckEntities(prevArcheTypeData.EntityCount);
            nextArcheTypeData.CheckPages(prevArcheTypeData._pagesCount);

            // Make sure nextLastPage isnt null
            var nextLastPage = *nextArcheTypeData.GetLastPageWithFreeSlot(bookManager);

            var nextLastPageEntities = stackalloc Entity[nextLastPage.SlotCount];
            if (nextLastPage.SlotCount > 0)
            {
                // Temp store nextLastPage slot count
                nextArcheTypeData.EntityCount -= nextLastPage.SlotCount;
                MemoryHelper.Copy(
                    nextArcheTypeData._entities + nextArcheTypeData.EntityCount,
                    nextLastPageEntities,
                    nextLastPage.SlotCount);
            }
            // Copy prev entities to end of next entities
            //  Overwrite nextLastPage slot count
            MemoryHelper.Copy(
                prevArcheTypeData._entities,
                nextArcheTypeData._entities + nextArcheTypeData.EntityCount,
                prevArcheTypeData.EntityCount);
            if (nextLastPage.SlotCount > 0)
            {
                // Copy nextLastPage slot count to end of entites
                MemoryHelper.Copy(
                    nextLastPageEntities,
                    nextArcheTypeData._entities + nextArcheTypeData.EntityCount + prevArcheTypeData.EntityCount,
                    nextLastPage.SlotCount);
            }
            for (var i = 0; i < prevArcheTypeData.EntityCount; i++)
            {
                // Update prev entities
                var entity = prevArcheTypeData._entities[i];
                var entityData = &allEntityDatas[entity.Id];
                entityData->ArcheTypeIndex = nextArcheTypeData.ArcheTypeIndex;
                entityData->EntityIndex = nextArcheTypeData.EntityCount + i;
            }
            nextArcheTypeData.EntityCount += prevArcheTypeData.EntityCount;

            // Copy prev pages to end of next pages
            //  Overwrite nextLastPage
            MemoryHelper.Copy(
                prevArcheTypeData._pages,
                nextArcheTypeData._pages + (nextArcheTypeData._pagesCount - 1),
                prevArcheTypeData._pagesCount);
            nextArcheTypeData._pagesCount += prevArcheTypeData._pagesCount - 1;

            // Is now where prevLastPage is
            var prevLastPage = nextArcheTypeData.GetLastPage();

            if (!prevLastPage->IsFull && nextLastPage.SlotCount > 0)
            {
                // Fill prevLastPage with nextLastPage
                var freeSlotCount = prevLastPage->SlotCapacity - prevLastPage->SlotCount;
                var transferSlotCount = Math.Min(freeSlotCount, nextLastPage.SlotCount);
                MemoryHelper.Copy(
                    nextLastPage.GetBuffer(nextLastPage.SlotCount - transferSlotCount),
                    prevLastPage->GetBuffer(prevLastPage->SlotCount),
                    transferSlotCount * nextArcheTypeData.ComponentsSizeInBytes);
                for (var i = 0; i < transferSlotCount; i++)
                {
                    var entity = nextArcheTypeData._entities[nextArcheTypeData.EntityCount + i];
                    var entityData = allEntityDatas[entity.Id];
                    prevLastPage->SetSlot(prevLastPage->SlotCount + i, ref entityData);
                    entityData.EntityIndex = nextArcheTypeData.EntityCount + i;
                    allEntityDatas[entity.Id] = entityData;
                }
                nextArcheTypeData.EntityCount += transferSlotCount;
                prevLastPage->SlotCount += transferSlotCount;
                nextLastPage.SlotCount -= transferSlotCount;
            }

            if (nextLastPage.SlotCount > 0)
            {
                // NextLastPage still has slots left
                //  Append to end of copied prev pages
                for (var i = 0; i < nextLastPage.SlotCount; i++)
                {
                    var entity = nextArcheTypeData._entities[nextArcheTypeData.EntityCount + i];
                    var entityData = &allEntityDatas[entity.Id];
                    entityData->EntityIndex = nextArcheTypeData.EntityCount + i;
                }
                nextArcheTypeData.EntityCount += nextLastPage.SlotCount;
                nextArcheTypeData._pages[nextArcheTypeData._pagesCount] = nextLastPage;
                nextArcheTypeData._pagesCount++;
            }
            else
            {
                // Dont need nextLast page anymore
                bookManager.ReturnPage1(nextLastPage);
            }

            prevArcheTypeData._pagesCount = 0;
            prevArcheTypeData.EntityCount = 0;
        }

        internal void PrecheckEntityAllocation(int count, MemoryBookManager bookManager)
        {
            CheckEntities(count);
            if (_slotsPerPage > 0)
            {
                bookManager.AllocatePages(count / _slotsPerPage +
                    (count % _slotsPerPage != 0 ? 1 : 0));
            }
        }

        internal Entity GetEntity(int entityIndex)
            => _entities[entityIndex];

        internal void GetEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount == 0)
                return;

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    _entities,
                    entitiesPtr,
                    EntityCount);
            }
        }

        internal EntityData AddEntity(Entity entity, MemoryBookManager bookManager)
        {
            PrecheckEntityAllocation(1, bookManager);
            var page = GetLastPageWithFreeSlot(bookManager);

            var entityData = new EntityData
            {
                ArcheTypeIndex = ArcheTypeIndex,
                EntityIndex = EntityCount
            };
            page->SetSlot(page->SlotCount, ref entityData);
            _entities[EntityCount] = entity;
            page->SlotCount++;
            EntityCount++;

            return entityData;
        }

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas, MemoryBookManager bookManager) =>
            RemoveEntityReorder(entity, allEntityDatas, bookManager);

        internal void RemoveAllEntities(EntityData* allEntityDatas, MemoryBookManager bookManager,
            ref Entity[] entities, ref int entitiesCount)
        {
            if (EntityCount == 0)
            {
                entitiesCount = 0;
                return;
            }

            fixed (Entity* entitiesPtr = &entities[0])
            {
                MemoryHelper.Copy(
                    _entities,
                    entitiesPtr,
                    EntityCount);
            }
            entitiesCount = EntityCount;

            if (_pagesCount > 0)
            {
                bookManager.ReturnPages1(_pages, 0, _pagesCount);
                _pagesCount = 0;
            }
            EntityCount = 0;
        }

        internal byte* GetComponentPtr(EntityData entityData, ComponentConfig config)
            => GetComponentOffsetPtr(entityData, GetComponentConfigOffset(config));

        internal byte* GetComponentOffsetPtr(EntityData entityData, ComponentConfigOffset configOffset)
            => DataBufferToComponent(entityData, configOffset);

        internal byte* GetSharedComponentPtr(ComponentConfig config)
            => GetSharedComponentOffsetPtr(GetComponentConfigOffset(config));

        internal byte* GetSharedComponentOffsetPtr(ComponentConfigOffset configOffset)
            => _sharedComponentsBuffer + configOffset.OffsetInBytes;

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
        {
            var configOffset = GetComponentConfigOffset(config);
            var componentIndex = startingIndex;
            for (var pageIndex = 0; pageIndex < _pagesCount; pageIndex++)
            {
                var page = _pages[pageIndex];
                var buffer = page.Buffer;
                for (int slotIndex = 0, slotOffset = configOffset.OffsetInBytes;
                    slotIndex < page.SlotCount;
                    slotIndex++, slotOffset += ComponentsSizeInBytes, componentIndex++)
                {
                    components[componentIndex] = *(TComponent*)(buffer + slotOffset);
                }
            }
        }

        internal void GetAllEntityComponents(EntityData entityData, ref IComponent[] components, int startingIndex)
        {
            var componentIndex = startingIndex;
            for (var i = 0; i < GeneralConfigs.Length; i++, componentIndex++)
            {
                var configOffset = GetComponentConfigOffset(GeneralConfigs[i]);
                components[componentIndex] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)DataBufferToComponent(entityData, configOffset),
                    configOffset.Config.ComponentType);
            }
            for (var i = 0; i < SharedConfigs.Length; i++, componentIndex++)
            {
                var configOffset = GetComponentConfigOffset(SharedConfigs[i]);
                components[componentIndex] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)GetSharedComponentOffsetPtr(configOffset),
                    configOffset.Config.ComponentType);
            }
            for (var i = 0; i < UniqueConfigs.Length; i++, componentIndex++)
            {
                var configOffset = GetComponentConfigOffset(UniqueConfigs[i]);
                components[componentIndex] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)DataBufferToComponent(entityData, configOffset),
                    configOffset.Config.ComponentType);
            }
        }

        internal void GetAllSharedComponents(ref ISharedComponent[] components, int startingIndex)
        {
            for (int i = 0, componentIndex = startingIndex;
                i < SharedConfigs.Length;
                i++, componentIndex++)
            {
                var configOffset = GetComponentConfigOffset(SharedConfigs[i]);
                components[componentIndex] = (ISharedComponent)Marshal.PtrToStructure(
                    (IntPtr)(_sharedComponentsBuffer + configOffset.OffsetInBytes),
                    configOffset.Config.ComponentType);
            }
        }

        internal void CopyComponentDatasToBuffer(IComponentData[] componentDatas, byte* componentsBuffer)
        {
            for (var i = 0; i < componentDatas.Length; i++)
            {
                var componentData = componentDatas[i];
                if (componentData.Config.UnmanagedSizeInBytes != 0)
                    componentData.CopyComponentData(componentsBuffer + GetComponentConfigOffset(componentData.Config).OffsetInBytes);
            }
        }

        internal void SetComponentsFromBuffer(byte* componentsBuffer, EntityData entityData) =>
            MemoryHelper.Copy(
                componentsBuffer,
                entityData.ComponentsBuffer,
                ComponentsSizeInBytes);

        internal void CopyComponents(EntityData srcEntityData, EntityData destEntityData) =>
            MemoryHelper.Copy(
                srcEntityData.ComponentsBuffer,
                destEntityData.ComponentsBuffer,
                ComponentsSizeInBytes);

        internal ComponentConfigOffset GetComponentConfigOffset(ComponentConfig config) => _configOffsets[config.ComponentIndex];

        internal void InternalDestroy()
        {
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (_entities != null)
            {
                MemoryHelper.Free(_entities);
                _entities = null;
            }
            if (_pages != null)
            {
                MemoryHelper.Free(_pages);
                _pages = null;
                _pagesCount = 0;
                _pagesLength = 0;
            }
            _slotsPerPage = 0;
            ComponentsSizeInBytes = 0;
            ArcheType.Dispose();
            ArcheType = new ArcheType();
            EntityCount = 0;
            GeneralConfigs = null;
            SharedConfigs = null;
            UniqueConfigs = null;
        }

        private void RemoveEntityReorder(Entity entity, EntityData* allEntityDatas, MemoryBookManager bookManager)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                var lastEntity = _entities[EntityCount - 1];
                if (entity != lastEntity)
                {
                    // Move last entity to removed entity spot
                    var lastEntityData = allEntityDatas[lastEntity.Id];

                    _entities[entityData.EntityIndex] = lastEntity;
                    MemoryHelper.Copy(
                        lastEntityData.ComponentsBuffer,
                        entityData.ComponentsBuffer,
                        ComponentsSizeInBytes);

                    allEntityDatas[lastEntity.Id] = entityData;
                }
            }
            var lastPage = GetLastPage();
            lastPage->SlotCount--;
            if (lastPage->SlotCount == 0)
            {
                bookManager.ReturnPage1(*lastPage);
                _pagesCount--;
            }
            EntityCount--;
        }

        private void CheckEntities(int count)
        {
            if (count > (_entitiesLength - EntityCount))
            {
                var newLength = (int)Math.Pow(2, (int)Math.Log(_entitiesLength + count, 2) + 1);
                _entities = _entities != null
                    ? MemoryHelper.ReallocCopy1(_entities, _entitiesLength, newLength)
                    : MemoryHelper.Alloc<Entity>(newLength);
                _entitiesLength = newLength;
            }
        }

        private void CheckPages(int count)
        {
            if (count > (_pagesLength - _pagesCount))
            {
                var newLength = (int)Math.Pow(2, (int)Math.Log(_pagesLength + count, 2) + 1);
                _pages = _pages != null
                    ? MemoryHelper.ReallocCopy1(_pages, _pagesLength, newLength)
                    : MemoryHelper.Alloc<MemoryPage>(newLength);
                _pagesLength = newLength;
            }
        }

        private MemoryPage* GetLastPage()
        {
            if (_pages == null || _pagesCount == 0)
                return null;
            return &_pages[_pagesCount - 1];
        }

        private MemoryPage* GetLastPageWithFreeSlot(MemoryBookManager bookManager)
        {
            var lastPage = GetLastPage();
            if (lastPage == null || lastPage->IsFull)
            {
                CheckPages(1);
                var page = bookManager.CheckoutPage();
                page.Reset(ComponentsSizeInBytes, _slotsPerPage);
                _pages[_pagesCount] = page;
                lastPage = &_pages[_pagesCount];
                _pagesCount++;
            }

            return lastPage;
        }

        private byte* DataBufferToComponent(EntityData entityData, ComponentConfig config) => DataBufferToComponent(entityData, GetComponentConfigOffset(config));

        private byte* DataBufferToComponent(EntityData entityData, ComponentConfigOffset configOffset) => entityData.ComponentsBuffer + configOffset.OffsetInBytes;
    }
}
