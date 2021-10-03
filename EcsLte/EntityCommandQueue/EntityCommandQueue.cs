using System.Collections.Generic;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityCommandQueue : IEcsContext, IEntityLife, IComponentLife
    {
        private readonly EntityCommandQueueData _data;
        private EcsContextData _ecsContextData;

        internal EntityCommandQueue(EcsContext context, EcsContextData ecsContextData, string name)
        {
            _data = ObjectCache.Pop<EntityCommandQueueData>();
            _data.Initialize();

            _ecsContextData = ecsContextData;

            CurrentContext = context;
            Name = name;
        }

        #region EscContext

        public EcsContext CurrentContext { get; }

        #endregion

        #region EntityCommandQueue

        public string Name { get; }

        public void RunCommands()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            lock (_ecsContextData.AllWatchers)
            {
                for (int i = 0; i < _ecsContextData.AllWatchers.Count; i++)
                    _ecsContextData.AllWatchers[i].ClearEntities();
            }

            lock (_data.Commands)
            {
                for (var i = 0; i < _data.Commands.Count; i++)
                    _data.Commands[i].ExecuteCommand(CurrentContext);
                _data.Commands.Clear();
            }
        }

        public void ClearCommands()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            lock (_data.Commands)
            {
                _data.Commands.Clear();
            }
        }

        private void AppendCommand(Entity entity, EntityCommand entityCommand)
        {
            lock (_data.Commands)
            {
                _data.Commands.Add(entityCommand);
            }
        }

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);
        }

        #endregion

        #region EntityLife

        public Entity CreateEntity()
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            var entity = CurrentContext.EnqueueEntityFromCommand();
            AppendCommand(entity, new CreateEntityCommand
            {
                QueuedEntity = entity
            });

            return entity;
        }

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            var entity = CurrentContext.EnqueueEntityFromCommand();
            AppendCommand(entity, new CreateEntityCommand
            {
                QueuedEntity = entity,
                Blueprint = blueprint
            });

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            var entities = CurrentContext.EnqueueEntitiesFromCommand(count);
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                AppendCommand(entity, new CreateEntityCommand
                {
                    QueuedEntity = entity
                });
            }

            return entities;
        }

        public Entity[] CreateEntities(int count, EntityBlueprint blueprint)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            var entities = CurrentContext.EnqueueEntitiesFromCommand(count);
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                AppendCommand(entity, new CreateEntityCommand
                {
                    QueuedEntity = entity,
                    Blueprint = blueprint
                });
            }

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AppendCommand(entity, new DestroyEntityCommand
            {
                QueuedEntity = entity
            });
        }

        public void DestroyEntities(ICollection<Entity> entities)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            foreach (var entity in entities)
                AppendCommand(entity, new DestroyEntityCommand
                {
                    QueuedEntity = entity
                });
        }

        #endregion

        #region ComponentLife

        public void AddUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique componentUnique)
            where TComponentUnique : IUniqueComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AddComponent(entity, componentUnique);
        }

        public void ReplaceUniqueComponent<TComponentUnique>(Entity entity, TComponentUnique newComponentUnique)
            where TComponentUnique : IUniqueComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            ReplaceComponent(entity, newComponentUnique);
        }

        public void RemoveUniqueComponent<TComponentUnique>(Entity entity)
            where TComponentUnique : IUniqueComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            RemoveComponent<TComponentUnique>(entity);
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AppendCommand(entity, new AddComponentEntityCommand<TComponent>
            {
                QueuedEntity = entity,
                Component = component
            });
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent newComponent)
            where TComponent : IComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AppendCommand(entity, new ReplaceComponentEntityCommand<TComponent>
            {
                QueuedEntity = entity,
                Component = newComponent
            });
        }

        public void RemoveComponent<TComponent>(Entity entity) where TComponent : IComponent
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AppendCommand(entity, new RemoveComponentEntityCommand<TComponent>(entity));
        }

        public void RemoveAllComponents(Entity entity)
        {
            if (CurrentContext.IsDestroyed)
                throw new EcsContextIsDestroyedException(CurrentContext);

            AppendCommand(entity, new RemoveAllComponentsEntityCommand(entity));
        }

        #endregion
    }
}