namespace EcsLte
{
    internal interface EntityCommand
    {
        void ExecuteCommand(EcsContext context);
    }

    internal struct CreateEntityCommand : EntityCommand
    {
        public Entity QueuedEntity { get; set; }
        public EntityBlueprint Blueprint { get; set; }
        public EcsContextData ContextData { get; set; }

        public void ExecuteCommand(EcsContext context)
        {
            ContextData.DequeueEntityFromCommand(QueuedEntity, Blueprint);
        }
    }

    internal struct DestroyEntityCommand : EntityCommand
    {
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(EcsContext context)
        {
            context.DestroyEntity(QueuedEntity);
        }
    }

    internal struct AddComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public TComponent Component { get; set; }
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(EcsContext context)
        {
            context.AddComponent(QueuedEntity, Component);
        }
    }

    internal struct ReplaceComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public TComponent Component { get; set; }
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(EcsContext context)
        {
            context.ReplaceComponent(QueuedEntity, Component);
        }
    }

    internal struct RemoveComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public Entity QueuedEntity { get; set; }

        public RemoveComponentEntityCommand(Entity queuedEntity)
        {
            QueuedEntity = queuedEntity;
        }

        public void ExecuteCommand(EcsContext context)
        {
            context.RemoveComponent<TComponent>(QueuedEntity);
        }
    }

    internal struct RemoveAllComponentsEntityCommand : EntityCommand
    {
        public Entity QueuedEntity { get; set; }

        public RemoveAllComponentsEntityCommand(Entity queuedEntity)
        {
            QueuedEntity = queuedEntity;
        }

        public void ExecuteCommand(EcsContext context)
        {
            context.RemoveAllComponents(QueuedEntity);
        }
    }
}