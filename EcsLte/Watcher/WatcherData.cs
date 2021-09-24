namespace EcsLte
{
    internal class WatcherData
    {
        private EcsContext _context;

        public EntityCollection Entities { get; private set; }

        public WatcherData()
        {
        }

        #region ObjectCache

        internal void Initialize(EcsContext context)
        {
            _context = context;

            Entities = context.CreateEntityCollection();
        }

        internal void Reset()
        {
            Entities.Reset();
            _context.RemoveEntityCollection(Entities);
        }

        #endregion
    }
}