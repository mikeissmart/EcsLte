namespace EcsLte.Exceptions
{
    public class GroupIsDestroyedException : EcsLteException
    {
        public GroupIsDestroyedException(Group group)
            : base($"Group '{group}' is already destroyed.",
                "Cannot use group after its been destroyed.")
        {
        }
    }
}