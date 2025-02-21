﻿namespace EcsLte
{
    public static class EntityQueryActions
    {
        #region Other

        public delegate void Other(int threadIndex, int index);

        #endregion

        #region Write 0

        public delegate void R0W0(int threadIndex, int index, Entity entity);

        public delegate void R1W0<T1>(int threadIndex, int index, Entity entity,
            in T1 component1)
            where T1 : IComponent;

        public delegate void R2W0<T1, T2>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2)
            where T1 : IComponent
            where T2 : IComponent;

        public delegate void R3W0<T1, T2, T3>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;

        public delegate void R4W0<T1, T2, T3, T4>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3, in T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;

        public delegate void R5W0<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R6W0<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R7W0<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R8W0<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            in T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 0

        #region Write 1

        public delegate void R0W1<T1>(int threadIndex, int index, Entity entity,
            ref T1 component1)
            where T1 : IComponent;

        public delegate void R1W1<T1, T2>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2)
            where T1 : IComponent
            where T2 : IComponent;

        public delegate void R2W1<T1, T2, T3>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;

        public delegate void R3W1<T1, T2, T3, T4>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3, in T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;

        public delegate void R4W1<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R5W1<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R6W1<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R7W1<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, in T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 1

        #region Write 2

        public delegate void R0W2<T1, T2>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2)
            where T1 : IComponent
            where T2 : IComponent;

        public delegate void R1W2<T1, T2, T3>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;

        public delegate void R2W2<T1, T2, T3, T4>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3, in T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;

        public delegate void R3W2<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3, in T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R4W2<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R5W2<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R6W2<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, in T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 2

        #region Write 3

        public delegate void R0W3<T1, T2, T3>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent;

        public delegate void R1W3<T1, T2, T3, T4>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, in T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;

        public delegate void R2W3<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, in T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R3W3<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, in T4 component4,
            in T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R4W3<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R5W3<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, in T4 component4,
            in T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 3

        #region Write 4

        public delegate void R0W4<T1, T2, T3, T4>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent;

        public delegate void R1W4<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            in T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R2W4<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            in T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R3W4<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            in T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R4W4<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            in T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 4

        #region Write 5

        public delegate void R0W5<T1, T2, T3, T4, T5>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent;

        public delegate void R1W5<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, in T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R2W5<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, in T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R3W5<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, in T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 5

        #region Write 6

        public delegate void R0W6<T1, T2, T3, T4, T5, T6>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent;

        public delegate void R1W6<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6, in T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R2W6<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6, in T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 6

        #region Write 7

        public delegate void R0W7<T1, T2, T3, T4, T5, T6, T7>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6, ref T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent;

        public delegate void R1W7<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6, ref T7 component7, in T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 7

        #region Write 8

        public delegate void R0W8<T1, T2, T3, T4, T5, T6, T7, T8>(int threadIndex, int index, Entity entity,
            ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4,
            ref T5 component5, ref T6 component6, ref T7 component7, ref T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent;

        #endregion Write 8
    }
}
