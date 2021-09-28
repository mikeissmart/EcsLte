using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    internal class ComponentPoolIndex<TComponent> where TComponent : IComponent
    {
        private static int _index = -1;
        private static bool _isRecordable;
        private static bool _isUnique;
        private static bool _isShared;
        private static bool _isPrimary;

        internal static int Index
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _index;
            }
        }

        internal static bool IsRecordable
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isRecordable;
            }
        }

        internal static bool IsUnique
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isUnique;
            }
        }

        internal static bool IsShared
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isShared;
            }
        }

        internal static bool IsPrimary
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isPrimary;
            }
        }

        internal Type ComponentType => typeof(TComponent);

        private static void GetData()
        {
            _index = ComponentPoolIndexes.Instance.GetComponentPoolIndex<TComponent>();
            _isRecordable = ComponentPoolIndexes.Instance.RecordableComponentPoolIndexes
                .Any(x => x == _index);
            _isUnique = ComponentPoolIndexes.Instance.UniqueComponentPoolIndexes
                .Any(x => x == _index);
            _isShared = ComponentPoolIndexes.Instance.SharedComponentPoolIndexes
                .Any(x => x == _index);
            _isPrimary = ComponentPoolIndexes.Instance.PrimaryComponentPoolIndexes
                .Any(x => x == _index);
        }
    }

    internal class ComponentPoolIndexes
    {
        private static ComponentPoolIndexes _instance;

        private Dictionary<Type, int> _componentPoolIndexTypes;

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
        internal int[] PrimaryComponentPoolIndexes { get; private set; }
        internal int Count => _componentPoolIndexTypes.Count;

        internal int GetComponentPoolIndex<TComponent>() where TComponent : IComponent
        {
            return _componentPoolIndexTypes[typeof(TComponent)];
        }

        internal int GetComponentPoolIndex(Type componentType)
        {
            return _componentPoolIndexTypes[componentType];
        }

        internal bool IsRecordableComponent(Type componentType)
        {
            var index = _componentPoolIndexTypes[componentType];
            return RecordableComponentPoolIndexes
                .Any(x => x == index);
        }

        internal bool IsUniqueComponent(Type componentType)
        {
            var index = _componentPoolIndexTypes[componentType];
            return UniqueComponentPoolIndexes
                .Any(x => x == index);
        }

        internal bool IsSharedComponent(Type componentType)
        {
            var index = _componentPoolIndexTypes[componentType];
            return SharedComponentPoolIndexes
                .Any(x => x == index);
        }

        internal bool IsPrimaryComponent(Type componentType)
        {
            var index = _componentPoolIndexTypes[componentType];
            return PrimaryComponentPoolIndexes
                .Any(x => x == index);
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
            var iPrimaryComponentType = typeof(IPrimaryComponent);
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

            _componentPoolIndexTypes = new Dictionary<Type, int>();
            var recordableComponentPoolIndexes = new List<int>();
            var uniqueComponentPoolIndexes = new List<int>();
            var sharedComponentPoolIndexes = new List<int>();
            var primaryComponentPoolIndexes = new List<int>();
            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                if (iRecordableComponentType.IsAssignableFrom(type))
                    recordableComponentPoolIndexes.Add(_componentPoolIndexTypes.Count);

                if (iUniqueComponentType.IsAssignableFrom(type))
                    uniqueComponentPoolIndexes.Add(_componentPoolIndexTypes.Count);

                if (iSharedComponentType.IsAssignableFrom(type))
                    sharedComponentPoolIndexes.Add(_componentPoolIndexTypes.Count);

                if (iPrimaryComponentType.IsAssignableFrom(type))
                    primaryComponentPoolIndexes.Add(_componentPoolIndexTypes.Count);

                _componentPoolIndexTypes.Add(type, _componentPoolIndexTypes.Count);
            }

            if (_componentPoolIndexTypes.Keys.Count == 0)
                throw new ComponentNoneException();

            var primaryAndSharedComponents = sharedComponentPoolIndexes
                .Where(x => primaryComponentPoolIndexes.Any(y => x == y))
                .ToArray();
            if (primaryAndSharedComponents.Length > 0)
                // TODO
                throw new Exception();

            AllComponentTypes = _componentPoolIndexTypes.Keys.ToArray();
            AllComponentPoolIndexes = _componentPoolIndexTypes.Values.ToArray();
            RecordableComponentPoolIndexes = recordableComponentPoolIndexes.ToArray();
            UniqueComponentPoolIndexes = uniqueComponentPoolIndexes.ToArray();
            SharedComponentPoolIndexes = sharedComponentPoolIndexes.ToArray();
            PrimaryComponentPoolIndexes = primaryComponentPoolIndexes.ToArray();
        }
    }
}