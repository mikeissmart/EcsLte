using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;
using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheType
{
    public struct ComponentData_ArcheType_Native : IDisposable
    {
        private static readonly int _entityDataSizeInBytes = Marshal.SizeOf(typeof(EntityData_ArcheType_Native*));
        private static readonly int _dataChunkPtrSizeInBytes = Marshal.SizeOf(typeof(DataChunk_ArcheType_Native*));

        private unsafe ComponentConfigIndex_ArcheType_Native* _configs;
        private int _configsLength;
        private unsafe ComponentConfigIndex_ArcheType_Native* _uniqueConfigs;
        private int _uniqueConfigsLength;
        /// <summary>
        /// [Component1,Component2],[Component1,Component2]
        /// </summary>
        private unsafe DataChunk_ArcheType_Native** _dataChunks;
        private int _lengthPerComponentOffsetInBytes;
        private int _capacityPerDataChunk;
        private int _dataChunksCount;
        private int _dataChunksLength;
        private unsafe DataChunk_ArcheType_Native* _lastDataChunk;
        private unsafe Entity* _entities;
        private int _entitiesLength;

        public Component_ArcheType_Native ArcheType { get; private set; }
        public int ArcheTypeIndex { get; private set; }
        public int EntityCount { get; private set; }
        public unsafe DataChunk_ArcheType_Native** DataChunks => _dataChunks;
        public int DataChunkCount => DataChunkCount;
        public int LengthPerComponentOffsetInBytes => _lengthPerComponentOffsetInBytes;

        public static unsafe ComponentData_ArcheType_Native* Alloc(Component_ArcheType_Native archeType, ComponentConfigIndex_ArcheType_Native* uniqueConfigs, int archeTypeIndex)
        {
            var data = MemoryHelper.Alloc<ComponentData_ArcheType_Native>(1);

            data->_configsLength = 0;
            data->_uniqueConfigsLength = 0;
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
            {
                if (archeType.ComponentConfigs[i].IsUnique)
                    data->_uniqueConfigsLength++;
                else
                    data->_configsLength++;
            }

            if (data->_configsLength > 0)
                data->_configs = MemoryHelper.Alloc<ComponentConfigIndex_ArcheType_Native>(data->_configsLength);
            if (data->_uniqueConfigsLength > 0)
                data->_uniqueConfigs = MemoryHelper.Alloc<ComponentConfigIndex_ArcheType_Native>(data->_uniqueConfigsLength);

            var componentOffsetInBytes = 0;
            for (int i = 0, componentArcheIndex = 0, uniqueArcheIndex = 0; i < archeType.ComponentConfigLength; i++)
            {
                var config = archeType.ComponentConfigs[i];
                if (!config.IsUnique)
                {
                    data->_configs[componentArcheIndex] = new ComponentConfigIndex_ArcheType_Native
                    {
                        Config = config,
                        Index = componentArcheIndex++,
                        OffsetInBytes = componentOffsetInBytes
                    };
                    componentOffsetInBytes += config.UnmanagedSizeInBytes;
                }
                else
                {
                    data->_uniqueConfigs[uniqueArcheIndex++] = uniqueConfigs[config.UniqueIndex];
                }
            }
            if (componentOffsetInBytes == 0)
                componentOffsetInBytes = 1;

            var dataChunkLengthInBytes = EcsSettings.UnmanagedDataChunkInBytes;
            data->_capacityPerDataChunk = dataChunkLengthInBytes / componentOffsetInBytes;
            data->_lengthPerComponentOffsetInBytes = componentOffsetInBytes;

            data->ArcheType = archeType;
            data->ArcheTypeIndex = archeTypeIndex;

            return data;
        }

        public unsafe void CopyEntities(Entity[] entities, int startingIndex)
        {
            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    _entities,
                    entitiesPtr,
                    EntityCount * TypeCache<Entity>.SizeInBytes);
            }
        }

        public unsafe Entity GetEntity(int entityIndex) => _entities[entityIndex];

        public unsafe void AddEntity(ArcheTypeFactory_ArcheType_Native archeTypeFactory, DataChunkCache_ArcheType_Native dataChunkCache, Entity entity, EntityData_ArcheType_Native* entityData)
        {
            var dataChunk = GetAvailableDataChunk(dataChunkCache);

            fixed (ComponentData_ArcheType_Native* selfPtr = &this)
            {
                entityData->ComponentArcheTypeData = selfPtr;
                //TODO uncomment after blueprintBenchmark-archeTypeFactory.SetEntitiesDirty(selfPtr);
            }
            entityData->DataChunk = dataChunk;
            entityData->DataChunkIndex = dataChunk->Count++;
            entityData->EntityIndex = EntityCount;

            _entities[EntityCount++] = entity;
        }

        public unsafe void RemoveEntity(ArcheTypeFactory_ArcheType_Native archeTypeFactory, DataChunkCache_ArcheType_Native dataChunkCache, EntityData_ArcheType_Native* entityData, EntityData_ArcheType_Native* entityDatas)
        {
            if (entityData->EntityIndex != EntityCount - 1)
            {
                // Move last entity to removed entity slot
                var lastEntity = _entities[EntityCount - 1];
                var lastEntityData = &entityDatas[lastEntity.Id];

                if (lastEntityData->DataChunk == entityData->DataChunk)
                {
                    // Is last data chunk, not last Entity
                    MemoryHelper.CopyBlock(
                        lastEntityData->DataChunk->Buffer,
                        lastEntityData->DataChunkIndex * _lengthPerComponentOffsetInBytes,
                        entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes,
                        _lengthPerComponentOffsetInBytes);
                }
                else
                {
                    // Is not last data chunk, not last Entity
                    MemoryHelper.Copy(
                        lastEntityData->DataChunk->Buffer + (lastEntityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                        entityData->DataChunk->Buffer + (entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                        _lengthPerComponentOffsetInBytes);
                    lastEntityData->DataChunk = entityData->DataChunk;
                }

                _entities[entityData->EntityIndex] = lastEntity;
                lastEntityData->EntityIndex = entityData->EntityIndex;
                lastEntityData->DataChunkIndex = entityData->DataChunkIndex;
            }
            _lastDataChunk->Count--;

            if (_lastDataChunk->Count == 0)
            {
                dataChunkCache.Cache(_lastDataChunk);
                _dataChunks[--_dataChunksCount] = null;
                if (_dataChunksCount > 0)
                    _lastDataChunk = _dataChunks[_dataChunksCount - 1];
                else
                    _lastDataChunk = null;
            }

            /*TODO uncomment after blueprintBenchmark-fixed (ComponentData_ArcheType_Native* selfPtr = &this)
            {
                archeTypeFactory.SetEntitiesDirty(selfPtr);
            }-TODO uncomment after blueprintBenchmark*/
            entityData->Clear();
            EntityCount--;
        }

        public unsafe void TransferEntity(ArcheTypeFactory_ArcheType_Native archeTypeFactory, DataChunkCache_ArcheType_Native dataChunkCache, ComponentData_ArcheType_Native* sourceArcheTypeData, Entity entity, EntityData_ArcheType_Native* entityData, EntityData_ArcheType_Native* entityDatas)
        {
            var nextEntityData = new EntityData_ArcheType_Native();
            AddEntity(archeTypeFactory, dataChunkCache, entity, &nextEntityData);

            var prevDataChunk = entityData->DataChunk;
            var prevIndexOffsetInBytes = entityData->DataChunkIndex * sourceArcheTypeData->_lengthPerComponentOffsetInBytes;

            var nextDataChunk = nextEntityData.DataChunk;
            var nextIndexOffsetInBytes = nextEntityData.DataChunkIndex * _lengthPerComponentOffsetInBytes;

            for (var i = 0; i < sourceArcheTypeData->_configsLength; i++)
            {
                var sourceConfigIndex = sourceArcheTypeData->_configs[i];
                if (GetComponentIndex(sourceConfigIndex, out var destConfigIndex))
                {
                    MemoryHelper.Copy(
                        prevDataChunk->Buffer + prevIndexOffsetInBytes + sourceConfigIndex.OffsetInBytes,
                        nextDataChunk->Buffer + nextIndexOffsetInBytes + destConfigIndex.OffsetInBytes,
                        sourceConfigIndex.Config.UnmanagedSizeInBytes);
                }
            }

            sourceArcheTypeData->RemoveEntity(archeTypeFactory, dataChunkCache, entityData, entityDatas);
            entityDatas[entity.Id] = nextEntityData;
        }

        public unsafe void SetEntityBlueprintData(EntityData_ArcheType_Native* entityData, byte* blueprintComponentsBuffer, int blueprintComponentsBufferLengthInBytes) => MemoryHelper.Copy(
                blueprintComponentsBuffer,
                entityData->DataChunk->Buffer + (entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                blueprintComponentsBufferLengthInBytes);

        public unsafe void SetComponent(EntityData_ArcheType_Native* entityData, ComponentConfig config, void* componentData)
        {
            GetComponentIndex(config, out var configIndex);
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;

            MemoryHelper.Copy(
                componentData,
                entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes,
                config.UnmanagedSizeInBytes);
        }

        public unsafe void* GetComponent(EntityData_ArcheType_Native* entityData, ComponentConfig config)
        {
            GetComponentIndex(config, out var configIndex);
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;

            return entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes;
        }

        public unsafe IComponent[] GetAllComponents(EntityData_ArcheType_Native* entityData, byte* uniqueComponents)
        {
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;
            var components = new IComponent[_configsLength + _uniqueConfigsLength];

            for (var i = 0; i < _configsLength; i++)
            {
                var configIndex = _configs[i];
                components[i] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)(entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes),
                    ComponentConfigs.Instance.AllComponentTypes[configIndex.Config.ComponentIndex]);
            }
            for (var i = 0; i < _uniqueConfigsLength; i++)
            {
                var configIndex = _uniqueConfigs[i];
                components[i + _configsLength] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)(uniqueComponents + configIndex.OffsetInBytes),
                    ComponentConfigs.Instance.AllComponentTypes[configIndex.Config.ComponentIndex]);
            }

            return components;
        }

        public unsafe void SetUniqueComponent(ComponentConfig config, void* componentData, byte* uniqueComponents)
        {
            GetUniqueComponentIndex(config, out var configIndex);

            MemoryHelper.Copy(
                componentData,
                uniqueComponents + configIndex.OffsetInBytes,
                config.UnmanagedSizeInBytes);
        }

        public unsafe void* GetUniqueComponent(ComponentConfig config, byte* uniqueComponents)
        {
            GetUniqueComponentIndex(config, out var configIndex);

            return uniqueComponents + configIndex.OffsetInBytes;
        }

        public unsafe void GetMappedComponentOffsets(ComponentConfig[] configs, ref int[] mappedComponentOffsets)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                GetComponentIndex(configs[i], out var configOffset);
                mappedComponentOffsets[i] = configOffset.OffsetInBytes;
            }
        }

        public unsafe void Dispose()
        {
            if (_configsLength > 0)
            {
                MemoryHelper.Free(_configs);
                _configs = null;
                _configsLength = 0;
            }
            if (_uniqueConfigsLength > 0)
            {
                MemoryHelper.Free(_uniqueConfigs);
                _uniqueConfigs = null;
                _uniqueConfigsLength = 0;
            }
            if (_dataChunks != null)
            {
                for (var i = 0; i < _dataChunksCount; i++)
                    MemoryHelper.Free(_dataChunks[i]);
                MemoryHelper.Free(_dataChunks);
                _dataChunks = null;
            }
            _lengthPerComponentOffsetInBytes = 0;
            _capacityPerDataChunk = 0;
            _dataChunksCount = 0;
            _dataChunksLength = 0;
            _lastDataChunk = null;
            if (_entitiesLength > 0)
            {
                MemoryHelper.Free(_entities);
                _entities = null;
                _entitiesLength = 0;
            }
        }

        private unsafe DataChunk_ArcheType_Native* GetAvailableDataChunk(DataChunkCache_ArcheType_Native dataChunkCache)
        {
            if (_lastDataChunk == null || _lastDataChunk->IsFull(_capacityPerDataChunk))
            {
                if (_dataChunksLength == 0)
                {
                    _dataChunks = (DataChunk_ArcheType_Native**)MemoryHelper.Alloc(_dataChunkPtrSizeInBytes);
                    _dataChunksLength = 1;
                    _entities = MemoryHelper.Alloc<Entity>(_capacityPerDataChunk);
                    _entitiesLength = _capacityPerDataChunk;
                }
                else if (_dataChunksCount == _dataChunksLength)
                {
                    _dataChunks = (DataChunk_ArcheType_Native**)MemoryHelper.Realloc(
                        _dataChunks,
                        _dataChunkPtrSizeInBytes * _dataChunksLength,
                        _dataChunkPtrSizeInBytes * (_dataChunksLength + 1));
                    _dataChunksLength++;
                    _entities = (Entity*)MemoryHelper.Realloc(
                        _entities,
                        _entitiesLength * TypeCache<Entity>.SizeInBytes,
                        (_entitiesLength + _capacityPerDataChunk) * TypeCache<Entity>.SizeInBytes);
                    _entitiesLength += _capacityPerDataChunk;
                }

                _lastDataChunk = dataChunkCache.GetDataChunk(false);
                _dataChunks[_dataChunksCount++] = _lastDataChunk;
            }

            return _lastDataChunk;
        }

        private unsafe bool GetComponentIndex(ComponentConfig config, out ComponentConfigIndex_ArcheType_Native configIndex)
        {
            configIndex = new ComponentConfigIndex_ArcheType_Native();
            for (var i = 0; i < _configsLength; i++)
            {
                var check = _configs[i];
                if (check.Config.ComponentIndex == config.ComponentIndex)
                {
                    configIndex = check;
                    return true;
                }
            }

            return false;
        }

        private unsafe bool GetComponentIndex(ComponentConfigIndex_ArcheType_Native config, out ComponentConfigIndex_ArcheType_Native configIndex)
        {
            configIndex = new ComponentConfigIndex_ArcheType_Native();
            for (var i = 0; i < _configsLength; i++)
            {
                var check = _configs[i];
                if (check.Config.ComponentIndex == config.Config.ComponentIndex)
                {
                    configIndex = check;
                    return true;
                }
            }

            return false;
        }

        private unsafe bool GetUniqueComponentIndex(ComponentConfig config, out ComponentConfigIndex_ArcheType_Native configIndex)
        {
            configIndex = new ComponentConfigIndex_ArcheType_Native();
            for (var i = 0; i < _uniqueConfigsLength; i++)
            {
                var check = _uniqueConfigs[i];
                if (check.Config.ComponentIndex == config.ComponentIndex)
                {
                    configIndex = check;
                    return true;
                }
            }

            return false;
        }

        private unsafe bool GetUniqueComponentIndex(ComponentConfigIndex_ArcheType_Native config, out ComponentConfigIndex_ArcheType_Native configIndex)
        {
            configIndex = new ComponentConfigIndex_ArcheType_Native();
            for (var i = 0; i < _uniqueConfigsLength; i++)
            {
                var check = _uniqueConfigs[i];
                if (check.Config.ComponentIndex == config.Config.ComponentIndex)
                {
                    configIndex = check;
                    return true;
                }
            }

            return false;
        }
    }
}
