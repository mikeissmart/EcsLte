using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
    public class EntityBlueprint
    {
        internal IEntityBlueprintComponentData[] AllBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] SharedBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] UniqueBlueprintComponents { get; private set; }
        internal ComponentEntityFactory ComponentEntityFactory { get; set; }
        internal unsafe ArcheTypeData* ArcheTypeData { get; set; }

        public EntityBlueprint() { }

        private unsafe EntityBlueprint(ComponentEntityFactory componentEntityFactory,
            ArcheTypeData* archeTypeData,
            IEntityBlueprintComponentData[] components)
        {
            if (components != null)
            {
                SharedBlueprintComponents = components
                    .Where(x => x.Config.IsShared)
                    .ToArray();
                UniqueBlueprintComponents = components
                    .Where(x => x.Config.IsUnique)
                    .ToArray();
            }
            ComponentEntityFactory = componentEntityFactory;
            ArcheTypeData = archeTypeData;
            AllBlueprintComponents = components;
        }

        public bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent
            => IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config) != -1;

        public TComponent GetComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var index = IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)AllBlueprintComponents[index].Component;
        }

        public unsafe EntityBlueprint AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index != -1)
                throw new EntityBlueprintAlreadyHasComponentException(typeof(TComponent));
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint(
                ComponentEntityFactory,
                null,
                CopyInsertSort(AllBlueprintComponents, blueprintComponent));
        }

        public unsafe EntityBlueprint RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));
            if (AllBlueprintComponents.Length == 1)
                return new EntityBlueprint(ComponentEntityFactory, null, null);

            return new EntityBlueprint(ComponentEntityFactory, null, AllBlueprintComponents
                .Where(x => x.Config != config)
                .ToArray());
        }

        public unsafe EntityBlueprint UpdateComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint(
                ComponentEntityFactory,
                index != -1 && !config.IsShared
                    ? ArcheTypeData
                    : null,
                index == -1
                    ? CopyInsertSort(AllBlueprintComponents, blueprintComponent)
                    : CopyReplace(AllBlueprintComponents, index, blueprintComponent));
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
