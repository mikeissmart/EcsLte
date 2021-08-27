namespace EcsLte
{
    public interface ISystem
    {
        void Initialize();
        void Execute();
        void Cleanup();
        void TearDown();
    }
}