namespace EcsLte.Exceptions
{
    public class WorldCreateOffThreadException : EcsLteException
    {
        public WorldCreateOffThreadException(string name)
            : base($"World '{name}' can only be created on main thread.",
                "Destroy world on main thread.")
        {
        }
    }
}