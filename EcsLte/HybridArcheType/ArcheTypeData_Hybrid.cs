using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EcsLte.HybridArcheType
{
    internal unsafe struct ArcheTypeData_Hybrid : IDisposable
    {
        private ComponentConfigOffset* _configOffsets;
        private byte* _dataBuffer;
        private int _dataBufferSizeInBytes;
        private int _dataBufferComponentsOffset;

        internal ArcheType_Hybrid ArcheType { get; private set; }
        internal int ArcheTypeIndex { get; private set; }
        internal int EntityCount { get; private set; }
        internal int EntityLength { get; private set; }
        internal int ComponentsSizeInBytes { get; private set; }
        internal ComponentConfig* UniqueConfigs { get; private set; }
        internal int UniqueConfigsLength { get; private set; }

        internal static ArcheTypeData_Hybrid* Alloc(ArcheType_Hybrid archeType, int archeTypeIndex)
        {
            var data = MemoryHelper.Alloc<ArcheTypeData_Hybrid>(1);
            data->Initialize(archeType, archeTypeIndex);

            return data;
        }

        internal static void TransferEntity(
            Entity entity,
            ArcheTypeData_Hybrid* nextArcheTypeData,
            ref EntityData_Hybrid[] allEntityDatas)
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

        internal void PreCheckEntityStateAllocation(int count) => CheckResize(count);

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

        internal Entity GetEntity(int entityIndex) => *(Entity*)(_dataBuffer + (entityIndex * TypeCache<Entity>.SizeInBytes));

        internal EntityData_Hybrid AddEntity(Entity entity, byte* componentsBuffer)
        {
            CheckResize(1);

            var entityData = new EntityData_Hybrid();
            fixed (ArcheTypeData_Hybrid* selfPtr = &this)
            {
                entityData.ArcheTypeData = selfPtr;
            }
            entityData.EntityIndex = EntityCount++;

            *(Entity*)(_dataBuffer + CalculateEntityOffset(entityData.EntityIndex)) = entity;
            MemoryHelper.Copy(
                componentsBuffer,
                _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex),
                ComponentsSizeInBytes);

            return entityData;
        }

        internal EntityData_Hybrid RemoveEntity(Entity entity, ref EntityData_Hybrid[] allEntityDatas)
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

            return new EntityData_Hybrid();
        }

        internal void CopyBlueprintComponentsBuffer(IEntityBlueprintComponentData[] blueprintComponents, byte* buffer)
        {
            for (var i = 0; i < blueprintComponents.Length; i++)
            {
                var blueprintComponent = blueprintComponents[i];
                if (blueprintComponent.Config.UnmanagedSizeInBytes != 0)
                    blueprintComponent.CopyComponentData(buffer + _configOffsets[i].OffsetInBytes);
            }
        }

        internal byte* GetComponent(EntityData_Hybrid entityData, ComponentConfig config)
        {
            GetComponentOffset(config, out var configOffset);
            return _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex) + configOffset.OffsetInBytes;
        }

        internal IComponent[] GetAllComponents(EntityData_Hybrid entityData)
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

        internal void SetComponent(EntityData_Hybrid entityData, void* component, ComponentConfig config)
        {
            GetComponentOffset(config, out var configOffset);
            MemoryHelper.Copy(
                component,
                _dataBuffer + CalculateComponentsOffset(entityData.EntityIndex) + configOffset.OffsetInBytes,
                config.UnmanagedSizeInBytes);
        }

        public void Dispose()
        {
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (_dataBuffer != null)
                MemoryHelper.Free(_dataBuffer);
            _dataBuffer = null;
            ArcheType = new ArcheType_Hybrid();
            ArcheTypeIndex = -1;
            ComponentsSizeInBytes = 0;
            if (UniqueConfigsLength > 0)
                MemoryHelper.Free(UniqueConfigs);
            UniqueConfigs = null;
            UniqueConfigsLength = 0;
        }

        private void CheckResize(int count)
        {
            if (count > (EntityLength - EntityCount))
            {
                int newEntityLength;
                if (EntityLength == 0 && count == 1)
                    // Could have unique components
                    newEntityLength = 1;
                else
                    newEntityLength = (int)Math.Pow(2, (int)Math.Log(EntityLength + count, 2) + 1);
                var newBufferLengthInBytes = newEntityLength *
                    (TypeCache<Entity>.SizeInBytes + ComponentsSizeInBytes);
                var newComponentsOffset = newEntityLength * TypeCache<Entity>.SizeInBytes;
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
                        EntityLength * ComponentsSizeInBytes);
                    MemoryHelper.Free(_dataBuffer);
                }
                _dataBuffer = newBuffer;
                _dataBufferSizeInBytes = newBufferLengthInBytes;
                _dataBufferComponentsOffset = newComponentsOffset;
                EntityLength = newEntityLength;
            }
        }

        private void Initialize(ArcheType_Hybrid archeType, int archeTypeIndex)
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

        private int CalculateEntityOffset(int entityIndex) => entityIndex * TypeCache<Entity>.SizeInBytes;

        private int CalculateComponentsOffset(int entityIndex) => (EntityLength * TypeCache<Entity>.SizeInBytes)
                + (entityIndex * ComponentsSizeInBytes);

        private bool GetComponentOffset(ComponentConfig config, out ComponentConfigOffset configOffset)
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

        private struct ComponentConfigOffset
        {
            public ComponentConfig Config;
            public int OffsetInBytes;
        }
    }
}
