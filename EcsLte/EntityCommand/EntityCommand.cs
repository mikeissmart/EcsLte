using System.Collections.Generic;

namespace EcsLte
{
    internal interface IEntityCommand
    {
        void Execute(EcsContext context);
    }

    internal class EntityCommand_CreateEntities : IEntityCommand
    {
        private readonly int _count;
        private readonly EntityBlueprint _blueprint;

        public EntityCommand_CreateEntities(int count, EntityBlueprint blueprint)
        {
            _count = count;
            _blueprint = blueprint;
        }

        public void Execute(EcsContext context) => context.EntityManager.CreateEntities(_count, _blueprint);
    }

    internal class EntityCommand_DestroyEntities : IEntityCommand
    {
        private readonly IEnumerable<Entity> _entities;

        public EntityCommand_DestroyEntities(IEnumerable<Entity> entities) => _entities = entities;

        public void Execute(EcsContext context) => context.EntityManager.DestroyEntities(_entities);
    }

    internal class EntityCommand_UpdateComponent<TComponent> : IEntityCommand
         where TComponent : unmanaged, IComponent
    {
        private readonly IEnumerable<Entity> _entities;
        private readonly TComponent _component;

        public EntityCommand_UpdateComponent(IEnumerable<Entity> entities, TComponent component)
        {
            _entities = entities;
            _component = component;
        }

        public void Execute(EcsContext context) => context.EntityManager.UpdateComponent(_entities, _component);
    }
}
