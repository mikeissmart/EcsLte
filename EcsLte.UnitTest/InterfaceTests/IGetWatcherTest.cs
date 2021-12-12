namespace EcsLte.UnitTest.InterfaceTests
{
	public interface IGetWatcherTest
	{
		void WatchAdded();
		void WatchUpdated();
		void WatchRemoved();
		void WatchAddedOrUpdated();
		void WatchAddedOrRemoved();
	}
}