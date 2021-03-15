namespace EcsLte.Exceptions
{
	public class WorldDoesNotHaveGroupException : EcsLteException
	{
		public WorldDoesNotHaveGroupException(World world, Group group)
			: base($"World '{world}' does not have group '{group}'.",
				  "Use same world that created group to use group.")
		{ }
	}
}