namespace EcsLte
{
    internal abstract class BaseKeyData
    {
        public EntityEventHandler EntityAddedEvent { get; set; }
        public EntityEventHandler EntityRemovedEvent { get; set; }

        internal virtual void Reset()
        {
            EntityAddedEvent.Clear();
            EntityRemovedEvent.Clear();
        }
    }

    internal class PrimaryKeyData : BaseKeyData
    {
        public Entity Entity { get; set; }

        internal override void Reset()
        {
            base.Reset();
            Entity = Entity.Null;
        }
    }

    internal class SharedKeyData : BaseKeyData
    {
        private EntityManager _entityManager;

        public EntityCollection Entities { get; set; }

        public SharedKeyData()
        {
        }

        internal void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
            Entities = _entityManager.CreateEntityCollection();
        }

        internal override void Reset()
        {
            base.Reset();
            _entityManager.RemoveEntityCollection(Entities);
        }
    }
}