using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    internal interface ISharedComponentDictionary
    {
        SharedDataIndex GetSharedDataIndex(IComponent component);
        ISharedComponentData GetComponentData(SharedDataIndex sharedDataIndex);

        void InternalDestroy();
    }

    internal unsafe class SharedComponentDictionary<TComponent> : ISharedComponentDictionary
        where TComponent : unmanaged, ISharedComponent
    {
        private readonly ComponentConfig _config;
        private Dictionary<TComponent, int> _indexes;
        private TComponent* _values;
        private int _valuesLength;
        private int _nextIndex;

        public SharedComponentDictionary()
        {
            _config = ComponentConfig<TComponent>.Config;
            _indexes = new Dictionary<TComponent, int>(SharedComparer.Comparer);
            _values = MemoryHelper.Alloc<TComponent>(1);
            _valuesLength = 1;
            _nextIndex = 1;
        }

        SharedDataIndex ISharedComponentDictionary.GetSharedDataIndex(IComponent component)
            => GetSharedDataIndex((TComponent)component);

        internal SharedDataIndex GetSharedDataIndex(TComponent component)
        {
            if (!_indexes.TryGetValue(component, out var index))
            {
                index = _nextIndex++;
                _indexes.Add(component, index);
                if (_valuesLength <= index)
                {
                    var newLength = Helper.NextPow2(_valuesLength + index);
                    _values = MemoryHelper.ReallocCopy(_values, _valuesLength, newLength);
                    _valuesLength = newLength;
                }
                _values[index] = component;
            }

            return new SharedDataIndex
            {
                SharedIndex = _config.SharedIndex,
                DataIndex = index
            };
        }

        internal TComponent GetComponent(SharedDataIndex sharedDataIndex)
        {
#if DEBUG
            if (sharedDataIndex.SharedIndex != _config.SharedIndex ||
                sharedDataIndex.DataIndex == 0 ||
                sharedDataIndex.DataIndex >= _valuesLength)
                throw new Exception();
#endif
            return _values[sharedDataIndex.DataIndex];
        }

        ISharedComponentData ISharedComponentDictionary.GetComponentData(SharedDataIndex sharedDataIndex)
            => GetComponentData(sharedDataIndex);

        public SharedComponentData<TComponent> GetComponentData(SharedDataIndex sharedDataIndex)
        {
#if DEBUG
            if (sharedDataIndex.SharedIndex != _config.SharedIndex ||
                sharedDataIndex.DataIndex == 0 ||
                sharedDataIndex.DataIndex >= _valuesLength)
                throw new Exception();
#endif
            return new SharedComponentData<TComponent>(_values[sharedDataIndex.DataIndex]);
        }

        public void InternalDestroy()
        {
            _indexes = null;
            MemoryHelper.Free(_values);
            _values = null;
            _valuesLength = 0;
        }

        private class SharedComparer : IEqualityComparer<TComponent>
        {
            internal static SharedComparer Comparer => new SharedComparer();

            public bool Equals(TComponent x, TComponent y)
                => x.Equals(y);

            public int GetHashCode(TComponent obj)
                => obj.GetHashCode();
        }
    }

    internal class SharedComponentDictionaries
    {
        private readonly ISharedComponentDictionary[] _dics;

        internal SharedComponentDictionaries()
        {
            _dics = new ISharedComponentDictionary[ComponentConfigs.Instance.AllSharedCount];
            var dicType = typeof(SharedComponentDictionary<>);
            for (var i = 0; i < _dics.Length; i++)
            {
                _dics[i] = (ISharedComponentDictionary)Activator
                    .CreateInstance(dicType.MakeGenericType(ComponentConfigs.Instance.AllSharedTypes[i]));
            }
        }

        internal SharedComponentDictionary<TComponent> GetDic<TComponent>()
            where TComponent : unmanaged, ISharedComponent
            => (SharedComponentDictionary<TComponent>)_dics[ComponentConfig<TComponent>.Config.SharedIndex];

        internal ISharedComponentDictionary GetDic(ComponentConfig config)
            => _dics[config.SharedIndex];

        internal void InternalDestroy()
        {
            for (var i = 0; i < _dics.Length; i++)
                _dics[i].InternalDestroy();
        }
    }
}
