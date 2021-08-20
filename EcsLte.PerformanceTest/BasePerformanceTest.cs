namespace EcsLte.PerformanceTest
{
    internal abstract class BasePerformanceTest
    {
        public abstract void PreRun();

        public abstract void Run();

        public abstract int ParallelRunCount();

        public abstract void RunParallel(int index, int startIndex, int endIndex);

        public abstract void PostRun();
    }
}