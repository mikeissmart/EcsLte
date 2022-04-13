using EcsLte.Data.Unmanaged;
using EcsLte.NativeArcheType;
using EcsLte.Utilities;
using System;
using System.Runtime.InteropServices;

namespace EcsLte.NativeArcheTypeContinous
{
    public struct ComponentData_ArcheType_Native_Continuous : IDisposable
    {
        private unsafe ComponentConfigIndex_ArcheType_Native* _configs;
        private int _configsLength;
        private unsafe ComponentConfigIndex_ArcheType_Native* _uniqueConfigs;
        private int _uniqueConfigsLength;
        /// <summary>
        /// [Component1,Component2],[Component1,Component2]
        /// </summary>
        private unsafe DataChunkCache_ArcheType_Native_Continuous* _dataChunkCache;
        private unsafe int* _dataChunkIndexes;
        private int _lengthPerComponentOffsetInBytes;
        private int _capacityPerDataChunk;
        private int _dataChunksCount;
        private int _dataChunksLength;
        private unsafe int _lastDataChunkIndex;
        private unsafe Entity* _entities;
        private int _entitiesLength;

        public Component_ArcheType_Native ArcheType { get; private set; }
        public int ArcheTypeIndex { get; private set; }
        public int EntityCount { get; private set; }
        public unsafe int* DataChunkIndexes => _dataChunkIndexes;
        public int DataChunkCount => DataChunkCount;
        public int LengthPerComponentOffsetInBytes => _lengthPerComponentOffsetInBytes;

        public static unsafe ComponentData_ArcheType_Native_Continuous* Alloc(Component_ArcheType_Native archeType, ComponentConfigIndex_ArcheType_Native* uniqueConfigs, DataChunkCache_ArcheType_Native_Continuous* dataChunkCache, int archeTypeIndex)
        {
            var data = MemoryHelper.Alloc<ComponentData_ArcheType_Native_Continuous>(1);

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
                    componentOffsetInBytes += config.UnmanagedInBytesSize;
                }
                else
                {
                    data->_uniqueConfigs[uniqueArcheIndex++] = uniqueConfigs[config.UniqueIndex];
                }
            }
            if (componentOffsetInBytes == 0)
                componentOffsetInBytes = 1;

            data->_dataChunkCache = dataChunkCache;
            data->_lastDataChunkIndex = -1;

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

        public unsafe void AddEntity(ArcheTypeFactory_ArcheType_Native_Continuous archeTypeFactory, Entity entity, EntityData_ArcheType_Native_Continuous* entityData)
        {
            if (entity.Id == 503439) //503805
                ;
            var dataChunkIndex = GetAvailableDataChunkIndex();
            var dataChunk = _dataChunkCache->GetDataChunk(dataChunkIndex);

            fixed (ComponentData_ArcheType_Native_Continuous* selfPtr = &this)
            {
                entityData->ComponentArcheTypeData = selfPtr;
                //TODO uncomment after blueprintBenchmark-archeTypeFactory.SetEntitiesDirty(selfPtr);
            }
            entityData->ChunkIndex = dataChunkIndex;
            entityData->DataChunkIndex = dataChunk->Count++;
            entityData->EntityIndex = EntityCount;

            if (EntityCount > _entitiesLength)
                ;

            try
            {
                _entities[EntityCount++] = entity;
            }
            catch (Exception)
            {
                ;
            }
        }

