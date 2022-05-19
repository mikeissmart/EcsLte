using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Linq;

namespace EcsLte
{
    public class EntityBlueprint
    {
        private EntityArcheType _archeType;

        public IComponent[] Components { get; private set; }
        internal IComponentData[] AllBlueprintComponents { get; private set; }
        internal IComponentData[] SharedBlueprintComponents { get; private set; }
        internal IComponentData[] UniqueBlueprintComponents { get; private set; }

        public EntityBlueprint()
        {
            Components = new IComponent[0];
            AllBlueprintComponents = new IComponentData[0];
            SharedBlueprintComponents = new IComponentData[0];
            UniqueBlueprintComponents = new IComponentData[0];
        }

        private EntityBlueprint(IComponentData[] allBlueprintComponents)
        {
            Components = allBlueprintComponents
                .Select(x => x.Component)
                .ToArray();
            AllBlueprintComponents = allBlueprintComponents;
            SharedBlueprintComponents = allBlueprintComponents
                .Where(x => x.Config.IsShared)
                .ToArray();
            UniqueBlueprintComponents = allBlueprintComponents
                .Where(x => x.Config.IsUnique)
                .ToArray();
        }

        private EntityBlueprint(EntityArcheType archeType,
            IComponentData[] allBlueprintComponents)
        {
            _archeType = archeType;
            Components = allBlueprintComponents
                .Select(x => x.Component)
                .ToArray();
            AllBlueprintComponents = allBlueprintComponents;
            SharedBlueprintComponents = allBlueprintComponents
                .Where(x => x.Config.IsShared)
                .ToArray();
            UniqueBlueprintComponents = allBlueprintComponents
                .Where(x => x.Config.IsUnique)
                .ToArray();
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
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
            var index = IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config);
            if (index != -1)
                throw new EntityBlueprintAlreadyHasComponentException(typeof(TComponent));

            return new EntityBlueprint(Helper.CopyInsertSort(AllBlueprintComponents, new ComponentData<TComponent>(component)));
        }

        public EntityBlueprint RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));
            if (AllBlueprintComponents.Length == 1)
                return new EntityBlueprint();

            return new EntityBlueprint(AllBlueprintComponents
                    .Where(x => x.Config != config)
                    .ToArray());
        }

        public EntityBlueprint UpdateComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                return AddComponent(component);

            var componentData = new ComponentData<TComponent>(component);

            var componentDatas = new IComponentData[AllBlueprintComponents.Length];
            Array.Copy(AllBlueprintComponents, componentDatas, componentDatas.Length);
            componentDatas[index] = componentData;

            if (config.IsShared)
                return new EntityBlueprint(componentDatas);
            else
                return new EntityBlueprint(_archeType, componentDatas);
        }

        public EntityArcheType GetEntityArcheType()
        {
            if (_archeType == null)
                _archeType = new EntityArcheType(AllBlueprintComponents, SharedBlueprintComponents);

            return _archeType;
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
    }
}
