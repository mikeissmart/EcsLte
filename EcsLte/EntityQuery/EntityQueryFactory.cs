using EcsLte.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal class EntityQueryFactory : IDisposable
    {
        private List<IndexDictionary<EntityQueryData>> _queryDataIndexes;
        private List<List<EntityQueryData>> _queryDatas;

        public EntityQueryFactory()
        {
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();
        }

        public EntityQueryData IndexQueryData(EntityQueryData queryData)
        {
            GetIndexDic(queryData.ConfigCount, out var indexDic, out var dataList);

            if (indexDic.GetIndex(queryData, out var index))
            {
                queryData.EntityQueryDataIndex = index;
                dataList.Add(queryData);

                return queryData;
            }

            return dataList[index];
        }

        public void GetIndexDic(int configCount,
            out IndexDictionary<EntityQueryData> indexDic,
            out List<EntityQueryData> dataList)
        {
            while (_queryDatas.Count <= configCount)
            {
                _queryDataIndexes.Add(new IndexDictionary<EntityQueryData>());
                _queryDatas.Add(new List<EntityQueryData>());
            }

            indexDic = _queryDataIndexes[configCount];
            dataList = _queryDatas[configCount];
        }

        public void Dispose()
        {

        }
    }
}
