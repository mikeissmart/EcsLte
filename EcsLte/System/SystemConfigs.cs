using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    internal class SystemConfigs
    {
        private static SystemConfigs _instance;
        private Dictionary<int, SystemConfig> _systemConfigIndexes;
        private Dictionary<Type, SystemConfig> _systemConfigTypes;

        private SystemConfigs() => Initialize();

        internal static SystemConfigs Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SystemConfigs();
                return _instance;
            }
        }

        internal SystemConfig[] AllSystemConfigs { get; private set; }
        internal SystemConfig[] AllInitializeConfigs { get; private set; }
        internal SystemConfig[] AllExecuteConfigs { get; private set; }
        internal SystemConfig[] AllCleanupConfigs { get; private set; }
        internal SystemConfig[] AllTearDownConfigs { get; private set; }
        internal SystemConfig[] AllAutoAddSystemConfigs { get; private set; }

        internal Type[] AllSystemTypes { get; private set; }
        internal Type[] AllInitializeTypes { get; private set; }
        internal Type[] AllExecuteTypes { get; private set; }
        internal Type[] AllCleanupTypes { get; private set; }
        internal Type[] AllTearDownTypes { get; private set; }
        internal Type[] AllAutoAddSystemTypes { get; private set; }

        internal int[] AllInitializeIndexes { get; private set; }
        internal int[] AllExecuteIndexes { get; private set; }
        internal int[] AllCleanupIndexes { get; private set; }
        internal int[] AllTearDownIndexes { get; private set; }
        internal int[] AlAutoAddSystemIndexes { get; private set; }

        internal int AllSystemCount { get; private set; }
        internal int AllInitializeCount { get; private set; }
        internal int AllExecuteCount { get; private set; }
        internal int AllCleanupCount { get; private set; }
        internal int AllTearDownCount { get; private set; }
        internal int AllAutoAddCount { get; private set; }

        internal SystemConfig GetConfig(Type componentType)
            => _systemConfigTypes[componentType];

        internal SystemConfig GetConfig(int componentIndex)
            => _systemConfigIndexes[componentIndex];

        private void Initialize()
        {
            var systemBaseType = typeof(SystemBase);
            var iInitializeSystem = typeof(IInitializeSystem);
            var iExecuteSystem = typeof(IExecuteSystem);
            var iCleanupSystem = typeof(ICleanupSystem);
            var iTearDownSystem = typeof(ITearDownSystem);
            var beforeSystemAttr = typeof(BeforeSystemAttribute);
            var afterSystemAttr = typeof(AfterSystemAttribute);
            var systemSorters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    systemBaseType.IsAssignableFrom(x))
                .Select(x => new SystemSorter(x, new SystemConfig
                {
                    IsInitialize = iInitializeSystem.IsAssignableFrom(x),
                    IsExecute = iExecuteSystem.IsAssignableFrom(x),
                    IsCleanup = iCleanupSystem.IsAssignableFrom(x),
                    IsTearDown = iTearDownSystem.IsAssignableFrom(x),
                    IsAutoAdd = x.GetCustomAttributes(typeof(DontAutoAddSystem), true).Length == 0
                }))
                .ToArray();

            foreach (var sorter in systemSorters)
            {
                foreach (var attr in (BeforeSystemAttribute[])sorter.SystemType.GetCustomAttributes(beforeSystemAttr, true))
                {
                    foreach (var item in systemSorters.Where(x => attr.Systems.Contains(x.SystemType)))
                        sorter.AddBefore(item);
                }

                foreach (var attr in (AfterSystemAttribute[])sorter.SystemType.GetCustomAttributes(afterSystemAttr, true))
                {
                    foreach (var item in systemSorters.Where(x => attr.Systems.Contains(x.SystemType)))
                        sorter.AddAfter(item);
                }

                // Check for errors
                if (sorter.HasErrors(out var error))
                    throw new SystemSortException(error);
            }
            Array.Sort(systemSorters);

            _systemConfigIndexes = new Dictionary<int, SystemConfig>();
            _systemConfigTypes = new Dictionary<Type, SystemConfig>();
            var systemIndex = 0;
            var initializeIndex = 0;
            var executeIndex = 0;
            var cleanupIndex = 0;
            var tearDownIndex = 0;
            foreach (var sorter in systemSorters)
            {
                var config = sorter.Config;

                config.SystemIndex = systemIndex++;
                if (config.IsInitialize)
                    config.InitializeIndex = initializeIndex++;
                if (config.IsExecute)
                    config.ExecuteIndex = executeIndex++;
                if (config.IsCleanup)
                    config.CleanupIndex = cleanupIndex++;
                if (config.IsTearDown)
                    config.TearDownIndex = tearDownIndex++;

                sorter.Config = config;

                _systemConfigIndexes.Add(config.SystemIndex, config);
                _systemConfigTypes.Add(sorter.SystemType, sorter.Config);
            }

            var initializeSystems = systemSorters
                .Where(x => x.Config.IsInitialize);
            var executeSystems = systemSorters
                .Where(x => x.Config.IsExecute);
            var cleanupSystems = systemSorters
                .Where(x => x.Config.IsCleanup);
            var tearDownSystems = systemSorters
                .Where(x => x.Config.IsTearDown);
            var autoAddSystems = systemSorters
                .Where(x => x.Config.IsAutoAdd);

            AllSystemConfigs = systemSorters
                .Select(x => x.Config)
                .ToArray();
            AllInitializeConfigs = initializeSystems
                .Select(x => x.Config)
                .ToArray();
            AllExecuteConfigs = executeSystems
                .Select(x => x.Config)
                .ToArray();
            AllCleanupConfigs = cleanupSystems
                .Select(x => x.Config)
                .ToArray();
            AllTearDownConfigs = tearDownSystems
                .Select(x => x.Config)
                .ToArray();
            AllAutoAddSystemConfigs = autoAddSystems
                .Select(x => x.Config)
                .ToArray();

            AllSystemTypes = systemSorters
                .Select(x => x.SystemType)
                .ToArray();
            AllInitializeTypes = initializeSystems
                .Select(x => x.SystemType)
                .ToArray();
            AllExecuteTypes = executeSystems
                .Select(x => x.SystemType)
                .ToArray();
            AllCleanupTypes = cleanupSystems
                .Select(x => x.SystemType)
                .ToArray();
            AllTearDownTypes = tearDownSystems
                .Select(x => x.SystemType)
                .ToArray();
            AllAutoAddSystemTypes = autoAddSystems
                .Select(x => x.SystemType)
                .ToArray();

            AllInitializeIndexes = initializeSystems
                .Select(x => x.Config.InitializeIndex)
                .ToArray();
            AllExecuteIndexes = executeSystems
                .Select(x => x.Config.ExecuteIndex)
                .ToArray();
            AllCleanupIndexes = cleanupSystems
                .Select(x => x.Config.CleanupIndex)
                .ToArray();
            AllTearDownIndexes = tearDownSystems
                .Select(x => x.Config.TearDownIndex)
                .ToArray();
            AlAutoAddSystemIndexes = autoAddSystems
                .Select(x => x.Config.SystemIndex)
                .ToArray();

            AllSystemCount = systemSorters.Count();
            AllInitializeCount = initializeSystems.Count();
            AllExecuteCount = executeSystems.Count();
            AllCleanupCount = cleanupSystems.Count();
            AllTearDownCount = tearDownSystems.Count();
            AllAutoAddCount = autoAddSystems.Count();
        }
    }
}