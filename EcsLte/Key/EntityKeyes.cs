using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EcsLte
{
	internal static class EntityKeyes
	{
		private static bool _isInitialized;
		private static Dictionary<Type, EntityKeyInfo> _sharedKeyLookup;
		private static Type[] _sharedKeyTypes;

		internal static Type[] AllSharedEntityKeyTypes { get => _sharedKeyTypes; }

		internal static EntityKeyInfo GetSharedEntityKeyInfo<TComponent>()
			where TComponent : IComponent
			=> _sharedKeyLookup[typeof(TComponent)];

		internal static void Initialize()
		{
			if (_isInitialized)
				return;

			_sharedKeyLookup = new Dictionary<Type, EntityKeyInfo>();

			ComponentIndexes.Initialize();

			foreach (var type in ComponentIndexes.AllComponentTypes)
			{
				var sharedKeyes = (SharedKeyAttribute[])type.GetCustomAttributes(typeof(SharedKeyAttribute), true);
				if (sharedKeyes.Length > 0)
				{
					var key = sharedKeyes[0];
					_sharedKeyLookup.Add(type, new EntityKeyInfo
					{
						ComponentType = type,
						Members = GetKeyMembers(type, key)
					});
				}
			}

			_isInitialized = true;
			_sharedKeyTypes = _sharedKeyLookup.Keys.ToArray();
		}

		private static IKeyMember[] GetKeyMembers(Type type, BaseKeyAttribute key)
		{
			var noKeyMember = new List<string>();
			var keyMemberBuffer = new List<IKeyMember>();
			foreach (var memberName in key.MemberNames)
			{
				var fieldInfo = type.GetField(memberName, BindingFlags.Public);
				var propInfo = type.GetProperty(memberName);

				if (fieldInfo != null)
					keyMemberBuffer.Add(new FieldKeyMember { Field = fieldInfo });
				else if (propInfo != null && propInfo.CanRead)
					keyMemberBuffer.Add(new PropertyKeyMember { Property = propInfo });
				else
					noKeyMember.Add(memberName);
			}

			if (noKeyMember.Count > 0)
				throw new EntityKeyMissingMembersException(type, noKeyMember.ToArray());
			if (keyMemberBuffer.Count == 0)
				throw new Exception("Key has no members.");

			return keyMemberBuffer.ToArray();
		}
	}
}