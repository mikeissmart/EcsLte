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
        internal ComponentConfig[] AllGeneralConfigs { get; private set; }
        internal ComponentConfig[] AllUniqueConfigs { get; private set; }
        internal ComponentConfig[] AllSharedConfigs { get; private set; }

        internal Type[] AllComponentTypes { get; private set; }
        internal Type[] AllGeneralTypes { get; private set; }
        internal Type[] AllUniqueTypes { get; private set; }
        internal Type[] AllSharedTypes { get; private set; }

        internal int[] AllComponentIndexes { get; private set; }
        internal int[] AllGeneralIndexes { get; private set; }
        internal int[] AllUniqueIndexes { get; private set; }
        internal int[] AllSharedIndexes { get; private set; }

        internal int AllComponentCount => AllComponentConfigs.Length;
        internal int AllGeneralCount => AllGeneralConfigs.Length;
        internal int AllUniqueCount => AllUniqueConfigs.Length;
        internal int AllSharedCount => AllSharedConfigs.Length;

        internal ComponentConfig GetConfig(Type componentType)
            => _componentConfigTypes[componentType];

        internal ComponentConfig GetConfig(int componentIndex)
            => AllComponentConfigs[componentIndex];

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iGeneralComponentType = typeof(IGeneralComponent);
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
            var GeneralIndex = 0;
            var uniqueIndex = 0;
            var sharedIndex = 0;
            var sharedNoEquatableOrGetHashCode = new List<Type>();
            var multipleComponentTypes = new List<Type>();
            var notBlittableTypes = new List<Type>();
            var configTypes = new List<ConfigType>();

            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                var config = new ComponentConfig();

                if (iGeneralComponentType.IsAssignableFrom(type))
                {
                    config.GeneralIndex = GeneralIndex++;
                    config.IsGeneral = true;
                }

                if (iUniqueComponentType.IsAssignableFrom(type))
                {
                    config.UniqueIndex = uniqueIndex++;
                    config.IsUnique = true;
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
                    config.UnmanagedSizeInBytes = Marshal.SizeOf(type);
                }
                else
                {
                    notBlittableTypes.Add(type);
                }

                configTypes.Add(new ConfigType
                {
                    Config = config,
                    Type = type
                });
            }

            if (configTypes.Count == 0)
                throw new ComponentNoneException();
            if (notBlittableTypes.Count > 0)
                throw new ComponentNotBlittalbeException(notBlittableTypes);
            if (multipleComponentTypes.Count > 0)
                throw new ComponentMultipleTypesException(multipleComponentTypes);
            if (sharedNoEquatableOrGetHashCode.Count > 0)
                throw new ComponentNoSharedEquatableHashCodeException(sharedNoEquatableOrGetHashCode);

            /*configTypes = configTypes
                .OrderBy(x => x.Config.BlittableIndex)
                .ThenBy(x => x.Config.GeneralIndex)
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
            AllGeneralConfigs = componentConfigIndexes
                .Where(x => x.IsGeneral)
                .OrderBy(x => x.GeneralIndex)
                .ToArray();
            AllUniqueConfigs = componentConfigIndexes
                .Where(x => x.IsUnique)
                .OrderBy(x => x.UniqueIndex)
                .ToArray();
            AllSharedConfigs = componentConfigIndexes
                .Where(x => x.IsShared)
                .OrderBy(x => x.SharedIndex)
                .ToArray();

            AllComponentTypes = _componentConfigTypes
                .OrderBy(x => x.Value.ComponentIndex)
                .Select(x => x.Key)
                .ToArray();
            AllGeneralTypes = _componentConfigTypes
                .Where(x => x.Value.IsGeneral)
                .OrderBy(x => x.Value.GeneralIndex)
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

            AllComponentIndexes = componentConfigIndexes
                .OrderBy(x => x.ComponentIndex)
                .Select(x => x.ComponentIndex)
                .ToArray();
            AllGeneralIndexes = componentConfigIndexes
                .Where(x => x.IsGeneral)
                .OrderBy(x => x.GeneralIndex)
                .Select(x => x.GeneralIndex)
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