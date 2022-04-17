using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace EcsLte
{
    internal class ComponentConfigs
    {
        private static ComponentConfigs _instance;
        private Dictionary<int, ComponentConfig> _componentConfigIndexes;
        private Dictionary<Type, ComponentConfig> _componentConfigTypes;

        private ComponentConfigs() => Initialize();

        internal static ComponentConfigs Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ComponentConfigs();
                return _instance;
            }
        }

        internal ComponentConfig[] AllComponentConfigs { get; private set; }
        internal ComponentConfig[] AllRecordableConfigs { get; private set; }
        internal ComponentConfig[] AllUniqueConfigs { get; private set; }
        internal ComponentConfig[] AllSharedConfigs { get; private set; }

        internal Type[] AllComponentTypes { get; private set; }
        internal Type[] AllRecordableTypes { get; private set; }
        internal Type[] AllUniqueTypes { get; private set; }
        internal Type[] AllSharedTypes { get; private set; }

        internal int[] AllRecordableIndexes { get; private set; }
        internal int[] AllUniqueIndexes { get; private set; }
        internal int[] AllSharedIndexes { get; private set; }

        internal int AllComponentCount { get; private set; }
        internal int RecordableComponentCount { get; private set; }
        internal int UniqueComponentCount { get; private set; }
        internal int SharedComponentCount { get; private set; }

        internal ComponentConfig GetConfig(Type componentType) => _componentConfigTypes[componentType];

        internal ComponentConfig GetConfig(int componentIndex) => _componentConfigIndexes[componentIndex];

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iRecordableComponentType = typeof(IRecordableComponent);
            var iUniqueComponentType = typeof(IUniqueComponent);
            var iSharedComponentType = typeof(ISharedComponent);
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    iComponentType.IsAssignableFrom(x));

            _componentConfigTypes = new Dictionary<Type, ComponentConfig>();
            _componentConfigIndexes = new Dictionary<int, ComponentConfig>();
            var recordableTypeIndexes = new Dictionary<int, Type>();
            var uniqueTypeIndexes = new Dictionary<int, Type>();
            var sharedTypeIndexes = new Dictionary<int, Type>();
            var sharedNoEquatableOrGetHashCode = new List<Type>();
            var recordableIndex = 0;
            var uniqueIndex = 0;
            var sharedIndex = 0;
            var equatableType = typeof(IEquatable<>);

            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                var componentIndex = _componentConfigIndexes.Count;
                var config = new ComponentConfig
                {
                    ComponentIndex = componentIndex
                };

                if (iRecordableComponentType.IsAssignableFrom(type))
                {
                    recordableTypeIndexes.Add(componentIndex, type);
                    config.RecordableIndex = recordableIndex++;
                    config.IsRecordable = true;
                }

                if (iUniqueComponentType.IsAssignableFrom(type))
                {
                    uniqueTypeIndexes.Add(componentIndex, type);
                    config.UniqueIndex = uniqueIndex++;
                    config.IsUnique = true;
                }

                if (iSharedComponentType.IsAssignableFrom(type))
                {
                    var sharedEquatableType = equatableType.MakeGenericType(type);
                    var hasEquatable = type.GetInterfaces().Any(x => x.IsGenericType && x == sharedEquatableType);
                    var getHashOverride = type.GetMethod("GetHashCode").DeclaringType == type;
                    if (!hasEquatable || !getHashOverride)
                        sharedNoEquatableOrGetHashCode.Add(type);
                    sharedTypeIndexes.Add(componentIndex, type);
                    config.SharedIndex = sharedIndex++;
                    config.IsShared = true;
                }

                if (IsBlittable(type))
                {
                    config.IsBlittable = true;
                    config.UnmanagedSizeInBytes = Marshal.SizeOf(type);
                }

                _componentConfigTypes.Add(type, config);
                _componentConfigIndexes.Add(componentIndex, config);
            }

            var recordableUnblittableTypes = _componentConfigTypes
                .Where(x => x.Value.IsRecordable && !x.Value.IsBlittable)
                .Select(x => x.Key)
                .ToArray();
            var uniqueSharedTypes = _componentConfigTypes
                .Where(x => x.Value.IsUnique && x.Value.IsShared)
                .Select(x => x.Key)
                .ToArray();

            if (_componentConfigTypes.Count == 0)
                throw new ComponentNoneException();

            if (recordableUnblittableTypes.Length > 0)
                throw new ComponentRecordableNotBlittable(recordableUnblittableTypes);

            if (uniqueSharedTypes.Length > 0)
                throw new ComponentUniqueSharedException(uniqueSharedTypes);

            if (sharedNoEquatableOrGetHashCode.Count > 0)
                throw new ComponentNoSharedEquatableHashCodeException(sharedNoEquatableOrGetHashCode.ToArray());

            AllComponentConfigs = _componentConfigTypes.Values.ToArray();
            AllRecordableConfigs = _componentConfigTypes.Values.Where(x => x.IsRecordable).ToArray();
            AllUniqueConfigs = _componentConfigTypes.Values.Where(x => x.IsUnique).ToArray();
            AllSharedConfigs = _componentConfigTypes.Values.Where(x => x.IsShared).ToArray();

            AllComponentTypes = _componentConfigTypes.Keys.ToArray();
            AllRecordableTypes = recordableTypeIndexes.Values.ToArray();
            AllUniqueTypes = uniqueTypeIndexes.Values.ToArray();
            AllSharedTypes = sharedTypeIndexes.Values.ToArray();

            AllRecordableIndexes = recordableTypeIndexes.Keys.ToArray();
            AllUniqueIndexes = uniqueTypeIndexes.Keys.ToArray();
            AllSharedIndexes = sharedTypeIndexes.Keys.ToArray();

            AllComponentCount = _componentConfigTypes.Count;
            RecordableComponentCount = recordableTypeIndexes.Count;
            UniqueComponentCount = uniqueTypeIndexes.Count;
            SharedComponentCount = sharedTypeIndexes.Count;
        }

        private bool IsBlittable(Type type)
        {
            try
            {
                GCHandle.Alloc(FormatterServices.GetUninitializedObject(type),
                    GCHandleType.Pinned)
                    .Free();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
