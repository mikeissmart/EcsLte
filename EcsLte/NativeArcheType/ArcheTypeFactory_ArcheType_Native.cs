using EcsLte.Data;
using EcsLte.Data.Unmanaged;
using System;
using System.Collections.Generic;

namespace EcsLte.NativeArcheType
{
    public class ArcheTypeFactory_ArcheType_Native : IDisposable
    {
        private List<IndexDictionary<Component_ArcheType_Native>> _archeTypeIndexes;
        /// <summary>
        /// ComponentData_ArcheType_Native*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        public ArcheTypeFactory_ArcheType_Native()
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
            out ComponentData_ArcheType_Native* archeTypeData)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            var isNew = false;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Native.Alloc(archeType, uniqueConfigs);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                isNew = true;
            }
            else
            {
                archeTypeData = (ComponentData_ArcheType_Native*)dataList[index].Ptr;
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
            out ComponentData_ArcheType_Native* archeTypeData,
            out ArcheTypeIndex_ArcheType_Native archeTypeIndex)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigLength, out var indexDic, out var dataList);
            var isNew = false;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Native.Alloc(archeType, uniqueConfigs);
                dataList.Add(new PtrWrapper { Ptr = archeTypeData });
                isNew = true;
            }
            else
            {
                archeTypeData = (ComponentData_ArcheType_Native*)dataList[index].Ptr;
            }

            archeTypeIndex = new ArcheTypeIndex_ArcheType_Native
            {
                ComponentsLength = archeType.ComponentConfigLength,
                Index = index,
            };

            return isNew;
        }

        public unsafe ComponentData_ArcheType_Native* GetArcheTypeData(ArcheTypeIndex_ArcheType_Native archeTypeIndex) => (ComponentData_ArcheType_Native*)_archeTypeDatas[archeTypeIndex.ComponentsLength][archeTypeIndex.Index].Ptr;

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
                    ((ComponentData_ArcheType_Native*)archeTypeData.Ptr)->Dispose();
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
