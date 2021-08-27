namespace EcsLte.PerformanceTest
{
    internal abstract class BasePerformanceTest
    {
        public abstract void PreRun();

        public abstract void Run();

        public abstract bool CanRunParallel();

        public abstract void RunParallel();

        public abstract void PostRun();
    }
}