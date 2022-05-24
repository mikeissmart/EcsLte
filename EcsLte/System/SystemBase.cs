namespace EcsLte
{
    public abstract class SystemBase
    {
        internal SystemConfig Config { get; set; }
        public EcsContext Context { get; internal set; }
        public bool IsActive { get; set; }

        protected SystemBase(EcsContext context, bool initialActiveState = true)
        {
            Context = context;
            IsActive = initialActiveState;
        }
    }
}