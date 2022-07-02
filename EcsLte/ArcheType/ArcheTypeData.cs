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
        private int _slotSizeInBytes;

        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal ArcheType ArcheType { get; private set; }
        internal int EntityCount { get; private set; }
        internal int ComponentsSizeInBytes => _slotSizeInBytes;
        internal ComponentConfig* UniqueConfigs { get; private set; }
        internal int UniqueConfigsLength { get; private set; }

        internal ArcheTypeData(ArcheType archeType, ArcheTypeIndex archeIndex) => Initialize(archeType, archeIndex);

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
                prevEntityData.Slot.Buffer,
                nextEntityData.Slot.Buffer,
                nextArcheTypeData._slotSizeInBytes);

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
                    nextLastPage.SlotCount * TypeCache<Entity>.SizeInBytes);
            }
            // Copy prev entities to end of next entities
            //  Overwrite nextLastPage slot count
            MemoryHelper.Copy(
                prevArcheTypeData._entities,
                nextArcheTypeData._entities + nextArcheTypeData.EntityCount,
                prevArcheTypeData.EntityCount * TypeCache<Entity>.SizeInBytes);
            if (nextLastPage.SlotCount > 0)
            {
                // Copy nextLastPage slot count to end of entites
                MemoryHelper.Copy(
                    nextLastPageEntities,
                    nextArcheTypeData._entities + nextArcheTypeData.EntityCount + prevArcheTypeData.EntityCount,
                    nextLastPage.SlotCount * TypeCache<Entity>.SizeInBytes);
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
                prevArcheTypeData._pagesCount * TypeCache<MemoryPage>.SizeInBytes);
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
                    transferSlotCount * (nextArcheTypeData._slotSizeInBytes));
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
                bookManager.ReturnPage(nextLastPage);
            }

            prevArcheTypeData._pagesCount = 0;
            prevArcheTypeData.EntityCount = 0;
        }

        internal void PrecheckEntityAllocation(int count, MemoryBookManager bookManager)
        {
            CheckEntities(count);
            bookManager.AllocatePages(count / _slotsPerPage +
                (count % _slotsPerPage != 0 ? 1 : 0));
        }

        internal void CopyEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount == 0)
                return;

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    _entities,
                    entitiesPtr,
                    EntityCount * TypeCache<Entity>.SizeInBytes);
            }
        }

        internal Entity GetEntity(int entityIndex)
            => _entities[entityIndex];

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

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas, MemoryBookManager bookManager) => RemoveEntityReorder(entity, allEntityDatas, bookManager);

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
                    EntityCount * TypeCache<Entity>.SizeInBytes);
            }
            entitiesCount = EntityCount;

            if (_pagesCount > 0)
            {
                bookManager.ReturnPages(_pages, _pagesCount, 0);
                _pagesCount = 0;
            }
            EntityCount = 0;
        }

        internal void CopyBlittableComponentDatasToBuffer(IComponentData[] blittableComponentDatas, byte* componentsBuffer)
        {
            for (var i = 0; i < blittableComponentDatas.Length; i++)
            {
                var componentData = blittableComponentDatas[i];
                var configOffset = GetComponentConfigOffset(componentData.Config);
                if (componentData.Config.UnmanagedSizeInBytes != 0)
                    componentData.CopyComponentData(componentsBuffer + configOffset.OffsetInBytes);
            }
        }

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : unmanaged, IComponent => GetComponentOffset<TComponent>(entityData, GetComponentConfigOffset(config));

        internal TComponent GetComponentOffset<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IComponent =>
            *(TComponent*)DataBufferToComponent(entityData, configOffset);

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IComponent
        {
            var configOffset = GetComponentConfigOffset(config);
            var componentIndex = 0;
            for (var pageIndex = 0; pageIndex < _pagesCount; pageIndex++)
            {
                var page = _pages[pageIndex];
                var buffer = page.Buffer;
                for (int slotIndex = 0, slotOffset = configOffset.OffsetInBytes; slotIndex < page.SlotCount;
                    slotIndex++,
                    slotOffset += _slotSizeInBytes,
                    componentIndex++)
                {
                    components[componentIndex + startingIndex] = *(TComponent*)(buffer + slotOffset);
                }
            }
        }

        internal IComponent[] GetAllComponents(EntityData entityData)
        {
            var components = new IComponent[ArcheType.ComponentConfigLength];
            for (var i = 0; i < ArcheType.ComponentConfigLength; i++)
            {
                var configOffset = GetComponentConfigOffset(ArcheType.ComponentConfigs[i]);
                components[i] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)DataBufferToComponent(entityData, configOffset),
                    ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
            }

            return components;
        }

        internal void SetComponent<TComponent>(EntityData entityData, TComponent component, ComponentConfig config)
            where TComponent : unmanaged, IComponent => SetComponentOffset(entityData, component, GetComponentConfigOffset(config));

        internal void SetComponentOffset<TComponent>(EntityData entityData, TComponent component, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IComponent => *(TComponent*)DataBufferToComponent(entityData, configOffset) = component;

        internal void SetAllComponents<TComponent>(TComponent component, ComponentConfig config)
            where TComponent : unmanaged, IComponent
        {
            var configOffset = GetComponentConfigOffset(config);
            for (var pageIndex = 0; pageIndex < _pagesCount; pageIndex++)
            {
                var page = _pages[pageIndex];
                var buffer = page.Buffer;
                for (int slotIndex = 0, slotOffset = configOffset.OffsetInBytes; slotIndex < page.SlotCount;
                    slotIndex++,
                    slotOffset += _slotSizeInBytes)
                {
                    *(TComponent*)(buffer + slotOffset) = component;
                }
            }
        }

        internal void SetComponentsBuffer(EntityData entityData, byte* componentsBuffer) => MemoryHelper.Copy(
                componentsBuffer,
                entityData.Slot.Buffer,
                _slotSizeInBytes);

        internal ComponentConfigOffset GetComponentConfigOffset(ComponentConfig config) => _configOffsets[config.ComponentIndex];

        private void Initialize(ArcheType archeType, ArcheTypeIndex archeIndex)
        {
            var uniqueConfigs = new List<ComponentConfig>();

            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(ComponentConfigs.Instance.AllComponentCount);
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                var configOffset = new ComponentConfigOffset
                {
                    Config = config,
                    ConfigIndex = i,
                };

                configOffset.OffsetInBytes = _slotSizeInBytes;
                if (config.UnmanagedSizeInBytes == 0)
                    _slotSizeInBytes++;
                else
                    _slotSizeInBytes += config.UnmanagedSizeInBytes;

                if (config.IsUnique)
                    uniqueConfigs.Add(config);

                _configOffsets[config.ComponentIndex] = configOffset;
            }

            _slotsPerPage = MemoryPage.PageBufferSizeInBytes / _slotSizeInBytes;
            ArcheTypeIndex = archeIndex;
            ArcheType = archeType;

            if (uniqueConfigs.Count > 0)
            {
                UniqueConfigs = MemoryHelper.Alloc<ComponentConfig>(uniqueConfigs.Count);
                UniqueConfigsLength = uniqueConfigs.Count;
                for (var i = 0; i < UniqueConfigsLength; i++)
                    UniqueConfigs[i] = uniqueConfigs[i];
            }
        }

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
            _slotSizeInBytes = 0;
            ArcheType.Dispose();
            ArcheType = new ArcheType();
            EntityCount = 0;
            if (UniqueConfigsLength > 0)
            {
                MemoryHelper.Free(UniqueConfigs);
                UniqueConfigs = null;
                UniqueConfigsLength = 0;
            }
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
                        lastEntityData.Slot.Buffer,
                        entityData.Slot.Buffer,
                        _slotSizeInBytes);

                    allEntityDatas[lastEntity.Id] = entityData;
                }
            }
            var lastPage = GetLastPage();
            lastPage->SlotCount--;
            if (lastPage->SlotCount == 0)
            {
                bookManager.ReturnPage(*lastPage);
                _pagesCount--;
            }
            EntityCount--;
        }

        private void CheckEntities(int count)
        {
            if (count > (_entitiesLength - EntityCount))
            {
                var newLength = (int)Math.Pow(2, (int)Math.Log(_entitiesLength + count, 2) + 1);
                var newBuffer = MemoryHelper.Alloc<Entity>(newLength);
                if (_entities != null)
                {
                    MemoryHelper.Copy(
                        _entities,
                        newBuffer,
                        EntityCount * TypeCache<Entity>.SizeInBytes);
                    MemoryHelper.Free(_entities);
                }
                _entities = newBuffer;
                _entitiesLength = newLength;
            }
        }

        private void CheckPages(int count)
        {
            if (count > (_pagesLength - _pagesCount))
            {
                var newLength = (int)Math.Pow(2, (int)Math.Log(_pagesLength + count, 2) + 1);
                var newBuffer = MemoryHelper.Alloc<MemoryPage>(newLength);
                if (_pages != null)
                {
                    MemoryHelper.Copy(
                        _pages,
                        newBuffer,
                        _pagesCount * TypeCache<MemoryPage>.SizeInBytes);
                    MemoryHelper.Free(_pages);
                }
                _pages = newBuffer;
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
                page.Reset(_slotSizeInBytes, _slotsPerPage);
                _pages[_pagesCount] = page;
                lastPage = &_pages[_pagesCount];
                _pagesCount++;
            }

            return lastPage;
        }

        private byte* DataBufferToComponent(EntityData entityData, ComponentConfig config) => DataBufferToComponent(entityData, GetComponentConfigOffset(config));

        private byte* DataBufferToComponent(EntityData entityData, ComponentConfigOffset configOffset) => entityData.Slot.BlittableBuffer + configOffset.OffsetInBytes;
    }
}
