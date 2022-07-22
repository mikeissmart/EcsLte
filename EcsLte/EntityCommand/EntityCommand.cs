using System.Collections.Generic;

namespace EcsLte
{
    internal interface IEntityCommand
    {
        void Execute(EcsContext context);
    }

    internal class EntityCommand_CreateEntities_EntityBlueprint : IEntityCommand
    {
        private readonly EntityBlueprint _blueprint;
        private readonly EntityState _state;
        private readonly int _count;

        public EntityCommand_CreateEntities_EntityBlueprint(EntityBlueprint blueprint, EntityState state, int count)
        {
            _blueprint = new EntityBlueprint(blueprint);
            _state = state;
            _count = count;
        }

        public void Execute(EcsContext context) => context.Entities.CreateEntities(_blueprint, _state, _count);
    }

    internal class EntityCommand_CreateEntities_EntityArcheType : IEntityCommand
    {
        private readonly EntityArcheType _archeType;
        private readonly EntityState _state;
        private readonly int _count;

        public EntityCommand_CreateEntities_EntityArcheType(EntityArcheType archeType, EntityState state, int count)
        {
            _archeType = new EntityArcheType(archeType);
            _state = state;
            _count = count;
        }

        public void Execute(EcsContext context) => context.Entities.CreateEntities(_archeType, _state, _count);
    }

    internal class EntityCommand_DestroyEntities : IEntityCommand
    {
        private readonly Entity[] _entities;

        public EntityCommand_DestroyEntities(Entity[] entities) => _entities = entities;

        public void Execute(EcsContext context) => context.Entities.DestroyEntities(_entities);
    }

    internal class EntityCommand_DestroyEntities_EntityArcheType : IEntityCommand
    {
        private readonly EntityArcheType _archeType;

        public EntityCommand_DestroyEntities_EntityArcheType(EntityArcheType archeType) => _archeType = new EntityArcheType(archeType);

        public void Execute(EcsContext context) => context.Entities.DestroyEntities(_archeType);
    }

    internal class EntityCommand_DestroyEntities_EntityFilter : IEntityCommand
    {
        private readonly EntityFilter _filter;

        public EntityCommand_DestroyEntities_EntityFilter(EntityFilter filter) => _filter = new EntityFilter(filter);

        public void Execute(EcsContext context) => context.Entities.DestroyEntities(_filter);
    }

    internal class EntityCommand_DestroyEntities_EntityTracker : IEntityCommand
    {
        private readonly EntityTracker _tracker;

        public EntityCommand_DestroyEntities_EntityTracker(EntityTracker tracker) => _tracker = new EntityTracker(tracker);

