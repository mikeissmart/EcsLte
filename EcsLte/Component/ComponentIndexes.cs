using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Component.Exceptions;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class ComponentIndex<TComponent> where TComponent : IComponent
    {
        private static int _index = -1;
        private static bool _isRecordable;
        private static bool _isUnique;

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

        public Type ComponentType => typeof(TComponent);

        private static void GetData()
        {
            _index = ComponentIndexes.Instance.GetComponentIndex<TComponent>();
            _isRecordable = ComponentIndexes.Instance.RecordableComponentIndexes
                .Any(x => x == _index);
            _isUnique = ComponentIndexes.Instance.UniqueComponentIndexes
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
        public int Count => _componentIndexTypeLookup.Count;

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

        public int GetComponentIndex<TComponent>() where TComponent : IComponent
        {
            return _componentIndexTypeLookup[typeof(TComponent)];
        }

        private void Initialize()
        {
            var iComponentType = typeof(IComponent);
            var iComponentRecordableType = typeof(IComponentRecordable);
            var iComponentUniqueType = typeof(IComponentUnique);
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
            foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
            {
                _componentIndexTypeLookup.Add(type, _componentIndexTypeLookup.Count);
                //nameLookup.Add(type.Name, nameLookup.Count);

                if (iComponentRecordableType.IsAssignableFrom(type))
                    recordableComponentIndexes.Add(_componentIndexTypeLookup.Count - 1);

                if (iComponentUniqueType.IsAssignableFrom(type))
                    uniqueComponentIndexes.Add(_componentIndexTypeLookup.Count - 1);
            }

            if (_componentIndexTypeLookup.Keys.Count == 0)
                throw new ComponentNoneException();

            AllComponentTypes = _componentIndexTypeLookup.Keys.ToArray();
            AllComponentIndexes = _componentIndexTypeLookup.Values.ToArray();
            RecordableComponentIndexes = recordableComponentIndexes.ToArray();
            UniqueComponentIndexes = uniqueComponentIndexes.ToArray();
        }
    }
}