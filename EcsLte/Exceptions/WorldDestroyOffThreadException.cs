namespace EcsLte.Exceptions
{
    public class WorldDestroyOffThreadException : EcsLteException
    {
        public WorldDestroyOffThreadException(World world)
            : base($"World '{world}' can only be destroyed on main thread.",
                "Destroy world on main thread.")
        {
        }
    }
}