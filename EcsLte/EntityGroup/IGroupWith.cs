namespace EcsLte
{
    public interface IGroupWith
    {
        EntityGroup GroupWith(IPrimaryComponent primaryComponent);
        EntityGroup GroupWith(ISharedComponent sharedComponent);
        EntityGroup GroupWith(params ISharedComponent[] sharedComponents);
    }
}