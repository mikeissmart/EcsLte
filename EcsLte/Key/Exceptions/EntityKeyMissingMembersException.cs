using System;

namespace EcsLte.Exceptions
{
	public class EntityKeyMissingMembersException : EcsLteException
	{
		public EntityKeyMissingMembersException(Type componentType, string[] memberNames)
			: base($"Component '{componentType.Name}' have missing member names.",
				  $"Missing member names are {string.Join("\n", memberNames)}")
		{ }
	}
}