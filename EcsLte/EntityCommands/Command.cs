using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal interface IEntityCommand
    {
        void Execute(EcsContext context, ref Entity[] cachedEntities);
    }

    internal interface IEntityQueryCommand : IEntityCommand
    {
        void QueryExecute(EcsContext context);
    }

    #region ComponentAdd

    internal class EntityCommand_AddComponent<TComponent> : IEntityCommand
            where TComponent : unmanaged, IGeneralComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_AddComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.AddComponent(_entity, _component);
    }

    internal class EntityCommand_AddManagedComponent<TComponent> : IEntityCommand
            where TComponent : IManagedComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_AddManagedComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.AddManagedComponent(_entity, _component);
    }

    internal class EntityCommand_AddSharedComponent<TComponent> : IEntityCommand
            where TComponent : unmanaged, ISharedComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_AddSharedComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.AddSharedComponent(_entity, _component);
    }

    #endregion

    #region ComponentRemove

    internal class EntityCommand_RemoveComponent<TComponent> : IEntityCommand
            where TComponent : unmanaged, IGeneralComponent
    {
        private Entity _entity;

        internal EntityCommand_RemoveComponent(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.RemoveComponent<TComponent>(_entity);
    }

    internal class EntityCommand_RemoveManagedComponent<TComponent> : IEntityCommand
        where TComponent : IManagedComponent
    {
        private Entity _entity;

        internal EntityCommand_RemoveManagedComponent(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.RemoveManagedComponent<TComponent>(_entity);
    }

    internal class EntityCommand_RemoveSharedComponent<TComponent> : IEntityCommand
        where TComponent : unmanaged, ISharedComponent
    {
        private Entity _entity;

        internal EntityCommand_RemoveSharedComponent(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.RemoveSharedComponent<TComponent>(_entity);
    }

    #endregion

    #region ComponentsAdd

    internal class EntityCommand_AddComponents<T1, T2> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;

        public EntityCommand_AddComponents(Entity entity,
            T1 component1,
            T2 component2)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;

        public EntityCommand_AddComponents(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3)
        {
            _entity = entity;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3, T4> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;

        public EntityCommand_AddComponents(Entity entity,
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

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3, T4, T5> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;

        public EntityCommand_AddComponents(Entity entity,
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

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;
        private readonly T6 _component6;

        public EntityCommand_AddComponents(Entity entity,
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

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6, T7> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        private readonly Entity _entity;
        private readonly T1 _component1;
        private readonly T2 _component2;
        private readonly T3 _component3;
        private readonly T4 _component4;
        private readonly T5 _component5;
        private readonly T6 _component6;
        private readonly T7 _component7;

        public EntityCommand_AddComponents(Entity entity,
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

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7);
    }

    internal class EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6, T7, T8> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
        where T8 : IComponent
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

        public EntityCommand_AddComponents(Entity entity,
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

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.AddComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7,
                _component8);
    }

    #endregion

    #region ComponentsRemove

    internal class EntityCommand_RemoveComponents<T1, T2> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3, T4> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3, T4>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3, T4, T5> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3, T4, T5>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3, T4, T5, T6>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6, T7> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3, T4, T5, T6, T7>(_entity);
    }

    internal class EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6, T7, T8> : IEntityCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
        where T8 : IComponent
    {
        private readonly Entity _entity;

        public EntityCommand_RemoveComponents(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.RemoveComponents<T1, T2, T3, T4, T5, T6, T7, T8>(_entity);
    }

    #endregion

    #region ComponentsUpdate

    internal class EntityCommand_UpdateComponents<T1> : IEntityQueryCommand
        where T1 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
        private readonly T1 _component1;

        public EntityCommand_UpdateComponents(Entity entity,
            T1 component1)
        {
            _entity = entity;
            _component1 = component1;
        }

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1);
    }

    internal class EntityCommand_UpdateComponents<T1, T2> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2,
            T3 component3)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3,
                _component4);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7)
        {
            _entity = entity;
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
            _component7 = component7;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7);
    }

    internal class EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8> : IEntityQueryCommand
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
        where T5 : IComponent
        where T6 : IComponent
        where T7 : IComponent
        where T8 : IComponent
    {
        private readonly Entity _entity;
        private readonly int _entityIndex;
        private readonly ArcheTypeIndex _archeTypeIndex;
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

        public EntityCommand_UpdateComponents(Entity entity, int entityIndex, ArcheTypeIndex archeTypeIndex,
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
            _entityIndex = entityIndex;
            _archeTypeIndex = archeTypeIndex;
            _component1 = component1;
            _component2 = component2;
            _component3 = component3;
            _component4 = component4;
            _component5 = component5;
            _component6 = component6;
            _component7 = component7;
            _component8 = component8;
        }

        public void Execute(EcsContext context, ref Entity[] cahcedEntities) =>
            context.Entities.UpdateComponents(_entity,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7,
                _component8);

        public void QueryExecute(EcsContext context) =>
            context.Entities.UpdateQueryComponents(_entity, _entityIndex, _archeTypeIndex,
                _component1,
                _component2,
                _component3,
                _component4,
                _component5,
                _component6,
                _component7,
                _component8);
    }

    #endregion

    #region ComponentUpdate

    internal class EntityCommand_UpdateComponent<TComponent> : IEntityCommand
            where TComponent : unmanaged, IGeneralComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_UpdateComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.UpdateComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateManagedComponent<TComponent> : IEntityCommand
            where TComponent : IManagedComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_UpdateManagedComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.UpdateManagedComponent(_entity, _component);
    }

    internal class EntityCommand_UpdateSharedComponent<TComponent> : IEntityCommand
            where TComponent : unmanaged, ISharedComponent
    {
        private Entity _entity;
        private TComponent _component;

        internal EntityCommand_UpdateSharedComponent(Entity entity, TComponent component)
        {
            _entity = entity;
            _component = component;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.UpdateSharedComponent(_entity, _component);
    }

    #endregion

    #region EntityCopyTo

    internal class EntityCommand_CopyEntityTo : IEntityCommand
    {
        private EntityManager _srcEntityManager;
        private Entity _srcEntity;

        internal EntityCommand_CopyEntityTo(EntityManager srcEntityManager, Entity srcEntity)
        {
            _srcEntityManager = srcEntityManager;
            _srcEntity = srcEntity;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CopyEntityTo(_srcEntityManager, _srcEntity);
    }

    internal class EntityCommand_CopyEntitiesTo : IEntityCommand
    {
        private EntityManager _srcEntityManager;
        private Entity[] _srcEntities;

        internal EntityCommand_CopyEntitiesTo(EntityManager srcEntityManager, Entity[] srcEntities)
        {
            _srcEntityManager = srcEntityManager;
            _srcEntities = srcEntities;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CopyEntitiesTo(_srcEntityManager, _srcEntities, 0, _srcEntities?.Length ?? 0,
                ref cachedEntities, 0);
    }

    internal class EntityCommand_CopyEntitiesTo_EntityArcheType : IEntityCommand
    {
        private EntityArcheType _srcArcheType;

        internal EntityCommand_CopyEntitiesTo_EntityArcheType(EntityArcheType srcArcheType) => _srcArcheType = srcArcheType;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CopyEntitiesTo(_srcArcheType, ref cachedEntities, 0);
    }

    #endregion

    #region EntityCreate

    internal class EntityCommand_CreateEntities : IEntityCommand
    {
        private int _count;

        internal EntityCommand_CreateEntities(int count) => _count = count;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CreateEntities(ref cachedEntities, _count);
    }

    internal class EntityCommand_CreateEntities_EntityArcheType : IEntityCommand
    {
        private EntityArcheType _archeType;
        private int _count;

        internal EntityCommand_CreateEntities_EntityArcheType(EntityArcheType archeType, int count)
        {
            _archeType = archeType;
            _count = count;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CreateEntities(_archeType, ref cachedEntities, _count);
    }

    internal class EntityCommand_CreateEntities_EntityBlueprint : IEntityCommand
    {
        private EntityBlueprint _blueprint;
        private int _count;

        internal EntityCommand_CreateEntities_EntityBlueprint(EntityBlueprint blueprint, int count)
        {
            _blueprint = blueprint;
            _count = count;
        }

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.CreateEntities(_blueprint, ref cachedEntities, _count);
    }

    #endregion

    #region EntityDestroy

    internal class EntityCommand_DestroyEntity : IEntityCommand
    {
        private Entity _entity;

        internal EntityCommand_DestroyEntity(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.DestroyEntity(_entity);
    }

    internal class EntityCommand_DestroyEntities : IEntityCommand
    {
        private Entity[] _entities;

        internal EntityCommand_DestroyEntities(Entity[] entities) => _entities = entities;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.DestroyEntities(_entities, 0, _entities.Length);
    }

    #endregion

    #region EntityDuplicate

    internal class EntityCommand_DuplicateEntity : IEntityCommand
    {
        private Entity _entity;

        internal EntityCommand_DuplicateEntity(Entity entity) => _entity = entity;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.DuplicateEntity(_entity);
    }

    internal class EntityCommand_DuplicateEntities : IEntityCommand
    {
        private Entity[] _entities;

        internal EntityCommand_DuplicateEntities(Entity[] entities) => _entities = entities;

        public void Execute(EcsContext context, ref Entity[] cachedEntities) =>
            context.Entities.DuplicateEntities(_entities, 0, _entities.Length,
                ref cachedEntities, 0);
    }

    #endregion
}
