using EcsLte.Data;
using EcsLte.Exceptions;
using EcsLte.Managed;
using EcsLte.ManagedArcheType;
using EcsLte.Native;
using EcsLte.NativeArcheType;
using EcsLte.NativeArcheTypeContinous;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public static class EcsContexts
    {
        private static readonly DictionaryDataCache<string, EcsContext> _ecsContexts =
            new DictionaryDataCache<string, EcsContext>(UpdateContextsCache);

        public static EcsContext[] Contexts => _ecsContexts.CachedData;
        public static EcsContext Default_Managed { get; set; } =
            CreateEcsContext_Managed("Default_Managed");
        public static EcsContext Default_Managed_ArcheType { get; set; } =
            CreateEcsContext_ArcheType_Managed("Default_Managed_ArcheType");
        public static EcsContext Default_Native { get; set; } =
            CreateEcsContext_Native("Default_Native");
        public static EcsContext Default_Native_ArcheType { get; set; } =
            CreateEcsContext_ArcheType_Native("Default_ArcheType_Native");
        public static EcsContext Default_Native_ArcheType_Continuous { get; set; } =
            CreateEcsContext_ArcheType_Native_Continuous("Default_ArcheType_Native_Continuous");

        public static bool HasContext(string name) =>
            _ecsContexts.Has(name);

        public static EcsContext GetContext(string name)
        {
            if (!HasContext(name))
                throw new EcsContextDoesNotExistException(name);

            return _ecsContexts.Get(name);
        }

        public static EcsContext CreateEcsContext_Managed(string name) => AddEcsContext(x => new EcsContext(x, new ComponentEntityFactory_Managed()), name);

        public static EcsContext CreateEcsContext_ArcheType_Managed(string name) => AddEcsContext(x => new EcsContext(x, new ComponentEntityFactory_ArcheType_Managed()), name);

        public static EcsContext CreateEcsContext_Native(string name) => AddEcsContext(x => new EcsContext(x, new ComponentEntityFactory_Native()), name);

        public static EcsContext CreateEcsContext_ArcheType_Native(string name) => AddEcsContext(x => new EcsContext(x, new ComponentEntityFactory_ArcheType_Native()), name);

        public static EcsContext CreateEcsContext_ArcheType_Native_Continuous(string name) => AddEcsContext(x => new EcsContext(x, new ComponentEntityFactory_ArcheType_Native_Continuous()), name);

        public static void DestroyContext(EcsContext context)
        {
            if (context == null)
                throw new ArgumentNullException();
            if (context.IsDestroyed)
                throw new EcsContextIsDestroyedException(context);

            context.InternalDestroy();

            _ecsContexts.Remove(context.Name);
        }

        private static EcsContext AddEcsContext(Func<string, EcsContext> func, string name)
        {
            if (HasContext(name))
                throw new EcsContextNameAlreadyExistException(name);

            var context = func.Invoke(name);
            _ecsContexts.Add(name, context);
            _ecsContexts.SetDirty();

            return context;
        }

        private static EcsContext[] UpdateContextsCache(Dictionary<string, EcsContext> uncached)
            => uncached.Values.ToArray();
    }
}
