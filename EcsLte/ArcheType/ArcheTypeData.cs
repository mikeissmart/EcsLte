using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal unsafe struct ArcheTypeData
    {
        private ComponentConfigOffset* _configOffsets;
        //private int _dataBufferChangesOffset;
        //private int _dataBufferComponentsOffset;

        /// <summary>
        /// [Entity1,Entity2],
        /// [Entity1Conponent1ChangeVersion,Entity1Conponent2ChangeVersion,Entity2Conponent1ChangeVersion,Entity2Conponent2ChangeVersion],
        /// [Component1,Component2,Component1,Component2]
        /// </summary>
        private Entity* _entityBuffer;
        private int* _versionBuffer;
        private byte* _componentBuffer;

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
            var prevVersionsBuffer = prevArcheTypeData->DataBufferToVersions(prevEntityData.EntityIndex);
            var prevComponentsBuffer = prevArcheTypeData->DataBufferToComponents(prevEntityData.EntityIndex);

            var nextEntityData = nextArcheTypeData->AddEntity(entity);
            var nextVersionsBuffer = nextArcheTypeData->DataBufferToVersions(nextEntityData.EntityIndex);
            var nextComponentsBuffer = nextArcheTypeData->DataBufferToComponents(nextEntityData.EntityIndex);

            MemoryHelper.Copy(
                prevVersionsBuffer,
                nextVersionsBuffer,
                nextArcheTypeData->ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);
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

            nextArcheTypeData->CheckResize(prevArcheTypeData->EntityCount);
            // Copy entities
            var nextEntityBuffer = nextArcheTypeData->DataBufferToEntity(nextArcheTypeData->EntityCount);
            MemoryHelper.Copy(
                prevArcheTypeData->_entityBuffer,
                nextEntityBuffer,
                prevArcheTypeData->EntityCount * TypeCache<Entity>.SizeInBytes);

            // Copy componentVersions
            var nextComponentsVersionBuffer = nextArcheTypeData->DataBufferToVersions(nextArcheTypeData->EntityCount);
            MemoryHelper.Copy(
                prevArcheTypeData->_versionBuffer,
                nextComponentsVersionBuffer,
                prevArcheTypeData->EntityCount * nextArcheTypeData->ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);

            // Copy components
            var nextComponentsBuffer = nextArcheTypeData->DataBufferToComponents(nextArcheTypeData->EntityCount);
            MemoryHelper.Copy(
                prevArcheTypeData->_componentBuffer,
                nextComponentsBuffer,
                prevArcheTypeData->EntityCount * nextArcheTypeData->ComponentsSizeInBytes);

            // Update EntityDatas
            var entityPtr = prevArcheTypeData->_entityBuffer;
            for (int i = 0; i < prevArcheTypeData->EntityCount; i++)
            {
                allEntityDatas[entityPtr[i].Id] = new EntityData
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
                    _entityBuffer,
                    entitiesPtr,
                    EntityCount * TypeCache<Entity>.SizeInBytes);
            }
        }

        internal Entity GetEntity(int entityIndex)
            => *DataBufferToEntity(entityIndex);

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
            *DataBufferToEntity(entityData.EntityIndex) = entity;

            return entityData;
        }

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas, ManagedComponentPools managedPools)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                // Move last entity to removed entity spot
                var lastEntity = *DataBufferToEntity(EntityCount - 1);
                var lastEntityData = allEntityDatas[lastEntity.Id];
                var lastComponentsVersionPtr = DataBufferToVersions(lastEntityData.EntityIndex);
                var lastComponentsPtr = DataBufferToComponents(lastEntityData.EntityIndex);

                var entityComponentsVersionPtr = DataBufferToVersions(entityData.EntityIndex);
                var entityComponentsPtr = DataBufferToComponents(entityData.EntityIndex);
                for (var i = 0; i < ManagedConfigsLength; i++)
                {
                    GetComponentConfigOffset(ManagedConfigs[i], out var configOffset);
                    managedPools.GetPool(configOffset.Config).ClearComponent(
                        ConvertToInt(entityComponentsPtr + configOffset.OffsetInBytes));
                }

                *DataBufferToEntity(entityData.EntityIndex) = lastEntity;
                MemoryHelper.Copy(
                    lastComponentsVersionPtr,
                    entityComponentsVersionPtr,
                    ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);
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
                        ConvertToInt(entityComponentsPtr + configOffset.OffsetInBytes));
                }
            }
            EntityCount--;
        }

        internal void ClearAllEntities() => EntityCount = 0;

        internal void CopyBlittableComponentDatasToBuffer(IComponentData[] blittableComponentDatas, int* versionsBuffer, byte* componentsBuffer)
        {
            for (var i = 0; i < blittableComponentDatas.Length; i++)
            {
                var componentData = blittableComponentDatas[i];
                GetComponentConfigOffset(componentData.Config, out var configOffset);
                if (componentData.Config.UnmanagedSizeInBytes != 0)
                    componentData.CopyBlittableComponentData(componentsBuffer + configOffset.OffsetInBytes);
                versionsBuffer[i] = 1;
            }
        }

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config)
            where TComponent : IComponent => Marshal.PtrToStructure<TComponent>((IntPtr)DataBufferToComponent(entityData.EntityIndex, config));

        internal TComponent GetComponent<TComponent>(EntityData entityData, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.GetComponent(ConvertToInt(DataBufferToComponent(entityData.EntityIndex, config)));

        internal IComponent GetComponent(EntityData entityData, ComponentConfig config, IManagedComponentPool managedPool) =>
            managedPool.GetComponent(ConvertToInt(DataBufferToComponent(entityData.EntityIndex, config)));

        internal TComponent GetComponentOffset<TComponent>(EntityData entityData, ComponentConfigOffset configOffset)
            where TComponent : IComponent =>
            Marshal.PtrToStructure<TComponent>((IntPtr)DataBufferToComponent(entityData.EntityIndex, configOffset));

        internal TComponent GetComponentOffset<TComponent>(EntityData entityData, ComponentConfigOffset configOffset, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent =>
            managedPool.GetComponent(ConvertToInt(DataBufferToComponent(entityData.EntityIndex, configOffset)));

        internal byte* GetComponentsPtr(EntityData entityData) => DataBufferToComponents(entityData.EntityIndex);

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var componentsBuffer = (IntPtr)DataBufferToComponents(0);
            for (int i = 0, componentOffset = configOffset.OffsetInBytes; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                components[i + startingIndex] = Marshal.PtrToStructure<TComponent>(componentsBuffer + componentOffset);
        }

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var componentsBuffer = (IntPtr)DataBufferToComponents(0);
            for (int i = 0, componentOffset = configOffset.OffsetInBytes; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
                components[i + startingIndex] = managedPool.GetComponent(ConvertToInt(componentsBuffer + componentOffset));
        }

        internal IComponent[] GetAllComponents(EntityData entityData, ManagedComponentPools managedPools)
        {
            var components = new IComponent[ArcheType.ComponentConfigLength];
            var componentsBuffer = (IntPtr)DataBufferToComponents(entityData.EntityIndex);
            for (var i = 0; i < components.Length; i++)
            {
                var configOffset = _configOffsets[i];
                components[i] = configOffset.Config.IsBlittable
                    ? (IComponent)Marshal.PtrToStructure(
                        componentsBuffer + configOffset.OffsetInBytes,
                        ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex])
                    : managedPools.GetPool(configOffset.Config)
                        .GetComponent(ConvertToInt(componentsBuffer + configOffset.OffsetInBytes));
            }

            return components;
        }

        internal void SetComponent<TComponent>(EntityData entityData, TComponent component, ComponentConfig config)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            SetComponentOffset(entityData, component, configOffset);
        }

        internal void SetComponent<TComponent>(EntityData entityData, TComponent component, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            SetComponentOffset(entityData, component, configOffset, managedPool);
        }

        internal void SetComponentOffset<TComponent>(EntityData entityData, TComponent component, ComponentConfigOffset configOffset)
            where TComponent : IComponent
        {
            Marshal.StructureToPtr(component, (IntPtr)DataBufferToComponent(entityData.EntityIndex, configOffset), false);
            IncComponentVersion(entityData.EntityIndex, configOffset);
        }

        internal void SetComponentOffset<TComponent>(EntityData entityData, TComponent component, ComponentConfigOffset configOffset, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            managedPool.SetComponent(ConvertToInt(DataBufferToComponent(entityData.EntityIndex, configOffset)), component);
            IncComponentVersion(entityData.EntityIndex, configOffset);
        }

        internal void SetAllComponents<TComponent>(TComponent component, ComponentConfig config)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var componentsBuffer = (IntPtr)DataBufferToComponent(0, configOffset);
            var versionsBuffer = DataBufferToVersion(0, configOffset);
            for (int i = 0, componentOffset = 0; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
            {
                Marshal.StructureToPtr(component, componentsBuffer + componentOffset, false);
                versionsBuffer[i * ArcheType.ComponentConfigLength]++;
            }
        }

        internal void SetAllComponents<TComponent>(TComponent component, ComponentConfig config, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            var componentsBuffer = (IntPtr)DataBufferToComponent(0, configOffset);
            var versionsBuffer = DataBufferToVersion(0, configOffset);
            for (int i = 0, componentOffset = 0; i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
            {
                managedPool.SetComponent(ConvertToInt(componentsBuffer + componentOffset), component);
                versionsBuffer[i * ArcheType.ComponentConfigLength]++;
            }
        }

        internal void SetComponentAndIndex<TComponent>(EntityData entityData, TComponent component, ComponentConfig config, int componentIndex, ManagedComponentPool<TComponent> managedPool)
            where TComponent : IComponent
        {
            GetComponentConfigOffset(config, out var configOffset);
            managedPool.SetComponent(componentIndex, component);
            Marshal.StructureToPtr(componentIndex, (IntPtr)DataBufferToComponent(entityData.EntityIndex, configOffset), false);
            IncComponentVersion(entityData.EntityIndex, configOffset);
        }

        internal void SetComponentAndIndex(EntityData entityData, IComponent component, ComponentConfig config, int componentIndex, IManagedComponentPool managedPool)
        {
            GetComponentConfigOffset(config, out var configOffset);
            managedPool.SetComponent(componentIndex, component);
            Marshal.StructureToPtr(componentIndex, (IntPtr)DataBufferToComponent(entityData.EntityIndex, configOffset), false);
            IncComponentVersion(entityData.EntityIndex, configOffset);
        }

        internal void SetComponentsBuffer(EntityData entityData, int* versionsBuffer, byte* componentsBuffer)
        {
            MemoryHelper.Copy(
                versionsBuffer,
                DataBufferToVersions(entityData.EntityIndex),
                ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);
            MemoryHelper.Copy(
                componentsBuffer,
                DataBufferToComponents(entityData.EntityIndex),
                ComponentsSizeInBytes);
        }

        internal int* GetVersionsPtr(EntityData entityData) => DataBufferToVersions(entityData.EntityIndex);

        internal int GetVersion(EntityData entityData, ComponentConfig config)
        {
            GetComponentConfigOffset(config, out var configOffset);
            return GetVersion(entityData, configOffset);
        }

        internal int GetVersion(EntityData entityData, ComponentConfigOffset configOffset) =>
            *DataBufferToVersion(entityData.EntityIndex, configOffset);

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
            if (_entityBuffer != null)
            {
                MemoryHelper.Free(_entityBuffer);
                _entityBuffer = null;
                MemoryHelper.Free(_versionBuffer);
                _versionBuffer = null;
                MemoryHelper.Free(_componentBuffer);
                _componentBuffer = null;
            }
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
                    OffsetInBytes = ComponentsSizeInBytes,
                    ConfigIndex = i
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

                var newEntityBuffer = MemoryHelper.Alloc<Entity>(newEntityCapacity);
                var newVersionBuffer = MemoryHelper.Alloc<int>(newEntityCapacity * ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);
                var newComponentBuffer = MemoryHelper.Alloc<byte>(newEntityCapacity * ComponentsSizeInBytes);
                if (_entityBuffer != null)
                {
                    MemoryHelper.Copy(
                        _entityBuffer,
                        newEntityBuffer,
                        EntityCapacity * TypeCache<Entity>.SizeInBytes);
                    MemoryHelper.Copy(
                        _versionBuffer,
                        newVersionBuffer,
                        EntityCapacity * ArcheType.ComponentConfigLength * TypeCache<int>.SizeInBytes);
                    MemoryHelper.Copy(
                        _componentBuffer,
                        newComponentBuffer,
                        EntityCapacity * ComponentsSizeInBytes);
                    MemoryHelper.Free(_entityBuffer);
                    MemoryHelper.Free(_versionBuffer);
                    MemoryHelper.Free(_componentBuffer);
                }
                _entityBuffer = newEntityBuffer;
                _versionBuffer = newVersionBuffer;
                _componentBuffer = newComponentBuffer;

                EntityCapacity = newEntityCapacity;
            }
        }

        private Entity* DataBufferToEntity(int entityIndex) => _entityBuffer + entityIndex;

        private int* DataBufferToVersions(int entityIndex) => _versionBuffer + (entityIndex * ArcheType.ComponentConfigLength);

        private int* DataBufferToVersion(int entityIndex, ComponentConfig config)
        {
            GetComponentConfigOffset(config, out var configOffset);
            return DataBufferToVersion(entityIndex, configOffset);
        }

        private int* DataBufferToVersion(int entityIndex, ComponentConfigOffset configOffset)
        {
            return DataBufferToVersions(entityIndex) + configOffset.ConfigIndex;
        }

        private byte* DataBufferToComponents(int entityIndex) => _componentBuffer + (entityIndex * ComponentsSizeInBytes);

        private byte* DataBufferToComponent(int entityIndex, ComponentConfig config)
        {
            GetComponentConfigOffset(config, out var configOffset);
            return DataBufferToComponent(entityIndex, configOffset);
        }

        private byte* DataBufferToComponent(int entityIndex, ComponentConfigOffset configOffset)
        {
            return DataBufferToComponents(entityIndex) + configOffset.OffsetInBytes;
        }

        private void IncComponentVersion(int entityIndex, ComponentConfigOffset configOffset)
        {
            var version = DataBufferToVersion(entityIndex, configOffset);
            *version = *version + 1;
        }

        internal void RemoveTransferedEntity(Entity entity, EntityData* allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                // Move last entity to removed entity spot
                var lastEntity = *DataBufferToEntity(EntityCount - 1);
                var lastEntityData = allEntityDatas[lastEntity.Id];

                *DataBufferToEntity(entityData.EntityIndex) = lastEntity;
                MemoryHelper.Copy(
                    DataBufferToComponents(lastEntityData.EntityIndex),
                    DataBufferToComponents(entityData.EntityIndex),
                    ComponentsSizeInBytes);

                lastEntityData.EntityIndex = entityData.EntityIndex;
                allEntityDatas[lastEntity.Id] = lastEntityData;
            }
            EntityCount--;
        }

        private int ConvertToInt(byte* ptr) =>
            Marshal.PtrToStructure<int>((IntPtr)ptr);

        private int ConvertToInt(IntPtr ptr) =>
            Marshal.PtrToStructure<int>(ptr);
    }
}