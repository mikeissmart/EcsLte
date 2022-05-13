using EcsLte.Exceptions;
using System;
using System.Linq;

namespace EcsLte
{
    public class SystemManager
    {
        private SystemBase[] _allSystems;
        private IInitializeSystem[] _initializeSystems;
        private IExecuteSystem[] _executeSystems;
        private ICleanupSystem[] _cleanupSystems;
        private ITearDownSystem[] _tearDownSystems;
        private IInitializeSystem[] _cachedInitializeSystems;
        private IExecuteSystem[] _cachedExecuteSystems;
        private ICleanupSystem[] _cachedCleanupSystems;
        private ITearDownSystem[] _cachedTearDownSystems;
        private bool _isInitializeSystemsDirty;
        private bool _isExecuteSystemsDirty;
        private bool _isCleanupSystemsDirty;
        private bool _isTearDownSystemsDirty;

        public EcsContext Context { get; private set; }

        internal SystemManager(EcsContext context)
        {
            _allSystems = new SystemBase[SystemConfigs.Instance.AllSystemCount];
            _initializeSystems = new IInitializeSystem[0];
            _executeSystems = new IExecuteSystem[0];
            _cleanupSystems = new ICleanupSystem[0];
            _tearDownSystems = new ITearDownSystem[0];
            _cachedInitializeSystems = new IInitializeSystem[0];
            _cachedExecuteSystems = new IExecuteSystem[0];
            _cachedCleanupSystems = new ICleanupSystem[0];
            _cachedTearDownSystems = new ITearDownSystem[0];
            _isInitializeSystemsDirty = true;
            _isExecuteSystemsDirty = true;
            _isCleanupSystemsDirty = true;
            _isTearDownSystemsDirty = true;
            Context = context;
        }

        public bool HasSystem<TSystem>() where TSystem : SystemBase
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var config = SystemConfig<TSystem>.Config;
            return _allSystems[config.SystemIndex] != null;
        }

        public TSystem GetSystem<TSystem>() where TSystem : SystemBase
        {
            if (!HasSystem<TSystem>())
            {
                throw new SystenNotHaveException(SystemConfigs.Instance
                    .AllSystemTypes[SystemConfig<TSystem>.Config.SystemIndex]);
            }

            return (TSystem)_allSystems[SystemConfig<TSystem>.Config.SystemIndex];
        }

