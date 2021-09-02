namespace EcsLte
{
    internal abstract class EntityCommand
    {
        protected EntityCommand(Entity entity)
        {
            QueuedEntity = entity;
        }

        public Entity QueuedEntity { get; }

        public abstract void ExecuteCommand(World world);
    }

    internal class CreateEntityCommand : EntityCommand
    {
        public CreateEntityCommand(Entity queuedEntity) : base(queuedEntity)
        {
        }

        public override void ExecuteCommand(World world)
        {
            world.EntityManager.DequeueEntityFromCommand(QueuedEntity);
        }
    }

    internal class DestroyEntityCommand : EntityCommand
    {
        public DestroyEntityCommand(Entity queuedEntity) : base(queuedEntity)
        {
        }

        public override void ExecuteCommand(World world)
        {
            world.EntityManager.DestroyEntity(QueuedEntity);
        }
    }

    internal class AddComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        private readonly TComponent _component;

        public AddComponentEntityCommand(Entity queuedEntity, TComponent component) : base(queuedEntity)
        {
            _component = component;
        }

        public override void ExecuteCommand(World world)
        {
            world.EntityManager.AddComponent(QueuedEntity, _component);
        }
    }

    internal class ReplaceComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        private readonly TComponent _newComponent;

        public ReplaceComponentEntityCommand(Entity queuedEntity, TComponent newComponent) : base(queuedEntity)
        {
            _newComponent = newComponent;
        }

        public override void ExecuteCommand(World world)
        {
            world.EntityManager.ReplaceComponent(QueuedEntity, _newComponent);
        }
    }

    internal class RemoveComponentEntityCommand<TComponent> : EntityCommand
        where TComponent : IComponent
    {
        public RemoveComponentEntityCommand(Entity queuedEntity) : base(queuedEntity)
        {
        }

        public override void ExecuteCommand(World world)
        {
            world.EntityManager.RemoveComponent<TComponent>(QueuedEntity);
        }
    }
}