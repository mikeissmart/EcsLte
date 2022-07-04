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

        public void Execute(EcsContext context) => context.CreateEntities(_count, _blueprint);
    }

    internal class EntityCommand_DestroyEntities : IEntityCommand
    {
        private readonly IEnumerable<Entity> _entities;

        public EntityCommand_DestroyEntities(IEnumerable<Entity> entities) => _entities = entities;

        public void Execute(EcsContext context) => context.DestroyEntities(_entities);
    }

    internal class EntityCommand_DestroyEntities_EntityArcheType : IEntityCommand
    {
        private readonly EntityArcheType _entityArcheType;

        public EntityCommand_DestroyEntities_EntityArcheType(EntityArcheType entityArcheType) => _entityArcheType = entityArcheType;

        public void Execute(EcsContext context) => context.DestroyEntities(_entityArcheType);
    }

    internal class EntityCommand_DestroyEntities_EntityQuery : IEntityCommand
    {
        private readonly EntityQuery _entityQuery;

        public EntityCommand_DestroyEntities_EntityQuery(EntityQuery entityQuery) => _entityQuery = entityQuery;

        public void Execute(EcsContext context) => context.DestroyEntities(_entityQuery);
    }

    internal class EntityCommand_TransferEntities : IEntityCommand
    {
        private readonly EcsContext _sourceContext;
        private readonly IEnumerable<Entity> _entities;
        private readonly bool _destroyEntities;

        public EntityCommand_TransferEntities(EcsContext sourceContext, IEnumerable<Entity> entities, bool destroyEntities)
        {
            _sourceContext = sourceContext;
            _entities = entities;
            _destroyEntities = destroyEntities;
        }

        public void Execute(EcsContext context) => context.TransferEntities(_sourceContext, _entities, _destroyEntities);
    }

    internal class EntityCommand_TransferEntities_EntityArcheType : IEntityCommand
    {
        private readonly EcsContext _sourceContext;
        private readonly EntityArcheType _entityArcheType;
        private readonly bool _destroyEntities;

        public EntityCommand_TransferEntities_EntityArcheType(EcsContext sourceContext, EntityArcheType entityArcheType, bool destroyEntities)
        {
            _sourceContext = sourceContext;
            _entityArcheType = entityArcheType;
            _destroyEntities = destroyEntities;
        }

        public void Execute(EcsContext context) => context.TransferEntities(_sourceContext, _entityArcheType, _destroyEntities);
    }

    internal class EntityCommand_TransferEntities_EntityQuery : IEntityCommand
    {
        private readonly EcsContext _sourceContext;
        private readonly EntityQuery _entityQuery;
        private readonly bool _destroyEntities;

        public EntityCommand_TransferEntities_EntityQuery(EcsContext sourceContext, EntityQuery entityQuery, bool destroyEntities)
        {
            _sourceContext = sourceContext;
            _entityQuery = entityQuery;
            _destroyEntities = destroyEntities;
        }

        public void Execute(EcsContext context) => context.TransferEntities(_sourceContext, _entityQuery, _destroyEntities);
    }

    internal class EntityCommand_UpdateComponent<TComponent> : IEntityCommand
         where TComponent : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly TComponent _component;

        public EntityCommand_UpdateComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context) => context.UpdateComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateSharedComponent_Entities<TComponent> : IEntityCommand
         where TComponent : unmanaged, ISharedComponent
    {
        private readonly IEnumerable<Entity> _entities;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponent_Entities(IEnumerable<Entity> entities, TComponent component)
        {
            _entities = entities;
            _component = component;
        }

        public void Execute(EcsContext context) => context.UpdateSharedComponent(_entities, _component);
    }

    internal class EntityCommand_UpdateSharedComponent_EntityArcheType<TComponent> : IEntityCommand
         where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityArcheType _entityArcheType;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponent_EntityArcheType(EntityArcheType entityArcheType, TComponent component)
        {
            _entityArcheType = entityArcheType;
            _component = component;
        }

        public void Execute(EcsContext context) => context.UpdateSharedComponent(_entityArcheType, _component);
    }

    internal class EntityCommand_UpdateSharedComponent_EntityQuery<TComponent> : IEntityCommand
         where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityQuery _entityQuery;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponent_EntityQuery(EntityQuery entityQuery, TComponent component)
        {
            _entityQuery = entityQuery;
            _component = component;
        }

        public void Execute(EcsContext context) => context.UpdateSharedComponent(_entityQuery, _component);
    }
}