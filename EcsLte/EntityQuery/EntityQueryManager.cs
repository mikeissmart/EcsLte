using EcsLte.Data;
using EcsLte.Exceptions;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityQueryManager
    {
        private List<IndexDictionary<EntityQueryData>> _queryDataIndexes;
        private List<List<EntityQueryData>> _queryDatas;

        public EcsContext Context { get; private set; }

        internal EntityQueryManager(EcsContext context)
        {
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();

            Context = context;
        }

        public EntityQuery CreateQuery()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return new EntityQuery(Context);
        }

        internal EntityQueryData IndexQueryData(EntityQueryData queryData)
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

        internal void GetIndexDic(int configCount,
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

        internal void InternalDestroy()
        {
            _queryDataIndexes = null;
            _queryDatas = null;
        }
    }
}
