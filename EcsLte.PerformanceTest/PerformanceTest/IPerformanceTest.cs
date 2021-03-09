namespace EcsLte.PerformanceTest
{
	internal interface IPerformanceTest
	{
		void PreRun();

		void Run();

		void PostRun();
	}
}