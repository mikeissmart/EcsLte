namespace EcsLte
{
    public abstract class SystemBase
    {
        public long InitializeMilliseconds { get; internal set; }
        public long ActivatedMilliseconds { get; internal set; }
        public long DeactivatedMilliseconds { get; internal set; }
        public long UpdateMilliseconds { get; internal set; }
        public long UninitializeMilliseconds { get; internal set; }

        public EcsContext Context { get; internal set; }
        public bool IsInitialized { get; internal set; }
        public bool IsActive { get; internal set; }

        internal SystemConfig Config { get; set; }

        public virtual void Activated() { }
        public virtual void Deactivated() { }

        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Uninitialize() { }
    }
}