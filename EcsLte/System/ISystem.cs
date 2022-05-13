namespace EcsLte
{
    public interface IInitializeSystem
    {
        bool IsActive { get; }

        void Initialize();
    }

    public interface IExecuteSystem
    {
        bool IsActive { get; }

        void Execute();
    }

    public interface ICleanupSystem
    {
        bool IsActive { get; }

        void Cleanup();
    }

    public interface ITearDownSystem
    {
        bool IsActive { get; }

        void TearDown();
    }
}
