using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

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

        internal static bool HasArcheTypeData(in ArcheTypeData[] archeTypeDatas, ArcheTypeIndex archeTypeIndex)
        {
            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                if (archeTypeDatas[i].ArcheTypeIndex == archeTypeIndex)
                    return true;
            }

            return false;
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

        internal static IComponentData[] CopyAddOrReplaceSort(in IComponentData[] src, IComponentData addReplace)
        {
            IComponentData[] dest;
            for (var i = 0; i < src.Length; i++)
            {
                if (src[i].Config == addReplace.Config)
                {
                    dest = new IComponentData[src.Length];
                    Array.Copy(src, dest, src.Length);

                    dest[i] = addReplace;
                    return dest;
                }
            }

            return CopyInsertSort(src, addReplace);
        }

        internal static IComponentData[] CopyAddOrReplaceSorts(in IComponentData[] src, IComponentData[] addReplaces)
        {
            var dest = new List<IComponentData>(src);
            for (var i = 0; i < addReplaces.Length; i++)
            {
                var addReplace = addReplaces[i];
                var replaced = false;
                for (var j = 0; j < dest.Count; j++)
                {
                    if (dest[j].Config == addReplace.Config)
                    {
                        dest[j] = addReplace;
                        break;
                    }
                }

                if (!replaced)
                    dest.Add(addReplace);
            }

            if (dest.Count != src.Length)
                dest.Sort();

            return dest.ToArray();
        }
    }
}