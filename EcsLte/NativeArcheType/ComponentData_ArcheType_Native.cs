using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EcsLte.Data.Unmanaged;
using EcsLte.Data.Unmanaged.Cache;
using EcsLte.Utilities;

namespace EcsLte.NativeArcheType
{
	public struct ComponentData_ArcheType_Native : IDisposable
	{
		private static int _entityDataSizeInBytes = Marshal.SizeOf(typeof(EntityData_ArcheType_Native*));
		private static int _dataChunkPtrSizeInBytes = Marshal.SizeOf(typeof(DataChunk_ArcheType_Native*));

		private unsafe ComponentConfigIndex_ArcheType_Native* _configs;
		private int _configsLength;
		/// <summary>
		/// [Entity,Component1,Component2],[Entity,Component1,Component2]
		/// </summary>
		private unsafe DataChunk_ArcheType_Native** _dataChunks;
		private int _lengthPerComponentOffsetInBytes;
		private int _capacityPerDataChunk;
		private int _dataChunksCount;
		private int _dataChunksLength;
		private unsafe DataChunk_ArcheType_Native* _lastDataChunk;
		private unsafe ComponentConfigIndex_ArcheType_Native* _uniqueConfigs;
		private int _uniqueConfigsLength;

		public Component_ArcheType_Native ArcheType { get; private set; }
		public int EntityCount { get; private set; }

		public unsafe static ComponentData_ArcheType_Native* Alloc(Component_ArcheType_Native archeType, ComponentConfigIndex_ArcheType_Native* uniqueConfigs)
        {
			var data = MemoryHelper.Alloc<ComponentData_ArcheType_Native>(1);

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

			var dataChunkLengthInBytes = EcsSettings.UnmanagedDataChunkInBytes;
			data->_capacityPerDataChunk = Math.Min(dataChunkLengthInBytes / componentOffsetInBytes,
				dataChunkLengthInBytes / TypeCache<Entity>.SizeInBytes);
			data->_lengthPerComponentOffsetInBytes = componentOffsetInBytes;

			data->ArcheType = archeType;

			return data;
		}

		public unsafe void AddEntity(Entity entity, EntityData_ArcheType_Native* entityData)
        {
			var dataChunk = GetAvailableDataChunk();

			fixed (ComponentData_ArcheType_Native* selfPtr = &this)
			{
				entityData->ComponentArcheTypeData = selfPtr;
			}
			entityData->DataChunk = dataChunk;
			entityData->Index = dataChunk->Count++;

			*(Entity*)(dataChunk->Buffer + (entityData->Index * _lengthPerComponentOffsetInBytes)) = entity;
			EntityCount++;
		}

		public unsafe void RemoveEntity(EntityData_ArcheType_Native* entityData, EntityData_ArcheType_Native* entityDatas)
		{
			if (_lastDataChunk != entityData->DataChunk ||
				(_lastDataChunk == entityData->DataChunk && _lastDataChunk->Count - 1 != entityData->Index))
				// Is not last entity
				TransferEntityInternaly(entityData, entityDatas, _lastDataChunk, _lastDataChunk->Count - 1);
			else
				// Is last entity
				_lastDataChunk->Count--;

			if (_lastDataChunk->Count == 0)
            {
				DataChunkCache_ArcheType_Native.Instance.Cache(_lastDataChunk);
				_dataChunksCount--;
				_dataChunks[_dataChunksCount] = null;
				if (_dataChunksCount > 0)
					_lastDataChunk = _dataChunks[_dataChunksCount - 1];
				else
					_lastDataChunk = null;
			}

			entityData->Clear();
			EntityCount--;
        }

		public unsafe void TransferEntity(ComponentData_ArcheType_Native* sourceArcheTypeData, Entity entity, EntityData_ArcheType_Native* entityData, EntityData_ArcheType_Native* entityDatas)
		{
			var prevArcheTypeData = entityData->ComponentArcheTypeData;
			var prevDataChunk = entityData->DataChunk;
			var prevIndex = entityData->Index;
			var prevIndexOffsetInBytes = entityData->Index * sourceArcheTypeData->_lengthPerComponentOffsetInBytes;

			sourceArcheTypeData->RemoveEntity(entityData, entityDatas);
			AddEntity(entity, entityData);

			var nextDataChunk = entityData->DataChunk;
			var nextIndex = entityData->Index;
			var nextIndexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			for (int i = 0; i < sourceArcheTypeData->_configsLength; i++)
            {
				var sourceConfigIndex = sourceArcheTypeData->_configs[i];
				if (GetComponentIndex(sourceConfigIndex, out var destConfigIndex))
				{
					MemoryHelper.Copy(
						prevDataChunk->Buffer + prevIndexOffsetInBytes + sourceConfigIndex.OffsetInBytes,
						nextDataChunk->Buffer + nextIndexOffsetInBytes + destConfigIndex.OffsetInBytes,
						sourceConfigIndex.Config.UnmanagedInBytesSize);
				}
            }
		}

