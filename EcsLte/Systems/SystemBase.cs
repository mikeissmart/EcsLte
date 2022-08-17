namespace EcsLte
{
    public abstract class SystemBase
    {
        public EcsContext Context { get; internal set; }
        public bool IsActive { get; internal set; }

        public virtual void Activated() { }
        public virtual void Deactivated() { }

        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Uninitialize() { }
    }
}