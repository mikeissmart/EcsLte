namespace EcsLte.Exceptions
{
	public class EcsContextIsDestroyedException : EcsLteException
	{
		public EcsContextIsDestroyedException(EcsContext context)
			: base($"EcsContext '{context}' is already destroyed.",
				"Cannot use context after its been destroyed.")
		{
		}
	}
}