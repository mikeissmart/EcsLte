using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommandQueue
    {
        private readonly List<IEntityCommand> _entityCommands;
        private readonly EcsContext _context;

        public string Name { get; private set; }

        internal EntityCommandQueue(EcsContext context, string name)
        {
            _entityCommands = new List<IEntityCommand>();
            _context = context;

            Name = name;
        }

        public void CreateEntity(EntityBlueprint blueprint) =>
            _entityCommands.Add(new EntityCommand_CreateEntities(1, blueprint));

        public void CreateEntities(int count, EntityBlueprint blueprint) =>
            _entityCommands.Add(new EntityCommand_CreateEntities(count, blueprint));

        public void DestroyEntity(Entity entity) =>
            _entityCommands.Add(new EntityCommand_DestroyEntities(new[] { entity }));

        public void DestroyEntities(IEnumerable<Entity> entities) =>
            _entityCommands.Add(new EntityCommand_DestroyEntities(entities));

        public void UpdateComponent<TComponent>(Entity entity, TComponent component) where TComponent : unmanaged, IComponent =>
            _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(new[] { entity }, component));

        public void UpdateComponent<TComponent>(IEnumerable<Entity> entities, TComponent component) where TComponent : unmanaged, IComponent =>
            _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(new List<Entity>(entities), component));

        public void UpdateComponent<TComponent>(EntityQuery query, TComponent component) where TComponent : unmanaged, IComponent =>
            _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(query.GetEntities(), component));

        public void ExecuteCommands()
        {
            foreach (var command in _entityCommands)
                command.Execute(_context);
            _entityCommands.Clear();
        }

        public void ClearCommands() => _entityCommands.Clear();
    }
}
