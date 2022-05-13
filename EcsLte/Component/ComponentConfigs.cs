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
        internal ComponentConfig[] AllSharedConfigs { get; private set; }
        internal ComponentConfig[] AllUniqueConfigs { get; private set; }

        internal Type[] AllComponentTypes { get; private set; }
        internal Type[] AllRecordableTypes { get; private set; }
        internal Type[] AllSharedTypes { get; private set; }
        internal Type[] AllUniqueTypes { get; private set; }

        internal int[] AllRecordableIndexes { get; private set; }
        internal int[] AllSharedIndexes { get; private set; }
        internal int[] AllUniqueIndexes { get; private set; }

        internal int AllComponentCount { get; private set; }
        internal int AllRecordableComponentCount { get; private set; }
        internal int AllSharedComponentCount { get; private set; }
        internal int AllUniqueComponentCount { get; private set; }

        internal ComponentConfig GetConfig(Type componentType)
            => _componentConfigTypes[componentType];

        internal ComponentConfig GetConfig(int componentIndex)
            => _componentConfigIndexes[componentIndex];

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

            var recordableIndex = 0;
            var uniqueIndex = 0;
            var sharedIndex = 0;
            var recordableNonBlittableErrorTypes = new List<Type>();
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

                if (iUniqueComponentType.IsAssignableFrom(type))
                {
                    config.UniqueIndex = uniqueIndex++;
                    config.IsUnique = true;
                    if (config.IsShared)
                        sharedUniqueErrorTypes.Add(type);
                }

                if (IsBlittable(type))
                {
                    config.IsBlittable = true;
                    config.UnmanagedSizeInBytes = Marshal.SizeOf(type);
                }
                else if (config.IsRecordable)
                {
                    recordableNonBlittableErrorTypes.Add(type);
                }

                configTypes.Add(new ConfigType
                {
                    Config = config,
                    Type = type
                });
            }

            if (configTypes.Count == 0)
                throw new ComponentNoneException();
            if (recordableNonBlittableErrorTypes.Count > 0)
                throw new ComponentRecordableNotBlittableException(recordableNonBlittableErrorTypes);
            if (sharedUniqueErrorTypes.Count > 0)
                throw new ComponentSharedUniqueException(sharedUniqueErrorTypes);
            if (sharedNoEquatableOrGetHashCode.Count > 0)
                throw new ComponentSharedUniqueException(sharedNoEquatableOrGetHashCode);

            configTypes = configTypes
                .OrderBy(x => x.Config.RecordableIndex)
                .ThenBy(x => x.Config.SharedIndex)
                .ThenBy(x => x.Config.UniqueIndex)
                .ThenBy(x => x.Type.Name)
                .ToList();

            _componentConfigTypes = new Dictionary<Type, ComponentConfig>();
            _componentConfigIndexes = new Dictionary<int, ComponentConfig>();
            var componentIndex = 0;
            foreach (var configType in configTypes)
            {
                configType.Config.ComponentIndex = componentIndex++;

                _componentConfigTypes.Add(configType.Type, configType.Config);
                _componentConfigIndexes.Add(componentIndex, configType.Config);
            }

            AllComponentConfigs = configTypes
                .OrderBy(x => x.Config.ComponentIndex)
                .Select(x => x.Config)
                .ToArray();
            AllRecordableConfigs = configTypes
                .Where(x => x.Config.IsRecordable)
                .OrderBy(x => x.Config.RecordableIndex)
                .Select(x => x.Config)
                .ToArray();
            AllSharedConfigs = configTypes
                .Where(x => x.Config.IsShared)
                .OrderBy(x => x.Config.SharedIndex)
                .Select(x => x.Config)
                .ToArray();
            AllUniqueConfigs = configTypes
                .Where(x => x.Config.IsUnique)
                .OrderBy(x => x.Config.UniqueIndex)
                .Select(x => x.Config)
                .ToArray();

            AllComponentTypes = configTypes
                .OrderBy(x => x.Config.ComponentIndex)
                .Select(x => x.Type)
                .ToArray();
            AllRecordableTypes = configTypes
                .Where(x => x.Config.IsRecordable)
                .OrderBy(x => x.Config.RecordableIndex)
                .Select(x => x.Type)
                .ToArray();
            AllSharedTypes = configTypes
                .Where(x => x.Config.IsShared)
                .OrderBy(x => x.Config.SharedIndex)
                .Select(x => x.Type)
                .ToArray();
            AllUniqueTypes = configTypes
                .Where(x => x.Config.IsUnique)
                .OrderBy(x => x.Config.UniqueIndex)
                .Select(x => x.Type)
                .ToArray();

            AllRecordableIndexes = configTypes
                .Where(x => x.Config.IsRecordable)
                .OrderBy(x => x.Config.RecordableIndex)
                .Select(x => x.Config.RecordableIndex)
                .ToArray();
            AllSharedIndexes = configTypes
                .Where(x => x.Config.IsShared)
                .OrderBy(x => x.Config.SharedIndex)
                .Select(x => x.Config.SharedIndex)
                .ToArray();
            AllUniqueIndexes = configTypes
                .Where(x => x.Config.IsUnique)
                .OrderBy(x => x.Config.UniqueIndex)
                .Select(x => x.Config.UniqueIndex)
                .ToArray();

            AllComponentCount = AllComponentTypes.Length;
            AllRecordableComponentCount = AllRecordableTypes.Length;
            AllSharedComponentCount = AllSharedTypes.Length;
            AllUniqueComponentCount = AllUniqueTypes.Length;
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
