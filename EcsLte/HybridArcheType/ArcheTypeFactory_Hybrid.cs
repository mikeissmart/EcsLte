using EcsLte.Data;
using EcsLte.Data.Unmanaged;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte.HybridArcheType
{
    internal unsafe class ArcheTypeFactory_Hybrid : IDisposable
    {
        private List<IndexDictionary<ArcheType_Hybrid>> _archeTypeIndexes;
        /// <summary>
        /// ArcheTypeData_Hybrid*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        internal ArcheTypeFactory_Hybrid()
        {
            _archeTypeIndexes = new List<IndexDictionary<ArcheType_Hybrid>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();
        }

        internal ArcheTypeData_Hybrid* GetArcheTypeDataFromBlueprint(EntityBlueprint_Hybrid blueprint, IIndexDictionary[] sharedComponentIndexes)
        {
            GetIndexDic(blueprint.AllBlueprintComponents.Length, out var indexDic, out var dataList);
            var archeType = GetArcheType(blueprint, sharedComponentIndexes);
            ArcheTypeData_Hybrid* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ArcheTypeData_Hybrid.Alloc(archeType, index);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
            }
            else
            {
                archeTypeData = (ArcheTypeData_Hybrid*)dataList[index].Ptr;
                archeType.Dispose();
            }

            return archeTypeData;
        }

        internal ArcheTypeData_Hybrid* GetArcheTypeDataFromArcheType(ref ArcheType_Hybrid archeType)
        {
            GetIndexDic(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            ArcheTypeData_Hybrid* archeTypeData;

            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ArcheTypeData_Hybrid.Alloc(archeType, index);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
            }
            else
            {
                archeTypeData = (ArcheTypeData_Hybrid*)dataList[index].Ptr;
                archeType.Dispose();
                archeType.ComponentConfigs = archeTypeData->ArcheType.ComponentConfigs;
                archeType.SharedComponentDataIndexes = archeTypeData->ArcheType.SharedComponentDataIndexes;
            }

            return archeTypeData;
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
                    ((ArcheTypeData_Hybrid*)archeTypeData.Ptr)->Dispose();
                    MemoryHelper.Free(archeTypeData.Ptr);
                }
            }
            _archeTypeDatas = null;
        }

        private static ArcheType_Hybrid GetArcheType(EntityBlueprint_Hybrid blueprint, IIndexDictionary[] sharedComponentIndexes)
        {
            var archeType = new ArcheType_Hybrid
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
            out IndexDictionary<ArcheType_Hybrid> indexDic,
            out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<ArcheType_Hybrid>());
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
