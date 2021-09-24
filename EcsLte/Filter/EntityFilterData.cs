namespace EcsLte
{
    internal class EntityFilterData
    {
        private EcsContext _context;

        public EntityCollection Entities { get; private set; }

        public EntityFilterData()
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