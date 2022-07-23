using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityCommands
    {
        private List<IEntityCommand> _entityCommands;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }
        public string Name { get; private set; }
        public bool IsDestroyed { get; private set; }

        internal EntityCommands(EcsContext context, string name)
        {
            _entityCommands = new List<IEntityCommand>();
            Context = context;
            _lockObj = new object();

            Name = name;
        }

        #region EntityCreate

        public void CreateEntity(EntityArcheType archeType, EntityState state)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CreateEntities_EntityArcheType(archeType, state, 1));
            }
        }

        public void CreateEntity(EntityBlueprint blueprint, EntityState state)
        {
            EntityBlueprint.AssertEntityBlueprint(blueprint);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CreateEntities_EntityBlueprint(blueprint, state, 1));
            }
        }

        public void CreateEntities(EntityArcheType archeType, EntityState state, int count)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0)
                return;

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CreateEntities_EntityArcheType(archeType, state, count));
            }
        }

        public void CreateEntities(EntityBlueprint blueprint, EntityState state, int count)
        {
            EntityBlueprint.AssertEntityBlueprint(blueprint);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0)
                return;

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CreateEntities_EntityBlueprint(blueprint, state, count));
            }
        }

        #endregion

        #region EntityDestroy

        public void DestroyEntity(Entity entity)
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DestroyEntities(new[] { entity }));
            }
        }

        public void DestroyEntities(in Entity[] entities) => DestroyEntities(entities, 0, entities?.Length ?? 0);

        public void DestroyEntities(in Entity[] entities, int startingIndex) => DestroyEntities(entities, startingIndex, (entities?.Length ?? 0) - startingIndex);

        public void DestroyEntities(in Entity[] entities, int startingIndex, int count)
        {
            AssertEntities(entities, startingIndex, count);

            lock (_lockObj)
            {
                AssertCommands();
                if (count == 0)
                    return;

                var e = new Entity[count];
                Array.Copy(entities, startingIndex, e, 0, count);
                _entityCommands.Add(new EntityCommand_DestroyEntities(e));
            }
        }

        public void DestroyEntities(EntityArcheType archeType)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityArcheType(archeType));
            }
        }

        public void DestroyEntities(EntityFilter filter)
        {
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityFilter(filter));
            }
        }

        public void DestroyEntities(EntityTracker tracker)
        {
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityTracker(tracker));
            }
        }

        public void DestroyEntities(EntityQuery query)
        {
            EntityQuery.AssertEntityQuery(query, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DestroyEntities_EntityQuery(query));
            }
        }

        #endregion

        #region EntityStateSet

        public void ChangeEntityState(Entity entity, EntityState state)
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_ChangeEntitiesState(new[] { entity }, state));
            }
        }

        public void ChangeEntityStates(in Entity[] entities, EntityState state) => ChangeEntityStates(entities, 0, entities?.Length ?? 0, state);

        public void ChangeEntityStates(in Entity[] entities, int startingIndex, EntityState state) => ChangeEntityStates(entities, startingIndex, (entities?.Length ?? 0) - startingIndex, state);

        public void ChangeEntityStates(in Entity[] entities, int startingIndex, int count, EntityState state)
        {
            AssertEntities(entities, startingIndex, count);

            lock (_lockObj)
            {
                AssertCommands();
                if (count == 0)
                    return;

                var e = new Entity[count];
                Array.Copy(entities, startingIndex, e, 0, count);
                _entityCommands.Add(new EntityCommand_ChangeEntitiesState(e, state));
            }
        }

        public void ChangeEntityStates(EntityArcheType archeType, EntityState state)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_ChangeEntitiesState_EntityArcheType(archeType, state));
            }
        }

        public void ChangeEntityStates(EntityFilter filter, EntityState state)
        {
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_ChangeEntitiesState_EntityFilter(filter, state));
            }
        }

        public void ChangeEntityStates(EntityTracker tracker, EntityState state)
        {
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_ChangeEntitiesState_EntityTracker(tracker, state));
            }
        }

        public void ChangeEntityStates(EntityQuery query, EntityState state)
        {
            EntityQuery.AssertEntityQuery(query, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_ChangeEntitiesState_EntityQuery(query, state));
            }
        }

        #endregion

        #region EntityDuplicate

        public void DuplicateEntity(Entity srcEntity, EntityState? state = null)
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DuplicateEntities(new[] { srcEntity }, state));
            }
        }

        public void DuplicateEntities(in Entity[] srcEntities,
            EntityState? state = null) => DuplicateEntities(srcEntities, 0, srcEntities?.Length ?? 0, state);

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex,
            EntityState? state = null) => DuplicateEntities(srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex, state);

        public void DuplicateEntities(in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            EntityState? state = null)
        {
            AssertEntities(srcEntities, srcStartingIndex, srcCount);
            if (srcCount == 0)
                return;

            lock (_lockObj)
            {
                AssertCommands();

                var e = new Entity[srcCount];
                Array.Copy(srcEntities, srcStartingIndex, e, 0, srcCount);
                _entityCommands.Add(new EntityCommand_DuplicateEntities(e, state));
            }
        }

        public void DuplicateEntities(EntityArcheType archeType,
            EntityState? state = null)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DuplicateEntities_EntityArcheType(archeType, state));
            }
        }

        public void DuplicateEntities(EntityFilter filter,
            EntityState? state = null)
        {
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DuplicateEntities_EntityFilter(filter, state));
            }
        }

        public void DuplicateEntities(EntityTracker tracker,
            EntityState? state = null)
        {
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DuplicateEntities_EntityTracker(tracker, state));
            }
        }

        public void DuplicateEntities(EntityQuery query,
            EntityState? state = null)
        {
            EntityQuery.AssertEntityQuery(query, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_DuplicateEntities_EntityQuery(query, state));
            }
        }

        #endregion

        #region EntityCopy

        public void CopyEntity(EntityManager srcEntityManager,
            Entity srcEntity,
            EntityState? state = null)
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CopyEntities(srcEntityManager, new[] { srcEntity }, state));
            }
        }

        public void CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities,
            EntityState? state = null) => CopyEntities(srcEntityManager, srcEntities, 0, srcEntities?.Length ?? 0, state);

        public void CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex,
            EntityState? state = null) => CopyEntities(srcEntityManager, srcEntities, srcStartingIndex, (srcEntities?.Length ?? 0) - srcStartingIndex, state);

        public void CopyEntities(EntityManager srcEntityManager,
            in Entity[] srcEntities, int srcStartingIndex, int srcCount,
            EntityState? state = null)
        {
            AssertEntities(srcEntities, srcStartingIndex, srcCount);
            if (srcCount == 0)
                return;

            lock (_lockObj)
            {
                AssertCommands();

                var e = new Entity[srcCount];
                Array.Copy(srcEntities, srcStartingIndex, e, 0, srcCount);
                _entityCommands.Add(new EntityCommand_CopyEntities(srcEntityManager, e, state));
            }
        }

        public void CopyEntities(EntityManager srcEntityManager,
            EntityArcheType archeType,
            EntityState? state = null)
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CopyEntities_EntityArcheType(srcEntityManager, archeType, state));
            }
        }

        public void CopyEntities(EntityManager srcEntityManager,
            EntityFilter filter,
            EntityState? state = null)
        {
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CopyEntities_EntityFilter(srcEntityManager, filter, state));
            }
        }

        public void CopyEntities(EntityTracker tracker,
            EntityState? state = null)
        {
            EntityTracker.AssertEntityTracker(tracker, tracker?.Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CopyEntities_EntityTracker(tracker, state));
            }
        }

        public void CopyEntities(EntityQuery query,
            EntityState? state = null)
        {
            EntityQuery.AssertEntityQuery(query, query?.Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_CopyEntities_EntityQuery(query, state));
            }
        }

        #endregion

        #region ComponentUpdate

        public void UpdateComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IGeneralComponent
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponent<TComponent>(entity, component));
            }
        }

        public void UpdateComponents<T1, T2>(Entity entity,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2>(entity,
                    component1,
                    component2));
            }
        }

        public void UpdateComponents<T1, T2, T3>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3>(entity,
                    component1,
                    component2,
                    component3));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4>(entity,
                    component1,
                    component2,
                    component3,
                    component4));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5>(entity,
                    component1,
                    component2,
                    component3,
                    component4,
                    component5));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6>(entity,
                    component1,
                    component2,
                    component3,
                    component4,
                    component5,
                    component6));
            }
        }

        public void UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config,
                ComponentConfig<T7>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7>(entity,
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
            T1 component1,
            T2 component2,
            T3 component3,
            T4 component4,
            T5 component5,
            T6 component6,
            T7 component7,
            T8 component8)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent
            where T7 : unmanaged, IComponent
            where T8 : unmanaged, IComponent
        {
            Helper.AssertDuplicateConfigs(
                ComponentConfig<T1>.Config,
                ComponentConfig<T2>.Config,
                ComponentConfig<T3>.Config,
                ComponentConfig<T4>.Config,
                ComponentConfig<T5>.Config,
                ComponentConfig<T6>.Config,
                ComponentConfig<T7>.Config,
                ComponentConfig<T8>.Config);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateComponents<T1, T2, T3, T4, T5, T6, T7, T8>(entity,
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

        #region ComponentUpdateShared

        public void UpdateSharedComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateSharedComponent<TComponent>(entity, component));
            }
        }

        public void UpdateSharedComponent<TComponent>(EntityArcheType archeType, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            EntityArcheType.AssertEntityArcheType(archeType);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateSharedComponent_EntityArcheType<TComponent>(archeType, component));
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityFilter filter, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            EntityFilter.AssertEntityFilter(filter);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateSharedComponents_EntityFilter<TComponent>(filter, component));
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityTracker tracker, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            EntityTracker.AssertEntityTracker(tracker, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateSharedComponents_EntityTracker<TComponent>(tracker, component));
            }
        }

        public void UpdateSharedComponents<TComponent>(EntityQuery query, TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            EntityQuery.AssertEntityQuery(query, Context);

            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateSharedComponents_EntityQuery<TComponent>(query, component));
            }
        }

        #endregion

        #region ComponentUpdateUnique

        public void UpdateUniqueComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : unmanaged, IUniqueComponent
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateUniqueComponent_Entity<TComponent>(entity, component));
            }
        }

        public void UpdateUniqueComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IUniqueComponent
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Add(new EntityCommand_UpdateUniqueComponent<TComponent>(component));
            }
        }

        #endregion

        public void ExecuteCommands()
        {
            lock (_lockObj)
            {
                AssertCommands();

                foreach (var command in _entityCommands)
                    command.Execute(Context);
                _entityCommands.Clear();
            }
        }

        public void ClearCommands()
        {
            lock (_lockObj)
            {
                AssertCommands();

                _entityCommands.Clear();
            }
        }

        internal void InternalDestroy()
        {
            lock (_lockObj)
            {
                _entityCommands = null;

                IsDestroyed = true;
            }
        }

        #region Assert

        internal void AssertCommands()
        {
            if (IsDestroyed)
                throw new EntityCommandsIsDestroyedException(this);
        }

        private static void AssertEntities(in Entity[] entities, int startingIndex, int count)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (startingIndex < 0 || startingIndex > entities.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= entities.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        #endregion
    }
}