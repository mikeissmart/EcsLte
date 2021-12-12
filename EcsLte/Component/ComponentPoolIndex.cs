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
		private Dictionary<int, ComponentPoolConfig> _componentPoolConfigIndexes;
		private Dictionary<Type, ComponentPoolConfig> _componentPoolConfigTypes;

		private ComponentPoolIndexes() => Initialize();

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
		internal Type[] AllRecordableTypes { get; private set; }
		internal Type[] AllUniqueTypes { get; private set; }
		internal Type[] AllSharedTypes { get; private set; }

		internal int[] AllRecordableIndexes { get; private set; }
		internal int[] AllUniqueIndexes { get; private set; }
		internal int[] AllSharedIndexes { get; private set; }

		internal int AllComponentCount { get; private set; }
		internal int RecordableComponentCount { get; private set; }
		internal int UniqueComponentCount { get; private set; }
		internal int SharedComponentCount { get; private set; }

		internal ComponentPoolConfig GetConfig(Type componentType) => _componentPoolConfigTypes[componentType];

		internal ComponentPoolConfig GetConfig(int componentPoolIndex) => _componentPoolConfigIndexes[componentPoolIndex];

		internal IComponentPool[] CreateComponentPools(int initialSize)
		{
			var componentPools = new IComponentPool[AllComponentCount];
			var poolType = typeof(ComponentPool<>);
			var args = new object[] { initialSize };
			for (var i = 0; i < AllComponentCount; i++)
				componentPools[i] =
					(IComponentPool)Activator.CreateInstance(poolType.MakeGenericType(AllComponentTypes[i]), args);

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
			var recordableTypeIndexes = new Dictionary<int, Type>();
			var uniqueTypeIndexes = new Dictionary<int, Type>();
			var sharedTypeIndexes = new Dictionary<int, Type>();
			var recordableIndex = 0;
			var uniqueIndex = 0;
			var sharedIndex = 0;
			foreach (var type in componentTypes.OrderBy(x => x.FullName.ToString()))
			{
				var poolIndex = _componentPoolConfigIndexes.Count;
				var config = new ComponentPoolConfig
				{
					PoolIndex = poolIndex
				};
				if (iRecordableComponentType.IsAssignableFrom(type))
				{
					recordableTypeIndexes.Add(poolIndex, type);
					config.RecordableIndex = recordableIndex++;
					config.IsRecordable = true;
				}

				if (iUniqueComponentType.IsAssignableFrom(type))
				{
					uniqueTypeIndexes.Add(poolIndex, type);
					config.UniqueIndex = uniqueIndex++;
					config.IsUnique = true;
				}

				if (iSharedComponentType.IsAssignableFrom(type))
				{
					sharedTypeIndexes.Add(poolIndex, type);
					config.SharedIndex = sharedIndex++;
					config.IsShared = true;
				}

				_componentPoolConfigTypes.Add(type, config);
				_componentPoolConfigIndexes.Add(poolIndex, config);
			}

			if (_componentPoolConfigTypes.Count == 0)
				throw new ComponentNoneException();

			AllComponentTypes = _componentPoolConfigTypes.Keys.ToArray();
			AllRecordableTypes = recordableTypeIndexes.Values.ToArray();
			AllUniqueTypes = uniqueTypeIndexes.Values.ToArray();
			AllSharedTypes = sharedTypeIndexes.Values.ToArray();

			AllRecordableIndexes = recordableTypeIndexes.Keys.ToArray();
			AllUniqueIndexes = uniqueTypeIndexes.Keys.ToArray();
			AllSharedIndexes = sharedTypeIndexes.Keys.ToArray();

			AllComponentCount = _componentPoolConfigTypes.Count;
			RecordableComponentCount = recordableTypeIndexes.Count;
			UniqueComponentCount = uniqueTypeIndexes.Count;
			SharedComponentCount = sharedTypeIndexes.Count;
		}
	}
}