using EcsLte.Exceptions;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommandQueue
    {
        private readonly List<IEntityCommand> _entityCommands;
        private readonly object _lockObj;

        public string Name { get; private set; }
        public EcsContext Context { get; private set; }

        internal EntityCommandQueue(EcsContext context, string name)
        {
            _entityCommands = new List<IEntityCommand>();
            Context = context;
            _lockObj = new object();

            Name = name;
        }

        public void CreateEntity(EntityBlueprint blueprint)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_CreateEntities(1, blueprint));
            }
        }

        public void CreateEntities(int count, EntityBlueprint blueprint)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_CreateEntities(count, blueprint));
            }
        }

        public void DestroyEntity(Entity entity)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_DestroyEntities(new[] { entity }));
            }
        }

        public void DestroyEntities(IEnumerable<Entity> entities)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_DestroyEntities(entities));
            }
        }

        public void DestroyEntities(EntityArcheType entityArcheType)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityArcheType(entityArcheType));
            }
        }

        public void DestroyEntities(EntityQuery entityQuery)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityQuery(entityQuery));
            }
        }

        public void TransferEntity(EcsContext sourceContext, Entity entity, bool destroyEntity)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_TransferEntities(sourceContext, new[] { entity }, destroyEntity));
            }
        }

        public void TransferEntities(EcsContext sourceContext, IEnumerable<Entity> entities, bool destroyEntities)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_TransferEntities(sourceContext, entities, destroyEntities));
            }
        }

        public void TransferEntities(EcsContext sourceContext, EntityArcheType entityArcheType, bool destroyEntities)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_TransferEntities_EntityArcheType(sourceContext, entityArcheType, destroyEntities));
            }
        }

        public void TransferEntities(EcsContext sourceContext, EntityQuery entityQuery, bool destroyEntities)
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_TransferEntities_EntityQuery(sourceContext, entityQuery, destroyEntities));
            }
        }

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(new[] { entity }, component));
            }
        }

        public void UpdateComponent<TComponent>(IEnumerable<Entity> entities, TComponent component) where TComponent : unmanaged, IComponent
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(new List<Entity>(entities), component));
            }
        }

        public void UpdateComponent<TComponent>(EntityArcheType entityArcheType, TComponent component) where TComponent : unmanaged, IComponent
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_UpdateComponent_EntityArcheType<TComponent>(entityArcheType, component));
            }
        }

        public void UpdateComponent<TComponent>(EntityQuery query, TComponent component) where TComponent : unmanaged, IComponent
        {
            lock (_lockObj)
            {
                _entityCommands.Add(new EntityCommand_UpdateComponent_EntityQuery<TComponent>(query, component));
            }
        }

        public void ExecuteCommands()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);
            lock (_lockObj)
            {
                foreach (var command in _entityCommands)
                    command.Execute(Context);
                _entityCommands.Clear();
            }
        }

        public void ClearCommands()
        {
            lock (_lockObj)
            {
                _entityCommands.Clear();
            }
        }
    }
}