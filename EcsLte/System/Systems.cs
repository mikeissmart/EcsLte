using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class Systems : ISystem
    {
        private readonly List<ISystem> _unsortedSystems;
        private List<ISystem> _sortedSystems;

        public Systems()
        {
            _unsortedSystems = new List<ISystem>();
        }

        public bool IsSorted { get; private set; }

        public virtual void Cleanup()
        {
            Sort();
            foreach (var system in _sortedSystems)
                system.Cleanup();
        }

        public virtual void Execute()
        {
            Sort();
            foreach (var system in _sortedSystems)
                system.Execute();
        }

        public virtual void Initialize()
        {
            Sort();
            foreach (var system in _sortedSystems)
                system.Initialize();
        }

        public virtual void TearDown()
        {
            Sort();
            foreach (var system in _sortedSystems)
                system.TearDown();
        }

        public List<ISystem> GetSystems()
        {
            Sort();
            return _sortedSystems;
        }

        public SystemSorter[] GetSystemSorters()
        {
            return SortBeforeAndAfter(GetChildSystems()).ToArray();
        }

        public virtual Systems Add(ISystem system)
        {
            _unsortedSystems.Add(system);
            IsSorted = false;

            return this;
        }

        public virtual void Sort()
        {
            if (IsSorted)
                return;
            IsSorted = true;

            _sortedSystems = SortBeforeAndAfter(GetChildSystems())
                .Select(x => x.System)
                .ToList();
        }

        private List<ISystem> GetChildSystems()
        {
            var childSystems = new List<ISystem>();
            foreach (var system in _unsortedSystems)
            {
                var systems = system as Systems;
                if (systems != null)
                    childSystems.AddRange(systems.GetChildSystems());
                else
                    childSystems.Add(system);
            }

            return childSystems;
        }

        private List<SystemSorter> SortBeforeAndAfter(List<ISystem> unsortedList)
        {
            var systemSorters = unsortedList.Select(x => new SystemSorter(x)).ToList();
            var iSystemType = typeof(ISystem);

            foreach (var sorter in systemSorters)
            {
                // Add befores
                foreach (var attr in (BeforeSystemAttribute[]) sorter.System.GetType()
                    .GetCustomAttributes(typeof(BeforeSystemAttribute), true))
                foreach (var linkedSorter in systemSorters
                    .Where(x => attr.Systems.Contains(x.System.GetType())))
                    sorter.AddBefore(linkedSorter);

                // Add afters
                foreach (var attr in (AfterSystemAttribute[]) sorter.System.GetType()
                    .GetCustomAttributes(typeof(AfterSystemAttribute), true))
                foreach (var linkedSorter in systemSorters
                    .Where(x => attr.Systems.Contains(x.System.GetType())))
                    sorter.AddAfter(linkedSorter);

                var error = sorter.CheckErrors();
                if (error != null)
                    throw new Exception(error);
            }

            systemSorters.Sort();
            return systemSorters;
        }
    }
}