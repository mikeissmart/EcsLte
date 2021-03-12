namespace EcsLte.Exceptions
{
	public class WorldDoesNotHaveEntityException : EcsLteException
	{
		public WorldDoesNotHaveEntityException(World world, Entity entity)
			: base($"World '{world}' does not have entity '{entity}', entity world id is '{entity}'.",
				  "Use same world that created entity to use entity.")
		{ }
	}
}