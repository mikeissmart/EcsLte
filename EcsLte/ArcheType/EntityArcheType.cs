using EcsLte.Exceptions;
using System;
using System.Linq;

namespace EcsLte
{
    public class EntityArcheType : IEquatable<EntityArcheType>
    {
        public Type[] ComponentTypes => ArcheTypeData.ComponentTypes;
        public ISharedComponent[] SharedComponents => ArcheTypeData.SharedComponents;
        internal EntityArcheTypeData ArcheTypeData { get; set; }

        public EntityArcheType() => ArcheTypeData = new EntityArcheTypeData();

        internal EntityArcheType(IComponentData[] allComponentDatas, IComponentData[] sharedComponentDatas) => ArcheTypeData = new EntityArcheTypeData(allComponentDatas, sharedComponentDatas);

        internal unsafe EntityArcheType(EcsContext context, ArcheTypeData* archeTypeData) => ArcheTypeData = new EntityArcheTypeData(context, archeTypeData);

        private EntityArcheType(EntityArcheTypeData data) => ArcheTypeData = data;

        public bool HasComponentType<TComponent>() where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < ArcheTypeData.ComponentConfigs.Length; i++)
            {
                if (ArcheTypeData.ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedComponentData<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            for (var i = 0; i < ArcheTypeData.SharedComponentDatas.Length; i++)
            {
                if (ArcheTypeData.SharedComponentDatas[i].ComponentEquals(component))
                    return true;
            }

            return false;
        }

        public EntityArcheType AddComponentType<TComponent>() where TComponent : IComponent
        {
            if (HasComponentType<TComponent>())
                throw new EntityArcheTypeAlreadyHasComponentException(typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
                throw new EntityArcheTypeSharedComponentException(typeof(TComponent));

            return new EntityArcheType(EntityArcheTypeData.AddComponentType(ArcheTypeData, config));
        }

        public EntityArcheType RemoveComponentType<TComponent>() where TComponent : IComponent
        {
            if (!HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
                throw new EntityArcheTypeSharedComponentException(typeof(TComponent));

            return new EntityArcheType(EntityArcheTypeData.RemoveComponentType(ArcheTypeData, config));
        }

        public TSharedComponent GetSharedComponent<TSharedComponent>() where TSharedComponent : ISharedComponent
        {
            if (!HasComponentType<TSharedComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TSharedComponent));

            var config = ComponentConfig<TSharedComponent>.Config;
            return (TSharedComponent)ArcheTypeData.SharedComponentDatas.First(x => x.Config == config).Component;
        }

        public EntityArcheType AddSharedComponent<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            if (HasComponentType<TSharedComponent>())
                throw new EntityArcheTypeAlreadyHasComponentException(typeof(TSharedComponent));

            return new EntityArcheType(EntityArcheTypeData.AddSharedComponent(ArcheTypeData, new ComponentData<TSharedComponent>(component)));
        }

        public EntityArcheType ReplaceSharedComponent<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            if (!HasComponentType<TSharedComponent>())
                return AddSharedComponent(component);

            return new EntityArcheType(EntityArcheTypeData.ReplaceSharedComponent(ArcheTypeData, new ComponentData<TSharedComponent>(component)));
        }

        public EntityArcheType RemoveSharedComponent<TSharedComponent>() where TSharedComponent : ISharedComponent
        {
            if (!HasComponentType<TSharedComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TSharedComponent));

            return new EntityArcheType(EntityArcheTypeData.RemoveSharedComponent(ArcheTypeData, ComponentConfig<TSharedComponent>.Config));
        }

        public EntityArcheType AddComponentTypeOrSharedComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            if (HasComponentType<TComponent>())
                throw new EntityArcheTypeAlreadyHasComponentException(typeof(TComponent));

            return ComponentConfig<TComponent>.Config.IsShared
                ? new EntityArcheType(EntityArcheTypeData.AddSharedComponent(ArcheTypeData, new ComponentData<TComponent>(component)))
                : AddComponentType<TComponent>();
        }

        public EntityArcheType ReplaceComponentTypeOrSharedComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            if (!HasComponentType<TComponent>())
                return AddComponentTypeOrSharedComponent(component);

            return ComponentConfig<TComponent>.Config.IsShared
                ? new EntityArcheType(EntityArcheTypeData.ReplaceSharedComponent(ArcheTypeData, new ComponentData<TComponent>(component)))
                : this;
        }

        public EntityArcheType RemoveComponentTypeOrSharedComponent<TComponent>() where TComponent : IComponent
        {
            if (!HasComponentType<TComponent>())
                throw new EntityArcheTypeNotHaveComponentException(typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            return config.IsShared
                ? new EntityArcheType(EntityArcheTypeData.RemoveSharedComponent(ArcheTypeData, config))
                : new EntityArcheType(EntityArcheTypeData.RemoveComponentType(ArcheTypeData, config));
        }

        public static bool operator !=(EntityArcheType lhs, EntityArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityArcheType lhs, EntityArcheType rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            return lhs.ArcheTypeData == rhs.ArcheTypeData;
        }

        public bool Equals(EntityArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityArcheType obj && this == obj;

        public override int GetHashCode() => ArcheTypeData.GetHashCode();
    }
}