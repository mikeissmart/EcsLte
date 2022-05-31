using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal unsafe struct ArcheTypeData
    {
        private ComponentConfigOffset* _configOffsets;

        /// <summary>
        /// [Entity1,Entity2],[Component1,Component2,Component1,Component2]
        /// </summary>
        private byte* _dataBuffer;

        private int _dataBufferSizeInBytes;
        private int _dataBufferComponentsOffset;

        internal ArcheType ArcheType { get; private set; }
        internal int EntityCount { get; private set; }
        internal int EntityCapacity { get; private set; }
        internal int ComponentsSizeInBytes { get; private set; }
        internal ComponentConfig* ManagedConfigs { get; private set; }
        internal int ManagedConfigsLength { get; private set; }
        internal ComponentConfig* UniqueConfigs { get; private set; }
        internal int UniqueConfigsLength { get; private set; }

        internal static ArcheTypeData* Alloc(ArcheType archeType)
        {
            var data = MemoryHelper.Alloc<ArcheTypeData>(1);
            data->Initialize(archeType);

            return data;
        }

        internal static EntityData TransferEntity(
            Entity entity,
            ArcheTypeData* nextArcheTypeData,
            EntityData* allEntityDatas)
        {
            var prevEntityData = allEntityDatas[entity.Id];
            var prevArcheTypeData = prevEntityData.ArcheTypeData;
            var prevComponentsBuffer = prevArcheTypeData->DataBufferToComponents(prevEntityData.EntityIndex);

            var nextEntityData = nextArcheTypeData->AddEntity(entity);
            var nextComponentsBuffer = nextArcheTypeData->DataBufferToComponents(nextEntityData.EntityIndex);

            MemoryHelper.Copy(
                prevComponentsBuffer,
                nextComponentsBuffer,
                nextArcheTypeData->ComponentsSizeInBytes);

            prevArcheTypeData->RemoveTransferedEntity(entity, allEntityDatas);
            allEntityDatas[entity.Id] = nextEntityData;

            return nextEntityData;
        }

        internal static void TransferAllEntities(
            ArcheTypeData* prevArcheTypeData,
            ArcheTypeData* nextArcheTypeData,
            EntityData* allEntityDatas)
        {
            if (prevArcheTypeData->EntityCount == 0)
                return;

            // Copy entities
            nextArcheTypeData->CheckResize(prevArcheTypeData->EntityCount);
            MemoryHelper.Copy(
                prevArcheTypeData->_dataBuffer,
                nextArcheTypeData->_dataBuffer + (nextArcheTypeData->EntityCount * TypeCache<Entity>.SizeInBytes),
                prevArcheTypeData->EntityCount * TypeCache<Entity>.SizeInBytes);

            // Copy components
            var prevComponentsBuffer = prevArcheTypeData->DataBufferToComponents(0);
            var nextComponentsBuffer = nextArcheTypeData->DataBufferToComponents(nextArcheTypeData->EntityCount);
            MemoryHelper.Copy(
                prevComponentsBuffer,
                nextComponentsBuffer,
                prevArcheTypeData->EntityCount * nextArcheTypeData->ComponentsSizeInBytes);

            // Update EntityDatas
            for (int i = 0, entityOffset = 0; i < prevArcheTypeData->EntityCount; i++, entityOffset += TypeCache<Entity>.SizeInBytes)
            {
                var entity = *(Entity*)(prevArcheTypeData->_dataBuffer + entityOffset);
                allEntityDatas[entity.Id] = new EntityData
                {
                    EntityIndex = nextArcheTypeData->EntityCount + i,
                    ArcheTypeData = nextArcheTypeData,
                };
            }

            // Update ArcheTypeDatas
            nextArcheTypeData->EntityCount += prevArcheTypeData->EntityCount;
            prevArcheTypeData->EntityCount = 0;
        }

        internal void PreCheckEntityAllocation(int count)
            => CheckResize(count);

        internal void CopyEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount == 0)
                return;

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    _dataBuffer,
                    entitiesPtr,
                    EntityCount * TypeCache<Entity>.SizeInBytes);
            }
        }

        internal Entity GetEntity(int entityIndex)
            => *(Entity*)(_dataBuffer + (entityIndex * TypeCache<Entity>.SizeInBytes));

        internal EntityData AddEntity(Entity entity)
        {
            CheckResize(1);

            var entityData = new EntityData
            {
                EntityIndex = EntityCount++
            };
            fixed (ArcheTypeData* selfPtr = &this)
            {
                entityData.ArcheTypeData = selfPtr;
            }
            *(Entity*)DataBufferToEntity(entityData.EntityIndex) = entity;

            return entityData;
        }

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas, ManagedComponentPools managedPools)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                // Move last entity to removed entity spot
                var lastEntity = *(Entity*)DataBufferToEntity(EntityCount - 1);
                var lastEntityData = allEntityDatas[lastEntity.Id];
                var lastComponentsPtr = DataBufferToComponents(lastEntityData.EntityIndex);

                var entityComponentsPtr = DataBufferToComponents(entityData.EntityIndex);
                for (var i = 0; i < ManagedConfigsLength; i++)
                {
                    GetComponentConfigOffset(ManagedConfigs[i], out var configOffset);
                    managedPools.GetPool(configOffset.Config).ClearComponent(
                        ConvertToIndex(entityComponentsPtr + configOffset.OffsetInBytes));
                }

                *(Entity*)DataBufferToEntity(entityData.EntityIndex) = lastEntity;
                MemoryHelper.Copy(
                    lastComponentsPtr,
                    entityComponentsPtr,
                    ComponentsSizeInBytes);

                lastEntityData.EntityIndex = entityData.EntityIndex;
                allEntityDatas[lastEntity.Id] = lastEntityData;
            }
            else if (ManagedConfigsLength > 0)
            {
                var entityComponentsPtr = DataBufferToComponents(entityData.EntityIndex);
                for (var i = 0; i < ManagedConfigsLength; i++)
                {
                    GetComponentConfigOffset(ManagedConfigs[i], out var configOffset);
                    managedPools.GetPool(configOffset.Config).ClearComponent(
                        ConvertToIndex(entityComponentsPtr + configOffset.OffsetInBytes));
                }
            }
            EntityCount--;
        }

        internal void ClearAllEntities() => EntityCount = 0;

        internal void CopyBlittableComponentDatasToBuffer(IComponentData[] blittableComponentDatas, byte* buffer)
        {
            for (var i = 0; i < blittableComponentDatas.Length; i++)
            {
                var componentData = blittableComponentDatas[i];
                if (componentData.Config.UnmanagedSizeInBytes != 0)
                {
                    GetComponentConfigOffset(componentData.Config, out var configOffset);
                    componentData.CopyBlittableComponentData(buffer + configOffset.OffsetInBytes);
                }
            }
        }

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IComponent => Marshal.PtrToStructure<TComponent>((IntPtr)DataBufferToComponent(entityData.EntityIndex, config));

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.GetComponent(ConvertToIndex(DataBufferToComponent(entityData.EntityIndex, config)));

        internal IComponent GetComponent(EntityData entityData, ComponentConfig config, IManagedComponentPool managedPool) =>
            managedPool.GetComponent(ConvertToIndex(DataBufferToComponent(entityData.EntityIndex, config)));

        internal TComponent GetComponentOffset<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IComponent => Marshal.PtrToStructure<TComponent>((IntPtr)DataBufferToComponents(entityData.EntityIndex) + configOffset.OffsetInBytes);

        internal TComponent GetComponentOffset<TComponent>(EntityData entityData, ComponentConfigOffset configOffset, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.GetComponent(ConvertToIndex(DataBufferToComponents(entityData.EntityIndex) + configOffset.OffsetInBytes));

        internal byte* GetComponentsPtr(EntityData entityData) => DataBufferToComponents(entityData.EntityIndex);

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var dataBuffer = DataBufferToComponents(0);
            for (int i = 0, componentOffset = configOffset.OffsetInBytes; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                components[i + startingIndex] = Marshal.PtrToStructure<TComponent>((IntPtr)(dataBuffer + componentOffset));
        }

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var dataBuffer = DataBufferToComponents(0);
            for (int i = 0, componentOffset = configOffset.OffsetInBytes; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                components[i + startingIndex] = managedPool.GetComponent(ConvertToIndex(dataBuffer + componentOffset));
        }

        internal IComponent[] GetAllComponents(EntityData entityData, ManagedComponentPools managedPools)
        {
            var components = new IComponent[ArcheType.ComponentConfigLength];
            var dataBuffer = DataBufferToComponents(entityData.EntityIndex);
            for (var i = 0; i < components.Length; i++)
            {
                var configOffset = _configOffsets[i];
                components[i] = configOffset.Config.IsBlittable
                    ? (IComponent)Marshal.PtrToStructure(
                        (IntPtr)(dataBuffer + configOffset.OffsetInBytes),
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex])
                    : managedPools.GetPool(configOffset.Config)
                        .GetComponent(ConvertToIndex(dataBuffer + configOffset.OffsetInBytes));
            }

            return components;
        }

        internal void SetComponent<TComponent>(EntityData entityData, TComponent component, ComponentConfig config)
            where TComponent : IComponent =>
            Marshal.StructureToPtr(component, (IntPtr)DataBufferToComponent(entityData.EntityIndex, config), false);

        internal void SetComponent<TComponent>(EntityData entityData, TComponent component, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.SetComponent(ConvertToIndex(DataBufferToComponent(entityData.EntityIndex, config)), component);

        internal void SetComponentOffset<TComponent>(EntityData entityData, TComponent component, ComponentConfigOffset configOffset)
            where TComponent : IComponent =>
            Marshal.StructureToPtr(component, (IntPtr)(DataBufferToComponents(entityData.EntityIndex) + configOffset.OffsetInBytes), false);

        internal void SetComponentOffset<TComponent>(EntityData entityData, TComponent component, ComponentConfigOffset configOffset, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.SetComponent(ConvertToIndex(DataBufferToComponents(entityData.EntityIndex) + configOffset.OffsetInBytes), component);

        internal void SetAllComponents<TComponent>(TComponent component, ComponentConfig config)
            where TComponent : IComponent
        {
            var dataBuffer = DataBufferToComponent(0, config);
            for (int i = 0, componentOffset = 0; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                Marshal.StructureToPtr(component, (IntPtr)(dataBuffer + componentOffset), false);
        }

        internal void SetAllComponents<TComponent>(TComponent component, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            var dataBuffer = DataBufferToComponent(0, config);
            for (int i = 0, componentOffset = 0; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                managedPool.SetComponent(ConvertToIndex(dataBuffer + componentOffset), component);
        }

        internal void SetComponentAndIndex<TComponent>(EntityData entityData, TComponent component, ComponentConfig config, int componentIndex, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            var ptr = DataBufferToComponent(entityData.EntityIndex, config);
            managedPool.SetComponent(componentIndex, component);
            Marshal.StructureToPtr(componentIndex, (IntPtr)ptr, false);
        }

        internal void SetComponentAndIndex(EntityData entityData, IComponent component, ComponentConfig config, int componentIndex, IManagedComponentPool managedPool)
        {
            var ptr = DataBufferToComponent(entityData.EntityIndex, config);
            managedPool.SetComponent(componentIndex, component);
            Marshal.StructureToPtr(componentIndex, (IntPtr)ptr, false);
        }

        internal void SetComponentsBuffer(EntityData entityData, byte* componentBuffer) => MemoryHelper.Copy(
                componentBuffer,
                DataBufferToComponents(entityData.EntityIndex),
                ComponentsSizeInBytes);

        internal bool GetComponentConfigOffset(ComponentConfig config, out ComponentConfigOffset configOffset)
        {
            configOffset = new ComponentConfigOffset();
            for (var i = 0; i < ArcheType.ComponentConfigLength; i++)
            {
                configOffset = _configOffsets[i];
                if (configOffset.Config == config)
                    return true;
            }

            return false;
        }

        internal void InternalDestroy()
        {
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (_dataBuffer != null)
                MemoryHelper.Free(_dataBuffer);
            _dataBuffer = null;
            ArcheType.Dispose();
            ArcheType = new ArcheType();
            EntityCount = 0;
            EntityCapacity = 0;
            ComponentsSizeInBytes = 0;
            if (ManagedConfigsLength > 0)
            {
                MemoryHelper.Free(ManagedConfigs);
                ManagedConfigs = null;
                ManagedConfigsLength = 0;
            }
            if (UniqueConfigsLength > 0)
            {
                MemoryHelper.Free(UniqueConfigs);
                UniqueConfigs = null;
                UniqueConfigsLength = 0;
            }
        }

        private void Initialize(ArcheType archeType)
        {
            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(archeType.ComponentConfigLength);

            var managedConfigs = new List<ComponentConfig>();
            var uniqueConfigs = new List<ComponentConfig>();
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (config.IsManaged)
                    managedConfigs.Add(config);
                if (config.IsUnique)
                    uniqueConfigs.Add(config);

                _configOffsets[i] = new ComponentConfigOffset
                {
                    Config = config,
                    OffsetInBytes = ComponentsSizeInBytes
                };
                if (config.UnmanagedSizeInBytes == 0)
                    ComponentsSizeInBytes++;
                else
                    ComponentsSizeInBytes += config.UnmanagedSizeInBytes;
            }
            if (managedConfigs.Count > 0)
            {
                ManagedConfigs = MemoryHelper.Alloc<ComponentConfig>(managedConfigs.Count);
                ManagedConfigsLength = managedConfigs.Count;
                for (var i = 0; i < ManagedConfigsLength; i++)
                    ManagedConfigs[i] = managedConfigs[i];
            }
            if (uniqueConfigs.Count > 0)
            {
                UniqueConfigs = MemoryHelper.Alloc<ComponentConfig>(uniqueConfigs.Count);
                UniqueConfigsLength = uniqueConfigs.Count;
                for (var i = 0; i < UniqueConfigsLength; i++)
                    UniqueConfigs[i] = uniqueConfigs[i];
            }
            ArcheType = archeType;
        }

        private void CheckResize(int count)
        {
            if (count > (EntityCapacity - EntityCount))
            {
                int newEntityCapacity;
                if (EntityCapacity == 0 && count == 1)
                    // Could have unique components
                    newEntityCapacity = 1;
                else
                    newEntityCapacity = (int)Math.Pow(2, (int)Math.Log(EntityCapacity + count, 2) + 1);
                var newBufferLengthInBytes = newEntityCapacity *
                    (TypeCache<Entity>.SizeInBytes + ComponentsSizeInBytes);
                var newComponentsOffset = newEntityCapacity * TypeCache<Entity>.SizeInBytes;
                var newBuffer = MemoryHelper.Alloc<byte>(newBufferLengthInBytes);
                if (_dataBuffer != null)
                {
                    MemoryHelper.Copy(
                        _dataBuffer,
                        newBuffer,
                        _dataBufferComponentsOffset);
                    MemoryHelper.Copy(
                        _dataBuffer + _dataBufferComponentsOffset,
                        newBuffer + newComponentsOffset,
                        EntityCapacity * ComponentsSizeInBytes);
                    MemoryHelper.Free(_dataBuffer);
                }
                _dataBuffer = newBuffer;
                _dataBufferSizeInBytes = newBufferLengthInBytes;
                _dataBufferComponentsOffset = newComponentsOffset;
                EntityCapacity = newEntityCapacity;
            }
        }

        private byte* DataBufferToEntity(int entityIndex) => _dataBuffer + entityIndex * TypeCache<Entity>.SizeInBytes;

        private byte* DataBufferToComponents(int entityIndex) => _dataBuffer + (EntityCapacity * TypeCache<Entity>.SizeInBytes) + (entityIndex * ComponentsSizeInBytes);

        private byte* DataBufferToComponent(int entityIndex, ComponentConfig config)
        {
            GetComponentConfigOffset(config, out var configOffset);
            return DataBufferToComponents(entityIndex) + configOffset.OffsetInBytes;
        }

        internal void RemoveTransferedEntity(Entity entity, EntityData* allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                // Move last entity to removed entity spot
                var lastEntity = *(Entity*)DataBufferToEntity(EntityCount - 1);
                var lastEntityData = allEntityDatas[lastEntity.Id];

                Marshal.StructureToPtr(lastEntity, (IntPtr)DataBufferToEntity(entityData.EntityIndex), false);
                MemoryHelper.Copy(
                    DataBufferToComponents(lastEntityData.EntityIndex),
                    DataBufferToComponents(entityData.EntityIndex),
                    ComponentsSizeInBytes);

                lastEntityData.EntityIndex = entityData.EntityIndex;
                allEntityDatas[lastEntity.Id] = lastEntityData;
            }
            EntityCount--;
        }

        private int ConvertToIndex(byte* ptr) =>
            Marshal.PtrToStructure<int>((IntPtr)ptr);
    }
}