using System;

namespace EcsLte.Utilities
{
    internal static class Helper
    {
        internal static T[] CopyInsertSort<T>(T[] source, T insert) where T : IComparable<T>
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
    }
}