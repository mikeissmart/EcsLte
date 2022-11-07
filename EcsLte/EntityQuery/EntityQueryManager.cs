namespace EcsLte
{
    public class EntityQueryManager
    {
        public EcsContext Context { get; private set; }

        internal EntityQueryManager(EcsContext context) => Context = context;

        public EntityQuery CreateQuery()
            => new EntityQuery(Context);

        public EntityQuery SetCommands(EntityCommands commands)
            => new EntityQuery(Context)
                .SetCommands(commands);

        public EntityQuery SetFilter(EntityFilter filter)
            => new EntityQuery(Context)
                .SetFilter(filter);

        public EntityQuery SetTracker(EntityTracker tracker)
            => new EntityQuery(Context)
                .SetTracker(tracker);

        #region ForEach

        #region Other

        public EntityQuery ForEach(int count, EntityQueryActions.Other action)
            => new EntityQuery(Context)
                .ForEach(count, action);

        #endregion

        #region Write 0

        public EntityQuery ForEach(EntityQueryActions.R0W0 action)
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1>(EntityQueryActions.R1W0<T1> action)
            where T1 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R2W0<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R3W0<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R4W0<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R5W0<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R6W0<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R7W0<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R8W0<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 1

        public EntityQuery ForEach<T1>(EntityQueryActions.R0W1<T1> action)
            where T1 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R1W1<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R2W1<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R3W1<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R4W1<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R5W1<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R6W1<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R7W1<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 2

        public EntityQuery ForEach<T1, T2>(EntityQueryActions.R0W2<T1, T2> action)
            where T1 : IComponent
            where T2 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R1W2<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R2W2<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R3W2<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R4W2<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R5W2<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R6W2<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 3

        public EntityQuery ForEach<T1, T2, T3>(EntityQueryActions.R0W3<T1, T2, T3> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R1W3<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R2W3<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R3W3<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R4W3<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R5W3<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 4

        public EntityQuery ForEach<T1, T2, T3, T4>(EntityQueryActions.R0W4<T1, T2, T3, T4> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R1W4<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R2W4<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R3W4<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R4W4<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 5

        public EntityQuery ForEach<T1, T2, T3, T4, T5>(EntityQueryActions.R0W5<T1, T2, T3, T4, T5> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R1W5<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R2W5<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R3W5<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 6

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6>(EntityQueryActions.R0W6<T1, T2, T3, T4, T5, T6> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R1W6<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R2W6<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 7

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7>(EntityQueryActions.R0W7<T1, T2, T3, T4, T5, T6, T7> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R1W7<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #region Write 8

        public EntityQuery ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(EntityQueryActions.R0W8<T1, T2, T3, T4, T5, T6, T7, T8> action)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
            => new EntityQuery(Context)
                .ForEach(action);

        #endregion

        #endregion

        internal void InternalDestroy()
        {

        }
    }
}
