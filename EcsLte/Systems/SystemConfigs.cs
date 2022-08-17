using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    internal static class SystemConfigs
    {
        private static readonly SystemConfigInit _instance = new SystemConfigInit();
        private static Dictionary<int, SystemConfig> _systemConfigIndexes;
        private static Dictionary<Type, SystemConfig> _systemConfigTypes;

        internal static SystemConfig[] AllSystemConfigs { get; private set; }
        internal static SystemConfig[] AllAutoAddSystemConfigs { get; private set; }

        internal static Type[] AllSystemTypes { get; private set; }
        internal static Type[] AllAutoAddSystemTypes { get; private set; }

        internal static int[] AllAutoAddSystemIndexes { get; private set; }

        internal static int AllSystemCount { get; private set; }
        internal static int AllAutoAddSystemCount { get; private set; }

        internal static SystemConfig GetConfig(Type componentType)
            => _systemConfigTypes[componentType];

        internal static SystemConfig GetConfig(int componentIndex)
            => _systemConfigIndexes[componentIndex];

        private static void Initialize()
        {
            var systemBaseType = typeof(SystemBase);
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
                    IsAutoAdd = x.GetCustomAttributes(typeof(SystemNotAutoAdd), true).Length == 0
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
            foreach (var sorter in systemSorters)
            {
                var config = sorter.Config;

                config.SystemIndex = systemIndex++;
                sorter.Config = config;

                _systemConfigIndexes.Add(config.SystemIndex, config);
                _systemConfigTypes.Add(sorter.SystemType, sorter.Config);
            }

            var autoAddSystems = systemSorters
                .Where(x => x.Config.IsAutoAdd);

            AllSystemConfigs = systemSorters
                .Select(x => x.Config)
                .ToArray();
            AllAutoAddSystemConfigs = autoAddSystems
                .Select(x => x.Config)
                .ToArray();

            AllSystemTypes = systemSorters
                .Select(x => x.SystemType)
                .ToArray();
            AllAutoAddSystemTypes = autoAddSystems
                .Select(x => x.SystemType)
                .ToArray();

            AllAutoAddSystemIndexes = autoAddSystems
                .Select(x => x.Config.SystemIndex)
                .ToArray();

            AllSystemCount = systemSorters.Count();
            AllAutoAddSystemCount = autoAddSystems.Count();
        }

        private class SystemConfigInit
        {
            internal SystemConfigInit() => SystemConfigs.Initialize();
        }
    }
}
