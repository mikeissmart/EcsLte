using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommands
    {
        private readonly List<IEntityCommand> _commands;
        private Entity[] _cachedEntities;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }
        public string Name { get; private set; }
        public bool IsDestroyed { get; set; }

        internal EntityCommands(EcsContext context, string name)
        {
            _commands = new List<IEntityCommand>();
            _cachedEntities = new Entity[0];
            _lockObj = new object();

            Context = context;
            Name = name;
        }

        #region ComponentAdd

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponent<TComponent>(entity, component));
            }
        }

        public void AddManagedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IManagedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddManagedComponent<TComponent>(entity, component));
            }
        }

        public void AddSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddSharedComponent<TComponent>(entity, component));
            }
        }

        #endregion

        #region ComponentRemove

        public void RemoveComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IGeneralComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_RemoveComponent<TComponent>(entity));
            }
        }

        public void RemoveManagedComponent<TComponent>(Entity entity)
            where TComponent : IManagedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_RemoveManagedComponent<TComponent>(entity));
            }
        }

        public void RemoveSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_RemoveSharedComponent<TComponent>(entity));
            }
        }

        #endregion

        #region ComponentsAdd

        public void AddComponents<T1, T2>(Entity entity,
            T1 component1, T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2>(
                        entity,
                        component1,
                        component2));
            }
        }

        public void AddComponents<T1, T2, T3>(Entity entity,
            T1 component1, T2 component2, T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3>(
                        entity,
                        component1,
                        component2,
                        component3));
            }
        }

        public void AddComponents<T1, T2, T3, T4>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3, T4>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4));
            }
        }

        public void AddComponents<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3, T4, T5>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
            }
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6));
            }
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6, T7>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7));
            }
        }

        public void AddComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7, T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_AddComponents<T1, T2, T3, T4, T5, T6, T7, T8>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7,
                        component8));
            }
        }

        #endregion

        #region ComponentsRemove

        public void RemoveComponents<T1, T2>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3, T4>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3, T4>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3, T4, T5>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3, T4, T5>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6, T7>(entity));
            }
        }

        public void RemoveComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Remove(new EntityCommand_RemoveComponents<T1, T2, T3, T4, T5, T6, T7, T8>(entity));
            }
        }

        #endregion

        #region ComponentsUpdate

        public void UpdateComponents<T1, T2>(Entity entity,
            T1 component1, T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2>(
                        entity,
                        component1,
                        component2));
            }
        }

        public void UpdateComponents<T1, T2, T3>(Entity entity,
            T1 component1, T2 component2, T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(
                        entity,
                        component1,
                        component2,
                        component3));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            T1 component1, T2 component2, T3 component3, T4 component4,
            T5 component5, T6 component6, T7 component7, T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(
                        entity,
                        component1,
                        component2,
                        component3,
                        component4,
                        component5,
                        component6,
                        component7,
                        component8));
            }
        }

        #endregion

        #region ComponentUpdate

        public void UpdateComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateComponent<TComponent>(entity, component));
            }
        }

        public void UpdateManagedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IManagedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateManagedComponent<TComponent>(entity, component));
            }
        }

        public void UpdateSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_UpdateSharedComponent<TComponent>(entity, component));
            }
        }

        #endregion

        #region EntityCopyTo

        public void CopyEntityTo(EntityManager srcEntityManager,
            Entity srcEntity)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CopyEntityTo(srcEntityManager, srcEntity));
            }
        }

        public void CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities) => CopyEntitiesTo(srcEntityManager, srcEntities, 0, srcEntities?.Length ?? 0);

        public void CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex) => CopyEntitiesTo(srcEntityManager, srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex);

        public void CopyEntitiesTo(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount)
        {
            AssertEntityCommands();

            var entities = new Entity[srcCount];
            Array.Copy(srcEntities, srcStartingIndex, entities, 0, srcCount);

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CopyEntitiesTo(srcEntityManager,
                        entities));
            }
        }

        public void CopyEntitiesTo(EntityArcheType srcArcheType)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CopyEntitiesTo_EntityArcheType(new EntityArcheType(srcArcheType)));
            }
        }

        #endregion

        #region EntityCreate

        public void CreateEntity()
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities(1));
            }
        }

        public void CreateEntity(EntityArcheType archeType)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities_EntityArcheType(new EntityArcheType(archeType), 1));
            }
        }

        public void CreateEntity(EntityBlueprint blueprint)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities_EntityBlueprint(new EntityBlueprint(blueprint), 1));
            }
        }

        public void CreateEntities(int count)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities(count));
            }
        }

        public void CreateEntities(EntityArcheType archeType, int count)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities_EntityArcheType(new EntityArcheType(archeType), count));
            }
        }

        public void CreateEntities(EntityBlueprint blueprint, int count)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_CreateEntities_EntityBlueprint(new EntityBlueprint(blueprint), count));
            }
        }

        #endregion

        #region EntityDestroy

        public void DestroyEntity(Entity entity)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_DestroyEntity(entity));
            }
        }

        public void DestroyEntities(in Entity[] entities) => DestroyEntities(entities, 0, entities?.Length ?? 0);

        public void DestroyEntities(in Entity[] entities, int startingIndex) => DestroyEntities(entities, startingIndex, (entities?.Length ?? 0) - startingIndex);

        public void DestroyEntities(in Entity[] entities, int startingIndex, int count)
        {
            AssertEntityCommands();

            var des = new Entity[count];
            Array.Copy(entities, startingIndex, des, 0, count);

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_DestroyEntities(des));
            }
        }

        #endregion

        #region EntityDuplicate

        public void DuplicateEntity(Entity srcEntity)
        {
            AssertEntityCommands();

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_DuplicateEntity(srcEntity));
            }
        }

        public void DuplicateEntities(in Entity[] srcEntities) => DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0);

        public void DuplicateEntities(in Entity[] srcEntities, int startingIndex) => DuplicateEntities(srcEntities, startingIndex, (srcEntities?.Length ?? 0) - startingIndex);

        public void DuplicateEntities(in Entity[] srcEntities, int startingIndex, int count)
        {
            AssertEntityCommands();

            var entities = new Entity[count];
            Array.Copy(srcEntities, startingIndex, entities, 0, count);

            lock (_lockObj)
            {
                _commands.Add(new EntityCommand_DuplicateEntities(entities));
            }
        }

        #endregion

        public void Execute()
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertEntityCommands();

            foreach (var command in _commands)
                command.Execute(Context, ref _cachedEntities);
            _commands.Clear();
        }

        public void Clear()
        {
            AssertEntityCommands();
            _commands.Clear();
        }

        internal void AppendQueryCommands(List<IEntityCommand> commands)
        {
            lock (_lockObj)
            {
                _commands.AddRange(commands);
            }
        }

        internal void InternalDestroy() => IsDestroyed = true;

        #region Assert

        internal static void AssertEntityCommands(EntityCommands commands, EcsContext context)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));
            if (commands.IsDestroyed)
                throw new EntityCommandsIsDestroyedException(commands);
            if (commands.Context != context)
                throw new EcsContextNotSameException(commands.Context, context);
        }

        private void AssertEntityCommands()
        {
            if (IsDestroyed)
                throw new EntityCommandsIsDestroyedException(this);
        }

        #endregion
    }
}
