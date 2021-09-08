namespace EcsLte
{
    internal interface EntityCommand
    {

        void ExecuteCommand(World world);
    }

    internal struct CreateEntityCommand : EntityCommand
    {
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(World world)
        {
            world.EntityManager.DequeueEntityFromCommand(QueuedEntity);
        }
    }

    internal struct DestroyEntityCommand : EntityCommand
    {
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(World world)
        {
            world.EntityManager.DestroyEntity(QueuedEntity);
        }
    }

    internal struct AddComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public TComponent Component { get; set; }
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(World world)
        {
            world.EntityManager.AddComponent(QueuedEntity, Component);
        }
    }

    internal struct ReplaceComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public TComponent Component { get; set; }
        public Entity QueuedEntity { get; set; }

        public void ExecuteCommand(World world)
        {
            world.EntityManager.ReplaceComponent(QueuedEntity, Component);
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

        public void ExecuteCommand(World world)
        {
            world.EntityManager.RemoveComponent<TComponent>(QueuedEntity);
        }
    }
}