		public unsafe void SetComponent(EntityData_ArcheType_Native* entityData, ComponentConfig config, void* componentData)
        {
			GetComponentIndex(config, out var configIndex);
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			MemoryHelper.Copy(
				componentData,
				entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes,
				config.UnmanagedInBytesSize);
		}

		public unsafe void* GetComponent(EntityData_ArcheType_Native* entityData, ComponentConfig config)
		{
			GetComponentIndex(config, out var configIndex);
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;

			return entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes;
		}

		public unsafe IComponent[] GetAllComponents(EntityData_ArcheType_Native* entityData, byte* uniqueComponents)
		{
			var indexOffsetInBytes = entityData->Index * _lengthPerComponentOffsetInBytes;
			var components = new IComponent[_configsLength + _uniqueConfigsLength];

			for (int i = 0; i < _configsLength; i++)
            {
				var configIndex = _configs[i];
				components[i] = (IComponent)Marshal.PtrToStructure(
					(IntPtr)(entityData->DataChunk->Buffer + indexOffsetInBytes + configIndex.OffsetInBytes),
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
			if (_dataChunks != null)
			{
				for (int i = 0; i < _dataChunksCount; i++)
					DataChunkCache_ArcheType_Native.Instance.Cache(_dataChunks[i]);
				MemoryHelper.Free(_dataChunks);
				_dataChunks = null;
			}
			_lengthPerComponentOffsetInBytes = 0;
			_capacityPerDataChunk = 0;
			_dataChunksCount = 0;
			_dataChunksLength = 0;
			_lastDataChunk = null;
			if (_uniqueConfigsLength > 0)
			{
				MemoryHelper.Free(_uniqueConfigs);
				_uniqueConfigs = null;
				_uniqueConfigsLength = 0;
			}
		}

		private unsafe DataChunk_ArcheType_Native* GetAvailableDataChunk()
        {
			if (_lastDataChunk == null || _lastDataChunk->IsFull(_capacityPerDataChunk))
			{
				if (_dataChunksLength == 0)
				{
					_dataChunks = (DataChunk_ArcheType_Native**)MemoryHelper.Alloc(_dataChunkPtrSizeInBytes);
					_dataChunksLength = 1;
				}
				else if (_dataChunksCount == _dataChunksLength)
				{
					_dataChunks = (DataChunk_ArcheType_Native**)MemoryHelper.Realloc(
						_dataChunks,
						_dataChunkPtrSizeInBytes * _dataChunksLength,
						_dataChunkPtrSizeInBytes * (_dataChunksLength + 1));
					_dataChunksLength++;
				}

				_lastDataChunk = DataChunkCache_ArcheType_Native.Instance.GetDataChunk(false);
				_dataChunks[_dataChunksCount] = _lastDataChunk;
				_dataChunksCount++;
			}

			return _lastDataChunk;
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

		private unsafe void TransferEntityInternaly(EntityData_ArcheType_Native* destEntityData, EntityData_ArcheType_Native* entityDatas, DataChunk_ArcheType_Native* sourceDataChunk, int sourceIndex)
		{
			var sourceEntity = (Entity*)(sourceDataChunk->Buffer + (sourceIndex * _lengthPerComponentOffsetInBytes));
			var sourceEntityData = &entityDatas[sourceEntity->Id];

			if (destEntityData->DataChunk == sourceDataChunk)
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
					destEntityData->DataChunk->Buffer + (destEntityData->Index * _lengthPerComponentOffsetInBytes),
					_lengthPerComponentOffsetInBytes);
				sourceEntityData->DataChunk = destEntityData->DataChunk;
			}
			sourceEntityData->Index = destEntityData->Index;
			sourceDataChunk->Count--;
		}
	}
}
