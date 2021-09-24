namespace EcsLte.UnitTest.InterfaceTests
{
    public interface IGetWatcherTest
    {
        void Added();
        void Updated();
        void Removed();
        void AddedOrUpdated();
        void AddedOrRemoved();
    }
}