using EcsLte.Data;
using EcsLte.Managed;
using EcsLte.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace EcsLte
{
    internal class EntityQueryData : IEquatable<EntityQueryData>, IEntityQueryData
    {
        private int _hashCode;
        private readonly DataCache<HashSet<Entity>, Entity[]> _entities;

        public ComponentConfig[] AllConfigs { get; set; } = new ComponentConfig[0];
        public ComponentConfig[] AnyConfigs { get; set; } = new ComponentConfig[0];
        public ComponentConfig[] NoneConfigs { get; set; } = new ComponentConfig[0];
        public IEntityQuery_SharedComponentData[] SharedComponents { get; set; } = new IEntityQuery_SharedComponentData[0];
        internal IComponentEntityFactory ComponentEntityFactory { get; private set; }
        public Entity[] Entities => _entities.CachedData;

        internal EntityQueryData(IComponentEntityFactory componentEntityFactory)
        {
            _entities = new DataCache<HashSet<Entity>, Entity[]>(
                UpdateEntitiesCache,
                new HashSet<Entity>(),
                null);
            ComponentEntityFactory = componentEntityFactory;
        }

        public bool HasEntity(Entity entity) => ComponentEntityFactory.EntityQueryHasEntity(this, entity);

        public void AddEntity(Entity entity)
        {
            _entities.UncachedData.Add(entity);
            _entities.SetDirty();
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.UncachedData.Remove(entity);
            _entities.SetDirty();
        }

        public void FilterEntity_Managed(Entity entity, EntityData_Managed entityData)
        {
            bool isDirty;
            if (IsFiltered_Managed(entityData))
                isDirty = _entities.UncachedData.Add(entity);
            else
                isDirty = _entities.UncachedData.Remove(entity);
            if (isDirty)
                _entities.SetDirty();
        }

        public unsafe void FilterEntity_Native(Entity entity, EntityData_Native* entityData, ComponentConfigOffset_Native* configOffsets)
        {
            bool isDirty;
            if (IsFiltered_Native(entityData, configOffsets))
                isDirty = _entities.UncachedData.Add(entity);
            else
                isDirty = _entities.UncachedData.Remove(entity);
            if (isDirty)
                _entities.SetDirty();
        }

        public bool IsFiltered_Managed(EntityData_Managed entityData)
        {
            foreach (var config in AllConfigs)
            {
                if (entityData.Components[config.ComponentIndex] == null)
                    return false;
            }
            var isAny = false;
            foreach (var config in AnyConfigs)
            {
                if (entityData.Components[config.ComponentIndex] != null)
                {
                    isAny = true;
                    break;
                }
            }
            if (!isAny)
                return false;
            foreach (var config in NoneConfigs)
            {
                if (entityData.Components[config.ComponentIndex] != null)
                    return false;
            }
            foreach (var componentData in SharedComponents)
            {
                if (componentData.IsEqual(entityData.Components[componentData.Config.ComponentIndex]))
                    return false;
            }
            return true;
        }

        public unsafe bool IsFiltered_Native(EntityData_Native* entityData, ComponentConfigOffset_Native* configOffsets)
        {
            foreach (var config in AllConfigs)
            {
                if (!entityData->GetHasComponent(config.ComponentIndex))
                    return false;
            }
            var isAny = false;
            foreach (var config in AnyConfigs)
            {
                if (entityData->GetHasComponent(config.ComponentIndex))
                {
                    isAny = true;
                    break;
                }
            }
            if (!isAny)
                return false;
            foreach (var config in NoneConfigs)
            {
                if (entityData->GetHasComponent(config.ComponentIndex))
                    return false;
            }
            foreach (var componentData in SharedComponents)
            {
                var configOffset = configOffsets[componentData.Config.ComponentIndex];
                var component = (IComponent)Marshal.PtrToStructure(
                    (IntPtr)entityData->Component(configOffset.OffsetInBytes),
                    ComponentConfigs.Instance.AllComponentTypes[configOffset.Config.ComponentIndex]);
                if (componentData.IsEqual(component))
                    return false;
            }
            return true;
        }

        internal bool CheckConfigsZero() => AllConfigs.Length == 0 &&
                AnyConfigs.Length == 0 &&
                NoneConfigs.Length == 0;

        public static bool operator !=(EntityQueryData lhs, EntityQueryData rhs) => !(lhs == rhs);

        public static bool operator ==(EntityQueryData lhs, EntityQueryData rhs) => lhs._hashCode == rhs._hashCode;

        public bool Equals(EntityQueryData other) => this == other;

        public override bool Equals(object other) => other is EntityQueryData obj && this == obj;

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode = -612338121;
                _hashCode = _hashCode * -1521134295 + AllConfigs.Length;
                _hashCode = _hashCode * -1521134295 + AnyConfigs.Length;
                _hashCode = _hashCode * -1521134295 + NoneConfigs.Length;
                foreach (var config in AllConfigs)
                    _hashCode = _hashCode * -1521134295 + config.GetHashCode();
                foreach (var config in AnyConfigs)
                    _hashCode = _hashCode * -1521134295 + config.GetHashCode();
                foreach (var config in NoneConfigs)
                    _hashCode = _hashCode * -1521134295 + config.GetHashCode();
                foreach (var sharedComponent in SharedComponents)
                    _hashCode = _hashCode * -1521134295 + sharedComponent.GetHashCode();
            }

            return _hashCode;
        }

        private static Entity[] UpdateEntitiesCache(HashSet<Entity> uncachedData)
            => uncachedData.ToArray();
    }
}
