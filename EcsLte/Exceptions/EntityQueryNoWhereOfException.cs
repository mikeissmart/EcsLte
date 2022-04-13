namespace EcsLte.Exceptions
{
    public class EntityQueryNoWhereOfException : EcsLteException
    {
        public EntityQueryNoWhereOfException()
            : base("EntityQuery has no applied WhereOf.")
        {
        }
    }
}
