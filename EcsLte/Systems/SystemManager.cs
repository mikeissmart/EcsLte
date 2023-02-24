using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace EcsLte
{
    public class SystemManager
    {
        private SystemBase[] _allSystems;
        private HashSet<SystemRunner> _initializeSystems;
        private HashSet<SystemRunner> _updateSystems;
        private HashSet<SystemRunner> _activeSystems;
        private HashSet<SystemRunner> _deactiveSystems;
        private HashSet<SystemRunner> _uninitializeSystems;
        private bool _isUpdateDirty;
        private SystemRunner[] _runInitializeSystems;
        private SystemRunner[] _runActiveSystems;
        private SystemRunner[] _runUpdateSystems;
        private SystemRunner[] _runDeactiveSystems;
        private SystemRunner[] _runUninitializeSystems;

        public EcsContext Context { get; private set; }

        internal SystemManager(EcsContext context)
        {
            _allSystems = new SystemBase[SystemConfigs.AllSystemCount];
            _initializeSystems = new HashSet<SystemRunner>();
            _activeSystems = new HashSet<SystemRunner>();
            _updateSystems = new HashSet<SystemRunner>();
            _deactiveSystems = new HashSet<SystemRunner>();
            _uninitializeSystems = new HashSet<SystemRunner>();
            _isUpdateDirty = false;
            _runInitializeSystems = new SystemRunner[0];
            _runActiveSystems = new SystemRunner[0];
            _runUpdateSystems = new SystemRunner[0];
            _runDeactiveSystems = new SystemRunner[0];
            _runUninitializeSystems = new SystemRunner[0];

            Context = context;
        }

        public bool HasSystem(SystemBase system)
        {
            Context.AssertContext();
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (system.Context == null)
                return false;
            if (system.Context != Context)
                return false;

            return _allSystems[system.Config.SystemIndex] == system;
        }

        public bool HasSystem<TSystem>()
            where TSystem : SystemBase
        {
            Context.AssertContext();

            var config = SystemConfig<TSystem>.Config;

            return _allSystems[config.SystemIndex] != null;
        }

        public bool HasSystem(Type systemType)
        {
            Context.AssertContext();
            if (systemType == null)
                throw new ArgumentNullException(nameof(systemType));

            var config = SystemConfigs.GetConfig(systemType);

            return _allSystems[config.SystemIndex] != null;
        }

        public SystemBase[] GetAllSystems()
        {
            var systems = new SystemBase[0];
            GetAllSystems(ref systems, 0);

            return systems;
        }

        public int GetAllSystems(ref SystemBase[] systems) =>
            GetAllSystems(ref systems, 0);

        public int GetAllSystems(ref SystemBase[] systems, int startingIndex)
        {
            Context.AssertContext();
            Helper.AssertAndResizeArray(ref systems, startingIndex, _updateSystems.Count);

            Helper.ArrayCopy(_updateSystems.Select(x => x.System).ToArray(), 0, systems, startingIndex, _updateSystems.Count);

            return _updateSystems.Count;
        }

        public TSystem AddOrGetSystem<TSystem>()
            where TSystem : SystemBase, new()
        {
            Context.AssertContext();

            return (TSystem)InternalAddSystem(SystemConfig<TSystem>.Config);
        }

        public SystemBase AddOrGetSystem(Type systemType)
        {
            Context.AssertContext();

            var config = SystemConfigs.GetConfig(systemType);

            return InternalAddSystem(config);
        }

        public void AutoAddSystems()
        {
            Context.AssertContext();

            for (var i = 0; i < SystemConfigs.AllAutoAddSystemCount; i++)
            {
                var config = SystemConfigs.AllAutoAddSystemConfigs[i];
                if (_allSystems[config.SystemIndex] == null)
                {
                    InternalAddSystem(config);
                }
            }
        }

        public void RemoveSystem(SystemBase system)
        {
            Context.AssertContext();
            AssertSystem(system);
            AssertNotHaveSystem(system.Config);

            InternalRemoveSystem(system.Config);
        }

        public void RemoveSystem<TSystem>()
            where TSystem : SystemBase
        {
            Context.AssertContext();

            var config = SystemConfig<TSystem>.Config;
            AssertNotHaveSystem(config);

            InternalRemoveSystem(config);
        }

        public void RemoveSystem(Type systemType)
        {
            Context.AssertContext();

            var config = SystemConfigs.GetConfig(systemType);
            AssertNotHaveSystem(config);

            InternalRemoveSystem(config);
        }

        public void RemoveAllSystems()
        {
            Context.AssertContext();

            for (var i = 0; i < _allSystems.Length; i++)
            {
                if (_allSystems[i] != null)
                    InternalRemoveSystem(SystemConfigs.AllSystemConfigs[i]);
            }
        }

        public void ActivateSystem<TSystem>()
            where TSystem : SystemBase
        {
            Context.AssertContext();

            var config = SystemConfig<TSystem>.Config;
            AssertNotHaveSystem(config);

            var system = _allSystems[config.SystemIndex];
            if (!system.IsActive)
                _activeSystems.Add(new SystemRunner(system, config));
        }

        public void ActivateSystem(SystemBase system)
        {
            Context.AssertContext();
            AssertSystem(system);
            AssertNotHaveSystem(system.Config);

            if (!system.IsActive)
                _activeSystems.Add(new SystemRunner(system, system.Config));
        }

        public void ActivateSystem(Type systemType)
        {
            Context.AssertContext();

            var config = SystemConfigs.GetConfig(systemType);
            AssertNotHaveSystem(config);

            var system = _allSystems[config.SystemIndex];
            if (!system.IsActive)
                _activeSystems.Add(new SystemRunner(system, system.Config));
        }

        public void ActivateAllSystems()
        {
            Context.AssertContext();

            foreach (var system in _updateSystems)
            {
                if (!system.System.IsActive)
                    _activeSystems.Add(system);
            }
        }

        public void DeactivateSystem<TSystem>()
            where TSystem : SystemBase
        {
            var config = SystemConfig<TSystem>.Config;

            Context.AssertContext();
            AssertNotHaveSystem(config);

            var system = _allSystems[config.SystemIndex];
            if (system.IsActive)
                _deactiveSystems.Add(new SystemRunner(system, config));
        }

        public void DeactivateSystem(SystemBase system)
        {
            Context.AssertContext();
            AssertSystem(system);
            AssertNotHaveSystem(system.Config);

            if (system.IsActive)
                _deactiveSystems.Add(new SystemRunner(system, system.Config));
        }

        public void DeactivateSystem(Type systemType)
        {
            Context.AssertContext();

            var config = SystemConfigs.GetConfig(systemType);
            AssertNotHaveSystem(config);

            var system = _allSystems[config.SystemIndex];
            if (system.IsActive)
                _deactiveSystems.Add(new SystemRunner(system, config));
        }

        public void DeactivateAllSystem()
        {
            Context.AssertContext();

            foreach (var system in _updateSystems)
            {
                if (system.System.IsActive)
                    _deactiveSystems.Add(system);
            }
        }

        /// <summary>
        /// Initialize, Activated, Update, Deactivated, Uninitialize
        /// </summary>
        public void RunSystems()
        {
            Context.AssertContext();

            if (_initializeSystems.Count > 0)
            {
                foreach (var runner in _initializeSystems)
                {
                    _updateSystems.Add(runner);
                    _allSystems[runner.Config.SystemIndex] = runner.System;
                }
                _isUpdateDirty = true;
            }
            if (_uninitializeSystems.Count > 0)
            {
                foreach (var runner in _uninitializeSystems)
                {
                    _updateSystems.Remove(runner);
                    _allSystems[runner.Config.SystemIndex] = null;
                }
                _isUpdateDirty = true;
            }

            Helper.ResizeRefArray(ref _runInitializeSystems, 0, _initializeSystems.Count);
            Helper.ResizeRefArray(ref _runActiveSystems, 0, _activeSystems.Count);
            Helper.ResizeRefArray(ref _runDeactiveSystems, 0, _deactiveSystems.Count);
            Helper.ResizeRefArray(ref _runUninitializeSystems, 0, _uninitializeSystems.Count);

            Array.Clear(_runDeactiveSystems, 0, _runDeactiveSystems.Length);

            _initializeSystems.CopyTo(_runInitializeSystems);
            _activeSystems.CopyTo(_runActiveSystems);
            _deactiveSystems.CopyTo(_runDeactiveSystems);
            _uninitializeSystems.CopyTo(_runUninitializeSystems);

            Array.Sort(_runInitializeSystems);
            Array.Sort(_runActiveSystems);
            Array.Sort(_runDeactiveSystems);
            Array.Sort(_runUninitializeSystems);

            if (_isUpdateDirty)
            {
                Helper.ResizeRefArray(ref _runUpdateSystems, 0, _updateSystems.Count);
                _updateSystems.CopyTo(_runUpdateSystems);
                Array.Sort(_runUpdateSystems);
                _isUpdateDirty = false;
            }

            var initalizeCount = _initializeSystems.Count;
            var activeCount = _activeSystems.Count;
            var updateCount = _updateSystems.Count;
            var deactiveCount = _deactiveSystems.Count;
            var uninitializeCount = _uninitializeSystems.Count;

            _initializeSystems.Clear();
            _activeSystems.Clear();
            _deactiveSystems.Clear();
            _uninitializeSystems.Clear();

            var stopwatch = new Stopwatch();
            for (var i = 0; i < initalizeCount; i++)
            {
                var system = _runInitializeSystems[i].System;
                system.IsInitialized = true;
                stopwatch.Start();
                system.Initialize();
                stopwatch.Stop();

                system.InitializeMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            for (var i = 0; i < activeCount; i++)
            {
                var system = _runActiveSystems[i].System;
                system.IsActive = true;
                stopwatch.Start();
                system.Activated();
                stopwatch.Stop();

                system.ActivatedMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            for (var i = 0; i < updateCount; i++)
            {
                var runner = _runUpdateSystems[i];
                if (runner.System.IsActive && !_runDeactiveSystems.Contains(runner))
                {
                    stopwatch.Start();
                    runner.System.Update();
                    stopwatch.Stop();

                    runner.System.UpdateMilliseconds = stopwatch.ElapsedMilliseconds;
                }
            }

            for (var i = 0; i < deactiveCount; i++)
            {
                var system = _runDeactiveSystems[i].System;
                system.IsActive = false;
                stopwatch.Start();
                system.Deactivated();
                stopwatch.Stop();

                system.DeactivatedMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            for (var i = 0; i < uninitializeCount; i++)
            {
                var system = _runUninitializeSystems[i].System;
                system.IsInitialized = false;
                stopwatch.Start();
                system.Uninitialize();
                stopwatch.Stop();

                system.UninitializeMilliseconds = stopwatch.ElapsedMilliseconds;
            }
        }

        internal void InternalDestroy()
        {
            _allSystems = null;
            _initializeSystems = null;
            _updateSystems = null;
            _activeSystems = null;
            _deactiveSystems = null;
            _uninitializeSystems = null;
            _runInitializeSystems = null;
            _runActiveSystems = null;
            _runUpdateSystems = null;
            _runDeactiveSystems = null;
            _runUninitializeSystems = null;
        }

        private SystemBase InternalAddSystem(SystemConfig config)
        {
            if (_allSystems[config.SystemIndex] != null)
                return _allSystems[config.SystemIndex];

            var runner = _initializeSystems.FirstOrDefault(x => x.Config == config);
            if (runner == null)
            {
                var system = (SystemBase)Activator.CreateInstance(config.SystemType);
                system.Context = Context;
                system.Config = config;

                runner = new SystemRunner(system, config);
                _activeSystems.Add(runner);
                _initializeSystems.Add(runner);

                _isUpdateDirty = true;
            }

            return runner.System;
        }

        private void InternalRemoveSystem(SystemConfig config)
        {
            var system = _allSystems[config.SystemIndex];

            var runner = new SystemRunner(system, config);
            if (system.IsActive)
                _deactiveSystems.Add(runner);
            _uninitializeSystems.Add(runner);

            _isUpdateDirty = true;
        }

        #region Assert

        private void AssertNotHaveSystem(SystemConfig config)
        {
            if (_allSystems[config.SystemIndex] == null)
            {
                throw new SystenNotHaveException(SystemConfigs
                    .AllSystemTypes[config.SystemIndex]);
            }
        }

        private void AssertAlreadyHaveSystem(SystemConfig config)
        {
            if (_allSystems[config.SystemIndex] != null)
            {
                throw new SystemAlreadyHaveException(SystemConfigs
                    .AllSystemTypes[config.SystemIndex]);
            }
        }

        internal void AssertSystem(SystemBase system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));
            if (system.Context == null)
                throw new SystenNotHaveException(system.GetType());
            if (system.Context != Context)
                throw new EcsContextNotSameException(system.Context, Context);
        }

        #endregion

        private class SystemRunner : IComparable<SystemRunner>, IEquatable<SystemRunner>
        {
            internal SystemBase System { get; set; }
            internal SystemConfig Config { get; set; }

            internal SystemRunner(SystemBase system, SystemConfig config)
            {
                System = system;
                Config = config;
            }

            #region Equals

            public static bool operator !=(SystemRunner lhs, SystemRunner rhs)
                => !(lhs == rhs);

            public static bool operator ==(SystemRunner lhs, SystemRunner rhs)
            {
                if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                    return true;
                if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                    return false;

                return lhs.Config == rhs.Config;
            }

            public bool Equals(SystemRunner other)
                => this == other;

            public override bool Equals(object other)
                => other is SystemRunner obj && this == obj;

            #endregion

            public int CompareTo(SystemRunner other)
                => Config.CompareTo(other.Config);
            public override int GetHashCode()
                => Config.GetHashCode();
        }
    }
}
