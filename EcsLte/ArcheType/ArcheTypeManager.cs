using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.Utilities;
using System.Collections.Generic;

namespace EcsLte
{
    public unsafe class ArcheTypeManager
    {
        private List<IndexDictionary<ArcheType>> _archeTypeIndexes;
        /// <summary>
        /// ArcheTypeData*
        /// </summary>
        private List<List<PtrWrapper>> _archeTypeDatas;

        public EcsContext Context { get; private set; }
        internal int ChangeVersion { get; private set; }

        internal ArcheTypeManager(EcsContext context)
        {
            _archeTypeIndexes = new List<IndexDictionary<ArcheType>>();
            _archeTypeDatas = new List<List<PtrWrapper>>();

            Context = context;
        }

        public EntityArcheType CreateEntityArcheType()
        {
            if (Context.IsDestroyed)
                throw new EcsContextIsDestroyedException(Context);

            return new EntityArcheType(Context);
        }

        internal ArcheTypeData* GetArcheTypeDataFromBlueprint(EntityBlueprint blueprint)
        {
            if (blueprint.Context == Context && blueprint.ArcheTypeIndex != null)
                return GetArcheTypeDataFromIndex(blueprint.ArcheTypeIndex.Value);

            blueprint.Context = Context;
            GetIndexDic(blueprint.AllBlueprintComponents.Length, out var indexDic, out var dataList);
            var archeType = GetArcheType(blueprint, Context.EntityManager.SharedComponentIndexes);
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

            if (blueprint.ArcheTypeIndex == null)
            {
                blueprint.ArcheTypeIndex = new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                };
            }

            return archeTypeData;
        }

        internal ArcheTypeData* GetArcheTypeDataFromEntityArcheType(EntityArcheType entityArcheType)
        {
            if (entityArcheType.ArcheTypeIndex != null)
                return GetArcheTypeDataFromIndex(entityArcheType.ArcheTypeIndex.Value);

            var archeType = GetArcheType(entityArcheType);
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
            }

            if (entityArcheType.ArcheTypeIndex == null)
            {
                entityArcheType.ArcheTypeIndex = new ArcheTypeIndex
                {
                    ComponentConfigLength = archeType.ComponentConfigLength,
                    Index = index
                };
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

        internal ArcheTypeData* GetArcheTypeDataFromIndex(ArcheTypeIndex archeTypeIndex) => (ArcheTypeData*)_archeTypeDatas
                [archeTypeIndex.ComponentConfigLength]
                [archeTypeIndex.Index]
                .Ptr;

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

        internal void InternalDestroy()
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

        private static ArcheType GetArcheType(EntityArcheType entityArcheType)
        {
            var archeType = new ArcheType
            {
                ComponentConfigLength = entityArcheType.ComponentConfigs.Length,
                ComponentConfigs = MemoryHelper.Alloc<ComponentConfig>(entityArcheType.ComponentConfigs.Length),
                SharedComponentDataLength = entityArcheType.SharedComponentDataIndexes.Length,
                SharedComponentDataIndexes = entityArcheType.SharedComponentDataIndexes.Length > 0
                    ? MemoryHelper.Alloc<SharedComponentDataIndex>(entityArcheType.SharedComponentDataIndexes.Length)
                    : null
            };
            for (var i = 0; i < archeType.ComponentConfigLength; i++)
                archeType.ComponentConfigs[i] = entityArcheType.ComponentConfigs[i];
            for (var i = 0; i < archeType.SharedComponentDataLength; i++)
                archeType.SharedComponentDataIndexes[i] = entityArcheType.SharedComponentDataIndexes[i];

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
