namespace EcsLte.Exceptions
{
    public class CollectorGroupMissingComponent : EcsLteException
    {
        public CollectorGroupMissingComponent()
            : base("Collection group does not have collectionTrigger component.",
                "When creating a collection the collectionTrigger component must be group allOf or anyOf components.")
        {
        }
    }
}