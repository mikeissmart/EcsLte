using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte.NativeArcheType
{
    public class ArcheTypeFactory_ArcheType_Native : IDisposable
    {
        private class ArcheTypeEntityQueries
        {
            public unsafe ComponentData_ArcheType_Native* ArcheTypeData { get; set; }
            public List<EntityQueryData_ArcheType> EntityQueryDatas { get; set; } = new List<EntityQueryData_ArcheType>();
        }

        private List<IndexDictionary<Component_ArcheType_Native>> _archeTypeIndexes;
        /// <summary>
        /// ComponentData_ArcheType_Native*
        /// </summary>
        private List<List<ArcheTypeEntityQueries>> _archeTypeDatas;
        private readonly unsafe ComponentData_ArcheType_Native* _defaultArcheTypeData;
        //TODO uncomment after blueprintBenchmark-private readonly Dictionary<int, EntityQueryData_ArcheType> _masterEntityQueryDatas;
        //TODO uncomment after blueprintBenchmark-private readonly List<EntityQueryData_ArcheType>[] _componentEntityQueryDatas;

        public unsafe ComponentData_ArcheType_Native* DefaultArcheTypeData => _defaultArcheTypeData;

        public unsafe ArcheTypeFactory_ArcheType_Native()
        {
            _archeTypeIndexes = new List<IndexDictionary<Component_ArcheType_Native>>();
            _archeTypeDatas = new List<List<ArcheTypeEntityQueries>>();
            /*TODO uncomment after blueprintBenchmark-_masterEntityQueryDatas = new Dictionary<int, EntityQueryData_ArcheType>();
            _componentEntityQueryDatas = new List<EntityQueryData_ArcheType>[ComponentConfigs.Instance.AllComponentCount];
            for (int i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
                _componentEntityQueryDatas[i] = new List<EntityQueryData_ArcheType>();-TODO uncomment after blueprintBenchmark*/

            GetArcheTypeData(new Component_ArcheType_Native(), null, out _defaultArcheTypeData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archeType"></param>
        /// <param name="uniqueConfigs"></param>
        /// <param name="archeTypeData"></param>
        /// <param name="archeTypeIndex"></param>
		/// <returns>true = new ArcheTypeData was created. Dont dispose archeType</returns>
        public unsafe bool GetArcheTypeData(
            Component_ArcheType_Native archeType,
            ComponentConfigIndex_ArcheType_Native* uniqueConfigs,
            out ComponentData_ArcheType_Native* archeTypeData)
        {
            var isNew = InternalGetArcheTypeData(archeType, uniqueConfigs, out archeTypeData, out _);

            return isNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archeType"></param>
        /// <param name="uniqueConfigs"></param>
        /// <param name="archeTypeData"></param>
        /// <param name="archeTypeIndex"></param>
		/// <returns>true = new ArcheTypeData was created. Dont dispose archeType</returns>
        public unsafe bool GetArcheTypeData(
            Component_ArcheType_Native archeType,
            ComponentConfigIndex_ArcheType_Native* uniqueConfigs,
            out ComponentData_ArcheType_Native* archeTypeData,
            out ArcheTypeIndex_ArcheType_Native archeTypeIndex)
        {
            var isNew = InternalGetArcheTypeData(archeType, uniqueConfigs, out archeTypeData, out var index);
            archeTypeIndex = new ArcheTypeIndex_ArcheType_Native
            {
                ComponentsLength = archeType.ComponentConfigLength,
                Index = index,
            };

            return isNew;
        }

        public unsafe ComponentData_ArcheType_Native* GetArcheTypeData(ArcheTypeIndex_ArcheType_Native archeTypeIndex) => _archeTypeDatas[archeTypeIndex.ComponentsLength][archeTypeIndex.Index].ArcheTypeData;

        /*TODO uncomment after blueprintBenchmark-public unsafe void SetEntitiesDirty(ComponentData_ArcheType_Native* archeTypeData)
        {
            var configCount = archeTypeData->ArcheType.ComponentConfigLength;
            var archeTypeIndex = archeTypeData->ArcheTypeIndex;
            foreach (var queryData in _archeTypeDatas[configCount][archeTypeIndex].EntityQueryDatas)
                queryData.SetEntitiesDirty();
        }

        public unsafe void AddToMasterQuery(EntityQuery_ArcheType entityQuery)
        {
            if (entityQuery.Data.CheckConfigsZero())
                throw new EntityQueryNoWhereOfException();

            if (_masterEntityQueryDatas.TryGetValue(entityQuery.Data.GetHashCode(), out var entityQueryData))
                // Make sure query is using singular data
                entityQuery.Data = entityQueryData;
            else
            {
                // Add to master query and component queries
                _masterEntityQueryDatas.Add(entityQuery.Data.GetHashCode(), entityQuery.Data);
                foreach (var config in entityQueryData.AllConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);
                foreach (var config in entityQueryData.AnyConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);
                foreach (var config in entityQueryData.NoneConfigs)
                    _componentEntityQueryDatas[config.ComponentIndex].Add(entityQueryData);

                foreach (var archeTypeDataList in _archeTypeDatas)
                {
                    foreach (var archeTypeQueryData in archeTypeDataList)
                    {
                        if (entityQueryData.FilterArcheType_Native(archeTypeQueryData.ArcheTypeData))
                            archeTypeQueryData.EntityQueryDatas.Add(entityQueryData);
                    }
                }
            }
        }-TODO uncomment after blueprintBenchmark*/

        public unsafe void Dispose()
        {
            foreach (var indexDic in _archeTypeIndexes)
            {
                foreach (var archeType in indexDic.Keys)
                    archeType.Dispose();
            }
            _archeTypeIndexes = null;
            foreach (var dataList in _archeTypeDatas)
            {
                foreach (var archeTypeData in dataList)
                {
                    archeTypeData.ArcheTypeData->Dispose();
                    MemoryHelper.Free(archeTypeData.ArcheTypeData);
                }
            }
            _archeTypeDatas = null;
        }

        private unsafe bool InternalGetArcheTypeData(
            Component_ArcheType_Native archeType,
            ComponentConfigIndex_ArcheType_Native* uniqueConfigs,
            out ComponentData_ArcheType_Native* archeTypeData,
            out int index)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            var isNew = false;
            if (indexDic.GetIndex(archeType, out index))
            {
                archeTypeData = ComponentData_ArcheType_Native.Alloc(archeType, uniqueConfigs, index);
                var archeTypeQueryData = new ArcheTypeEntityQueries
                {
                    ArcheTypeData = archeTypeData
                };
                dataList.Add(archeTypeQueryData);
                isNew = true;

                /*TODO uncomment after blueprintBenchmark-for (var i = 0; i < archeType.ComponentConfigLength; i++)
                {
                    var config = archeType.ComponentConfigs[i];
                    foreach (var entityQueryData in _componentEntityQueryDatas[config.ComponentIndex])
                    {
                        if (entityQueryData.FilterArcheType_Native(archeTypeData))
                            archeTypeQueryData.EntityQueryDatas.Add(entityQueryData);
                    }
                }-TODO uncomment after blueprintBenchmark*/
            }
            else
            {
                archeTypeData = dataList[index].ArcheTypeData;
            }

            return isNew;
        }

        private void GetIndexDictionaryAndArcheTypeDataList(
            int configCount,
            out IndexDictionary<Component_ArcheType_Native> indexDic,
            out List<ArcheTypeEntityQueries> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<Component_ArcheType_Native>());
                _archeTypeDatas.Add(new List<ArcheTypeEntityQueries>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
