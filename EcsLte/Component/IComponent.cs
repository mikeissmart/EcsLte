namespace EcsLte
{
    public interface IComponent
    {
    }

    public interface IGeneralComponent : IComponent
    {
    }

    public interface ISharedComponent : IComponent
    {
    }

    public interface IUniqueComponent : IComponent
    {
    }
}