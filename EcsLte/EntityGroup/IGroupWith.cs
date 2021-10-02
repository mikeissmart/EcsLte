namespace EcsLte
{
    public interface IGroupWith
    {
        EntityGroup GroupWith(ISharedComponent sharedComponent);
        EntityGroup GroupWith(params ISharedComponent[] sharedComponents);
    }
}