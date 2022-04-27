using EcsLte.Data;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal unsafe class ArcheTypeFactory
    {
        private List<IndexDictionary<ArcheType>> _archeTypeIndexes;
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        public int ChangeVersion { get; private set; }

        internal ArcheTypeFactory()
        {
            _archeTypeIndexes = new List<IndexDictionary<ArcheType>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();
        }

        internal ArcheTypeData* GetArcheTypeDataFromBlueprint(EntityBlueprint blueprint, IIndexDictionary[] sharedComponentIndexes)
        {
            GetIndexDic(blueprint.AllBlueprintComponents.Length, out var indexDic, out var dataList);
            var archeType = GetArcheType(blueprint, sharedComponentIndexes);
            ArcheTypeData* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                });
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                ChangeVersion++;
            }
            else
            {
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;
                archeType.Dispose();
            }

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeDataFromArcheType(ref ArcheType archeType)
        {
            GetIndexDic(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ArcheTypeData.Alloc(archeType, new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                });
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                ChangeVersion++;
            }
            else
            {
                archeTypeData = (ArcheTypeData*)dataList[index].Ptr;
                archeType.Dispose();
                archeType = archeTypeData->ArcheType;
            }

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeDataFromIndex(ArcheTypeIndex archeTypeIndex)
        {
            return (ArcheTypeData*)_archeTypeDatas
                [archeTypeIndex.ComponentConfigLength]
                [archeTypeIndex.Index]
                .Ptr;
        }

        internal unsafe void UpdateArcheTypeDatas(EntityQueryData queryData)
        {
            queryData.ArcheTypeDatas.Clear();
            for (var i = queryData.ConfigCount; i < _archeTypeDatas.Count; i++)
            {
                var dataList = _archeTypeDatas[i];
                for (var j = 0; j < dataList.Count; j++)
                {
                    var archeTypePtr = dataList[j];
                    if (queryData.IsFiltered(((ArcheTypeData*)archeTypePtr.Ptr)->ArcheType))
                        queryData.ArcheTypeDatas.Add(archeTypePtr);
                }
            }

            queryData.ArcheTypeChangeVersion = ChangeVersion;
        }

        public void Dispose()
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
                    ((ArcheTypeData*)archeTypeData.Ptr)->Dispose();
                    MemoryHelper.Free(archeTypeData.Ptr);
                }
            }
            _archeTypeDatas = null;
        }

        private static ArcheType GetArcheType(EntityBlueprint blueprint, IIndexDictionary[] sharedComponentIndexes)
        {
            var archeType = new ArcheType
            {
                ComponentConfigLength = blueprint.AllBlueprintComponents.Length,
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(blueprint.AllBlueprintComponents.Length),
                SharedComponentDataLength = blueprint.SharedBlueprintComponents.Length,
                SharedComponentDataIndexes = blueprint.SharedBlueprintComponents.Length > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(blueprint.SharedBlueprintComponents.Length)
                    : null
            };
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
                archeType.ComponentConfigs[i] = blueprint.AllBlueprintComponents[i].Config;
            for (var i = 0; i < archeType.SharedComponentDataLength; i++)
            {
                var blueprintComponent = blueprint.SharedBlueprintComponents[i];
                var config = blueprintComponent.Config;

                archeType.SharedComponentDataIndexes[i] = new SharedComponentDataIndex
                {
                    SharedIndex = config.SharedIndex,
                    SharedDataIndex = sharedComponentIndexes[config.SharedIndex]
                            .GetIndexObj(blueprintComponent.Component)
                };
            }

            return archeType;
        }

        private void GetIndexDic(int configCount,
            out IndexDictionary<ArcheType> indexDic,
            out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<ArcheType>());
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
