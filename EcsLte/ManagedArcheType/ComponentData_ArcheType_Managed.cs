using System;
using System.Collections.Generic;

namespace EcsLte.ManagedArcheType
{
    public class ComponentData_ArcheType_Managed
    {
        private static readonly int _componentPoolInitLength = 4;

        private Dictionary<ComponentConfig, int> _configs;
        private IComponentPool_ArcheType_Managed[] _componentPools;
        private ComponentConfig[] _uniqueConfigs;
        private Entity[] _entities;
        private int _componentPoolEntityLength;

        public Component_ArcheType_Managed ArcheType { get; private set; }
        public int ArcheTypeIndex { get; private set; }
        public int EntityCount { get; private set; }

        public static ComponentData_ArcheType_Managed Alloc(Component_ArcheType_Managed archeType, int archeTypeIndex)
        {
            var data = new ComponentData_ArcheType_Managed
            {
                _configs = new Dictionary<ComponentConfig, int>()
            };
            var uniqueConfigs = new List<ComponentConfig>();

            if (archeType.ComponentConfigs != null)
            {
                foreach (var config in archeType.ComponentConfigs)
                {
                    if (config.IsUnique)
                        uniqueConfigs.Add(config);
                    else
                        data._configs.Add(config, data._configs.Count);
                }
            }
            data._uniqueConfigs = uniqueConfigs.ToArray();

            var componentPoolType = typeof(ComponentPool_ArcheType_Managed<>);
            var args = new object[] { _componentPoolInitLength };
            data._componentPools = new IComponentPool_ArcheType_Managed[data._configs.Count];
            foreach (var configPair in data._configs)
            {
                data._componentPools[configPair.Value] = (IComponentPool_ArcheType_Managed)Activator
                    .CreateInstance(componentPoolType.MakeGenericType(
                        ComponentConfigs.Instance.AllComponentTypes[configPair.Key.ComponentIndex]), args);
            }

            data._entities = new Entity[_componentPoolInitLength];

            data._componentPoolEntityLength = _componentPoolInitLength;

            data.ArcheTypeIndex = archeTypeIndex;
            data.ArcheType = archeType;

            return data;
        }

        public void CopyEntities(Entity[] entities, int startingIndex) => Array.Copy(_entities, 0, entities, startingIndex, EntityCount);

        public void AddEntity(ArcheTypeFactory_ArcheType_Managed archeTypeFactory, Entity entity, EntityData_ArcheType_Managed entityData)
        {
            if (EntityCount == _componentPoolEntityLength)
            {
                _componentPoolEntityLength *= 2;
                Array.Resize(ref _entities, _componentPoolEntityLength);
                foreach (var componentPool in _componentPools)
                    componentPool.Resize(_componentPoolEntityLength);
            }

            _entities[EntityCount] = entity;

            //TODO uncomment after blueprintBenchmark-archeTypeFactory.SetEntitiesDirty(this);
            entityData.ComponentArcheTypeData = this;
            entityData.Index = EntityCount;
            EntityCount++;
        }

        public void RemoveEntity(ArcheTypeFactory_ArcheType_Managed archeTypeFactory, EntityData_ArcheType_Managed entityData, EntityData_ArcheType_Managed[] entityDatas)
        {
            if (entityData.Index != EntityCount - 1)
            {
                // Is not last entity
                var sourceEntity = _entities[EntityCount - 1];
                var sourceEntityData = entityDatas[sourceEntity.Id];

                foreach (var componentPool in _componentPools)
                    componentPool.MoveComponent(sourceEntityData.Index, entityData.Index);
                sourceEntityData.Index = entityData.Index;

                _entities[entityData.Index] = sourceEntity;
                sourceEntityData.Index = entityData.Index;
            }

            //TODO uncomment after blueprintBenchmark-archeTypeFactory.SetEntitiesDirty(this);
            entityData.ComponentArcheTypeData = null;
            entityData.Index = -1;
            EntityCount--;
        }

        public void TransferEntity(ArcheTypeFactory_ArcheType_Managed archeTypeFactory, ComponentData_ArcheType_Managed sourceArcheTypeData, Entity entity, EntityData_ArcheType_Managed entityData, EntityData_ArcheType_Managed[] entityDatas)
        {
            var prevIndex = entityData.Index;

            sourceArcheTypeData.RemoveEntity(archeTypeFactory, entityData, entityDatas);
            AddEntity(archeTypeFactory, entity, entityData);

            foreach (var sourceConfigPair in sourceArcheTypeData._configs)
            {
                if (_configs.TryGetValue(sourceConfigPair.Key, out var destConfigIndex))
                {
                    _componentPools[destConfigIndex].SetComponent(
                        entityData.Index,
                        sourceArcheTypeData._componentPools[sourceConfigPair.Value].GetComponent(prevIndex));
                }
            }
        }

        internal void SetEntityBlueprintData(EntityData_ArcheType_Managed entityData, EntityBlueprintData_ArcheType_Managed blueprintData)
        {
            for (var i = 0; i < blueprintData.Configs.Length; i++)
                _componentPools[i].SetComponent(entityData.Index, blueprintData.Components[i]);
        }

        public void SetComponent(EntityData_ArcheType_Managed entityData, ComponentConfig config, IComponent component) => _componentPools[_configs[config]].SetComponent(entityData.Index, component);

        public IComponent GetComponent(EntityData_ArcheType_Managed entityData, ComponentConfig config) => _componentPools[_configs[config]].GetComponent(entityData.Index);

        public IComponent[] GetAllComponents(EntityData_ArcheType_Managed entityData, IComponent[] uniqueComponents)
        {
            var components = new IComponent[_configs.Count + _uniqueConfigs.Length];

            for (var i = 0; i < _configs.Count; i++)
                components[i] = _componentPools[i].GetComponent(entityData.Index);
            for (var i = 0; i < _uniqueConfigs.Length; i++)
                components[i + _configs.Count] = uniqueComponents[_uniqueConfigs[i].UniqueIndex];

            return components;
        }

        public void SetUniqueComponent(ComponentConfig config, IComponent component, IComponent[] uniqueComponents) => uniqueComponents[config.UniqueIndex] = component;

        public IComponent GetUniqueComponent(ComponentConfig config, IComponent[] uniqueComponents) => uniqueComponents[config.UniqueIndex];

        public void GetMappedComponentIndexes(ComponentConfig[] configs, ref int[] mappedComponentIndexes)
        {
            for (var i = 0; i < configs.Length; i++)
                mappedComponentIndexes[i] = _configs[configs[i]];
        }

        public Entity GetEntityQueryForEachComponents(int entityIndex, int[] mappedComponentIndexes, ref IComponent[] components)
        {
            for (var i = 0; i <= mappedComponentIndexes.Length; i++)
                components[i] = _componentPools[mappedComponentIndexes[i]].GetComponent(mappedComponentIndexes[entityIndex]);

            return _entities[entityIndex];
        }
    }
}
