using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcsLte;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest
{
    public abstract class BasePrePostTest
    {
        public EcsContext Context { get; private set; }

        [TestInitialize]
        public void PreTest()
        {
            Context = EcsContexts.CreateContext("UnitTest");
        }

        [TestCleanup]
        public void PostTest()
        {
            if (!Context.IsDestroyed)
                EcsContexts.DestroyContext(Context);
            Context = null;
        }

        protected Entity[] TestCreateEntities<T1>(EcsContext context, int entityCount,
            T1 component1)
            where T1 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1);
            return context.CreateEntities(entityCount, blueprint);
        }

        protected Entity[] TestCreateEntities<T1, T2>(EcsContext context, int entityCount,
            T1 component1,
            T2 component2)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2);
            return context.CreateEntities(entityCount, blueprint);
        }

        protected Entity[] TestCreateEntities<T1, T2, T3>(EcsContext context, int entityCount,
            T1 component1,
            T2 component2,
            T3 component3)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
        {
            var blueprint = new EntityBlueprint()
                .AddComponent(component1)
                .AddComponent(component2)
                .AddComponent(component3);
            return context.CreateEntities(entityCount, blueprint);
        }
    }
}
