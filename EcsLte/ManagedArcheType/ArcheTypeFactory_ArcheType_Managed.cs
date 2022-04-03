using EcsLte.Data;
using System.Collections.Generic;

namespace EcsLte.ManagedArcheType
{
    public class ArcheTypeFactory_ArcheType_Managed
    {
        private readonly List<IndexDictionary<Component_ArcheType_Managed>> _archeTypeIndexes;
        private readonly List<List<ComponentData_ArcheType_Managed>> _archeTypeDatas;

        public ArcheTypeFactory_ArcheType_Managed()
        {
            _archeTypeIndexes = new List<IndexDictionary<Component_ArcheType_Managed>>();
            _archeTypeDatas = new List<List<ComponentData_ArcheType_Managed>>();
        }

        public ComponentData_ArcheType_Managed GetArcheTypeData(Component_ArcheType_Managed archeType)
        {
            GetIndexDictionaryAndArcheTypeDataList(archeType.ComponentConfigs.Length, out var indexDic, out var dataList);
            ComponentData_ArcheType_Managed archeTypeData;
            if (indexDic.GetIndex(archeType, out var index))
            {
                archeTypeData = ComponentData_ArcheType_Managed.Alloc(archeType);
                dataList.Add(archeTypeData);
            }
            else
            {
                archeTypeData = dataList[index];
            }

            return archeTypeData;
        }

        private void GetIndexDictionaryAndArcheTypeDataList(
            int configCount,
            out IndexDictionary<Component_ArcheType_Managed> indexDic,
            out List<ComponentData_ArcheType_Managed> dataList)
        {
            while (_archeTypeIndexes.Count <= configCount)
            {
                _archeTypeIndexes.Add(new IndexDictionary<Component_ArcheType_Managed>());
                _archeTypeDatas.Add(new List<ComponentData_ArcheType_Managed>());
            }

            indexDic = _archeTypeIndexes[configCount];
            dataList = _archeTypeDatas[configCount];
        }
    }
}
