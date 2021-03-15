using EcsLte.Exceptions;
using System;
using System.Collections.Generic;

namespace EcsLte
{
	public class KeyManager
	{
		private readonly Dictionary<Type, Dictionary<Group, ISharedKey>> _sharedKeyLookup;
		private readonly EntityManager _entityManager;
		private readonly GroupManager _groupManager;

		internal KeyManager(World world, EntityManager entityManager, GroupManager groupManager)
		{
			EntityKeyes.Initialize();

			_sharedKeyLookup = new Dictionary<Type, Dictionary<Group, ISharedKey>>();
			_entityManager = entityManager;
			_groupManager = groupManager;

			World = world;

			_groupManager.AnyGroupDestroyed.Subscribe(OnAnyGroupDestroyed);

			foreach (var keyType in EntityKeyes.AllSharedEntityKeyTypes)
				_sharedKeyLookup.Add(keyType, new Dictionary<Group, ISharedKey>());
		}

		public World World { get; private set; }

		public SharedKey<TComponent> GetSharedKey<TComponent>(Group group)
			where TComponent : IComponent
		{
			var componentType = typeof(TComponent);
			if (!_sharedKeyLookup.ContainsKey(componentType))
				throw new ComponentNotSharedKeyException(componentType);
			if (group == null)
				throw new ArgumentNullException();
			if (group.IsDestroyed)
				throw new GroupIsDestroyedException(group);
			if (group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, group);

			var keyesLookup = _sharedKeyLookup[componentType];
			if (!keyesLookup.TryGetValue(group, out ISharedKey entityKey))
			{
				var entityKeyInfo = EntityKeyes.GetSharedEntityKeyInfo<TComponent>();
				entityKey = new SharedKey<TComponent>(_entityManager, entityKeyInfo, group);
				keyesLookup.Add(group, entityKey);
			}

			return (SharedKey<TComponent>)entityKey;
		}

		public void DestroySharedKey(ISharedKey sharedKey)
		{
			if (sharedKey == null)
				throw new ArgumentNullException();
			if (sharedKey.IsDestroyed)
				throw new SharedKeyIsDestroyedException(sharedKey);
			if (sharedKey.Group.GroupManager.World != World)
				throw new WorldDoesNotHaveGroupException(World, sharedKey.Group);

			sharedKey.IsDestroyed = true;
			_sharedKeyLookup[sharedKey.ComponentType].Remove(sharedKey.Group);
		}

		private void OnAnyGroupDestroyed(Group group)
		{
			foreach (var typeLookup in _sharedKeyLookup)
				typeLookup.Value.Remove(group);
		}
	}
}