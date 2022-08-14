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

        internal static void ResizeRefEntities(ref Entity[] entities, int startingIndex, int count)
        {
            if (startingIndex + count > entities.Length)
                Array.Resize(ref entities, startingIndex + count);
        }

        internal static void ResizeRefComponents<TComponent>(ref TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (startingIndex + count > components.Length)
                Array.Resize(ref components, startingIndex + count);
        }

        internal static void ResizeRefArcheTypes(ref EntityArcheType[] archeTypes, int startingIndex, int count)
        {
            if (startingIndex + count > archeTypes.Length)
                Array.Resize(ref archeTypes, startingIndex + count);
        }

        #region Assert

        internal static void AssertEntities(in Entity[] entities, int startingIndex)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (startingIndex < 0 || startingIndex > entities.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
        }

        internal static void AssertEntities(in Entity[] entities, int startingIndex, int count)
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

        internal static void AssertAndResizeEntities(ref Entity[] entities, int startingIndex, int count)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            if (startingIndex < 0 || startingIndex > entities.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefEntities(ref entities, startingIndex, count);
        }

        internal static void AssertComponents<TComponent>(in TComponent[] components, int startingIndex)
            where TComponent : IComponent
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if (startingIndex < 0 || startingIndex > components.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
        }

        internal static void AssertComponents<TComponent>(in TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if (startingIndex < 0 || startingIndex > components.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= components.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        internal static void AssertAndResizeComponents<TComponent>(ref TComponent[] components, int startingIndex, int count)
            where TComponent : IComponent
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if (startingIndex < 0 || startingIndex > components.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefComponents(ref components, startingIndex, count);
        }

        internal static void AssertArcheTypes(in EntityArcheType[] archeTypes, int startingIndex)
        {
            if (archeTypes == null)
                throw new ArgumentNullException(nameof(archeTypes));
            if (startingIndex < 0 || startingIndex > archeTypes.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
        }

        internal static void AssertArcheTypes(in EntityArcheType[] archeTypes, int startingIndex, int count)
        {
            if (archeTypes == null)
                throw new ArgumentNullException(nameof(archeTypes));
            if (startingIndex < 0 || startingIndex > archeTypes.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startingIndex + count < 0 || startingIndex + count >= archeTypes.Length + 1)
                throw new ArgumentOutOfRangeException();
        }

        internal static void AssertAndResizeArcheTypes(ref EntityArcheType[] archeTypes, int startingIndex, int count)
        {
            if (archeTypes == null)
                throw new ArgumentNullException(nameof(archeTypes));
            if (startingIndex < 0 || startingIndex > archeTypes.Length)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            ResizeRefArcheTypes(ref archeTypes, startingIndex, count);
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
