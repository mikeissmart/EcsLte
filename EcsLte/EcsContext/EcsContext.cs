using EcsLte.Exceptions;
using System;

namespace EcsLte
{
    public class EcsContext
    {
        public string Name { get; private set; }
        public bool IsDestroyed { get; private set; }
        public EntityCommandsManager Commands { get; private set; }
        public ArcheTypeManager ArcheTypes { get; private set; }
        public EntityManager Entities { get; private set; }
        public EntityFilterManager Filters { get; private set; }
        public SystemManager Systems { get; private set; }
        public EntityTrackerManager Tracking { get; private set; }
        public EntityQueryManager Queries { get; private set; }
        internal SharedComponentDictionaries SharedComponentDics { get; private set; }
        internal bool StructChangeAvailable { get; set; }

        internal EcsContext(string name)
        {
            Name = name;
            Commands = new EntityCommandsManager(this);
            Entities = new EntityManager(this);
            Filters = new EntityFilterManager(this);
            Systems = new SystemManager(this);
            SharedComponentDics = new SharedComponentDictionaries();
            ArcheTypes = new ArcheTypeManager(this);
            Tracking = new EntityTrackerManager(this);
            Queries = new EntityQueryManager(this);

            StructChangeAvailable = true;
        }

        internal void InternalDestroy()
        {
            Commands.InternalDestroy();
            Entities.InternalDestroy();
            Filters.InternalDestroy();
            Systems.InternalDestroy();
            Tracking.InternalDestroy();
            SharedComponentDics.InternalDestroy();
            Queries.InternalDestroy();
            ArcheTypes.InternalDestroy();

            IsDestroyed = true;
        }

        internal static void AssertContext(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);
        }

        internal void AssertContext()
        {
            if (IsDestroyed)
                throw new EcsContextIsDestroyedException(this);
        }

        internal void AssertStructualChangeAvailable()
        {
            if (!StructChangeAvailable)
                throw new EcsContextStrualChangeNotAvailableException();
        }
    }
}
