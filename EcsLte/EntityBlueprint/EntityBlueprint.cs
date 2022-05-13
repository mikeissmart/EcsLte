using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Linq;

namespace EcsLte
{
    public class EntityBlueprint
    {
        private EntityArcheType _prevEntityArcheType;

        internal IEntityBlueprintComponentData[] AllBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] SharedBlueprintComponents { get; private set; }
        internal IEntityBlueprintComponentData[] UniqueBlueprintComponents { get; private set; }
        internal EcsContext Context { get; set; }
        internal ArcheTypeIndex? ArcheTypeIndex { get; set; }

        public EntityBlueprint() { }

        private EntityBlueprint(EcsContext context,
            ArcheTypeIndex? archeTypeIndex,
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
            Context = context;
            ArcheTypeIndex = archeTypeIndex;
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

        public EntityBlueprint AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index != -1)
                throw new EntityBlueprintAlreadyHasComponentException(typeof(TComponent));
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint(
                Context,
                null,
                Helper.CopyInsertSort(AllBlueprintComponents, blueprintComponent));
        }

        public EntityBlueprint RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));
            if (AllBlueprintComponents.Length == 1)
                return new EntityBlueprint(Context, null, null);

            return new EntityBlueprint(Context, null, AllBlueprintComponents
                .Where(x => x.Config != config)
                .ToArray());
        }

        public EntityBlueprint UpdateComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            var blueprintComponent = new EntityBlueprintComponentData<TComponent>(component, config);

            return new EntityBlueprint(
                Context,
                index != -1 && !config.IsShared
                    ? ArcheTypeIndex
                    : null,
                index == -1
                    ? Helper.CopyInsertSort(AllBlueprintComponents, blueprintComponent)
                    : CopyReplace(AllBlueprintComponents, index, blueprintComponent));
        }

        public EntityArcheType GetEntityArcheType(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);

            if (_prevEntityArcheType == null || _prevEntityArcheType.Context != context)
            {
                if (AllBlueprintComponents == null)
                    throw new ArgumentNullException(nameof(AllBlueprintComponents));
                _prevEntityArcheType = new EntityArcheType(context, this);
            }

            return _prevEntityArcheType;
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

        /* TODO remove
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
        }*/

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
