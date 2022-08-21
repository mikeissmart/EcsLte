using EcsLte.Exceptions;
using System;

namespace EcsLte.Utilities
{
    internal static class Helper
    {
        internal static int NextPow2(int n) => (int)Math.Pow(2, (int)Math.Log(n, 2) + 1);

        internal static void CheckArrayLength<T>(ref T[] srcArray, int startingIndex, int count)
        {
            if (startingIndex + count > srcArray.Length)
                Array.Resize(ref srcArray, startingIndex + count);
        }

        internal static T[] CopyInsertSort<T>(in T[] source, T insert) where T : IComparable<T>
        {
            T[] destination;
            if (source != null && source.Length > 0)
            {
                destination = new T[source.Length + 1];
                var index = source.Length - 1;
                for (; index >= 0; index--)
                {
                    if (source[index].CompareTo(insert) < 0)
                        break;
                }

                // Copy before insert
                if (index >= 0)
                    Array.Copy(source, 0, destination, 0, index + 1);

                // insert
                destination[index + 1] = insert;

                // Copy after insert
                if (source.Length - 1 != index)
                    Array.Copy(source, index + 1, destination, index + 2, source.Length - (index + 1));
            }
            else
            {
                destination = new T[1];
                destination[0] = insert;
            }

            return destination;
        }

        internal static void ResizeRefArray<T>(ref T[] array, int startingIndex, int count)
        {
            if (startingIndex + count > array.Length)
                Array.Resize(ref array, startingIndex + count);
        }

        #region Assert

        internal static void AssertArray<T>(in T[] array, int startingIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (startingIndex < 0 || startingIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
        }

        internal static void AssertArray<T>(in T[] array, int startingIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (startingIndex < 0 || startingIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= array.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        internal static void AssertAndResizeArray<T>(ref T[] array, int startingIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (startingIndex < 0 || startingIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefArray(ref array, startingIndex, count);
        }

        internal static void AssertDuplicateConfigs(params ComponentConfig[] configs)
        {
            for (var i = 0; i < configs.Length; i++)
            {
                for (var j = i + 1; j < configs.Length; j++)
                {
                    if (configs[i] == configs[j])
                        throw new ComponentDuplicateException(configs[i].ComponentType);
                }
            }
        }

        #endregion
    }
}
