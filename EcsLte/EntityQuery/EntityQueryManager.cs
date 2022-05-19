using EcsLte.Data;
using System.Collections.Generic;

namespace EcsLte
{
    internal class EntityQueryManager
    {
        private List<IndexDictionary<EntityQueryData>> _queryDataIndexes;
        private List<List<EntityQueryData>> _queryDatas;
        private readonly object _lockObj;

        public EcsContext Context { get; private set; }

        internal EntityQueryManager(EcsContext context)
        {
            _queryDataIndexes = new List<IndexDictionary<EntityQueryData>>();
            _queryDatas = new List<List<EntityQueryData>>();
            _lockObj = new object();

            Context = context;
        }

        internal void UpdateEntityQuery(EntityQuery query)
        {
            lock (_lockObj)
            {
                if (!query.QueryData.ContextQueryData.ContainsKey(Context))
                {
                    GetIndexDic(query.QueryData.ConfigCount, out var indexDic, out var dataList);

                    if (indexDic.GetIndex(query.QueryData, out var index))
                    {
                        query.QueryData.ContextQueryData.Add(Context, new EntityQueryEcsContextData
                        {
                            EntityQueryDataIndex = index
                        });
                        dataList.Add(query.QueryData);
                    }
                    else
                    {
                        query.QueryData = dataList[index];
                    }
                }
                Context.ArcheTypeManager.UpdateEntityQuery(query);
            }
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
            lock (_lockObj)
            {
                _queryDataIndexes = null;
                _queryDatas = null;
            }
        }
    }
}
