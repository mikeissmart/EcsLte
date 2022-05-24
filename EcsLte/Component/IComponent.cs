namespace EcsLte
{
    public interface IComponent
    {
    }

    public interface IRecordableComponent : IComponent
    {
    }

    public interface ISharedComponent : IComponent
    {
    }

    public interface IUniqueComponent : IComponent
    {
    }
}