using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public class ComponentIndex<TComponent> where TComponent : IComponent
	{
		private static int _index = -1;

		public static int Index
		{
			get
			{
				if (_index == -1)
					_index = ComponentIndexes.Instance.GetComponentIndex<TComponent>();
				return _index;
			}
		}

		public Type ComponentType { get => typeof(TComponent); }
	}

	internal class ComponentIndexes
	{
		private static ComponentIndexes _instance;

		private Dictionary<Type, int> _componentIndexTypeLookup;
		private Type[] _componentTypes;
		private int[] _recordableComponentIndexes;

		private ComponentIndexes() => Initialize();

		public static ComponentIndexes Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ComponentIndexes();
				return _instance;
			}
		}

		public Type[] AllComponentTypes { get => _componentTypes; }
		public int[] RecordableComponentIndexes { get => _recordableComponentIndexes; }
		public int Count { get => _componentIndexTypeLookup.Count; }

		public int GetComponentIndex(Type componentType)
			=> _componentIndexTypeLookup[componentType];

		public int GetComponentIndex<TComponent>() where TComponent : IComponent
			=> _componentIndexTypeLookup[typeof(TComponent)];

		private void Initialize()
		{
			var iComponentType = typeof(IComponent);
			var iComponentRecordableType = typeof(IComponentRecordable);
			var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
				   .SelectMany(x => x.GetTypes())
				   .Where(x =>
					   x.IsPublic &&
					   !x.IsAbstract &&
					   iComponentType.IsAssignableFrom(x));

			var componentsWrongType = new List<Type>();
			foreach (var type in componentTypes)
			{
				if (!type.IsValueType)
					componentsWrongType.Add(type);
			}
			if (componentsWrongType.Count != 0)
				throw new ComponentNotStructException(componentsWrongType.ToArray());

			_componentIndexTypeLookup = new Dictionary<Type, int>();
			var nameLookup = new Dictionary<string, int>();
			var recordableComponentIndexes = new List<int>();
			foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
			{
				_componentIndexTypeLookup.Add(type, _componentIndexTypeLookup.Count);
				nameLookup.Add(type.Name, nameLookup.Count);

				if (iComponentRecordableType.IsAssignableFrom(type))
					recordableComponentIndexes.Add(_componentIndexTypeLookup.Count - 1);
			}

			if (_componentIndexTypeLookup.Keys.Count == 0)
				throw new NoComponentsException();

			_componentTypes = _componentIndexTypeLookup.Keys.ToArray();
			_recordableComponentIndexes = recordableComponentIndexes.ToArray();
		}
	}
}