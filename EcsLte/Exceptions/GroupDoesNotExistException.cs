namespace EcsLte.Exceptions
{
    public class GroupDoesNotExistException : EcsLteException
    {
        public GroupDoesNotExistException(World world, Group group)
            : base($"World '{world}' does not have group '{group}'.",
                "Use same world that created group to use group.")
        {
        }
    }
}