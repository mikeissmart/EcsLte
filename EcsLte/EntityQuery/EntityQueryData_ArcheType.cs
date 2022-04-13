using EcsLte.Data.Unmanaged;
using EcsLte.ManagedArcheType;
using EcsLte.NativeArcheType;
using EcsLte.NativeArcheTypeContinous;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityQueryData_ArcheType : IEquatable<EntityQueryData_ArcheType>, IEntityQueryData
    {
        private int _hashCode;
        private readonly HashSet<ComponentData_ArcheType_Managed> _archeTypes_Managed;
        private readonly HashSet<PtrWrapper> _archeTypes_Native;
        private readonly HashSet<PtrWrapper> _archeTypes_Native_Continuous;
        private Entity[] _entities;
        private bool _isEntitiesDirty;

        public HashSet<ComponentData_ArcheType_Managed> ArcheType_Managed => _archeTypes_Managed;
        internal HashSet<PtrWrapper> ArcheType_Native => _archeTypes_Native;
        internal HashSet<PtrWrapper> ArcheType_Native_Continuous => _archeTypes_Native_Continuous;

        public ComponentConfig[] AllConfigs { get; set; } = new ComponentConfig[0];
        public ComponentConfig[] AnyConfigs { get; set; } = new ComponentConfig[0];
        public ComponentConfig[] NoneConfigs { get; set; } = new ComponentConfig[0];
        public SharedComponentDataIndex[] SharedComponents { get; set; } = new SharedComponentDataIndex[0];
        internal IComponentEntityFactory ComponentEntityFactory { get; private set; }
        public Entity[] Entities
        {
            get
            {
                if (_isEntitiesDirty)
                {
                    _entities = ComponentEntityFactory.EntityQueryGetEntities(this);
                    _isEntitiesDirty = false;
                }

                return _entities;
            }
        }

        internal EntityQueryData_ArcheType(IComponentEntityFactory componentEntityFactory)
        {
            _archeTypes_Managed = new HashSet<ComponentData_ArcheType_Managed>();
            _archeTypes_Native = new HashSet<PtrWrapper>();
            _archeTypes_Native_Continuous = new HashSet<PtrWrapper>();
            _isEntitiesDirty = true;

            ComponentEntityFactory = componentEntityFactory;
        }

        public bool HasEntity(Entity entity) => ComponentEntityFactory.EntityQueryHasEntity(this, entity);

        public void SetEntitiesDirty() => _isEntitiesDirty = true;

        public bool HasArcheTypeData_Managed(ComponentData_ArcheType_Managed archeTypeData)
            => _archeTypes_Managed.Contains(archeTypeData);

        public unsafe bool HasArcheTypeData_Native(ComponentData_ArcheType_Native* archeTypeData)
            => _archeTypes_Native.Any(x => x.Ptr == archeTypeData);

        public unsafe bool HasArcheTypeData_Native_Continuous(ComponentData_ArcheType_Native_Continuous* archeTypeData)
            => _archeTypes_Native_Continuous.Any(x => x.Ptr == archeTypeData);

        public bool FilterArcheType_Managed(ComponentData_ArcheType_Managed archeTypeData)
        {
            bool isDirty;
            if (IsFiltered_Managed(archeTypeData))
                isDirty = _archeTypes_Managed.Add(archeTypeData);
            else
                isDirty = _archeTypes_Managed.Remove(archeTypeData);
            if (isDirty)
                _isEntitiesDirty = true;
            return isDirty;
        }

        public unsafe bool FilterArcheType_Native(ComponentData_ArcheType_Native* archeTypeData)
        {
            bool isDirty;
            if (IsFiltered_Native(archeTypeData))
                isDirty = _archeTypes_Native.Add(new PtrWrapper { Ptr = archeTypeData });
            else
                isDirty = _archeTypes_Native.Remove(new PtrWrapper { Ptr = archeTypeData });
            if (isDirty)
                _isEntitiesDirty = true;
            return isDirty;
        }

        public unsafe bool FilterArcheType_Native_Continuous(ComponentData_ArcheType_Native_Continuous* archeTypeData)
        {
            bool isDirty;
            if (IsFiltered_Native_Continuous(archeTypeData))
                isDirty = _archeTypes_Native_Continuous.Add(new PtrWrapper { Ptr = archeTypeData });
            else
                isDirty = _archeTypes_Native_Continuous.Remove(new PtrWrapper { Ptr = archeTypeData });
            if (isDirty)
                _isEntitiesDirty = true;
            return isDirty;
        }

        public bool IsFiltered_Managed(ComponentData_ArcheType_Managed archeTypeData)
        {
            foreach (var config in AllConfigs)
            {
                if (!archeTypeData.ArcheType.ComponentConfigs.Any(x => x == config))
                    return false;
            }
            var isAny = false;
            foreach (var config in AnyConfigs)
            {
                if (archeTypeData.ArcheType.ComponentConfigs.Any(x => x == config))
                {
                    isAny = true;
                    break;
                }
            }
            if (!isAny)
                return false;
            foreach (var config in NoneConfigs)
            {
                if (archeTypeData.ArcheType.ComponentConfigs.Any(x => x == config))
                    return false;
            }
            return true;
        }

        public unsafe bool IsFiltered_Native(ComponentData_ArcheType_Native* archeTypeData)
        {
            foreach (var config in AllConfigs)
            {
                if (!archeTypeData->ArcheType.HasComponentConfig(config))
                    return false;
            }
            var isAny = false;
            foreach (var config in AnyConfigs)
            {
                if (archeTypeData->ArcheType.HasComponentConfig(config))
                {
                    isAny = true;
                    break;
                }
            }
            if (!isAny)
                return false;
            foreach (var config in NoneConfigs)
            {
                if (archeTypeData->ArcheType.HasComponentConfig(config))
                    return false;
            }
            return true;
        }

        public unsafe bool IsFiltered_Native_Continuous(ComponentData_ArcheType_Native_Continuous* archeTypeData)
        {
            foreach (var config in AllConfigs)
            {
                if (!archeTypeData->ArcheType.HasComponentConfig(config))
                    return false;
            }
            var isAny = false;
            foreach (var config in AnyConfigs)
            {
                if (archeTypeData->ArcheType.HasComponentConfig(config))
                {
                    isAny = true;
                    break;
                }
            }
            if (!isAny)
                return false;
            foreach (var config in NoneConfigs)
            {
                if (archeTypeData->ArcheType.HasComponentConfig(config))
                    return false;
            }
            return true;
        }

        internal bool CheckConfigsZero() => AllConfigs.Length == 0 &&
                AnyConfigs.Length == 0 &&
                NoneConfigs.Length == 0;

        public static bool operator !=(EntityQueryData_ArcheType lhs, EntityQueryData_ArcheType rhs) => !(lhs == rhs);

        public static bool operator ==(EntityQueryData_ArcheType lhs, EntityQueryData_ArcheType rhs) => lhs._hashCode == rhs._hashCode;

        public bool Equals(EntityQueryData_ArcheType other) => this == other;

        public override bool Equals(object other) => other is EntityQueryData_ArcheType obj && this == obj;

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
    }
}
