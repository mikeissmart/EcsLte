using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class SystemManagerData
    {
        public DataCache<Dictionary<Type, SystemBase>, SystemBase[]> Systems;

        public SystemManagerData()
        {
            Systems = new DataCache<Dictionary<Type, SystemBase>, SystemBase[]>(
                new Dictionary<Type, SystemBase>(),
                UpdateCachedSystems);
        }

        public void Reset()
        {
            Systems.UncachedData.Clear();
            Systems.IsDirty = true;
            Array.Clear(Systems.CachedData, 0, Systems.CachedData.Length);
        }

        public SystemSorter[] GetSystemSorters()
        {
            return LocalGetSystemSorters(Systems.UncachedData);
        }

        private static SystemSorter[] LocalGetSystemSorters(Dictionary<Type, SystemBase> unsorted)
        {
            var systemSorters = unsorted.Values
                .Where(x => x.IsActive)
                .Select(x => new SystemSorter(x))
                .ToList();
            var systemBaseType = typeof(SystemBase);

            // Generate befores and afters
            foreach (var systemSorter in systemSorters)
            {
                foreach (var attr in (BeforeSystemAttribute[])systemSorter.System.GetType()
                    .GetCustomAttributes(typeof(BeforeSystemAttribute), true))
                {
                    // Apply before to systemSorter
                    foreach (var linkedSystemSorter in systemSorters
                        .Where(x => attr.Systems.Contains(x.System.GetType())))
                    {
                        systemSorter.AddBefore(linkedSystemSorter);
                    }
                }

                foreach (var attr in (AfterSystemAttribute[])systemSorter.System.GetType()
                    .GetCustomAttributes(typeof(AfterSystemAttribute), true))
                {
                    // Apply after to systemSorter
                    foreach (var linkedSystemSorter in systemSorters
                        .Where(x => attr.Systems.Contains(x.System.GetType())))
                    {
                        systemSorter.AddAfter(linkedSystemSorter);
                    }
                }

                // Check for errors
                var error = systemSorter.CheckErrors();
                if (error != null)
                    throw new SystemSortException(error);
            }

            systemSorters.Sort();
            return systemSorters.ToArray();
        }

        private static SystemBase[] UpdateCachedSystems(Dictionary<Type, SystemBase> uncachedData)
        {
            return LocalGetSystemSorters(uncachedData)
                .Select(x => x.System)
                .ToArray();
        }
    }
}