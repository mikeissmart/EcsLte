using EcsLte.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    internal interface ISharedComponentIndexDictionary : IIndexDictionary
    {
        IComponentData GetComponentData(SharedComponentDataIndex dataIndex);
    }

    internal class SharedComponentIndexDictionary<TComponent> : IndexDictionary<TComponent>, ISharedComponentIndexDictionary
            where TComponent : IComponent
    {
        public ComponentData<TComponent> GetComponentData(SharedComponentDataIndex dataIndex)=>
            new ComponentData<TComponent>(GetKey(dataIndex.SharedDataIndex));

        IComponentData ISharedComponentIndexDictionary.GetComponentData(SharedComponentDataIndex dataIndex) =>
            new ComponentData<TComponent>((TComponent)GetObject(dataIndex.SharedDataIndex));
    }

    internal class SharedComponentIndexDictionaries
    {
        private ISharedComponentIndexDictionary[] _sharedComponentIndexes;

        internal SharedComponentIndexDictionaries()
        {
            _sharedComponentIndexes = new ISharedComponentIndexDictionary[ComponentConfigs.Instance.AllSharedCount];
            var indexDicType = typeof(SharedComponentIndexDictionary<>);
            for (var i = 0; i < _sharedComponentIndexes.Length; i++)
            {
                _sharedComponentIndexes[i] = (ISharedComponentIndexDictionary)Activator
                    .CreateInstance(indexDicType
                        .MakeGenericType(ComponentConfigs.Instance.AllSharedTypes[i]));
            }
        }

        internal SharedComponentIndexDictionary<TComponent> GetSharedIndexDic<TComponent>()
            where TComponent : IComponent =>
            (SharedComponentIndexDictionary<TComponent>)_sharedComponentIndexes[ComponentConfig<TComponent>.Config.SharedIndex];

        internal ISharedComponentIndexDictionary GetSharedIndexDic(ComponentConfig config) =>
            _sharedComponentIndexes[config.SharedIndex];

        internal SharedComponentDataIndex GetDataIndex(ComponentConfig config, IComponent component)
        {
            var indexDic = GetSharedIndexDic(config);
            return new SharedComponentDataIndex
            {
                SharedIndex = config.SharedIndex,
                SharedDataIndex = indexDic.GetIndexObj(component)
            };
        }

        internal SharedComponentDataIndex GetDataIndex<TComponent>(TComponent component)
            where TComponent : IComponent
        {
            var indexDic = GetSharedIndexDic<TComponent>();
            return new SharedComponentDataIndex
            {
                SharedIndex = ComponentConfig<TComponent>.Config.SharedIndex,
                SharedDataIndex = indexDic.GetIndex(component)
            };
        }

        internal void Clear()
        {
            for (var i = 0; i < _sharedComponentIndexes.Length; i++)
                _sharedComponentIndexes[i].Clear();
        }
    }
}
