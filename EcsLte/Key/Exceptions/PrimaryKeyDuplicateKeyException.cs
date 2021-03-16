namespace EcsLte.Exceptions
{
	public class PrimaryKeyDuplicateKeyException : EcsLteException
	{
		public PrimaryKeyDuplicateKeyException(IPrimaryKey primaryKey, IComponent component)
			: base($"PrimaryKey  '{primaryKey}' is alrady has component key '{component}'.",
				  "Cannot have duplicate primary keyes. Use shared key instead.")
		{ }
	}
}