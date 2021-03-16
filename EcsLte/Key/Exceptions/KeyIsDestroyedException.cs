namespace EcsLte.Exceptions
{
	public class KeyIsDestroyedException : EcsLteException
	{
		public KeyIsDestroyedException(BaseKey key)
			: base($"Key  '{key}' is already destroyed.",
				  "Cannot use key after its been destroyed.")
		{ }
	}
}