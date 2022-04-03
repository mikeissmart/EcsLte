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
        private ComponentEntityData__ArcheType_Managed[] _entityDatas;
        private int _componentPoolEntityLength;

        public Component_ArcheType_Managed ArcheType { get; private set; }
        public int EntityCount { get; private set; }

        public static ComponentData_ArcheType_Managed Alloc(Component_ArcheType_Managed archeType)
        {
            var data = new ComponentData_ArcheType_Managed
            {
                _configs = new Dictionary<ComponentConfig, int>()
            };
            var uniqueConfigs = new List<ComponentConfig>();

            foreach (var config in archeType.ComponentConfigs)
            {
                if (config.IsUnique)
                    uniqueConfigs.Add(config);
                else
                    data._configs.Add(config, data._configs.Count);
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

            data._entityDatas = new ComponentEntityData__ArcheType_Managed[_componentPoolInitLength];

            data._componentPoolEntityLength = _componentPoolInitLength;

            data.ArcheType = archeType;

            return data;
        }

        public void AddEntity(Entity entity, EntityData_ArcheType_Managed entityData)
        {
            if (EntityCount == _componentPoolEntityLength)
            {
                _componentPoolEntityLength *= 2;
                Array.Resize(ref _entityDatas, _componentPoolEntityLength);
                foreach (var componentPool in _componentPools)
                    componentPool.Resize(_componentPoolEntityLength);
            }

            _entityDatas[EntityCount] = new ComponentEntityData__ArcheType_Managed
            {
                Entity = entity,
                EntityData = entityData
            };
            entityData.ComponentArcheTypeData = this;
            entityData.Index = EntityCount;
            EntityCount++;
        }

        public void RemoveEntity(EntityData_ArcheType_Managed entityData)
        {
            if (entityData.Index != EntityCount - 1)
            {
                // Is not last entity
                var moveEntityData = _entityDatas[EntityCount - 1];
                moveEntityData.EntityData.Index = entityData.Index;
                _entityDatas[entityData.Index] = moveEntityData;
                _entityDatas[EntityCount - 1] = null;
            }
            else
            {
                // Is last entity
                _entityDatas[entityData.Index] = null;
            }

            entityData.ComponentArcheTypeData = null;
            entityData.Index = -1;
            EntityCount--;
        }

        public void TransferEntity(ComponentData_ArcheType_Managed sourceArcheTypeData, Entity entity, EntityData_ArcheType_Managed entityData)
        {
            var prevIndex = entityData.Index;

            sourceArcheTypeData.RemoveEntity(entityData);
            AddEntity(entity, entityData);

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
    }
}
