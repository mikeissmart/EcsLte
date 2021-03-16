using System;
using System.Collections.Generic;

namespace EcsLte
{
	internal class EntityKeyes
	{
		private static EntityKeyes _instance;

		private Type[] _primaryKeyTypes;
		private Type[] _sharedKeyTypes;

		private EntityKeyes() => Initialize();

		public static EntityKeyes Instance
		{
			get
			{
				if (_instance == null)
					_instance = new EntityKeyes();
				return _instance;
			}
		}

		internal Type[] AllPrimaryEntityKeyTypes { get => _primaryKeyTypes; }
		internal Type[] AllSharedEntityKeyTypes { get => _sharedKeyTypes; }

		internal void Initialize()
		{
			var primaryKeyTypes = new List<Type>();
			var sharedKeyTypes = new List<Type>();
			foreach (var type in ComponentIndexes.Instance.AllComponentTypes)
			{
				var sharedKeyes = (SharedKeyAttribute[])type.GetCustomAttributes(typeof(SharedKeyAttribute), true);
				if (sharedKeyes.Length > 0)
					sharedKeyTypes.Add(type);

				var primaryKeyes = (PrimaryKeyAttribute[])type.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
				if (primaryKeyes.Length > 0)
					primaryKeyTypes.Add(type);
			}

			_primaryKeyTypes = primaryKeyTypes.ToArray();
			_sharedKeyTypes = sharedKeyTypes.ToArray();
		}
	}
}