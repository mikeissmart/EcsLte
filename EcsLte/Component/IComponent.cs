namespace EcsLte
{
    public interface IComponent
    {
    }

    public interface IRecordableComponent : IComponent
    {
    }

    public interface IUniqueComponent : IComponent
    {
    }

    public interface ISharedComponent : IComponent
    {
    }

    public interface IPrimaryComponent : IComponent
    {
    }
}