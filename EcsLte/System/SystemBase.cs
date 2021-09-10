namespace EcsLte
{
    public abstract class SystemBase
    {
        public World CurrentWorld { get; internal set; }
        public bool IsActive { get; internal set; }

        public SystemBase(bool initialActiveState = true)
        {
            IsActive = initialActiveState;
        }

        public virtual void Initialize() { }
        public virtual void Execute() { }
        public virtual void Cleanup() { }
        public virtual void TearDown() { }
    }
}