        public unsafe void RemoveEntity(ArcheTypeFactory_ArcheType_Native_Continuous archeTypeFactory, EntityData_ArcheType_Native_Continuous* entityData, EntityData_ArcheType_Native_Continuous* entityDatas)
        {
            var lastEntity = _entities[EntityCount - 1];
            var lastEntityData = &entityDatas[lastEntity.Id];
            var lastDataChunk = _dataChunkCache->GetDataChunk(lastEntityData->ChunkIndex);

            if (entityData->EntityIndex != EntityCount - 1)
            {
                // Move last entity to removed entity slot
                var destDataChunk = _dataChunkCache->GetDataChunk(entityData->ChunkIndex);

                if (lastEntityData->ChunkIndex == entityData->ChunkIndex)
                {
                    // Is last data chunk, not last Entity
                    MemoryHelper.CopyBlock(
                        lastDataChunk->Buffer,
                        lastEntityData->DataChunkIndex * _lengthPerComponentOffsetInBytes,
                        entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes,
                        _lengthPerComponentOffsetInBytes);
                }
                else
                {
                    MemoryHelper.Copy(
                        lastDataChunk->Buffer + (lastEntityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                        destDataChunk->Buffer + (entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                        _lengthPerComponentOffsetInBytes);
                    lastEntityData->ChunkIndex = entityData->ChunkIndex;
                }

                _entities[entityData->EntityIndex] = lastEntity;
                lastEntityData->EntityIndex = entityData->EntityIndex;
                lastEntityData->DataChunkIndex = entityData->DataChunkIndex;
            }
            lastDataChunk->Count--;

            // Is last entity
            if (lastDataChunk->Count == 0)
            {
                _dataChunkCache->CacheDataChunkIndex(_lastDataChunkIndex);
                _dataChunksCount--;
                if (_dataChunksCount > 0)
                    _lastDataChunkIndex = _dataChunkIndexes[_dataChunksCount - 1];
                else
                    _lastDataChunkIndex = -1;
            }

            /*TODO uncomment after blueprintBenchmark-fixed (ComponentData_ArcheType_Native_Continuous* selfPtr = &this)
            {
                archeTypeFactory.SetEntitiesDirty(selfPtr);
            }-TODO uncomment after blueprintBenchmark*/
            EntityCount--;
            entityData->Clear();
        }

        public unsafe void TransferEntity(ArcheTypeFactory_ArcheType_Native_Continuous archeTypeFactory, ComponentData_ArcheType_Native_Continuous* sourceArcheTypeData, Entity entity, EntityData_ArcheType_Native_Continuous* entityData, EntityData_ArcheType_Native_Continuous* entityDatas)
        {
            var nextEntityData = new EntityData_ArcheType_Native_Continuous();
            AddEntity(archeTypeFactory, entity, &nextEntityData);

            var predDataChunk = _dataChunkCache->GetDataChunk(entityData->ChunkIndex);
            var prevIndexOffsetInBytes = entityData->DataChunkIndex * sourceArcheTypeData->_lengthPerComponentOffsetInBytes;

            var nextDataChunk = _dataChunkCache->GetDataChunk(nextEntityData.ChunkIndex);
            var nextIndexOffsetInBytes = nextEntityData.DataChunkIndex * _lengthPerComponentOffsetInBytes;

            for (var i = 0; i < sourceArcheTypeData->_configsLength; i++)
            {
                var sourceConfigIndex = sourceArcheTypeData->_configs[i];
                if (GetComponentIndex(sourceConfigIndex, out var destConfigIndex))
                {
                    MemoryHelper.Copy(
                        predDataChunk->Buffer + prevIndexOffsetInBytes + sourceConfigIndex.OffsetInBytes,
                        nextDataChunk->Buffer + nextIndexOffsetInBytes + destConfigIndex.OffsetInBytes,
                        sourceConfigIndex.Config.UnmanagedInBytesSize);
                }
            }

            sourceArcheTypeData->RemoveEntity(archeTypeFactory, entityData, entityDatas);
            entityDatas[entity.Id] = nextEntityData;
        }

        public unsafe void SetEntityBlueprintData(EntityData_ArcheType_Native_Continuous* entityData, byte* blueprintComponentsBuffer, int blueprintComponentsBufferLengthInBytes) => MemoryHelper.Copy(
                blueprintComponentsBuffer,
                _dataChunkCache->GetDataChunk(entityData->ChunkIndex)->Buffer + (entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes),
                blueprintComponentsBufferLengthInBytes);

        public unsafe void SetComponent(EntityData_ArcheType_Native_Continuous* entityData, ComponentConfig config, void* componentData)
        {
            GetComponentIndex(config, out var configIndex);
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;

            MemoryHelper.Copy(
                componentData,
                _dataChunkCache->GetDataChunk(entityData->ChunkIndex)->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes,
                config.UnmanagedInBytesSize);
        }

        public unsafe void* GetComponent(EntityData_ArcheType_Native_Continuous* entityData, ComponentConfig config)
        {
            GetComponentIndex(config, out var configIndex);
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;

            return _dataChunkCache->GetDataChunk(entityData->ChunkIndex)->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes;
        }

        public unsafe IComponent[] GetAllComponents(EntityData_ArcheType_Native_Continuous* entityData, byte* uniqueComponents)
        {
            var indexOffsetInBytes = entityData->DataChunkIndex * _lengthPerComponentOffsetInBytes;
            var components = new IComponent[_configsLength + _uniqueConfigsLength];
            var dataChunk = _dataChunkCache->GetDataChunk(entityData->ChunkIndex);

            for (var i = 0; i < _configsLength; i++)
            {
                var configIndex = _configs[i];
                components[i] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)(dataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes),
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
                config.UnmanagedInBytesSize);
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
            if (_dataChunkIndexes != null)
            {
                _dataChunkCache->CacheDataChunkIndexes(_dataChunkIndexes, _dataChunksCount);
                MemoryHelper.Free(_dataChunkIndexes);
                _dataChunkIndexes = null;
            }
            _lengthPerComponentOffsetInBytes = 0;
            _capacityPerDataChunk = 0;
            _dataChunksCount = 0;
            _dataChunksLength = 0;
            _lastDataChunkIndex = -1;
            if (_entitiesLength > 0)
            {
                MemoryHelper.Free(_entities);
                _entities = null;
                _entitiesLength = 0;
            }
        }

        private unsafe int GetAvailableDataChunkIndex()
        {
            if (_lastDataChunkIndex == -1 || _dataChunkCache->GetDataChunk(_lastDataChunkIndex)->IsFull(_capacityPerDataChunk))
            {
                if (_dataChunksLength == 0)
                {
                    _dataChunkIndexes = MemoryHelper.Alloc<int>(1);
                    _dataChunksLength = 1;
                    _entities = MemoryHelper.Alloc<Entity>(_capacityPerDataChunk);
                    _entitiesLength = _capacityPerDataChunk;
                }
                else if (_dataChunksCount == _dataChunksLength)
                {
                    _dataChunkIndexes = (int*)MemoryHelper.Realloc(
                        _dataChunkIndexes,
                        TypeCache<int>.SizeInBytes * _dataChunksLength,
                        TypeCache<int>.SizeInBytes * (_dataChunksLength + 1));
                    _dataChunksLength++;
                    _entities = (Entity*)MemoryHelper.Realloc(
                        _entities,
                        _entitiesLength * TypeCache<Entity>.SizeInBytes,
                        (_entitiesLength + _capacityPerDataChunk) * TypeCache<Entity>.SizeInBytes);
                    _entitiesLength += _capacityPerDataChunk;
                }

                _lastDataChunkIndex = _dataChunkCache->GetDataChunkIndex();
                _dataChunkIndexes[_dataChunksCount++] = _lastDataChunkIndex;
            }

            return _lastDataChunkIndex;
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
