using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Utilities;

namespace EcsLte
{
    public class EntityCommandPlayback
    {
        private readonly List<EntityCommand> _commands;

        internal EntityCommandPlayback(World world, string name)
        {
            _commands = new List<EntityCommand>();

            Name = name;
            CurrentWorld = world;
        }

        public string Name { get; }
        public World CurrentWorld { get; }

        public Entity CreateEntity()
        {
            var entity = CurrentWorld.EntityManager.EnqueueEntityFromCommand();
            AppendCommand(entity, new CreateEntityCommand
            {
                QueuedEntity = entity
            });

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = CurrentWorld.EntityManager.EnqueueEntitiesFromCommand(count);
            foreach (var entity in entities)
                AppendCommand(entity, new CreateEntityCommand
                {
                    QueuedEntity = entity
                });

            return entities;
        }

        public void DestroyEntity(Entity entity)
        {
            AppendCommand(entity, new DestroyEntityCommand
            {
                QueuedEntity = entity
            });
        }

        public void DestroyEntities(Entity[] entities)
        {
            foreach (var entity in entities)
                AppendCommand(entity, new DestroyEntityCommand
                {
                    QueuedEntity = entity
                });
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            AppendCommand(entity, new AddComponentEntityCommand<TComponent>
            {
                QueuedEntity = entity,
                Component = component
            });
        }

        public void ReplaceComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            AppendCommand(entity, new ReplaceComponentEntityCommand<TComponent>
            {
                QueuedEntity = entity,
                Component = component
            });
        }

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            AppendCommand(entity, new RemoveComponentEntityCommand<TComponent>(entity));
        }

        public void RunCommands()
        {
            lock (_commands)
            {
                for (int i = 0; i < _commands.Count; i++)
                    _commands[i].ExecuteCommand(CurrentWorld);
                _commands.Clear();
            }
        }

        public void ClearCommands()
        {
            lock (_commands)
            {
                _commands.Clear();
            }
        }

        private void AppendCommand(Entity entity, EntityCommand entityCommand)
        {
            lock (_commands)
            {
                _commands.Add(entityCommand);
            }
        }
    }
}