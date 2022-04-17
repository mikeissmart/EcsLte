using EcsLte.Exceptions;
using System;
using System.Linq;

namespace EcsLte.HybridArcheType
{
    public class EntityBlueprint_Hybrid
    {
        internal IEntityBlueprintComponentData[] AllBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] SharedBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] UniqueBlueprintComponents { get; private set; }
        internal ComponentEntityFactory_Hybrid ComponentEntityFactory { get; set; }
        internal unsafe ArcheTypeData_Hybrid* ArcheTypeData { get; set; }

        public EntityBlueprint_Hybrid() { }

        private EntityBlueprint_Hybrid(ComponentEntityFactory_Hybrid componentEntityFactory, IEntityBlueprintComponentData[] components)
        {
            ComponentEntityFactory = componentEntityFactory;
            AllBlueprintComponents = components;
            if (components != null)
            {
                SharedBlueprintComponents = components
                    .Where(x => x.Config.IsShared)
                    .ToArray();
                UniqueBlueprintComponents = components
                    .Where(x => x.Config.IsUnique)
                    .ToArray();
            }
        }

        private unsafe EntityBlueprint_Hybrid(ComponentEntityFactory_Hybrid componentEntityFactory, ArcheTypeData_Hybrid* archeTypeData, IEntityBlueprintComponentData[] components)
        {
            ComponentEntityFactory = componentEntityFactory;
            ArcheTypeData = archeTypeData;
            AllBlueprintComponents = components;
            if (components != null)
            {
                SharedBlueprintComponents = components
                    .Where(x => x.Config.IsShared)
                    .ToArray();
                UniqueBlueprintComponents = components
                    .Where(x => x.Config.IsUnique)
                    .ToArray();
            }
        }

        public bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent => IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config) != -1;

        public TComponent GetComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var index = IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)AllBlueprintComponents[index].Component;
        }

        public EntityBlueprint_Hybrid AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index != -1)
                throw new EntityBlueprintAlreadyHasComponentException(typeof(TComponent));
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint_Hybrid(
                ComponentEntityFactory,
                index == -1
                    ? CopyInsertSort(AllBlueprintComponents, blueprintComponent)
                    : CopyReplace(AllBlueprintComponents, index, blueprintComponent));
        }

        public unsafe EntityBlueprint_Hybrid UpdateComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint_Hybrid(
                ComponentEntityFactory,
                index != -1 && !config.IsShared
                    ? ArcheTypeData
                    : null,
                index == -1
                    ? CopyInsertSort(AllBlueprintComponents, blueprintComponent)
                    : CopyReplace(AllBlueprintComponents, index, blueprintComponent));
        }

        public EntityBlueprint_Hybrid RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
            {
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));
            }
            else if (AllBlueprintComponents.Length == 1)
            {
                return new EntityBlueprint_Hybrid(ComponentEntityFactory, null);
            }
            else
            {
                return new EntityBlueprint_Hybrid(ComponentEntityFactory, AllBlueprintComponents
                    .Where(x => x.Config != config)
                    .ToArray());
            }
        }

        private int IndexOfBlueprintComponent(ComponentConfig config)
        {
            if (AllBlueprintComponents != null)
            {
                for (var i = 0; i < AllBlueprintComponents.Length; i++)
                {
                    var blueprintComponent = AllBlueprintComponents[i];
                    if (blueprintComponent.Config == config)
                        return i;
                }
            }

            return -1;
        }

        private static IEntityBlueprintComponentData[] CopyInsertSort(IEntityBlueprintComponentData[] source, IEntityBlueprintComponentData insert)
        {
            IEntityBlueprintComponentData[] destination;
            if (source != null)
            {
                destination = new IEntityBlueprintComponentData[source.Length + 1];
                var index = source.Length - 1;
                for (; index >= 0; index--)
                {
                    if (source[index].Config.ComponentIndex < insert.Config.ComponentIndex)
                        break;
                }

                // Copy before insert
                if (index >= 0)
                    Array.Copy(source, 0, destination, 0, index + 1);

                // insert
                destination[index + 1] = insert;

                // Copy after insert
                if (source.Length - 1 != index)
                    Array.Copy(source, index + 1, destination, index + 2, source.Length - (index + 1));
            }
            else
            {
                destination = new IEntityBlueprintComponentData[1];
                destination[0] = insert;
            }

            return destination;
        }

        private static IEntityBlueprintComponentData[] CopyReplace(IEntityBlueprintComponentData[] source, int index, IEntityBlueprintComponentData insert)
        {
            IEntityBlueprintComponentData[] destination;
            if (source != null)
            {
                destination = new IEntityBlueprintComponentData[source.Length];
                Array.Copy(source, destination, source.Length);
                destination[index] = insert;
            }
            else
            {
                destination = new IEntityBlueprintComponentData[1];
                destination[0] = insert;
            }

            return destination;
        }
    }
}