        public void Execute(EcsContext context)
        {
            context.Entities.DestroyEntities(_tracker);
            _tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_DestroyEntities_EntityQuery : IEntityCommand
    {
        private readonly EntityQuery _query;

        public EntityCommand_DestroyEntities_EntityQuery(EntityQuery query) => _query = new EntityQuery(query);

        public void Execute(EcsContext context)
        {
            context.Entities.DestroyEntities(_query);
            if (_query.Tracker != null)
                _query.Tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_ChangeEntitiesState : IEntityCommand
    {
        private readonly Entity[] _entities;
        private readonly EntityState _state;

        public EntityCommand_ChangeEntitiesState(Entity[] entities, EntityState state)
        {
            _entities = entities;
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.SetEntityStates(_entities, _state);
    }

    internal class EntityCommand_ChangeEntitiesState_EntityArcheType : IEntityCommand
    {
        private readonly EntityArcheType _archeType;
        private readonly EntityState _state;

        public EntityCommand_ChangeEntitiesState_EntityArcheType(EntityArcheType archeType, EntityState state)
        {
            _archeType = new EntityArcheType(archeType);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.SetEntityStates(_archeType, _state);
    }

    internal class EntityCommand_ChangeEntitiesState_EntityFilter : IEntityCommand
    {
        private readonly EntityFilter _filter;
        private readonly EntityState _state;

        public EntityCommand_ChangeEntitiesState_EntityFilter(EntityFilter filter, EntityState state)
        {
            _filter = new EntityFilter(filter);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.SetEntityStates(_filter, _state);
    }

    internal class EntityCommand_ChangeEntitiesState_EntityTracker : IEntityCommand
    {
        private readonly EntityTracker _tracker;
        private readonly EntityState _state;

        public EntityCommand_ChangeEntitiesState_EntityTracker(EntityTracker tracker, EntityState state)
        {
            _tracker = new EntityTracker(tracker);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.SetEntityStates(_tracker, _state);
            _tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_ChangeEntitiesState_EntityQuery : IEntityCommand
    {
        private readonly EntityQuery _query;
        private readonly EntityState _state;

        public EntityCommand_ChangeEntitiesState_EntityQuery(EntityQuery query, EntityState state)
        {
            _query = new EntityQuery(query);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.SetEntityStates(_query, _state);
            if (_query.Tracker != null)
                _query.Tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_DuplicateEntities : IEntityCommand
    {
        private readonly Entity[] _entities;
        private readonly EntityState? _state;

        public EntityCommand_DuplicateEntities(Entity[] entities, EntityState? state)
        {
            _entities = entities;
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.DuplicateEntities(_entities, _state);
    }

    internal class EntityCommand_DuplicateEntities_EntityArcheType : IEntityCommand
    {
        private readonly EntityArcheType _archeType;
        private readonly EntityState? _state;

        public EntityCommand_DuplicateEntities_EntityArcheType(EntityArcheType archeType, EntityState? state)
        {
            _archeType = new EntityArcheType(archeType);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.DuplicateEntities(_archeType, _state);
    }

    internal class EntityCommand_DuplicateEntities_EntityFilter : IEntityCommand
    {
        private readonly EntityFilter _filter;
        private readonly EntityState? _state;

        public EntityCommand_DuplicateEntities_EntityFilter(EntityFilter filter, EntityState? state)
        {
            _filter = new EntityFilter(filter);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.DuplicateEntities(_filter, _state);
    }

    internal class EntityCommand_DuplicateEntities_EntityTracker : IEntityCommand
    {
        private readonly EntityTracker _tracker;
        private readonly EntityState? _state;

        public EntityCommand_DuplicateEntities_EntityTracker(EntityTracker tracker, EntityState? state)
        {
            _tracker = new EntityTracker(tracker);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.DuplicateEntities(_tracker, _state);
            _tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_DuplicateEntities_EntityQuery : IEntityCommand
    {
        private readonly EntityQuery _query;
        private readonly EntityState? _state;

        public EntityCommand_DuplicateEntities_EntityQuery(EntityQuery query, EntityState? state)
        {
            _query = new EntityQuery(query);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.DuplicateEntities(_query, _state);
            if (_query.Tracker != null)
                _query.Tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_CopyEntities : IEntityCommand
    {
        private readonly EntityManager _srcEntityManager;
        private readonly Entity[] _entities;
        private readonly EntityState? _state;

        public EntityCommand_CopyEntities(EntityManager srcEntityManager, Entity[] entities, EntityState? state)
        {
            _srcEntityManager = srcEntityManager;
            _entities = entities;
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.CopyEntities(_srcEntityManager, _entities, _state);
    }

    internal class EntityCommand_CopyEntities_EntityArcheType : IEntityCommand
    {
        private readonly EntityManager _srcEntityManager;
        private readonly EntityArcheType _archeType;
        private readonly EntityState? _state;

        public EntityCommand_CopyEntities_EntityArcheType(EntityManager srcEntityManager, EntityArcheType archeType, EntityState? state)
        {
            _srcEntityManager = srcEntityManager;
            _archeType = new EntityArcheType(archeType);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.CopyEntities(_srcEntityManager, _archeType, _state);
    }

    internal class EntityCommand_CopyEntities_EntityFilter : IEntityCommand
    {
        private readonly EntityManager _srcEntityManager;
        private readonly EntityFilter _filter;
        private readonly EntityState? _state;

        public EntityCommand_CopyEntities_EntityFilter(EntityManager srcEntityManager, EntityFilter filter, EntityState? state)
        {
            _srcEntityManager = srcEntityManager;
            _filter = new EntityFilter(filter);
            _state = state;
        }

        public void Execute(EcsContext context) => context.Entities.CopyEntities(_srcEntityManager, _filter, _state);
    }

    internal class EntityCommand_CopyEntities_EntityTracker : IEntityCommand
    {
        private readonly EntityTracker _tracker;
        private readonly EntityState? _state;

        public EntityCommand_CopyEntities_EntityTracker(EntityTracker tracker, EntityState? state)
        {
            _tracker = new EntityTracker(tracker);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.CopyEntities(_tracker, _state);
            _tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_CopyEntities_EntityQuery : IEntityCommand
    {
        private readonly EntityQuery _query;
        private readonly EntityState? _state;

        public EntityCommand_CopyEntities_EntityQuery(EntityQuery query, EntityState? state)
        {
            _query = new EntityQuery(query);
            _state = state;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.CopyEntities(_query, _state);
            if (_query.Tracker != null)
                _query.Tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_UpdateComponent<TComponent> : IEntityCommand
        where TComponent : unmanaged, IGeneralComponent
    {
        private readonly Entity _entity;
        private readonly TComponent _component;

        public EntityCommand_UpdateComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateComponents<T1, T2> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3,
            _component4);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3,
            _component4,
            _component5);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;
        private readonly T6 _component6;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3,
            _component4,
            _component5,
            _component6);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
        where T7 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;
        private readonly T6 _component6;
        private readonly T7 _component7;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
            _component7 = component7;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3,
            _component4,
            _component5,
            _component6,
            _component7);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8> : IEntityCommand
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
        where T7 : unmanaged, IComponent
        where T8 : unmanaged, IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;
        private readonly T6 _component6;
        private readonly T7 _component7;
        private readonly T8 _component8;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7,
            T8 component8)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
            _component7 = component7;
            _component8 = component8;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateComponents(_entity,
            _component1,
            _component2,
            _component3,
            _component4,
            _component5,
            _component6,
            _component7,
            _component8);
    }

    internal class EntityCommand_UpdateSharedComponent<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly Entity _entity;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateSharedComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateSharedComponent_EntityArcheType<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityArcheType _archeType;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponent_EntityArcheType(EntityArcheType archeType, TComponent component)
        {
            _archeType = new EntityArcheType(archeType);
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateSharedComponent(_archeType, _component);
    }

    internal class EntityCommand_UpdateSharedComponents_EntityFilter<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityFilter _filter;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponents_EntityFilter(EntityFilter filter, TComponent component)
        {
            _filter = new EntityFilter(filter);
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateSharedComponents(_filter, _component);
    }

    internal class EntityCommand_UpdateSharedComponents_EntityTracker<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityTracker _tracker;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponents_EntityTracker(EntityTracker tracker, TComponent component)
        {
            _tracker = new EntityTracker(tracker);
            _component = component;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.UpdateSharedComponents(_tracker, _component);
            _tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_UpdateSharedComponents_EntityQuery<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly EntityQuery _query;
        private readonly TComponent _component;

        public EntityCommand_UpdateSharedComponents_EntityQuery(EntityQuery query, TComponent component)
        {
            _query = new EntityQuery(query);
            _component = component;
        }

        public void Execute(EcsContext context)
        {
            context.Entities.UpdateSharedComponents(_query, _component);
            if (_query.Tracker != null)
                _query.Tracker.InternalDestroy();
        }
    }

    internal class EntityCommand_UpdateUniqueComponent_Entity<TComponent> : IEntityCommand
        where TComponent : unmanaged, IUniqueComponent
    {
        private readonly Entity _entity;
        private readonly TComponent _component;

        public EntityCommand_UpdateUniqueComponent_Entity(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateUniqueComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateUniqueComponent<TComponent> : IEntityCommand
        where TComponent : unmanaged, IUniqueComponent
    {
        private readonly TComponent _component;

        public EntityCommand_UpdateUniqueComponent(TComponent component)
        {
            _component = component;
        }

        public void Execute(EcsContext context) => context.Entities.UpdateUniqueComponent(_component);
    }
}