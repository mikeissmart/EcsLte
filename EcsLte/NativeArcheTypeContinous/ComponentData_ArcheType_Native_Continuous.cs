using EcsLte.Data.Unmanaged;
using EcsLte.NativeArcheType;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte.NativeArcheTypeContinous
{
    public struct ComponentData_ArcheType_Native_Continuous : IDisposable
	{
		//private static int _entityDataSizeInBytes = Marshal.SizeOf(typeof(EntityData_ArcheType_Native*));
		//private static int _dataChunkPtrSizeInBytes = Marshal.SizeOf(typeof(DataChunk_ArcheType_Native*));

		private unsafe ComponentConfigIndex_ArcheType_Native* _configs;
		private int _configsLength;
		/// <summary>
		/// [Entity,Component1,Component2],[Entity,Component1,Component2]
		/// </summary>
		private unsafe DataChunkCache_ArcheType_Native_Continuous* _dataChunkCache;
		private unsafe int* _dataChunkIndexes;
		private int _lengthPerComponentOffsetInBytes;
		private int _capacityPerDataChunk;
		private int _dataChunksCount;
		private int _dataChunksLength;
		private unsafe int _lastDataChunkIndex;
		private unsafe ComponentConfigIndex_ArcheType_Native* _uniqueConfigs;
		private int _uniqueConfigsLength;

		public Component_ArcheType_Native ArcheType { get; private set; }
		public int EntityCount { get; private set; }

		public unsafe static ComponentData_ArcheType_Native_Continuous* Alloc(Component_ArcheType_Native archeType, ComponentConfigIndex_ArcheType_Native* uniqueConfigs, DataChunkCache_ArcheType_Native_Continuous* dataChunkCache)
		{
			var data = MemoryHelper.Alloc<ComponentData_ArcheType_Native_Continuous>(1);

			data->_configsLength = 0;
			data->_uniqueConfigsLength = 0;
			for (int i = 0; i < archeType.ComponentConfigLength; i++)
			{
				if (archeType.ComponentConfigs[i].IsUnique)
					data->_uniqueConfigsLength++;
				else
					data->_configsLength++;
			}

			var componentOffsetInBytes = TypeCache<Entity>.SizeInBytes;
			if (data->_configsLength > 0)
				data->_configs = MemoryHelper.Alloc<ComponentConfigIndex_ArcheType_Native>(data->_configsLength);
			if (data->_uniqueConfigsLength > 0)
				data->_uniqueConfigs = MemoryHelper.Alloc<ComponentConfigIndex_ArcheType_Native>(data->_uniqueConfigsLength);

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

			data->_dataChunkCache = dataChunkCache;
			data->_lastDataChunkIndex = -1;

			var dataChunkLengthInBytes = EcsSettings.UnmanagedDataChunkInBytes;
			data->_capacityPerDataChunk = Math.Min(dataChunkLengthInBytes / componentOffsetInBytes,
				dataChunkLengthInBytes / TypeCache<Entity>.SizeInBytes);
			data->_lengthPerComponentOffsetInBytes = componentOffsetInBytes;

			data->ArcheType = archeType;

			return data;
		}

		public unsafe void SetEntityBlueprintData(EntityData_ArcheType_Native_Continuous* entityData, byte* blueprintComponentsBuffer, int blueprintComponentsBufferLengthInBytes)
		{
			MemoryHelper.Copy(
				blueprintComponentsBuffer,
				_dataChunkCache->GetDataChunk(entityData->DataChunkIndex)->Buffer + TypeCache<Entity>.SizeInBytes + (entityData->Index * _lengthPerComponentOffsetInBytes),
				blueprintComponentsBufferLengthInBytes);
		}

		public unsafe void AddEntity(Entity entity, EntityData_ArcheType_Native_Continuous* entityData)
		{
			var dataChunkIndex = GetAvailableDataChunkIndex();
			var dataChunk = _dataChunkCache->GetDataChunk(dataChunkIndex);

			fixed (ComponentData_ArcheType_Native_Continuous* selfPtr = &this)
			{
				entityData->ComponentArcheTypeData = selfPtr;
			}
			entityData->DataChunkIndex = dataChunkIndex;
			entityData->Index = dataChunk->Count++;

			*(Entity*)(dataChunk->Buffer + (entityData->Index * _lengthPerComponentOffsetInBytes)) = entity;
			EntityCount++;
		}

		public unsafe void RemoveEntity(EntityData_ArcheType_Native_Continuous* entityData, EntityData_ArcheType_Native_Continuous* entityDatas)
		{
			RemoveEntityInternaly(entityData, entityDatas);
			entityData->Clear();
		}

		public unsafe void TransferEntity(ComponentData_ArcheType_Native_Continuous* sourceArcheTypeData, Entity entity, EntityData_ArcheType_Native_Continuous* entityData, EntityData_ArcheType_Native_Continuous* entityDatas)
		{
			var prevEntityData = *entityData;
			AddEntity(entity, entityData);

			var prevArcheTypeData = prevEntityData.ComponentArcheTypeData;
			var prevDataChunkIndex = prevEntityData.DataChunkIndex;
			var predDataChunk = _dataChunkCache->GetDataChunk(prevDataChunkIndex);
			var prevIndex = prevEntityData.Index;
			var prevIndexOffsetInBytes = prevEntityData.Index * sourceArcheTypeData->_lengthPerComponentOffsetInBytes;

			var nextDataChunkIndex = entityData->DataChunkIndex;
			var nextDataChunk = _dataChunkCache->GetDataChunk(nextDataChunkIndex);
			var nextIndex = entityData->Index;
			var nextIndexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			for (int i = 0; i < sourceArcheTypeData->_configsLength; i++)
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

			sourceArcheTypeData->RemoveEntityInternaly(&prevEntityData, entityDatas);
		}

		public unsafe void SetComponent(EntityData_ArcheType_Native_Continuous* entityData, ComponentConfig config, void* componentData)
		{
			GetComponentIndex(config, out var configIndex);
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			MemoryHelper.Copy(
				componentData,
				_dataChunkCache->GetDataChunk(entityData->DataChunkIndex)->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes,
				config.UnmanagedInBytesSize);
		}

		public unsafe void* GetComponent(EntityData_ArcheType_Native_Continuous* entityData, ComponentConfig config)
		{
			GetComponentIndex(config, out var configIndex);
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			return _dataChunkCache->GetDataChunk(entityData->DataChunkIndex)->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes;
		}

		public unsafe IComponent[] GetAllComponents(EntityData_ArcheType_Native_Continuous* entityData, byte* uniqueComponents)
		{
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;
			var components = new IComponent[_configsLength + _uniqueConfigsLength];
			var dataChunk = _dataChunkCache->GetDataChunk(entityData->DataChunkIndex);

			for (int i = 0; i < _configsLength; i++)
			{
				var configIndex = _configs[i];
				components[i] = (IComponent)Marshal.PtrToStructure(
					(IntPtr)(dataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes),
					ComponentConfigs.Instance.AllComponentTypes[configIndex.Config.ComponentIndex]);
			}
			for (int i = 0; i < _uniqueConfigsLength; i++)
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

		public unsafe void Dispose()
		{
			if (_configsLength > 0)
			{
				MemoryHelper.Free(_configs);
				_configs = null;
				_configsLength = 0;
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
			if (_uniqueConfigsLength > 0)
			{
				MemoryHelper.Free(_uniqueConfigs);
				_uniqueConfigs = null;
				_uniqueConfigsLength = 0;
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
				}
				else if (_dataChunksCount == _dataChunksLength)
				{
					_dataChunkIndexes = (int*)MemoryHelper.Realloc(
						_dataChunkIndexes,
						_dataChunksLength * TypeCache<int>.SizeInBytes,
						(_dataChunksLength + 1) * TypeCache<int>.SizeInBytes);
					_dataChunksLength++;
				}

				_lastDataChunkIndex = _dataChunkCache->GetDataChunkIndex();
				_dataChunkIndexes[_dataChunksCount++] = _lastDataChunkIndex;
			}

			return _lastDataChunkIndex;
		}

		private unsafe bool GetComponentIndex(ComponentConfig config, out ComponentConfigIndex_ArcheType_Native configIndex)
		{
			configIndex = new ComponentConfigIndex_ArcheType_Native();
			for (int i = 0; i < _configsLength; i++)
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
			for (int i = 0; i < _configsLength; i++)
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
			for (int i = 0; i < _uniqueConfigsLength; i++)
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
			for (int i = 0; i < _uniqueConfigsLength; i++)
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

		private unsafe void RemoveEntityInternaly(EntityData_ArcheType_Native_Continuous* entityData, EntityData_ArcheType_Native_Continuous* entityDatas)
		{
			var lastDataChunk = _dataChunkCache->GetDataChunk(_lastDataChunkIndex);
			if (_lastDataChunkIndex != entityData->DataChunkIndex ||
				(_lastDataChunkIndex == entityData->DataChunkIndex && lastDataChunk->Count - 1 != entityData->Index))
				// Is not last entity
				TransferEntityInternaly(entityData, entityDatas, lastDataChunk, lastDataChunk->Count - 1);
			else
				// Is last entity
				lastDataChunk->Count--;

			if (lastDataChunk->Count == 0)
			{
				_dataChunkCache->CacheDataChunkIndex(_lastDataChunkIndex);
				_dataChunksCount--;
				if (_dataChunksCount > 0)
					_lastDataChunkIndex = _dataChunkIndexes[_dataChunksCount - 1];
				else
					_lastDataChunkIndex = -1;
			}

			EntityCount--;
		}

		private unsafe void TransferEntityInternaly(EntityData_ArcheType_Native_Continuous* destEntityData, EntityData_ArcheType_Native_Continuous* entityDatas, DataChunk_ArcheType_Native_Continuous* sourceDataChunk, int sourceIndex)
		{
			var sourceEntity = (Entity*)(sourceDataChunk->Buffer + (sourceIndex * _lengthPerComponentOffsetInBytes));
			var sourceEntityData = &entityDatas[sourceEntity->Id];

			var destDataChunk = _dataChunkCache->GetDataChunk(destEntityData->DataChunkIndex);

			if (destDataChunk == sourceDataChunk)
			{
				MemoryHelper.CopyBlock(
					sourceDataChunk->Buffer,
					(sourceIndex * _lengthPerComponentOffsetInBytes),
					(destEntityData->Index * _lengthPerComponentOffsetInBytes),
					_lengthPerComponentOffsetInBytes);
			}
			else
			{
				MemoryHelper.Copy(
					sourceDataChunk->Buffer + (sourceIndex * _lengthPerComponentOffsetInBytes),
					destDataChunk->Buffer + (destEntityData->Index * _lengthPerComponentOffsetInBytes),
					_lengthPerComponentOffsetInBytes);
				sourceEntityData->DataChunkIndex = destEntityData->DataChunkIndex;
			}
			sourceEntityData->Index = destEntityData->Index;
			sourceDataChunk->Count--;
		}
	}
}
