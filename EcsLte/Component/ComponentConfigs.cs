using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace EcsLte
{
    internal static class ComponentConfigs
    {
        private static ComponentConfigInit _instance = new ComponentConfigInit();
        private static Dictionary<Type, ComponentConfig> _componentConfigTypes;

        internal static ComponentConfig[] AllComponentConfigs { get; private set; }
        internal static ComponentConfig[] AllGeneralConfigs { get; private set; }
        internal static ComponentConfig[] AllManagedConfigs { get; private set; }
        internal static ComponentConfig[] AllSharedConfigs { get; private set; }

        internal static Type[] AllComponentTypes { get; private set; }
        internal static Type[] AllGeneralTypes { get; private set; }
        internal static Type[] AllManagedTypes { get; private set; }
        internal static Type[] AllSharedTypes { get; private set; }

        internal static int[] AllComponentIndexes { get; private set; }
        internal static int[] AllGeneralIndexes { get; private set; }
        internal static int[] AllManagedIndexes { get; private set; }
        internal static int[] AllSharedIndexes { get; private set; }

        internal static int AllComponentCount { get => AllComponentConfigs.Length; }
        internal static int AllGeneralCount { get => AllGeneralConfigs.Length; }
        internal static int AllManagedCount { get => AllManagedConfigs.Length; }
        internal static int AllSharedCount { get => AllSharedConfigs.Length; }

        internal static IComponentAdapter[] AllComponentAdapters { get; private set; }
        internal static IComponentAdapter[] AllGeneralAdapters { get; private set; }
        internal static IComponentAdapter[] AllManagedAdapters { get; private set; }
        internal static IComponentAdapter[] AllSharedAdapters { get; private set; }

        public static ComponentConfig GetConfig(Type componentType)
            => _componentConfigTypes[componentType];

        public static ComponentConfig GetConfig(int componentIndex)
            => AllComponentConfigs[componentIndex];

        internal static IComponentPool[] CreateComponentPools(ComponentConfigOffset[] manageConfigs)
        {
            var pools = new IComponentPool[manageConfigs.Length];
            var poolType = typeof(ComponentPool<>);

            for (var i = 0; i < manageConfigs.Length; i++)
            {
                pools[i] = (IComponentPool)Activator.CreateInstance(
                    poolType.MakeGenericType(AllManagedTypes[manageConfigs[i].Config.ManagedIndex]));
            }

            return pools;
        }

        private static void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iGeneralComponentType = typeof(IGeneralComponent);
            var iManagedComponentType = typeof(IManagedComponent);
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
            var managedIndex = 0;
            var sharedIndex = 0;
            var configTypes = new List<ConfigType>();
            var errorSharedNoEquatableOrGetHashCode = new List<Type>();
            var errorGeneralSharedTypes = new List<Type>();
            var errorManagedGeneralSharedTypes = new List<Type>();
            var errorBlittableManagedTypes = new List<Type>();
            var errorNonBlittableNotManagedTypes = new List<Type>();

            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                var config = new ComponentConfig();

                if (iGeneralComponentType.IsAssignableFrom(type))
                {
                    config.GeneralIndex = GeneralIndex++;
                    config.IsGeneral = true;
                }

                if (iSharedComponentType.IsAssignableFrom(type))
                {
                    config.SharedIndex = sharedIndex++;
                    config.IsShared = true;
                    var sharedEquatableType = equatableType.MakeGenericType(type);
                    var hasEquatable = type.GetInterfaces().Any(x => x.IsGenericType && x == sharedEquatableType);
                    var getHashOverride = type.GetMethod("GetHashCode").DeclaringType == type;
                    if (!hasEquatable || !getHashOverride)
                        errorSharedNoEquatableOrGetHashCode.Add(type);

                    if (config.IsGeneral)
                        errorGeneralSharedTypes.Add(type);
                }

                if (iManagedComponentType.IsAssignableFrom(type))
                {
                    config.ManagedIndex = managedIndex++;
                    config.IsManaged = true;
                    if (config.IsShared || config.IsGeneral)
                        errorManagedGeneralSharedTypes.Add(type);
                }

                if (IsBlittable(type))
                {
                    // TODO need to check how flag components play in unmanaged arrays
                    config.UnmanagedSizeInBytes = Math.Max(1, Marshal.SizeOf(type));
                    if (config.IsManaged)
                        errorBlittableManagedTypes.Add(type);
                }
                else
                {
                    if (!config.IsManaged)
                        errorNonBlittableNotManagedTypes.Add(type);
                }

                configTypes.Add(new ConfigType
                {
                    Config = config,
                    Type = type
                });
            }

            if (configTypes.Count == 0)
                throw new ComponentNoneException();
            if (errorSharedNoEquatableOrGetHashCode.Count > 0)
                throw new ComponentNoSharedEquatableHashCodeException(errorSharedNoEquatableOrGetHashCode);
            if (errorGeneralSharedTypes.Count > 0)
                throw new ComponentGeneralSharedException(errorGeneralSharedTypes);
            if (errorManagedGeneralSharedTypes.Count > 0)
                throw new ComponentManagedGeneralSharedException(errorGeneralSharedTypes);
            if (errorBlittableManagedTypes.Count > 0)
                throw new ComponentBlittableManagedException(errorBlittableManagedTypes);
            if (errorNonBlittableNotManagedTypes.Count > 0)
                throw new ComponentNotBlittableNotManagedException(errorNonBlittableNotManagedTypes);

            /*configTypes = configTypes
                .OrderBy(x => x.Config.BlittableIndex)
                .ThenBy(x => x.Config.GeneralIndex)
                .ThenBy(x => x.Config.SharedIndex)
                .ThenBy(x => x.Config.ManagedIndex)
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
            AllManagedConfigs = componentConfigIndexes
                .Where(x => x.IsManaged)
                .OrderBy(x => x.ManagedIndex)
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
            AllManagedTypes = _componentConfigTypes
                .Where(x => x.Value.IsManaged)
                .OrderBy(x => x.Value.ManagedIndex)
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
            AllManagedIndexes = componentConfigIndexes
                .Where(x => x.IsManaged)
                .OrderBy(x => x.ManagedIndex)
                .Select(x => x.ManagedIndex)
                .ToArray();
            AllSharedIndexes = componentConfigIndexes
                .Where(x => x.IsShared)
                .OrderBy(x => x.SharedIndex)
                .Select(x => x.SharedIndex)
                .ToArray();

            var adapterGeneralType = typeof(ComponentGeneralAdapter<>);
            var adapterManagedType = typeof(ComponentManagedAdapter<>);
            var adapterSharedType = typeof(ComponentSharedAdapter<>);
            AllGeneralAdapters = _componentConfigTypes
                .Where(x => x.Value.IsGeneral)
                .OrderBy(x => x.Value.GeneralIndex)
                .Select(x => (IComponentAdapter)Activator.CreateInstance(
                    adapterGeneralType.MakeGenericType(x.Key)))
                .ToArray();
            AllManagedAdapters = _componentConfigTypes
                .Where(x => x.Value.IsManaged)
                .OrderBy(x => x.Value.ManagedIndex)
                .Select(x => (IComponentAdapter)Activator.CreateInstance(
                    adapterManagedType.MakeGenericType(x.Key)))
                .ToArray();
            AllSharedAdapters = _componentConfigTypes
                .Where(x => x.Value.IsShared)
                .OrderBy(x => x.Value.SharedIndex)
                .Select(x => (IComponentAdapter)Activator.CreateInstance(
                    adapterSharedType.MakeGenericType(x.Key)))
                .ToArray();
            var allAdapterList = new List<IComponentAdapter>(AllGeneralAdapters);
            allAdapterList.AddRange(AllManagedAdapters);
            allAdapterList.AddRange(AllSharedAdapters);
            AllComponentAdapters = allAdapterList
                .OrderBy(x => x.Config.ComponentIndex)
                .ToArray();
        }

        private static bool IsBlittable(Type type)
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
            internal ComponentConfig Config;
            internal Type Type;
        }

        private class ComponentConfigInit
        {
            internal ComponentConfigInit()
            {
                ComponentConfigs.Initialize();
            }
        }
    }
}