namespace EcsLte
{
    internal class EntityKeyData
    {
        private EcsContext _context;

        public EntityCollection Entities { get; private set; }
        public IPrimaryKey _primaryKey;
        public ISharedKey[] _sharedKeyes;
        public IComponentPrimaryKey _primaryComponent;
        public IComponentSharedKey[] _sharedComponents;

        public EntityKeyData()
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