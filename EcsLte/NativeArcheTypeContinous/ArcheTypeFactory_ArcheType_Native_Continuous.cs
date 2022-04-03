using EcsLte.Data;
using EcsLte.Data.Unmanaged;
using EcsLte.NativeArcheType;
using System;
using System.Collections.Generic;

namespace EcsLte.NativeArcheTypeContinous
{
    public class ArcheTypeFactory_ArcheType_Native_Continuous : IDisposable
    {
        private List<IndexDictionary<Component_ArcheType_Native>> _archeTypeIndexes;
        /// <summary>
        /// ComponentData_ArcheType_Native_Continuous*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        public ArcheTypeFactory_ArcheType_Native_Continuous()
        {
            _archeTypeIndexes = new List<IndexDictionary<Component_ArcheType_Native>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();
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
            DataChunkCache_ArcheType_Native_Continuous* dataChunkCache,
            out ComponentData_ArcheType_Native_Continuous* archeTypeData)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            var isNew = false;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Native_Continuous.Alloc(archeType, uniqueConfigs, dataChunkCache);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                isNew = true;
            }
            else
            {
                archeTypeData = (ComponentData_ArcheType_Native_Continuous*)dataList[index].Ptr;
            }

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
            DataChunkCache_ArcheType_Native_Continuous* dataChunkCache,
            out ComponentData_ArcheType_Native_Continuous* archeTypeData,
            out ArcheTypeIndex_ArcheType_Native archeTypeIndex)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            var isNew = false;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Native_Continuous.Alloc(archeType, uniqueConfigs, dataChunkCache);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                isNew = true;
            }
            else
            {
                archeTypeData = (ComponentData_ArcheType_Native_Continuous*)dataList[index].Ptr;
            }

            archeTypeIndex = new ArcheTypeIndex_ArcheType_Native
            {
                ComponentsLength = archeType.ComponentConfigLength,
                Index = index,
            };

            return isNew;
        }

        public unsafe ComponentData_ArcheType_Native_Continuous* GetArcheTypeData(ArcheTypeIndex_ArcheType_Native archeTypeIndex) => (ComponentData_ArcheType_Native_Continuous*)_archeTypeDatas[archeTypeIndex.ComponentsLength][archeTypeIndex.Index].Ptr;

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
                    ((ComponentData_ArcheType_Native_Continuous*)archeTypeData.Ptr)->Dispose();
            }
            _archeTypeDatas = null;
        }

        private void GetIndexDictionaryAndArcheTypeDataList(
            int configCount,
            out IndexDictionary<Component_ArcheType_Native> indexDic,
            out List<PtrWrapper> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<Component_ArcheType_Native>());
                _archeTypeDatas.Add(new List<PtrWrapper>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
