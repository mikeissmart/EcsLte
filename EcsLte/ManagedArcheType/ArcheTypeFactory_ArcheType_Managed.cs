using EcsLte.Data;
using System.Collections.Generic;

namespace EcsLte.ManagedArcheType
{
    public class ArcheTypeFactory_ArcheType_Managed
    {
        private class ArcheTypeEntityQueries
        {
            public ComponentData_ArcheType_Managed ArcheTypeData { get; set; }
            public List<EntityQueryData_ArcheType> EntityQueryDatas { get; set; } = new List<EntityQueryData_ArcheType>();
        }

        private readonly List<IndexDictionary<Component_ArcheType_Managed>> _archeTypeIndexes;
        private readonly List<List<ArcheTypeEntityQueries>> _archeTypeDatas;
        /// <summary>
        /// EntityQuery_ArcheType.Data.GetHashCode()
        /// </summary>
        //TODO uncomment after blueprintBenchmark-private readonly Dictionary<int, EntityQueryData_ArcheType> _masterEntityQueryDatas;
        //TODO uncomment after blueprintBenchmark-private readonly List<EntityQueryData_ArcheType>[] _componentEntityQueryDatas;


        public ComponentData_ArcheType_Managed DefaultArcheTypeData { get; private set; }

        public ArcheTypeFactory_ArcheType_Managed()
        {
            _archeTypeIndexes = new List<IndexDictionary<Component_ArcheType_Managed>>();
            _archeTypeDatas = new List<List<ArcheTypeEntityQueries>>();
            /*TODO uncomment after blueprintBenchmark-_masterEntityQueryDatas = new Dictionary<int, EntityQueryData_ArcheType>();
            _componentEntityQueryDatas = new List<EntityQueryData_ArcheType>[ComponentConfigs.Instance.AllComponentCount];
            for (int i = 0; i < ComponentConfigs.Instance.AllComponentCount; i++)
                _componentEntityQueryDatas[i] = new List<EntityQueryData_ArcheType>();-TODO uncomment after blueprintBenchmark*/

            DefaultArcheTypeData = GetArcheTypeData(new Component_ArcheType_Managed());
        }

        public ComponentData_ArcheType_Managed GetArcheTypeData(Component_ArcheType_Managed archeType)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigs?.Length ?? 0, out var indexDic, out var dataList);
            ComponentData_ArcheType_Managed archeTypeData;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Managed.Alloc(archeType, index);
                var archeTypeQueryData = new ArcheTypeEntityQueries
                {
                    ArcheTypeData = archeTypeData
                };
                dataList.Add(archeTypeQueryData);

                /*TODO uncomment after blueprintBenchmark-if (archeType.ComponentConfigs != null)
                {
                    foreach (var config in archeType.ComponentConfigs)
                    {
                        foreach (var entityQueryData in _componentEntityQueryDatas[config.ComponentIndex])
                        {
                            if (entityQueryData.FilterArcheType_Managed(archeTypeData))
                                archeTypeQueryData.EntityQueryDatas.Add(entityQueryData);
                        }
                    }
                }-TODO uncomment after blueprintBenchmark*/
            }
            else
            {
                archeTypeData = dataList[index].ArcheTypeData;
            }

            return archeTypeData;
        }

        /*TODO uncomment after blueprintBenchmark-public void SetEntitiesDirty(ComponentData_ArcheType_Managed archeTypeData)
        {
            var configCount = archeTypeData.ArcheType.ComponentConfigs?.Length ?? 0;
            var archeTypeIndex = archeTypeData.ArcheTypeIndex;
            foreach (var queryData in _archeTypeDatas[configCount][archeTypeIndex].EntityQueryDatas)
                queryData.SetEntitiesDirty();
        }

        public void AddToMasterQuery(EntityQuery_ArcheType entityQuery)
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
                        if (entityQueryData.FilterArcheType_Managed(archeTypeQueryData.ArcheTypeData))
                            archeTypeQueryData.EntityQueryDatas.Add(entityQueryData);
                    }
                }
            }
        }-TODO uncomment after blueprintBenchmark*/

        private void GetIndexDictionaryAndArcheTypeDataList(
            int configCount,
            out IndexDictionary<Component_ArcheType_Managed> indexDic,
            out List<ArcheTypeEntityQueries> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<Component_ArcheType_Managed>());
                _archeTypeDatas.Add(new List<ArcheTypeEntityQueries>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