        public SystemBase[] GetAllSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _allSystems.Where(x => x != null).ToArray();
        }

        public IInitializeSystem[] GetAllInitializeSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _initializeSystems.ToArray();
        }
        public IExecuteSystem[] GetAllExecuteSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _executeSystems.ToArray();
        }

        public ICleanupSystem[] GetAllCleanupSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _cleanupSystems.ToArray();
        }

        public ITearDownSystem[] GetAllTearDownSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return _tearDownSystems.ToArray();
        }

        public TSystem AddSystem<TSystem>() where TSystem : SystemBase
        {
            if (HasSystem<TSystem>())
            {
                throw new SystemAlreadyHasException(SystemConfigs.Instance
                    .AllSystemTypes[SystemConfig<TSystem>.Config.SystemIndex]);
            }

            var system = (TSystem)Activator.CreateInstance(typeof(TSystem), new[] { Context });
            var config = SystemConfig<TSystem>.Config;

            system.Config = config;
            _allSystems[config.SystemIndex] = system;
            if (config.IsInitialize)
                UpdateInitialize();
            if (config.IsExecute)
                UpdateExecute();
            if (config.IsCleanup)
                UpdateCleanup();
            if (config.IsTearDown)
                UpdateTearDown();

            return system;
        }

        public void AutoAddSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            var initializeAdded = false;
            var executeAdded = false;
            var cleanupAdded = false;
            var tearDownAdded = false;
            var args = new[] { Context };

            for (var i = 0; i < SystemConfigs.Instance.AllAutoAddCount; i++)
            {
                var config = SystemConfigs.Instance.AllAutoAddSystemConfigs[i];
                if (_allSystems[config.SystemIndex] != null)
                    continue;

                var type = SystemConfigs.Instance.AllAutoAddSystemTypes[i];
                var system = (SystemBase)Activator.CreateInstance(type, args);
                system.Config = config;

                _allSystems[config.SystemIndex] = system;
                initializeAdded |= config.IsInitialize;
                executeAdded |= config.IsExecute;
                cleanupAdded |= config.IsCleanup;
                tearDownAdded |= config.IsTearDown;
            }

            if (initializeAdded)
                UpdateInitialize();
            if (executeAdded)
                UpdateExecute();
            if (cleanupAdded)
                UpdateCleanup();
            if (tearDownAdded)
                UpdateTearDown();
        }

        public void RunInitializeSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (_isInitializeSystemsDirty)
            {
                if (_cachedInitializeSystems.Length != _initializeSystems.Length)
                    _cachedInitializeSystems = _initializeSystems.ToArray();
                else
                    Array.Copy(_initializeSystems, _cachedInitializeSystems, _initializeSystems.Length);
                _isInitializeSystemsDirty = false;
            }

            for (var i = 0; i < _cachedInitializeSystems.Length; i++)
            {
                var system = _cachedInitializeSystems[i];
                if (system.IsActive)
                    system.Initialize();
            }
        }

        public void RunExecuteSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (_isExecuteSystemsDirty)
            {
                if (_cachedExecuteSystems.Length != _executeSystems.Length)
                    _cachedExecuteSystems = _executeSystems.ToArray();
                else
                    Array.Copy(_executeSystems, _cachedExecuteSystems, _executeSystems.Length);
                _isExecuteSystemsDirty = false;
            }

            for (var i = 0; i < _cachedExecuteSystems.Length; i++)
            {
                var system = _cachedExecuteSystems[i];
                if (system.IsActive)
                    system.Execute();
            }
        }

        public void RunCleanupSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (_isCleanupSystemsDirty)
            {
                if (_cachedCleanupSystems.Length != _cleanupSystems.Length)
                    _cachedCleanupSystems = _cleanupSystems.ToArray();
                else
                    Array.Copy(_cleanupSystems, _cachedCleanupSystems, _cleanupSystems.Length);
                _isCleanupSystemsDirty = false;
            }

            for (var i = 0; i < _cachedCleanupSystems.Length; i++)
            {
                var system = _cachedCleanupSystems[i];
                if (system.IsActive)
                    system.Cleanup();
            }
        }

        public void RunTearDownSystems()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            if (_isTearDownSystemsDirty)
            {
                if (_cachedTearDownSystems.Length != _tearDownSystems.Length)
                    _cachedTearDownSystems = _tearDownSystems.ToArray();
                else
                    Array.Copy(_tearDownSystems, _cachedTearDownSystems, _tearDownSystems.Length);
                _isTearDownSystemsDirty = false;
            }

            for (var i = 0; i < _cachedTearDownSystems.Length; i++)
            {
                var system = _cachedTearDownSystems[i];
                if (system.IsActive)
                    system.TearDown();
            }
        }

        internal void InternalDestroy()
        {
            _allSystems = null;
            _initializeSystems = null;
            _executeSystems = null;
            _cleanupSystems = null;
            _tearDownSystems = null;
            _cachedInitializeSystems = null;
            _cachedExecuteSystems = null;
            _cachedCleanupSystems = null;
            _cachedTearDownSystems = null;
            _isInitializeSystemsDirty = true;
            _isExecuteSystemsDirty = true;
            _isCleanupSystemsDirty = true;
            _isTearDownSystemsDirty = true;
        }

        private void UpdateInitialize()
        {
            _initializeSystems = _allSystems
                .Where(x => x != null && x.Config.IsInitialize)
                .Select(x => (IInitializeSystem)x)
                .ToArray();
            _isInitializeSystemsDirty = true;
        }

        private void UpdateExecute()
        {
            _executeSystems = _allSystems
                .Where(x => x != null && x.Config.IsExecute)
                .Select(x => (IExecuteSystem)x)
                .ToArray();
            _isExecuteSystemsDirty = true;
        }

        private void UpdateCleanup()
        {
            _cleanupSystems = _allSystems
                .Where(x => x != null && x.Config.IsCleanup)
                .Select(x => (ICleanupSystem)x)
                .ToArray();
            _isCleanupSystemsDirty = true;
        }

        private void UpdateTearDown()
        {
            _tearDownSystems = _allSystems
                .Where(x => x != null && x.Config.IsTearDown)
                .Select(x => (ITearDownSystem)x)
                .ToArray();
            _isTearDownSystemsDirty = true;
        }
    }
}
