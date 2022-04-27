using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte
{
    internal unsafe struct ArcheTypeData
    {
        private ComponentConfigOffset* _configOffsets;
        // TODO benchmark TransferAllEntities vs for loop UpdateComponentNoCheck 
        //  if its too slow might think about changing buffer layout
        //  May want to change layout to this when recordable is implemented
        //  Changing to this layout may speed up transfering but will slow down CreatingEntities
        //      [Entity1,Entity2],[Component1,Component1],[Component2,Component2]
        /// <summary>
        /// [Entity1,Entity2],[Component1,Component2,Component1,Component2]
        /// </summary>
        private byte* _dataBuffer;
        private int _dataBufferSizeInBytes;
        private int _dataBufferComponentsOffset;

        internal ArcheType ArcheType { get; private set; }
        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal int EntityCount { get; private set; }
        internal int EntityCapacity { get; private set; }
        internal int ComponentsSizeInBytes { get; private set; }
        internal ComponentConfig* UniqueConfigs { get; private set; }
        internal int UniqueConfigsLength { get; private set; }

        internal static ArcheTypeData* Alloc(ArcheType archeType, ArcheTypeIndex archeTypeIndex)
        {
            var data = MemoryHelper.Alloc<ArcheTypeData>(1);
            data->Initialize(archeType, archeTypeIndex);

            return data;
        }

        internal static void TransferEntity(
            Entity entity,
            ArcheTypeData* nextArcheTypeData,
            ref EntityData[] allEntityDatas)
        {
            var prevEntityData = allEntityDatas[entity.Id];
            var prevArcheTypeData = prevEntityData.ArcheTypeData;
            var prevComponentsBuffer = prevArcheTypeData->_dataBuffer
                + prevArcheTypeData->CalculateComponentsOffset(prevEntityData.EntityIndex);

            var nextTempComponentBuffer = stackalloc byte[nextArcheTypeData->ComponentsSizeInBytes];
            for (var i = 0; i < prevArcheTypeData->ArcheType.ComponentConfigLength; i++)
            {
                var prevConfigOffset = prevArcheTypeData->_configOffsets[i];
                if (prevConfigOffset.Config.UnmanagedSizeInBytes != 0 &&
                    nextArcheTypeData->GetComponentOffset(prevArcheTypeData->ArcheType.ComponentConfigs[i], out var configOffset))
                {
                    MemoryHelper.Copy(
                        prevComponentsBuffer + prevConfigOffset.OffsetInBytes,
                        nextTempComponentBuffer + configOffset.OffsetInBytes,
                        configOffset.Config.UnmanagedSizeInBytes);
                }
            }

            prevArcheTypeData->RemoveEntity(entity, ref allEntityDatas);
            allEntityDatas[entity.Id] = nextArcheTypeData->AddEntity(entity, nextTempComponentBuffer);
        }

        internal static void TransferAllEntities(
            ArcheTypeData* prevArcheTypeData,
            ArcheTypeData* nextArcheTypeData,
            ref EntityData[] allEntityDatas)
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
            var prevComponentsBuffer = prevArcheTypeData->_dataBuffer
                + prevArcheTypeData->CalculateComponentsOffset(0);
            var nextComponentsBuffer = nextArcheTypeData->_dataBuffer
                + nextArcheTypeData->CalculateComponentsOffset(nextArcheTypeData->EntityCount);
            for (var configIndex = 0; configIndex < prevArcheTypeData->ArcheType.ComponentConfigLength; configIndex++)
            {
                var prevConfigOffset = prevArcheTypeData->_configOffsets[configIndex];
                if (prevConfigOffset.Config.UnmanagedSizeInBytes != 0 &&
                    nextArcheTypeData->GetComponentOffset(prevArcheTypeData->ArcheType.ComponentConfigs[configIndex], out var nextConfigOffset))
                {
                    for (int entityIndex = 0; entityIndex < prevArcheTypeData->EntityCount; entityIndex++,
                        prevConfigOffset.OffsetInBytes += prevArcheTypeData->ComponentsSizeInBytes,
                        nextConfigOffset.OffsetInBytes += nextArcheTypeData->ComponentsSizeInBytes)
                    {
                        MemoryHelper.Copy(
                            prevComponentsBuffer + prevConfigOffset.OffsetInBytes,
                            nextComponentsBuffer + nextConfigOffset.OffsetInBytes,
                            nextConfigOffset.Config.UnmanagedSizeInBytes);
                    }
                }
            }

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

        internal void PreCheckEntityStateAllocation(int count)
            => CheckResize(count);

        internal void CopyEntities(ref Entity[] entities, int startingIndex)
        {
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

        internal EntityData AddEntity(Entity entity, byte* componentsBuffer)
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

            *(Entity*)(_dataBuffer + CalculateEntityOffset(entityData.EntityIndex)) = entity;
            MemoryHelper.Copy(
                componentsBuffer,
                _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex),
                ComponentsSizeInBytes);

            return entityData;
        }

        internal EntityData RemoveEntity(Entity entity, ref EntityData[] allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                // Move last entity to removed entity spot
                var lastEntity = *(Entity*)(_dataBuffer + ((EntityCount - 1) * TypeCache<Entity>.SizeInBytes));
                var lastEntityData = allEntityDatas[lastEntity.Id];

                *(Entity*)(_dataBuffer + CalculateEntityOffset(entityData.EntityIndex)) = lastEntity;
                MemoryHelper.Copy(
                    _dataBuffer + CalculateComponentsOffset(lastEntityData.EntityIndex),
                    _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex),
                    ComponentsSizeInBytes);

                lastEntityData.EntityIndex = entityData.EntityIndex;
                allEntityDatas[lastEntity.Id] = lastEntityData;
            }
            EntityCount--;

            return new EntityData();
        }

        internal byte* GetComponentPtr(EntityData entityData, ComponentConfig config)
        {
            GetComponentOffset(config, out var configOffset);
            return _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex) + configOffset.OffsetInBytes;
        }

        internal byte* GetComponentsPtr(EntityData entityData)
            => _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex);

        internal void CopyBlueprintComponentsBuffer(IEntityBlueprintComponentData[] blueprintComponents, byte* buffer)
        {
            for (var i = 0; i < blueprintComponents.Length; i++)
            {
                var blueprintComponent = blueprintComponents[i];
                if (blueprintComponent.Config.UnmanagedSizeInBytes != 0)
                    blueprintComponent.CopyComponentData(buffer + _configOffsets[i].OffsetInBytes);
            }
        }

        internal IComponent[] GetAllComponents(EntityData entityData)
        {
            var components = new IComponent[ArcheType.ComponentConfigLength];

            for (var i = 0; i < ArcheType.ComponentConfigLength; i++)
            {
                var configOffset = _configOffsets[i];
                components[i] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)(_dataBuffer + CalculateComponentsOffset(entityData.EntityIndex) + configOffset.OffsetInBytes),
                    ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
            }

            return components;
        }

        internal void SetComponent(EntityData entityData, void* component, ComponentConfig config)
        {
            GetComponentOffset(config, out var configOffset);
            MemoryHelper.Copy(
                component,
                _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex) + configOffset.OffsetInBytes,
                config.UnmanagedSizeInBytes);
        }

        internal void SetAllComponents(void* component, ComponentConfig config)
        {
            GetComponentOffset(config, out var configOffset);
            for (int i = 0, componentOffset = CalculateComponentsOffset(i); i < EntityCount; i++, componentOffset += ComponentsSizeInBytes)
            {
                MemoryHelper.Copy(
                    component,
                    _dataBuffer + CalculateComponentsOffset(i) + configOffset.OffsetInBytes,
                    config.UnmanagedSizeInBytes);
            }
        }

        public bool GetComponentOffset(ComponentConfig config, out ComponentConfigOffset configOffset)
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

        public void Dispose()
        {
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (_dataBuffer != null)
                MemoryHelper.Free(_dataBuffer);
            _dataBuffer = null;
            ArcheType = new ArcheType();
            ArcheTypeIndex = new ArcheTypeIndex();
            EntityCount = 0;
            EntityCapacity = 0;
            ComponentsSizeInBytes = 0;
            if (UniqueConfigsLength > 0)
                MemoryHelper.Free(UniqueConfigs);
            UniqueConfigs = null;
            UniqueConfigsLength = 0;
        }

        private void Initialize(ArcheType archeType, ArcheTypeIndex archeTypeIndex)
        {
            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(archeType.ComponentConfigLength);

            var uniqueConfigs = new List<ComponentConfig>();
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
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
            if (ComponentsSizeInBytes == 0)
                ComponentsSizeInBytes = 1;
            ArcheType = archeType;
            ArcheTypeIndex = archeTypeIndex;
            if (uniqueConfigs.Count > 0)
            {
                UniqueConfigs = MemoryHelper.Alloc<ComponentConfig>(uniqueConfigs.Count);
                UniqueConfigsLength = uniqueConfigs.Count;
                for (var i = 0; i < uniqueConfigs.Count; i++)
                    UniqueConfigs[i] = uniqueConfigs[i];
            }
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

        private int CalculateEntityOffset(int entityIndex)
            => entityIndex * TypeCache<Entity>.SizeInBytes;

        private int CalculateComponentsOffset(int entityIndex)
            => (EntityCapacity * TypeCache<Entity>.SizeInBytes) + (entityIndex * ComponentsSizeInBytes);
    }
}
