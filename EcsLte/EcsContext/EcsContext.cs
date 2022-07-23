using EcsLte.Exceptions;
using System;

namespace EcsLte
{
    public unsafe class EcsContext
    {
        public string Name { get; private set; }
        public bool IsDestroyed { get; private set; }
        public EntityCommandsManager Commands { get; private set; }
        public EntityManager Entities { get; private set; }
        public SystemsManager Systems { get; private set; }
        public EntityTrackerManager Tracking { get; private set; }
        internal SharedComponentIndexDictionaries SharedIndexDics { get; private set; }
        internal ArcheTypeDataManager ArcheTypeManager { get; private set; }

        internal EcsContext(string name)
        {
            Name = name;
            Commands = new EntityCommandsManager(this);
            Entities = new EntityManager(this);
            Systems = new SystemsManager(this);
            SharedIndexDics = new SharedComponentIndexDictionaries();
            ArcheTypeManager = new ArcheTypeDataManager(this);
            Tracking = new EntityTrackerManager(this);
        }

        internal void InternalDestroy()
        {
            Commands.InternalDestroy();
            Entities.InternalDestroy();
            Systems.InternalDestroy();
            Tracking.InternalDestroy();
            SharedIndexDics.Clear();
            ArcheTypeManager.InternalDestroy();

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
    }
}