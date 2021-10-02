using System.Collections.Generic;
using EcsLte.Utilities;

namespace EcsLte
{
    internal class EntityGroupData
    {
        private readonly DataCache<List<ComponentArcheTypeData>, ComponentArcheTypeData[]> _archeTypeDatas;
        private EcsContextData _ecsContextData;

        public EntityGroupData()
        {
            _archeTypeDatas = new DataCache<List<ComponentArcheTypeData>, ComponentArcheTypeData[]>(
                new List<ComponentArcheTypeData>(),
                UpdateCachedData);
        }

        internal EntityCollection Entities { get; private set; }
        internal ISharedComponent[] SharedComponents { get; private set; }

        internal ComponentArcheTypeData[] GetComponentArcheTypeDatas()
        {
            return _archeTypeDatas.CachedData;
        }

        internal void AddComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
        {
            lock (_archeTypeDatas)
            {
                _archeTypeDatas.UncachedData.Add(archeTypeData);
                _archeTypeDatas.SetDirty();
            }
        }

        internal bool RemoveComponentArcheTypeData(ComponentArcheTypeData archeTypeData)
        {
            lock (_archeTypeDatas)
            {
                if (_archeTypeDatas.UncachedData.Remove(archeTypeData))
                {
                    _archeTypeDatas.SetDirty();
                    return true;
                }
            }

            return false;
        }

        private static ComponentArcheTypeData[] UpdateCachedData(List<ComponentArcheTypeData> uncachedData)
        {
            return uncachedData.ToArray();
        }

        #region ObjectCache

        internal void Initialize(EcsContextData ecsContextData,
            ComponentArcheTypeData[] archeTypeDatas,
            ISharedComponent[] sharedComponents)
        {
            _ecsContextData = ecsContextData;
            _archeTypeDatas.UncachedData.AddRange(archeTypeDatas);
            _archeTypeDatas.SetDirty();

            Entities = ecsContextData.CreateEntityCollection();
            SharedComponents = sharedComponents;
        }

        internal void Reset()
        {
            _ecsContextData.RemoveEntityCollection(Entities);
            _archeTypeDatas.UncachedData.Clear();
            _archeTypeDatas.SetDirty();

            SharedComponents = null;
        }

        #endregion
    }
}