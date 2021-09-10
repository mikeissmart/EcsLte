using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class SystemSorter : IComparable<SystemSorter>
    {
        private readonly HashSet<SystemSorter> m_afters = new HashSet<SystemSorter>();
        private readonly HashSet<SystemSorter> m_befores = new HashSet<SystemSorter>();

        public SystemSorter(SystemBase system)
        {
            System = system;
        }

        public SystemSorter[] LinkBefores => m_befores.ToArray();
        public SystemSorter[] LinkAfters => m_afters.ToArray();
        public SystemBase System { get; protected set; }
        public string SystemName => System.GetType().Name;

        public int CompareTo(SystemSorter other)
        {
            return other == null
                ? 0
                : InAfters(other)
                    ? 1
                    : -1;
        }

        private bool InBefores(SystemSorter systemSorter)
        {
            if (m_befores.Contains(systemSorter))
                return true;

            foreach (var link in m_befores)
                if (link.InBefores(systemSorter))
                    return true;

            return false;
        }

        private bool InAfters(SystemSorter systemSorter)
        {
            if (m_afters.Contains(systemSorter))
                return true;

            foreach (var link in m_afters)
                if (link.InAfters(systemSorter))
                    return true;

            return false;
        }

        private string CheckBefores(SystemSorter systemSorter, HashSet<SystemSorter> alreadyChecked)
        {
            if (!alreadyChecked.Add(this))
                // Prevents infinate loops
                return null;

            if (m_befores.Contains(systemSorter))
                return string.Format("'{0}' : system can't loop back to '{1}' before system.",
                    SystemName, systemSorter.SystemName);

            foreach (var link in m_befores)
            {
                var error = link.CheckBefores(systemSorter, alreadyChecked);
                if (error != null)
                    return SystemName + "->" + error;
            }

            return null;
        }

        private string CheckAfters(SystemSorter systemSorter, HashSet<SystemSorter> alreadyChecked)
        {
            if (!alreadyChecked.Add(this))
                // Prevents infinate loops
                return null;

            if (m_afters.Contains(systemSorter))
                return string.Format("'{0}' : system can't loop back to '{1}' after system.",
                    SystemName, systemSorter.SystemName);

            foreach (var link in m_afters)
            {
                var error = link.CheckAfters(systemSorter, alreadyChecked);
                if (error != null)
                    return SystemName + "->" + error;
            }

            return null;
        }

        public void AddBefore(SystemSorter systemSorter)
        {
            m_befores.Add(systemSorter);
            systemSorter.m_afters.Add(this);
        }

        public void AddAfter(SystemSorter systemSorter)
        {
            m_afters.Add(systemSorter);
            systemSorter.m_befores.Add(this);
        }

        public string CheckErrors()
        {
            foreach (var before in m_befores)
                if (m_afters.Contains(before))
                    return string.Format("'{0}' : system can't have '{1}' system set before and after.",
                        SystemName, before.SystemName);

            var error = CheckBefores(this, new HashSet<SystemSorter>());
            if (error != null)
                return error;

            error = CheckAfters(this, new HashSet<SystemSorter>());
            if (error != null)
                return error;

            return null;
        }

        public override string ToString()
        {
            return SystemName;
        }
    }
}