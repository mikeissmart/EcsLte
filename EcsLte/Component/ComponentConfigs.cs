using EcsLte.Exceptions;
using EcsLte.Utilities;
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
        internal ComponentConfig[] AllBlittableConfigs { get; private set; }
        internal ComponentConfig[] AllManagedConfigs { get; private set; }

        internal Type[] AllComponentTypes { get; private set; }
        internal Type[] AllRecordableTypes { get; private set; }
        internal Type[] AllUniqueTypes { get; private set; }
        internal Type[] AllSharedTypes { get; private set; }
        internal Type[] AllBlittableTypes { get; private set; }
        internal Type[] AllManagedTypes { get; private set; }

        internal int[] AllComponentIndexes { get; private set; }
        internal int[] AllRecordableIndexes { get; private set; }
        internal int[] AllUniqueIndexes { get; private set; }
        internal int[] AllSharedIndexes { get; private set; }
        internal int[] AllBlittableIndexes { get; private set; }
        internal int[] AllManagedIndexes { get; private set; }

        internal int AllComponentCount => AllComponentConfigs.Length;
        internal int AllRecordableCount => AllRecordableConfigs.Length;
        internal int AllUniqueCount => AllUniqueConfigs.Length;
        internal int AllSharedCount => AllSharedConfigs.Length;
        internal int AllBlittableCount => AllBlittableConfigs.Length;
        internal int AllManagedCount => AllManagedConfigs.Length;

        internal ComponentConfig GetConfig(Type componentType)
            => _componentConfigTypes[componentType];

        internal ComponentConfig GetConfig(int componentIndex)
            => AllComponentConfigs[componentIndex];

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iRecordableComponentType = typeof(IRecordableComponent);
            var iUniqueComponentType = typeof(IUniqueComponent);
            var iSharedComponentType = typeof(ISharedComponent);
            var equatableType = typeof(IEquatable<>);
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    iComponentType.IsAssignableFrom(x));

            _componentConfigTypes = new Dictionary<Type, ComponentConfig>();
            var recordableIndex = 0;
            var uniqueIndex = 0;
            var sharedIndex = 0;
            var blittableIndex = 0;
            var managedIndex = 0;
            var sharedNoEquatableOrGetHashCode = new List<Type>();
            var sharedUniqueErrorTypes = new List<Type>();
            var configTypes = new List<ConfigType>();

            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                var config = new ComponentConfig();

                if (iRecordableComponentType.IsAssignableFrom(type))
                {
                    config.RecordableIndex = recordableIndex++;
                    config.IsRecordable = true;
                }

                if (iUniqueComponentType.IsAssignableFrom(type))
                {
                    config.UniqueIndex = uniqueIndex++;
                    config.IsUnique = true;
                    if (config.IsShared)
                        sharedUniqueErrorTypes.Add(type);
                }

                if (iSharedComponentType.IsAssignableFrom(type))
                {
                    config.SharedIndex = sharedIndex++;
                    config.IsShared = true;
                    var sharedEquatableType = equatableType.MakeGenericType(type);
                    var hasEquatable = type.GetInterfaces().Any(x => x.IsGenericType && x == sharedEquatableType);
                    var getHashOverride = type.GetMethod("GetHashCode").DeclaringType == type;
                    if (!hasEquatable || !getHashOverride)
                        sharedNoEquatableOrGetHashCode.Add(type);
                }

                if (IsBlittable(type))
                {
                    config.BlittableIndex = blittableIndex++;
                    config.IsBlittable = true;
                    config.UnmanagedSizeInBytes = Marshal.SizeOf(type);
                }
                else
                {
                    config.ManagedIndex = managedIndex++;
                    config.IsManaged = true;
                    config.UnmanagedSizeInBytes = TypeCache<int>.SizeInBytes;
                }

                configTypes.Add(new ConfigType
                {
                    Config = config,
                    Type = type
                });
            }

            if (configTypes.Count == 0)
                throw new ComponentNoneException();
            if (sharedUniqueErrorTypes.Count > 0)
                throw new ComponentSharedUniqueException(sharedUniqueErrorTypes);
            if (sharedNoEquatableOrGetHashCode.Count > 0)
                throw new ComponentSharedUniqueException(sharedNoEquatableOrGetHashCode);

            /*configTypes = configTypes
                .OrderBy(x => x.Config.BlittableIndex)
                .ThenBy(x => x.Config.RecordableIndex)
                .ThenBy(x => x.Config.SharedIndex)
                .ThenBy(x => x.Config.UniqueIndex)
                .ThenBy(x => x.Type.Name)
                .ToList();*/

            _componentConfigTypes = new Dictionary<Type, ComponentConfig>();
            var componentConfigIndexes = new List<ComponentConfig>();
            foreach (var configType in configTypes)
            {
                configType.Config.ComponentIndex = componentConfigIndexes.Count();
                componentConfigIndexes.Add(configType.Config);
                _componentConfigTypes.Add(configType.Type, configType.Config);
            }

            AllComponentConfigs = componentConfigIndexes
                .OrderBy(x => x.ComponentIndex)
                .ToArray();
            AllRecordableConfigs = componentConfigIndexes
                .Where(x => x.IsRecordable)
                .OrderBy(x => x.RecordableIndex)
                .ToArray();
            AllUniqueConfigs = componentConfigIndexes
                .Where(x => x.IsUnique)
                .OrderBy(x => x.UniqueIndex)
                .ToArray();
            AllSharedConfigs = componentConfigIndexes
                .Where(x => x.IsShared)
                .OrderBy(x => x.SharedIndex)
                .ToArray();
            AllBlittableConfigs = componentConfigIndexes
                .Where(x => x.IsBlittable)
                .OrderBy(x => x.BlittableIndex)
                .ToArray();
            AllManagedConfigs = componentConfigIndexes
                .Where(x => x.IsManaged)
                .OrderBy(x => x.ManagedIndex)
                .ToArray();

            AllComponentTypes = _componentConfigTypes
                .OrderBy(x => x.Value.ComponentIndex)
                .Select(x => x.Key)
                .ToArray();
            AllRecordableTypes = _componentConfigTypes
                .Where(x => x.Value.IsRecordable)
                .OrderBy(x => x.Value.RecordableIndex)
                .Select(x => x.Key)
                .ToArray();
            AllUniqueTypes = _componentConfigTypes
                .Where(x => x.Value.IsUnique)
                .OrderBy(x => x.Value.UniqueIndex)
                .Select(x => x.Key)
                .ToArray();
            AllSharedTypes = _componentConfigTypes
                .Where(x => x.Value.IsShared)
                .OrderBy(x => x.Value.SharedIndex)
                .Select(x => x.Key)
                .ToArray();
            AllBlittableTypes = _componentConfigTypes
                .Where(x => x.Value.IsBlittable)
                .OrderBy(x => x.Value.BlittableIndex)
                .Select(x => x.Key)
                .ToArray();
            AllManagedTypes = _componentConfigTypes
                .Where(x => x.Value.IsManaged)
                .OrderBy(x => x.Value.ManagedIndex)
                .Select(x => x.Key)
                .ToArray();

            AllComponentIndexes = componentConfigIndexes
                .OrderBy(x => x.ComponentIndex)
                .Select(x => x.ComponentIndex)
                .ToArray();
            AllRecordableIndexes = componentConfigIndexes
                .Where(x => x.IsRecordable)
                .OrderBy(x => x.RecordableIndex)
                .Select(x => x.RecordableIndex)
                .ToArray();
            AllUniqueIndexes = componentConfigIndexes
                .Where(x => x.IsUnique)
                .OrderBy(x => x.UniqueIndex)
                .Select(x => x.UniqueIndex)
                .ToArray();
            AllSharedIndexes = componentConfigIndexes
                .Where(x => x.IsShared)
                .OrderBy(x => x.SharedIndex)
                .Select(x => x.SharedIndex)
                .ToArray();
            AllBlittableIndexes = componentConfigIndexes
                .Where(x => x.IsBlittable)
                .OrderBy(x => x.BlittableIndex)
                .Select(x => x.BlittableIndex)
                .ToArray();
            AllManagedIndexes = componentConfigIndexes
                .Where(x => x.IsManaged)
                .OrderBy(x => x.ManagedIndex)
                .Select(x => x.ManagedIndex)
                .ToArray();
        }

        private bool IsBlittable(Type type)
        {
            try
            {
                GCHandle.Alloc(
                    FormatterServices.GetUninitializedObject(type),
                    GCHandleType.Pinned)
                    .Free();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private class ConfigType
        {
            public ComponentConfig Config;
            public Type Type;
        }
    }
}
