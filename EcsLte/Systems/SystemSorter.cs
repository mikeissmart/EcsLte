using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal class SystemSorter : IComparable<SystemSorter>
    {
        private readonly HashSet<SystemSorter> _afters;
        private readonly HashSet<SystemSorter> _befores;

        internal Type SystemType { get; private set; }
        internal SystemConfig Config { get; set; }
        internal string SystemName => SystemType.Name;

        internal SystemSorter(Type systemType, SystemConfig config)
        {
            _afters = new HashSet<SystemSorter>();
            _befores = new HashSet<SystemSorter>();

            SystemType = systemType;
            Config = config;
        }

        internal void AddBefore(SystemSorter sorter)
        {
            _befores.Add(sorter);
            sorter._afters.Add(this);
        }

        internal void AddAfter(SystemSorter sorter)
        {
            _afters.Add(sorter);
            sorter._befores.Add(this);
        }

        internal bool HasErrors(out string error)
        {
            error = "";
            foreach (var item in _befores)
            {
                if (_afters.Contains(item))
                {
                    error = string.Format("'{0}' : system can't loop back to '{1}' before system.",
                        SystemName, item.SystemName);
                    return true;
                }
            }

            error = CheckBefores(this, new HashSet<SystemSorter>());
            if (error != null)
                return true;

            error = CheckAfters(this, new HashSet<SystemSorter>());
            if (error != null)
                return true;

            return false;
        }

        public int CompareTo(SystemSorter other)
            => other == null
                ? 0
                : InAfters(other)
                    ? 1
                    : -1;

        private bool InBefores(SystemSorter sorter)
        {
            if (_befores.Contains(sorter))
                return true;

            foreach (var item in _befores)
            {
                if (item.InBefores(sorter))
                    return true;
            }

            return false;
        }

        private bool InAfters(SystemSorter sorter)
        {
            if (_afters.Contains(sorter))
                return true;

            foreach (var item in _afters)
            {
                if (item.InAfters(sorter))
                    return true;
            }

            return false;
        }

        private string CheckBefores(SystemSorter sorter, HashSet<SystemSorter> alreadyChecked)
        {
            if (!alreadyChecked.Add(this))
                // Prevents infinate loops
                return null;

            if (_befores.Contains(sorter))
            {
                return string.Format("'{0}' : system can't loop back to '{1}' before system.",
                    SystemName, sorter.SystemName);
            }

            foreach (var item in _befores)
            {
                var error = CheckBefores(sorter, alreadyChecked);
                if (error != null)
                    return SystemName + "->" + error;
            }

            return null;
        }

        private string CheckAfters(SystemSorter sorter, HashSet<SystemSorter> alreadyChecked)
        {
            if (!alreadyChecked.Add(this))
                // Prevents infinate loops
                return null;

            if (_afters.Contains(sorter))
            {
                return string.Format("'{0}' : system can't loop back to '{1}' after system.",
                    SystemName, sorter.SystemName);
            }

            foreach (var item in _afters)
            {
                var error = CheckAfters(sorter, alreadyChecked);
                if (error != null)
                    return SystemName + "->" + error;
            }

            return null;
        }
    }
}
