using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class ComponentIndex<TComponent> where TComponent : IComponent
    {
        private static int _index = -1;
        private static bool _isRecordable;
        private static bool _isUnique;
        private static bool _isSharedKey;
        private static bool _isPrimaryKey;

        public static int Index
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _index;
            }
        }

        public static bool IsRecordable
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isRecordable;
            }
        }

        public static bool IsUnique
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isUnique;
            }
        }

        public static bool IsSharedKey
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isSharedKey;
            }
        }

        public static bool IsPrimaryKey
        {
            get
            {
                if (_index == -1)
                    GetData();
                return _isPrimaryKey;
            }
        }

        public Type ComponentType => typeof(TComponent);

        private static void GetData()
        {
            _index = ComponentIndexes.Instance.GetComponentIndex<TComponent>();
            _isRecordable = ComponentIndexes.Instance.RecordableComponentIndexes
                .Any(x => x == _index);
            _isUnique = ComponentIndexes.Instance.UniqueComponentIndexes
                .Any(x => x == _index);
            _isSharedKey = ComponentIndexes.Instance.SharedKeyComponentIndexes
                .Any(x => x == _index);
            _isPrimaryKey = ComponentIndexes.Instance.PrimaryKeyComponentIndexes
                .Any(x => x == _index);
        }
    }

    public class ComponentIndexes
    {
        private static ComponentIndexes _instance;

        private Dictionary<Type, int> _componentIndexTypeLookup;

        private ComponentIndexes()
        {
            Initialize();
        }

        public static ComponentIndexes Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ComponentIndexes();
                return _instance;
            }
        }

        public Type[] AllComponentTypes { get; private set; }
        public int[] AllComponentIndexes { get; private set; }
        public int[] RecordableComponentIndexes { get; private set; }
        public int[] UniqueComponentIndexes { get; private set; }
        public int[] SharedKeyComponentIndexes { get; private set; }
        public int[] PrimaryKeyComponentIndexes { get; private set; }
        public int Count => _componentIndexTypeLookup.Count;

        public int GetComponentIndex<TComponent>() where TComponent : IComponent
        {
            return _componentIndexTypeLookup[typeof(TComponent)];
        }

        public int GetComponentIndex(Type componentType)
        {
            return _componentIndexTypeLookup[componentType];
        }

        public bool IsComponentRecordable(Type componentType)
        {
            var index = _componentIndexTypeLookup[componentType];
            return RecordableComponentIndexes
                .Any(x => x == index);
        }

        public bool IsComponentUnique(Type componentType)
        {
            var index = _componentIndexTypeLookup[componentType];
            return UniqueComponentIndexes
                .Any(x => x == index);
        }

        public bool IsSharedKey(Type componentType)
        {
            var index = _componentIndexTypeLookup[componentType];
            return SharedKeyComponentIndexes
                .Any(x => x == index);
        }

        public bool IsPrimaryKey(Type componentType)
        {
            var index = _componentIndexTypeLookup[componentType];
            return PrimaryKeyComponentIndexes
                .Any(x => x == index);
        }

        internal IComponentPool[] CreateComponentPools(int initialSize)
        {
            var componentPools = new IComponentPool[Count];
            var poolType = typeof(ComponentPool<>);
            var args = new object[] { initialSize };
            for (int i = 0; i < Count; i++)
            {
                componentPools[i] = (IComponentPool)Activator.CreateInstance(poolType.MakeGenericType(AllComponentTypes[i]), args);
            }

            return componentPools;
        }

        internal Dictionary<int, IPrimaryKey> CreatePrimaryKeyes()
        {
            var keyes = new Dictionary<int, IPrimaryKey>();
            var keyType = typeof(PrimaryKey<>);
            foreach (var index in PrimaryKeyComponentIndexes)
                keyes.Add(index,
                    (IPrimaryKey)Activator.CreateInstance(keyType.MakeGenericType(AllComponentTypes[index])));

            return keyes;
        }

        internal Dictionary<int, ISharedKey> CreateSharedKeyes()
        {
            var keyes = new Dictionary<int, ISharedKey>();
            var keyType = typeof(SharedKey<>);
            foreach (var index in SharedKeyComponentIndexes)
                keyes.Add(index,
                    (ISharedKey)Activator.CreateInstance(keyType.MakeGenericType(AllComponentTypes[index])));

            return keyes;
        }

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iComponentRecordableType = typeof(IComponentRecordable);
            var iComponentUniqueType = typeof(IComponentUnique);
            var iComponentSharedKeyType = typeof(IComponentSharedKey);
            var iComponentPrimaryKeyType = typeof(IComponentPrimaryKey);
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

            _componentIndexTypeLookup = new Dictionary<Type, int>();
            //var nameLookup = new Dictionary<string, int>();
            var recordableComponentIndexes = new List<int>();
            var uniqueComponentIndexes = new List<int>();
            var sharedKeyComponentIndexes = new List<int>();
            var primaryKeyComponentIndexes = new List<int>();
            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                if (iComponentRecordableType.IsAssignableFrom(type))
                    recordableComponentIndexes.Add(_componentIndexTypeLookup.Count);

                if (iComponentUniqueType.IsAssignableFrom(type))
                    uniqueComponentIndexes.Add(_componentIndexTypeLookup.Count);

                if (iComponentSharedKeyType.IsAssignableFrom(type))
                    sharedKeyComponentIndexes.Add(_componentIndexTypeLookup.Count);

                if (iComponentPrimaryKeyType.IsAssignableFrom(type))
                    primaryKeyComponentIndexes.Add(_componentIndexTypeLookup.Count);

                _componentIndexTypeLookup.Add(type, _componentIndexTypeLookup.Count);
                //nameLookup.Add(type.Name, nameLookup.Count);
            }

            if (_componentIndexTypeLookup.Keys.Count == 0)
                throw new ComponentNoneException();

            AllComponentTypes = _componentIndexTypeLookup.Keys.ToArray();
            AllComponentIndexes = _componentIndexTypeLookup.Values.ToArray();
            RecordableComponentIndexes = recordableComponentIndexes.ToArray();
            UniqueComponentIndexes = uniqueComponentIndexes.ToArray();
            SharedKeyComponentIndexes = sharedKeyComponentIndexes.ToArray();
            PrimaryKeyComponentIndexes = primaryKeyComponentIndexes.ToArray();
        }
    }
}