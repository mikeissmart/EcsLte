using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EcsLte
{
    internal unsafe class ArcheTypeData
    {
        private Entity* _entities;
        private int _entitiesLength;
        private IComponentPool[] _managedPools;
        private ComponentConfigOffset* _configOffsets;
        private ComponentBuffer* _generalBuffers;

        internal ArcheType ArcheType { get; private set; }
        internal ArcheTypeIndex ArcheTypeIndex { get; private set; }
        internal int EntityCount { get; private set; }
        internal ComponentConfigOffset[] GeneralConfigs { get; private set; }
        internal ComponentConfigOffset[] ManagedConfigs { get; private set; }
        internal ComponentConfigOffset[] SharedConfigs { get; private set; }
        internal ISharedComponentData[] SharedComponentDatas { get; private set; }

        internal ArcheTypeData(ArcheType archeType, ArcheTypeIndex archeTypeIndex, SharedComponentDictionaries sharedDics)
        {
            var generalConfigs = new List<ComponentConfigOffset>();
            var managedConfigs = new List<ComponentConfigOffset>();
            var sharedConfigs = new List<ComponentConfigOffset>();

            var generalIndex = 0;
            var managedIndex = 0;
            var sharedIndex = 0;
            _configOffsets = MemoryHelper.Alloc<ComponentConfigOffset>(ComponentConfigs.AllComponentCount);
            for (var i = 0; i < archeType.ConfigsLength; i++)
            {
                var config = archeType.Configs[i];
                var configOffset = new ComponentConfigOffset
                {
                    Config = config
                };

                if (config.IsGeneral)
                {
                    configOffset.ConfigIndex = generalIndex++;
                    generalConfigs.Add(configOffset);
                }
                else if (config.IsManaged)
                {
                    configOffset.ConfigIndex = managedIndex++;
                    managedConfigs.Add(configOffset);
                }
                else
                {
                    configOffset.ConfigIndex = sharedIndex++;
                    sharedConfigs.Add(configOffset);
                }

                _configOffsets[config.ComponentIndex] = configOffset;
            }

            _entities = MemoryHelper.Alloc<Entity>(1);
            _entitiesLength = 1;

            ArcheType = archeType;
            ArcheTypeIndex = archeTypeIndex;

            GeneralConfigs = generalConfigs.ToArray();
            if (GeneralConfigs.Length > 0)
            {
                _generalBuffers = MemoryHelper.Alloc<ComponentBuffer>(GeneralConfigs.Length);
                for (var i = 0; i < GeneralConfigs.Length; i++)
                {
                    var configOffset = GeneralConfigs[i];
                    _generalBuffers[i] = new ComponentBuffer
                    {
                        Buffer = MemoryHelper.Alloc<byte>(configOffset.Config.UnmanagedSizeInBytes),
                        ComponentSize = configOffset.Config.UnmanagedSizeInBytes
                    };
                }
            }

            ManagedConfigs = managedConfigs.ToArray();
            _managedPools = ComponentConfigs.CreateComponentPools(ManagedConfigs);

            SharedConfigs = sharedConfigs.ToArray();
            SharedComponentDatas = new ISharedComponentData[SharedConfigs.Length];
            if (SharedConfigs.Length > 0)
            {
                for (var i = 0; i < SharedConfigs.Length; i++)
                {
                    SharedComponentDatas[i] = sharedDics.GetDic(SharedConfigs[i].Config)
                        .GetComponentData(archeType.SharedDataIndexes[i]);
                }
            }
        }

        internal static void TransferEntity(
            Entity entity,
            EntityData* allEntityDatas,
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData)
        {
            var prevEntityData = allEntityDatas[entity.Id];
            var nextEntityData = nextArcheTypeData.AddEntity(entity, false);

            for (var i = 0; i < prevArcheTypeData.GeneralConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.GeneralConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    MemoryHelper.Copy(
                        prevArcheTypeData._generalBuffers[prevConfigOffset.ConfigIndex].Ptr(prevEntityData.EntityIndex),
                        nextArcheTypeData._generalBuffers[nextConfigOffset.ConfigIndex].Ptr(nextEntityData.EntityIndex),
                        prevConfigOffset.Config.UnmanagedSizeInBytes);
                }
            }
            for (var i = 0; i < prevArcheTypeData.ManagedConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.ManagedConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._managedPools[nextConfigOffset.ConfigIndex].CopyFrom(
                        prevArcheTypeData._managedPools[prevConfigOffset.ConfigIndex],
                        prevEntityData.EntityIndex,
                        nextEntityData.EntityIndex,
                        1);
                }
            }

            prevArcheTypeData.RemoveEntity(entity, allEntityDatas);
            allEntityDatas[entity.Id] = nextEntityData;
        }

        internal static void TransferAllEntities(
            EntityData* allEntityDatas,
            ArcheTypeData prevArcheTypeData,
            ArcheTypeData nextArcheTypeData)
        {
            if (prevArcheTypeData.EntityCount == 0)
                return;

            nextArcheTypeData.CheckCapacity(prevArcheTypeData.EntityCount);

            MemoryHelper.Copy(
                prevArcheTypeData._entities,
                nextArcheTypeData._entities + nextArcheTypeData.EntityCount,
                prevArcheTypeData.EntityCount);

            for (var i = 0; i < prevArcheTypeData.GeneralConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.GeneralConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    MemoryHelper.Copy(
                        prevArcheTypeData._generalBuffers[prevConfigOffset.ConfigIndex].Buffer,
                        nextArcheTypeData._generalBuffers[nextConfigOffset.ConfigIndex].Ptr(nextArcheTypeData.EntityCount),
                        prevConfigOffset.Config.UnmanagedSizeInBytes * prevArcheTypeData.EntityCount);
                }
            }
            for (var i = 0; i < prevArcheTypeData.ManagedConfigs.Length; i++)
            {
                var prevConfigOffset = prevArcheTypeData.ManagedConfigs[i];
                if (nextArcheTypeData.HasConfigOffset(prevConfigOffset.Config, out var nextConfigOffset))
                {
                    nextArcheTypeData._managedPools[nextConfigOffset.ConfigIndex].CopyFrom(
                        prevArcheTypeData._managedPools[prevConfigOffset.ConfigIndex],
                        0,
                        nextArcheTypeData.EntityCount,
                        prevArcheTypeData.EntityCount);
                }
            }

            for (int i = 0, entityIndex = nextArcheTypeData.EntityCount;
                i < prevArcheTypeData.EntityCount;
                i++, entityIndex++)
            {
                var entity = prevArcheTypeData._entities[i];
                allEntityDatas[entity.Id] = new EntityData
                {
                    ArcheTypeIndex = nextArcheTypeData.ArcheTypeIndex,
                    EntityIndex = entityIndex
                };
            }

            nextArcheTypeData.EntityCount += prevArcheTypeData.EntityCount;
            prevArcheTypeData.EntityCount = 0;
        }


        internal void CheckCapacity(int count)
        {
            if (count > (_entitiesLength - EntityCount))
            {
                var newLength = Helper.NextPow2(_entitiesLength + count);
                _entities = MemoryHelper.ReallocCopy(_entities, _entitiesLength, newLength);
                for (var i = 0; i < GeneralConfigs.Length; i++)
                {
                    var genBuffer = _generalBuffers[i];
                    _generalBuffers[i] = new ComponentBuffer
                    {
                        Buffer = MemoryHelper.ReallocCopy(genBuffer.Buffer,
                            genBuffer.ComponentSize * _entitiesLength,
                            genBuffer.ComponentSize * newLength),
                        ComponentSize = genBuffer.ComponentSize
                    };
                }
                for (var i = 0; i < _managedPools.Length; i++)
                    _managedPools[i].Resize(newLength);

                _entitiesLength = newLength;
            }
        }

        internal Entity GetEntity(int entityIndex)
            => _entities[entityIndex];

        /// <summary>
        /// Doesnt resize ref entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="startingIndex"></param>
        /// <exception cref="Exception"></exception>
        internal void GetEntities(ref Entity[] entities, int startingIndex)
        {
            if (EntityCount == 0)
                return;
#if DEBUG
            if (entities.Length < EntityCount + startingIndex)
                throw new Exception();
#endif

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    _entities,
                    entitiesPtr,
                    EntityCount);
            }
        }

        internal EntityData AddEntity(Entity entity, bool clearComponents)
        {
            CheckCapacity(1);

            _entities[EntityCount] = entity;

            if (clearComponents)
            {
                for (var i = 0; i < GeneralConfigs.Length; i++)
                    _generalBuffers[i].Clear(EntityCount);
                for (var i = 0; i < _managedPools.Length; i++)
                    _managedPools[i].Clear(EntityCount);
            }

            return new EntityData
            {
                ArcheTypeIndex = ArcheTypeIndex,
                EntityIndex = EntityCount++
            };
        }

        internal void AddEntities(in Entity[] entities, int startingIndex, int count,
            EntityData* allEntityDatas, bool clearComponents)
        {
            CheckCapacity(count);

            fixed (Entity* entitiesPtr = &entities[startingIndex])
            {
                MemoryHelper.Copy(
                    entitiesPtr,
                    _entities + EntityCount,
                    count);
            }

            if (clearComponents)
            {
                for (var i = 0; i < GeneralConfigs.Length; i++)
                    _generalBuffers[i].ClearRange(EntityCount, count);
                for (var i = 0; i < _managedPools.Length; i++)
                    _managedPools[i].ClearRange(EntityCount, count);
            }

            for (int i = 0, index = startingIndex;
                i < count;
                i++, index++)
            {
                allEntityDatas[entities[index].Id] = new EntityData
                {
                    ArcheTypeIndex = ArcheTypeIndex,
                    EntityIndex = EntityCount++
                };
            }
        }

        internal void RemoveEntity(Entity entity, EntityData* allEntityDatas)
        {
            var entityData = allEntityDatas[entity.Id];
            if (EntityCount > 1)
            {
                var lastIndex = EntityCount - 1;
                var lastEntity = _entities[lastIndex];
                if (entity != lastEntity)
                {
                    // Move last entity to removed entity index
                    _entities[entityData.EntityIndex] = lastEntity;
                    for (var i = 0; i < GeneralConfigs.Length; i++)
                        _generalBuffers[i].CopySameArray(lastIndex, entityData.EntityIndex, 1);
                    for (var i = 0; i < _managedPools.Length; i++)
                        _managedPools[i].CopySameArray(lastIndex, entityData.EntityIndex, 1);

                    allEntityDatas[lastEntity.Id].EntityIndex = entityData.EntityIndex;
                }
            }

            EntityCount--;
        }

        internal void RemoveAllEntities()
        {
            EntityCount = 0;
        }

        internal void GetAllEntityComponents(int entityIndex, ref IComponent[] components, int startingIndex)
        {
            var componentIndex = startingIndex;
            for (var i = 0; i < GeneralConfigs.Length; i++)
            {
                components[componentIndex++] = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)_generalBuffers[i].Ptr(entityIndex),
                    GeneralConfigs[i].Config.ComponentType);
            }
            for (var i = 0; i < ManagedConfigs.Length; i++)
            {
                components[componentIndex++] = _managedPools[i]
                    .GetComponent(entityIndex);
            }
            for (var i = 0; i < SharedConfigs.Length; i++)
            {
                components[componentIndex++] = SharedComponentDatas[i].Component;
            }
        }

        internal byte* GetComponentPtr(int entityIndex, ComponentConfig config)
            => GetComponentPtr(entityIndex, GetConfigOffset(config));

        internal byte* GetComponentPtr(int entityIndex, ComponentConfigOffset configOffset)
            => _generalBuffers[configOffset.ConfigIndex].Ptr(entityIndex);

        internal IComponentPool GetManagedComponentPool(ComponentConfig config)
            => GetManagedComponentPool(GetConfigOffset(config));

        internal IComponentPool GetManagedComponentPool(ComponentConfigOffset configOffset)
            => _managedPools[configOffset.ConfigIndex];

        internal IComponent GetSharedComponentData(ComponentConfig config)
            => GetSharedComponentData(GetConfigOffset(config));

        internal IComponent GetSharedComponentData(ComponentConfigOffset configOffset)
            => SharedComponentDatas[configOffset.ConfigIndex].Component;

        internal TComponent GetComponent<TComponent>(int entityIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponent<TComponent>(entityIndex, GetConfigOffset(config));

        internal TComponent GetComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : unmanaged, IGeneralComponent
            => *(TComponent*)_generalBuffers[configOffset.ConfigIndex].Ptr(entityIndex);

        internal void GetComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : unmanaged, IGeneralComponent
        {
            if (EntityCount == 0)
                return;

            var configOffset = GetConfigOffset(config);
            fixed (TComponent* componentPtr = &components[startingIndex])
            {
                MemoryHelper.Copy(
                    (TComponent*)_generalBuffers[configOffset.ConfigIndex].Buffer,
                    componentPtr,
                    EntityCount);
            }
        }

        internal TComponent GetManagedComponent<TComponent>(int entityIndex, ComponentConfig config)
            where TComponent : IManagedComponent
            => GetManagedComponent<TComponent>(entityIndex, GetConfigOffset(config));

        internal TComponent GetManagedComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex]).GetComponent(entityIndex);

        internal ref TComponent GetManagedComponentRef<TComponent>(int entityIndex, ComponentConfigOffset configOffset)
            where TComponent : IManagedComponent
            => ref ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex]).GetComponentRef(entityIndex);

        internal void GetManagedComponents<TComponent>(ref TComponent[] components, int startingIndex, ComponentConfig config)
            where TComponent : IManagedComponent
        {
            if (EntityCount == 0)
                return;

            var configOffset = GetConfigOffset(config);
            ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex])
                .GetComponents(ref components, startingIndex);
        }

        internal TComponent GetSharedComponent<TComponent>(ComponentConfig config)
            where TComponent : unmanaged, ISharedComponent
            => GetSharedComponent<TComponent>(GetConfigOffset(config));

        internal TComponent GetSharedComponent<TComponent>(ComponentConfigOffset configOffset)
            where TComponent : unmanaged, ISharedComponent
            => ((SharedComponentData<TComponent>)SharedComponentDatas[configOffset.ConfigIndex]).Component;

        internal void SetComponent<TComponent>(int entityIndex, ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => SetComponent(entityIndex, GetConfigOffset(config), component);

        internal void SetComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
            => *(TComponent*)_generalBuffers[configOffset.ConfigIndex].Ptr(entityIndex) = component;

        internal void SetComponents<TComponent>(int startingIndex, int count, ComponentConfig config, in TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            var configOffset = GetConfigOffset(config);
            var buffer = (TComponent*)_generalBuffers[configOffset.ConfigIndex].Buffer;
            for (var i = 0; i < count; i++)
                buffer[startingIndex++] = component;
        }

        internal void SetManagedComponent<TComponent>(int entityIndex, ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => SetManagedComponent(entityIndex, GetConfigOffset(config), component);

        internal void SetManagedComponent<TComponent>(int entityIndex, ComponentConfigOffset configOffset, in TComponent component)
            where TComponent : IManagedComponent
            => ((ComponentPool<TComponent>)_managedPools[configOffset.ConfigIndex]).SetComponent(entityIndex, component);

        internal void SetManagedComponents<TComponent>(int startingIndex, int count, ComponentConfig config, in TComponent component)
            where TComponent : IManagedComponent
            => ((ComponentPool<TComponent>)_managedPools[GetConfigOffset(config).ConfigIndex])
                .SetComponents(startingIndex, count, component);

        internal void CopyComponentsSameArcheTypeData(int srcEntityIndex, int destEntityIndex, int count)
        {
            for (var i = 0; i < GeneralConfigs.Length; i++)
                _generalBuffers[i].CopySameArray(srcEntityIndex, destEntityIndex, count);
            for (var i = 0; i < ManagedConfigs.Length; i++)
                _managedPools[i].CopySameArray(srcEntityIndex, destEntityIndex, count);
        }

        internal void CopyComponentsDifferentArcheTypeDataSameComponents(ArcheTypeData srcArcheTypeData,
            int srcEntityIndex, int destEntityIndex, int count)
        {
            for (var i = 0; i < GeneralConfigs.Length; i++)
                _generalBuffers[i].CopyFrom(srcArcheTypeData._generalBuffers[i].Buffer, srcEntityIndex, destEntityIndex, count);
            for (var i = 0; i < ManagedConfigs.Length; i++)
                _managedPools[i].CopyFrom(srcArcheTypeData._managedPools[i], srcEntityIndex, destEntityIndex, count);
        }

        internal ComponentConfigOffset GetConfigOffset(ComponentConfig config)
        {
#if DEBUG
            var checkConfig = _configOffsets[config.ComponentIndex];
            if (checkConfig.Config != config)
                throw new Exception();
#endif
            return _configOffsets[config.ComponentIndex];
        }

        internal bool HasConfigOffset(ComponentConfig config, out ComponentConfigOffset configOffset)
        {
            configOffset = _configOffsets[config.ComponentIndex];
            if (configOffset.Config != config)
                return false;

            return true;
        }

        internal bool HasConfig(ComponentConfig config)
            => _configOffsets[config.ComponentIndex].Config == config;

        internal void InternalDestroy()
        {
            MemoryHelper.Free(_entities);
            _entitiesLength = 0;
            _managedPools = null;
            MemoryHelper.Free(_configOffsets);
            _configOffsets = null;
            if (GeneralConfigs.Length > 0)
                MemoryHelper.Free(_generalBuffers);
            _generalBuffers = null;
            SharedComponentDatas = null;
            ArcheTypeIndex = new ArcheTypeIndex();
            ArcheType.Dispose();
            ArcheType = new ArcheType();
            EntityCount = 0;
            GeneralConfigs = null;
            ManagedConfigs = null;
            SharedConfigs = null;
        }

        private struct ComponentBuffer
        {
            public byte* Buffer;
            public int ComponentSize;

            public byte* Ptr(int index)
                => Buffer + (ComponentSize * index);

            public void CopySameArray(int srcIndex, int destIndex, int count)
            {
                MemoryHelper.Copy(
                    Buffer + (ComponentSize * srcIndex),
                    Buffer + (ComponentSize * destIndex),
                    ComponentSize * count);
            }

            public void CopyFrom(byte* srcBuffer, int srcIndex, int destIndex, int count)
            {
                var srcBufferPtr = srcBuffer + (ComponentSize * srcIndex);
                var selfBufferPtr = Buffer + (ComponentSize * destIndex);

                MemoryHelper.Copy(
                    srcBufferPtr,
                    selfBufferPtr,
                    ComponentSize * count);
            }

            public void Clear(int index)
            {
                MemoryHelper.Clear(Buffer + (ComponentSize * index), ComponentSize);
            }

            public void ClearRange(int index, int count)
            {
                MemoryHelper.Clear(
                    Buffer + (ComponentSize * index),
                    ComponentSize * count);
            }

            public void ClearAll(int count)
            {
                MemoryHelper.Clear(Buffer, ComponentSize * count);
            }
        }
    }
}
