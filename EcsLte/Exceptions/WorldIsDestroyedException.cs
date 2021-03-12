namespace EcsLte.Exceptions
{
	public class WorldIsDestroyedException : EcsLteException
	{
		public WorldIsDestroyedException(World world)
			: base($"World '{world}' is already destroyed.",
				  "Cannot use world after its been destroyed.")
		{ }
	}
}