using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class KeyManager
	{
		private readonly Dictionary<Type, Dictionary<Group, ISharedKey>> _sharedKeyLookup;
		private readonly Dictionary<Type, Dictionary<Group, IPrimaryKey>> _primaryKeyLookup;
		private readonly EntityManager _entityManager;
		private readonly GroupManager _groupManager;

		internal KeyManager(World world, EntityManager entityManager, GroupManager groupManager)
		{
			_sharedKeyLookup = new Dictionary<Type, Dictionary<Group, ISharedKey>>();
			_primaryKeyLookup = new Dictionary<Type, Dictionary<Group, IPrimaryKey>>();
			_entityManager = entityManager;
			_groupManager = groupManager;

			World = world;

			_groupManager.AnyGroupDestroyed.Subscribe(OnAnyGroupDestroyed);

			foreach (var keyType in EntityKeyes.Instance.AllSharedEntityKeyTypes)
				_sharedKeyLookup.Add(keyType, new Dictionary<Group, ISharedKey>());
			foreach (var keyType in EntityKeyes.Instance.AllPrimaryEntityKeyTypes)
				_primaryKeyLookup.Add(keyType, new Dictionary<Group, IPrimaryKey>());
		}

		public World World { get; private set; }

		public SharedKey<TComponent> GetSharedKey<TComponent>(Group group)
			where TComponent : IComponent
		{
			var componentType = typeof(TComponent);
			if (!_sharedKeyLookup.ContainsKey(componentType))
				throw new ComponentDoesNotHaveSharedKey(componentType);
			if (group == null)
				throw new ArgumentNullException();
			if (group.IsDestroyed)
				throw new GroupIsDestroyedException(group);
			if (group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, group);

			var keyesLookup = _sharedKeyLookup[componentType];
			if (!keyesLookup.TryGetValue(group, out ISharedKey entityKey))
			{
				entityKey = new SharedKey<TComponent>(_entityManager, group);
				keyesLookup.Add(group, entityKey);
			}

			return (SharedKey<TComponent>)entityKey;
		}

		public void DestroySharedKey(ISharedKey sharedKey)
		{
			if (sharedKey == null)
				throw new ArgumentNullException();
			if (sharedKey.IsDestroyed)
				throw new KeyIsDestroyedException((BaseKey)sharedKey);
			if (sharedKey.Group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, sharedKey.Group);

			((BaseKey)sharedKey).IsDestroyed = true;
			_sharedKeyLookup[sharedKey.ComponentType].Remove(sharedKey.Group);
		}

		public PrimaryKey<TComponent> GetPrimaryKey<TComponent>(Group group)
			where TComponent : IComponent
		{
			var componentType = typeof(TComponent);
			if (!_primaryKeyLookup.ContainsKey(componentType))
				throw new ComponentDoesNotHavePrimaryKey(componentType);
			if (group == null)
				throw new ArgumentNullException();
			if (group.IsDestroyed)
				throw new GroupIsDestroyedException(group);
			if (group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, group);

			var keyesLookup = _primaryKeyLookup[componentType];
			if (!keyesLookup.TryGetValue(group, out IPrimaryKey entityKey))
			{
				entityKey = new PrimaryKey<TComponent>(_entityManager, group);
				keyesLookup.Add(group, entityKey);
			}

			return (PrimaryKey<TComponent>)entityKey;
		}

		public void DestroyPrimaryKey(IPrimaryKey primaryKey)
		{
			if (primaryKey == null)
				throw new ArgumentNullException();
			if (primaryKey.IsDestroyed)
				throw new KeyIsDestroyedException((BaseKey)primaryKey);
			if (primaryKey.Group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, primaryKey.Group);

			((BaseKey)primaryKey).IsDestroyed = true;
			_primaryKeyLookup[primaryKey.ComponentType].Remove(primaryKey.Group);
		}

		private void OnAnyGroupDestroyed(Group group)
		{
			foreach (var typeLookup in _sharedKeyLookup)
				typeLookup.Value.Remove(group);
		}
	}
}