namespace EcsLte.Exceptions
{
    public class EntityBlueprintNoComponentsException : EcsLteException
    {
        public EntityBlueprintNoComponentsException()
            : base($"EntityBlueprint has no components.")
        {
        }
    }
}