using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    internal class ComponentPoolIndex<TComponent> where TComponent : IComponent
    {
        private static ComponentPoolConfig _config;
        private static bool _gotConfig;

        internal static ComponentPoolConfig Config
        {
            get
            {
                if (!_gotConfig)
                {
                    _config = ComponentPoolIndexes.Instance.GetConfig(typeof(TComponent));
                    _gotConfig = true;
                }
                return _config;
            }
        }
    }

    internal class ComponentPoolIndexes
    {
        private static ComponentPoolIndexes _instance;

        private Dictionary<Type, ComponentPoolConfig> _componentPoolConfigTypes;
        private Dictionary<int, ComponentPoolConfig> _componentPoolConfigIndexes;

        private ComponentPoolIndexes()
        {
            Initialize();
        }

        internal static ComponentPoolIndexes Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ComponentPoolIndexes();
                return _instance;
            }
        }

        internal Type[] AllComponentTypes { get; private set; }
        internal int[] AllComponentPoolIndexes { get; private set; }
        internal int[] RecordableComponentPoolIndexes { get; private set; }
        internal int[] UniqueComponentPoolIndexes { get; private set; }
        internal int[] SharedComponentPoolIndexes { get; private set; }
        internal int Count => _componentPoolConfigIndexes.Count;

        internal ComponentPoolConfig GetConfig(Type componentType)
        {
            return _componentPoolConfigTypes[componentType];
        }

        internal ComponentPoolConfig GetConfig(int componentPoolIndex)
        {
            return _componentPoolConfigIndexes[componentPoolIndex];
        }

        internal IComponentPool[] CreateComponentPools(int initialSize)
        {
            var componentPools = new IComponentPool[Count];
            var poolType = typeof(ComponentPool<>);
            var args = new object[] { initialSize };
            for (int i = 0; i < Count; i++)
                componentPools[i] = (IComponentPool)Activator.CreateInstance(poolType.MakeGenericType(AllComponentTypes[i]), args);

            return componentPools;
        }

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iRecordableComponentType = typeof(IRecordableComponent);
            var iUniqueComponentType = typeof(IUniqueComponent);
            var iSharedComponentType = typeof(ISharedComponent);
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    iComponentType.IsAssignableFrom(x));

            var componentsWrongType = new List<Type>();
            foreach (var type in componentTypes)
                if (!type.IsValueType)
                    componentsWrongType.Add(type);
            if (componentsWrongType.Count != 0)
                throw new ComponentNotStructException(componentsWrongType.ToArray());

            _componentPoolConfigTypes = new Dictionary<Type, ComponentPoolConfig>();
            _componentPoolConfigIndexes = new Dictionary<int, ComponentPoolConfig>();
            var recordableComponentPoolIndexes = new List<int>();
            var uniqueComponentPoolIndexes = new List<int>();
            var sharedComponentPoolIndexes = new List<int>();
            var index = 0;
            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                var config = new ComponentPoolConfig
                {
                    Index = index
                };
                if (iRecordableComponentType.IsAssignableFrom(type))
                {
                    recordableComponentPoolIndexes.Add(index);
                    config.IsRecordable = true;
                }

                if (iUniqueComponentType.IsAssignableFrom(type))
                {
                    uniqueComponentPoolIndexes.Add(index);
                    config.IsUnique = true;
                }

                if (iSharedComponentType.IsAssignableFrom(type))
                {
                    sharedComponentPoolIndexes.Add(index);
                    config.IsShared = true;
                }

                _componentPoolConfigTypes.Add(type, config);
                _componentPoolConfigIndexes.Add(index, config);
                index++;
            }

            if (index == 0)
                throw new ComponentNoneException();

            AllComponentTypes = _componentPoolConfigTypes.Keys.ToArray();
            AllComponentPoolIndexes = _componentPoolConfigIndexes.Keys.ToArray();
            RecordableComponentPoolIndexes = recordableComponentPoolIndexes.ToArray();
            UniqueComponentPoolIndexes = uniqueComponentPoolIndexes.ToArray();
            SharedComponentPoolIndexes = sharedComponentPoolIndexes.ToArray();
        }
    }
}