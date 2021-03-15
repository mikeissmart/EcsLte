namespace EcsLte.Exceptions
{
	public class SharedKeyIsDestroyedException : EcsLteException
	{
		public SharedKeyIsDestroyedException(ISharedKey sharedKey)
			: base($"SharedKey  '{sharedKey}' is already destroyed.",
				  "Cannot use shared key after its been destroyed.")
		{ }
	}
}