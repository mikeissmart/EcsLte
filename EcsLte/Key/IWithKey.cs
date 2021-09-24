namespace EcsLte
{
    public interface IWithKey
    {
        EntityKey WithKey(IComponentPrimaryKey primaryKey);
        EntityKey WithKey(IComponentSharedKey sharedKey);
        EntityKey WithKey(params IComponentSharedKey[] sharedKeyes);
    }
}