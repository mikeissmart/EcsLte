namespace EcsLte.Exceptions
{
    public class EntityFilterAlreadyExistException : EcsLteException
    {
        public EntityFilterAlreadyExistException(string name)
            : base($"EntityFilter with name '{name}' already exists.")
        { }
    }
}
