using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Linq;

namespace EcsLte
{
    public class EntityArcheType : IEquatable<EntityArcheType>
    {
        public EcsContext Context { get; private set; }
        public ComponentConfig[] ComponentConfigs { get; private set; } = new ComponentConfig[0];
        public SharedComponentDataIndex[] SharedComponentDataIndexes { get; private set; } = new SharedComponentDataIndex[0];
        internal ArcheTypeIndex? ArcheTypeIndex { get; set; }

        internal EntityArcheType(EcsContext context) => Context = context;

        internal EntityArcheType(EcsContext context, EntityBlueprint blueprint)
        {
            Context = context;
            ComponentConfigs = blueprint.AllBlueprintComponents
                .Select(x => x.Config)
                .ToArray();
            SharedComponentDataIndexes = blueprint.SharedBlueprintComponents
                .Select(x => context.EntityManager.GetSharedComponentDataIndex((ISharedComponent)x.Component, x.Config))
                .ToArray();
            ArcheTypeIndex = blueprint.ArcheTypeIndex;
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < ComponentConfigs.Length; i++)
            {
                if (ComponentConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasSharedComponent<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var sharedDataIndex = Context.EntityManager.GetSharedComponentDataIndex(component);
            for (var i = 0; i < SharedComponentDataIndexes.Length; i++)
            {
                if (SharedComponentDataIndexes[i] == sharedDataIndex)
                    return true;
            }

            return false;
        }

        public EntityArcheType AddComponent<TComponent>() where TComponent : IComponent
        {
            if (HasComponent<TComponent>())
                throw new EntityArcheTypeAlreadyHasComponentException(typeof(TComponent));

            var config = ComponentConfig<TComponent>.Config;
            if (config.IsShared)
                throw new EntityArcheTypeSharedComponentException(typeof(TComponent));

            var archeType = new EntityArcheType(Context)
            {
                ComponentConfigs = Helper.CopyInsertSort(ComponentConfigs, config),
                SharedComponentDataIndexes = SharedComponentDataIndexes
            };

            return archeType;
        }

        public EntityArcheType AddSharedComponent<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            if (HasComponent<TSharedComponent>())
                throw new EntityArcheTypeAlreadyHasComponentException(typeof(TSharedComponent));

            var config = ComponentConfig<TSharedComponent>.Config;
            var sharedDataIndex = Context.EntityManager.GetSharedComponentDataIndex(component);

            var archeType = new EntityArcheType(Context)
            {
                ComponentConfigs = Helper.CopyInsertSort(ComponentConfigs, config),
                SharedComponentDataIndexes = Helper.CopyInsertSort(SharedComponentDataIndexes, sharedDataIndex)
            };

            return archeType;
        }

        public EntityArcheType ReplaceSharedComponent<TSharedComponent>(TSharedComponent component) where TSharedComponent : ISharedComponent
        {
            if (!HasComponent<TSharedComponent>())
                return AddSharedComponent(component);

            var config = ComponentConfig<TSharedComponent>.Config;
            var sharedDataIndex = Context.EntityManager.GetSharedComponentDataIndex(component);

            var archeType = new EntityArcheType(Context)
            {
                ComponentConfigs = ComponentConfigs,
                SharedComponentDataIndexes = new SharedComponentDataIndex[SharedComponentDataIndexes.Length]
            };

            Array.Copy(SharedComponentDataIndexes, archeType.SharedComponentDataIndexes, SharedComponentDataIndexes.Length);
            for (var i = 0; i < SharedComponentDataIndexes.Length; i++)
            {
                if (SharedComponentDataIndexes[i].SharedIndex == config.SharedIndex)
                {
                    archeType.SharedComponentDataIndexes[i] = sharedDataIndex;
                    break;
                }
            }

            return archeType;
        }

        public static bool operator !=(EntityArcheType lhs, EntityArcheType rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityArcheType lhs, EntityArcheType rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.Context != rhs.Context)
                return false;
            if (lhs.ComponentConfigs.Length != rhs.ComponentConfigs.Length)
                return false;
            if (lhs.SharedComponentDataIndexes.Length != rhs.SharedComponentDataIndexes.Length)
                return false;

            for (var i = 0; i < lhs.ComponentConfigs.Length; i++)
            {
                if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.SharedComponentDataIndexes.Length; i++)
            {
                if (lhs.SharedComponentDataIndexes[i] != rhs.SharedComponentDataIndexes[i])
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArcheType other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityArcheType obj && this == obj;

        public override int GetHashCode()
        {
            var hashCode = HashCodeHelper.StartHashCode();
            for (var i = 0; i < ComponentConfigs.Length; i++)
                hashCode = hashCode.AppendHashCode(ComponentConfigs[i]);
            for (var i = 0; i < SharedComponentDataIndexes.Length; i++)
                hashCode = hashCode.AppendHashCode(SharedComponentDataIndexes[i]);

            return hashCode.HashCode;
        }
    }
}
