using System;
using System.Linq;

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

        internal static Type[] GetComponentConfigTypes(ComponentConfig[] configs)
        {
            var types = new Type[configs.Length];
            for (var i = 0; i < configs.Length; i++)
                types[i] = ComponentConfigs.Instance.AllComponentTypes[configs[i].ComponentIndex];

            return types;
        }

        internal static IComponentData[] CopyAddOrReplaceSort(IComponentData[] source, IComponentData addReplace)
        {
            IComponentData[] dest;
            if (source.Any(x => x.Config == addReplace.Config))
            {
                dest = new IComponentData[source.Length];
                Array.Copy(source, dest, source.Length);
                for (var i = 0; i < dest.Length; i++)
                {
                    if (dest[i].Config == addReplace.Config)
                    {
                        dest[i] = addReplace;
                        break;
                    }
                }
            }
            else
            {
                dest = new IComponentData[source.Length + 1];
                Array.Copy(source, dest, source.Length);
                dest[source.Length] = addReplace;
                Array.Sort(dest);
            }

            return dest;
        }
    }
}