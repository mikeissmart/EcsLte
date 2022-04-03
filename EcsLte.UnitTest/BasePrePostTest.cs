using EcsLte.Managed;
using EcsLte.ManagedArcheType;
using EcsLte.Native;
using EcsLte.NativeArcheType;
using EcsLte.NativeArcheTypeContinous;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest
{
    public abstract class BasePrePostTest<TEcsContextType> where TEcsContextType : IEcsContextType
    {
        public EcsContext Context { get; private set; }

        [TestInitialize]
        public void PreTest()
        {
            if (typeof(TEcsContextType) == typeof(EcsContextType_Managed))
                Context = EcsContexts.CreateEcsContext_Managed("Managed_Test");
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Managed_ArcheType))
                Context = EcsContexts.CreateEcsContext_ArcheType_Managed("ArcheType_Managed_Test");
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native))
                Context = EcsContexts.CreateEcsContext_Native("Native_Test");
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native_ArcheType))
                Context = EcsContexts.CreateEcsContext_ArcheType_Native("ArcheType_Native_Test");
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native_ArcheType_Continuous))
                Context = EcsContexts.CreateEcsContext_ArcheType_Native_Continuous("ArcheType_Native_Continuous_Test_Continuous");
            else
                throw new InvalidOperationException();
        }

        [TestCleanup]
        public void PostTest()
        {
            if (!Context.IsDestroyed)
                EcsContexts.DestroyContext(Context);
            Context = null;
        }

        protected IEntityBlueprint GetBlueprint()
        {
            if (typeof(TEcsContextType) == typeof(EcsContextType_Managed))
                return new EntityBlueprint_Managed();
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Managed_ArcheType))
                return new EntityBlueprint_ArcheType_Managed();
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native))
                return new EntityBlueprint_Native();
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native_ArcheType))
                return new EntityBlueprint_ArcheType_Native();
            else if (typeof(TEcsContextType) == typeof(EcsContextType_Native_ArcheType_Continuous))
                return new EntityBlueprint_ArcheType_Native_Continuous();
            else
                throw new InvalidOperationException();
        }
    }
}
