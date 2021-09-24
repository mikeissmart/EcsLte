using System.Collections.Generic;

namespace EcsLte
{
    /*internal class SubEntityKey
    {
        private EntityManager _entityManager;

        public IComponent Component { get; set; }
        public List<Entity> Entities { get; set; }
        public EntityEventHandler EntityAdded { get; set; }
        public EntityEventHandler EntityRemoved { get; set; }

        public SubEntityKey()
        {

        }

        public override int GetHashCode()
        {
            return Component.GetHashCode();
        }

        #region Events

        private void PrimaryKeyOnEntityAdded(Entity entity)
        {
            lock (Entities)
            {
                Entities.Add(entity);
            }
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            EntityAdded.Invoke(entity);
        }

        private void PrimaryKeyOnEntityRemoved(Entity entity)
        {
            lock (Entities)
            {
                Entities.Clear();
            }
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            EntityRemoved.Invoke(entity);
        }

        private void SharedKeyOnEntityAdded(Entity entity)
        {
            lock (Entities)
            {
                Entities.Add(entity);
            }
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] += OnEntityWillBeDestroyed;
            EntityAdded.Invoke(entity);
        }

        private void SharedKeyOnEntityRemoved(Entity entity)
        {
            lock (Entities)
            {
                Entities.Remove(entity);
            }
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
            EntityRemoved.Invoke(entity);
        }

        private void OnEntityWillBeDestroyed(Entity entity)
        {
            lock (Entities)
            {
                Entities.Remove(entity);
            }
            _entityManager.EntityWillBeDestroyedEvents[entity.Id] -= OnEntityWillBeDestroyed;
        }

        #endregion

        #region ObjectCache

        internal void Initialize(EcsContext context)
        {
        }

        internal void Reset()
        {
            Entities.Clear();
        }

        #endregion
    }*/
}