using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    internal class EntityArcheTypeData : IEquatable<EntityArcheTypeData>
    {
        private int _hashCode;
        private readonly Dictionary<EcsContext, ArcheTypeIndex> _archeTypeIndexes;
        private readonly object _lockObj;

        internal Type[] ComponentTypes { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }
        internal ComponentConfig[] ComponentConfigs { get; private set; }
        internal IComponentData[] SharedComponentDatas { get; private set; }

        internal EntityArcheTypeData()
        {
            _archeTypeIndexes = new Dictionary<EcsContext, ArcheTypeIndex>();
            _lockObj = new object();
            ComponentTypes = new Type[0];
            SharedComponents = new ISharedComponent[0];
            ComponentConfigs = new ComponentConfig[0];
            SharedComponentDatas = new IComponentData[0];
        }

        internal EntityArcheTypeData(IComponentData[] allComponentDatas, IComponentData[] sharedComponentDatas)
        {
            _archeTypeIndexes = new Dictionary<EcsContext, ArcheTypeIndex>();
            _lockObj = new object();
            ComponentConfigs = allComponentDatas
                .Select(x => x.Config)
                .ToArray();
            SharedComponentDatas = sharedComponentDatas;
            ComponentTypes = allComponentDatas
                .Select(x => EcsLte.ComponentConfigs.Instance.AllComponentTypes[x.Config.ComponentIndex])
                .ToArray();
            SharedComponents = sharedComponentDatas
                .Select(x => (ISharedComponent)x.Component)
                .ToArray();
        }

        internal unsafe EntityArcheTypeData(EcsContext context, ArcheTypeData* archeTypeData)
        {
            _archeTypeIndexes = new Dictionary<EcsContext, ArcheTypeIndex>
            {
                { context, archeTypeData->ArcheTypeIndex }
            };
            _lockObj = new object();

            var archeType = archeTypeData->ArcheType;
            ComponentConfigs = new ComponentConfig[archeType.ComponentConfigLength];
            fixed (ComponentConfig* configsPtr = &ComponentConfigs[0])
            {
                MemoryHelper.Copy(
                    archeType.ComponentConfigs,
                    configsPtr,
                    TypeCache<ComponentConfig>.SizeInBytes * archeType.ComponentConfigLength);
            }
            SharedComponentDatas = new IComponentData[archeType.SharedComponentDataLength];
            for (var i = 0; i < archeType.SharedComponentDataLength; i++)
            {
                var sharedDataIndex = archeType.SharedComponentDataIndexes[i];
                var config = EcsLte.ComponentConfigs.Instance.AllSharedConfigs[sharedDataIndex.SharedIndex];
                SharedComponentDatas[i] = context.SharedIndexDics.GetSharedIndexDic(config)
                    .GetComponentData(sharedDataIndex);
            }
            ComponentTypes = ComponentConfigs
                .Select(x => EcsLte.ComponentConfigs.Instance.AllComponentTypes[x.ComponentIndex])
                .ToArray();
            SharedComponents = SharedComponentDatas
                .Select(x => (ISharedComponent)x.Component)
                .ToArray();
        }

        internal bool TryGetArcheTypeIndex(EcsContext context, out ArcheTypeIndex archeTypeIndex) => _archeTypeIndexes.TryGetValue(context, out archeTypeIndex);

        internal void AddArcheTypeIndex(EcsContext context, ArcheTypeIndex archeTypeIndex)
        {
            lock (_lockObj)
            {
                _archeTypeIndexes.Add(context, archeTypeIndex);
            }
        }

        internal static EntityArcheTypeData AddComponentType(EntityArcheTypeData source, ComponentConfig config)
        {
            var dest = new EntityArcheTypeData
            {
                ComponentConfigs = Helper.CopyInsertSort(source.ComponentConfigs, config),
                SharedComponentDatas = source.SharedComponentDatas
            };

            dest.ComponentTypes = ComponentConfigsToTypes(dest.ComponentConfigs);
            dest.SharedComponents = source.SharedComponents;

            return dest;
        }

        internal static EntityArcheTypeData RemoveComponentType(EntityArcheTypeData source, ComponentConfig config)
        {
            var dest = new EntityArcheTypeData
            {
                ComponentConfigs = new ComponentConfig[source.ComponentConfigs.Length - 1],
                SharedComponentDatas = source.SharedComponentDatas
            };

            for (int sourceIndex = 0, destIndex = 0; sourceIndex < source.ComponentConfigs.Length; sourceIndex++)
            {
                if (source.ComponentConfigs[sourceIndex] != config)
                    dest.ComponentConfigs[destIndex++] = config;
            }

            dest.ComponentTypes = ComponentConfigsToTypes(dest.ComponentConfigs);
            dest.SharedComponents = source.SharedComponents;

            return dest;
        }

        internal static EntityArcheTypeData AddSharedComponent(EntityArcheTypeData source, IComponentData component)
        {
            var dest = new EntityArcheTypeData
            {
                ComponentConfigs = Helper.CopyInsertSort(source.ComponentConfigs, component.Config),
                SharedComponentDatas = Helper.CopyInsertSort(source.SharedComponentDatas, component)
            };

            dest.ComponentTypes = ComponentConfigsToTypes(dest.ComponentConfigs);
            dest.SharedComponents = SharedComponentDatasToSharedComponents(dest.SharedComponentDatas);

            return dest;
        }

        internal static EntityArcheTypeData ReplaceSharedComponent(EntityArcheTypeData source, IComponentData component)
        {
            var dest = new EntityArcheTypeData
            {
                ComponentConfigs = source.ComponentConfigs,
                SharedComponentDatas = new IComponentData[source.SharedComponentDatas.Length]
            };
            Array.Copy(source.SharedComponentDatas, dest.SharedComponentDatas, dest.SharedComponentDatas.Length);

            dest.ComponentTypes = source.ComponentTypes;
            dest.SharedComponents = new ISharedComponent[source.SharedComponents.Length];
            Array.Copy(source.SharedComponents, dest.SharedComponents, dest.SharedComponents.Length);

            for (var i = 0; i < dest.SharedComponentDatas.Length; i++)
            {
                if (dest.SharedComponentDatas[i].Config == component.Config)
                {
                    dest.SharedComponentDatas[i] = component;
                    dest.SharedComponents[i] = (ISharedComponent)component.Component;
                }
            }

            return dest;
        }

        internal static EntityArcheTypeData RemoveSharedComponent(EntityArcheTypeData source, ComponentConfig config)
        {
            var dest = new EntityArcheTypeData
            {
                ComponentConfigs = new ComponentConfig[source.ComponentConfigs.Length - 1],
                SharedComponentDatas = new IComponentData[source.SharedComponentDatas.Length - 1],
                ComponentTypes = new Type[source.ComponentTypes.Length - 1],
                SharedComponents = new ISharedComponent[source.SharedComponents.Length - 1]
            };

            for (int sourceIndex = 0, destIndex = 0; sourceIndex < source.ComponentConfigs.Length; sourceIndex++)
            {
                if (source.ComponentConfigs[sourceIndex] != config)
                {
                    dest.ComponentConfigs[destIndex] = config;
                    dest.ComponentTypes[destIndex] = source.ComponentTypes[sourceIndex];
                    destIndex++;
                }
            }
            for (int sourceIndex = 0, destIndex = 0; sourceIndex < source.SharedComponentDatas.Length; sourceIndex++)
            {
                if (source.SharedComponentDatas[sourceIndex].Config != config)
                {
                    dest.SharedComponentDatas[destIndex] = source.SharedComponentDatas[sourceIndex];
                    dest.SharedComponents[destIndex] = source.SharedComponents[sourceIndex];
                    destIndex++;
                }
            }

            return dest;
        }

        private static Type[] ComponentConfigsToTypes(ComponentConfig[] configs)
        {
            var types = new Type[configs.Length];
            for (var i = 0; i < configs.Length; i++)
                types[i] = EcsLte.ComponentConfigs.Instance.AllComponentTypes[configs[i].ComponentIndex];

            return types;
        }

        private static ISharedComponent[] SharedComponentDatasToSharedComponents(IComponentData[] componentDatas)
        {
            var components = new ISharedComponent[componentDatas.Length];
            for (var i = 0; i < componentDatas.Length; i++)
                components[i] = (ISharedComponent)componentDatas[i].Component;

            return components;
        }

        public static bool operator !=(EntityArcheTypeData lhs, EntityArcheTypeData rhs) => !(lhs == rhs);

        public static bool operator ==(EntityArcheTypeData lhs, EntityArcheTypeData rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs.ComponentConfigs.Length != rhs.ComponentConfigs.Length ||
                lhs.SharedComponentDatas.Length != rhs.SharedComponentDatas.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs.ComponentConfigs.Length; i++)
            {
                if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs.SharedComponentDatas.Length; i++)
            {
                if (!lhs.SharedComponentDatas[i].Equals(rhs.SharedComponentDatas[i]))
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArcheTypeData other) => this == other;

        public override bool Equals(object other) => other is EntityArcheTypeData obj && this == obj;

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(ComponentTypes.Length)
                    .AppendHashCode(SharedComponents.Length);
                foreach (var type in ComponentTypes)
                    hashCode = hashCode.AppendHashCode(type);
                foreach (var component in SharedComponents)
                    hashCode = hashCode.AppendHashCode(component);
                _hashCode = hashCode.HashCode;
            }

            return _hashCode;
        }
    }
}