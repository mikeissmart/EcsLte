using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;
using EcsLte.Utilities;

namespace EcsLte
{
    public class SystemManager
    {
        private SystemManagerData _data;

        internal SystemManager(World world)
        {
            _data = ObjectCache.Pop<SystemManagerData>();

            CurrentWorld = world;
        }

        public World CurrentWorld { get; }

        #region SystemLife

        public SystemBase[] GetSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _data.Systems.UncachedData.Values.ToArray();
        }

        public SystemBase[] GetActiveSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _data.Systems.CachedData;
        }

        public SystemSorter[] GetSystemSorters()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            return _data.GetSystemSorters();
        }

        public bool HasSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                return false;

            return _data.Systems.UncachedData.ContainsKey(typeof(TSystem));
        }

        public TSystem GetSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systemType = typeof(TSystem);
            if (!_data.Systems.UncachedData.ContainsKey(systemType))
                throw new SystemDoesNotSystemException(CurrentWorld, systemType);

            return (TSystem)_data.Systems.UncachedData[systemType];
        }

        public SystemBase[] AutoAddSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systems = new List<SystemBase>();
            foreach (var systemType in Systems.Instance.AutoAddSystemTypes)
            {
                if (!_data.Systems.UncachedData.ContainsKey(systemType))
                {
                    var system = (SystemBase)Activator.CreateInstance(systemType);
                    system.CurrentWorld = CurrentWorld;

                    _data.Systems.UncachedData.Add(systemType, system);
                    _data.Systems.IsDirty = true;

                    systems.Add(system);
                }
            }

            return systems.ToArray();
        }

        public TSystem AddSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systemType = typeof(TSystem);
            if (!_data.Systems.UncachedData.ContainsKey(systemType))
            {
                var system = (SystemBase)Activator.CreateInstance(systemType);
                system.CurrentWorld = CurrentWorld;

                _data.Systems.UncachedData.Add(systemType, system);
                _data.Systems.IsDirty = true;

                return (TSystem)system;
            }
            else
                throw new SystemAlreadyHasSystemException(CurrentWorld, systemType);
        }

        public void RemoveSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systemType = typeof(TSystem);
            if (_data.Systems.UncachedData.ContainsKey(systemType))
            {
                _data.Systems.UncachedData.Remove(systemType);
                _data.Systems.IsDirty = true;
            }
            else
                throw new SystemDoesNotSystemException(CurrentWorld, systemType);
        }

        public void ActivateSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systemType = typeof(TSystem);
            if (_data.Systems.UncachedData.TryGetValue(systemType, out var system))
            {
                if (!system.IsActive)
                {
                    system.IsActive = true;
                    _data.Systems.IsDirty = true;
                }
            }
            else
                throw new SystemDoesNotSystemException(CurrentWorld, systemType);
        }

        public void DeactivateSystem<TSystem>()
            where TSystem : SystemBase
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            var systemType = typeof(TSystem);
            if (_data.Systems.UncachedData.TryGetValue(systemType, out var system))
            {
                if (system.IsActive)
                {
                    system.IsActive = false;
                    _data.Systems.IsDirty = true;
                }
            }
            else
                throw new SystemDoesNotSystemException(CurrentWorld, systemType);
        }

        #endregion

        #region SystemExecution

        public void InitializeSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            for (int i = 0; i < _data.Systems.CachedData.Length; i++)
                _data.Systems.CachedData[i].Initialize();
        }

        public void ExecuteSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            for (int i = 0; i < _data.Systems.CachedData.Length; i++)
                _data.Systems.CachedData[i].Execute();
        }

        public void CleanupSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            for (int i = 0; i < _data.Systems.CachedData.Length; i++)
                _data.Systems.CachedData[i].Cleanup();
        }

        public void TearDownSystems()
        {
            if (CurrentWorld.IsDestroyed)
                throw new WorldIsDestroyedException(CurrentWorld);

            for (int i = 0; i < _data.Systems.CachedData.Length; i++)
                _data.Systems.CachedData[i].TearDown();
        }

        #endregion

        internal void InternalDestroy()
        {
            _data.Reset();
            ObjectCache.Push(_data);
        }
    }
}