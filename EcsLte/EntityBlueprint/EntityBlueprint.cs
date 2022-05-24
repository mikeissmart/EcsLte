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
        internal IComponentData[] AllComponentDatas { get; private set; }
        internal IComponentData[] SharedComponentDatas { get; private set; }
        internal IComponentData[] UniqueComponentDatas { get; private set; }
        internal IComponentData[] BlittableComponentDatas { get; private set; }
        internal IComponentData[] ManagedComponentDatas { get; private set; }

        public EntityBlueprint()
        {
            Components = new IComponent[0];
            AllComponentDatas = new IComponentData[0];
            SharedComponentDatas = new IComponentData[0];
            UniqueComponentDatas = new IComponentData[0];
            BlittableComponentDatas = new IComponentData[0];
            ManagedComponentDatas = new IComponentData[0];
        }

        private EntityBlueprint(EntityArcheType archeType, IComponentData[] allComponentDatas)
        {
            _archeType = archeType;
            Components = allComponentDatas
                .Select(x => x.Component)
                .ToArray();
            AllComponentDatas = allComponentDatas;
            SharedComponentDatas = allComponentDatas
                .Where(x => x.Config.IsShared)
                .ToArray();
            UniqueComponentDatas = allComponentDatas
                .Where(x => x.Config.IsUnique)
                .ToArray();
            BlittableComponentDatas = allComponentDatas
                .Where(x => x.Config.IsBlittable)
                .ToArray();
            ManagedComponentDatas = allComponentDatas
                .Where(x => x.Config.IsManaged)
                .ToArray();
        }

        public bool HasComponent<TComponent>() where TComponent : IComponent
            => IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config) != -1;

        public TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            var index = IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));

            return (TComponent)AllComponentDatas[index].Component;
        }

        public EntityBlueprint AddComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            var index = IndexOfBlueprintComponent(ComponentConfig<TComponent>.Config);
            if (index != -1)
                throw new EntityBlueprintAlreadyHasComponentException(typeof(TComponent));

            return new EntityBlueprint(null, Helper.CopyInsertSort(AllComponentDatas, new ComponentData<TComponent>(component)));
        }

        public EntityBlueprint RemoveComponent<TComponent>() where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                throw new EntityBlueprintNotHaveComponentException(typeof(TComponent));
            if (AllComponentDatas.Length == 1)
                return new EntityBlueprint();

            return new EntityBlueprint(null,
                AllComponentDatas
                    .Where(x => x.Config != config)
                    .ToArray());
        }

        public EntityBlueprint UpdateComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            var config = ComponentConfig<TComponent>.Config;
            var index = IndexOfBlueprintComponent(config);
            if (index == -1)
                return AddComponent(component);

            var componentData = new ComponentData<TComponent>(component);

            var componentDatas = new IComponentData[AllComponentDatas.Length];
            Array.Copy(AllComponentDatas, componentDatas, componentDatas.Length);
            componentDatas[index] = componentData;

            if (config.IsShared)
                return new EntityBlueprint(null, componentDatas);
            else
                return new EntityBlueprint(_archeType, componentDatas);
        }

        public EntityArcheType GetEntityArcheType()
        {
            if (_archeType == null)
                _archeType = new EntityArcheType(AllComponentDatas, SharedComponentDatas);

            return _archeType;
        }

        private int IndexOfBlueprintComponent(ComponentConfig config)
        {
            if (AllComponentDatas != null)
            {
                for (var i = 0; i < AllComponentDatas.Length; i++)
                {
                    var blueprintComponent = AllComponentDatas[i];
                    if (blueprintComponent.Config == config)
                        return i;
                }
            }

            return -1;
        }
    }
}