using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
	public static class ComponentIndex<TComponent> where TComponent : IComponent
	{
		private static int _index = -1;

		public static int Index
		{
			get
			{
				if (_index == -1)
					_index = ComponentIndexes.GetComponentIndex<TComponent>();
				return _index;
			}
		}
	}

	internal static class ComponentIndexes
	{
		private static Dictionary<Type, int> _componentIndexTypeLookup = InitializeAll();
		private static Dictionary<string, int> _componentIndexNameLookup;
		private static Type[] _componentTypes;
		private static int[] _recordableComponentIndexes;

		public static Type[] AllComponentTypes { get => _componentTypes; }
		public static int[] RecordableComponentIndexes { get => _recordableComponentIndexes; }
		public static int Count { get => _componentIndexTypeLookup.Count; }

		public static int GetComponentIndex(Type componentType)
			=> _componentIndexTypeLookup[componentType];

		public static int GetComponentIndex<TComponent>() where TComponent : IComponent
			=> _componentIndexTypeLookup[typeof(TComponent)];

		public static int GetComponentIndex(string componentName)
			=> _componentIndexNameLookup[componentName];

		private static Dictionary<Type, int> InitializeAll()
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

			var typeLookup = new Dictionary<Type, int>();
			var nameLookup = new Dictionary<string, int>();
			var recordableComponentIndexes = new List<int>();
			foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
			{
				typeLookup.Add(type, typeLookup.Count);
				nameLookup.Add(type.Name, nameLookup.Count);

				if (iComponentRecordableType.IsAssignableFrom(type))
					recordableComponentIndexes.Add(typeLookup.Count - 1);
			}
			_componentTypes = typeLookup.Keys.ToArray();
			_recordableComponentIndexes = recordableComponentIndexes.ToArray();
			_componentIndexTypeLookup = typeLookup;

			return _componentIndexTypeLookup;
		}
	}
}