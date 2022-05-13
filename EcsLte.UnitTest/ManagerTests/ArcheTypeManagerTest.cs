using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.ManagerTests
{
    [TestClass]
    public class ArcheTypeManagerTest : BasePrePostTest
    {
        [TestMethod]
        public void CreateEntityArcheType()
        {
            var archeType = Context.ArcheTypeManager.CreateEntityArcheType();

            Assert.IsTrue(archeType != null);
            Assert.IsTrue(archeType.Context == Context);
            Assert.IsTrue(archeType.ComponentConfigs.Length == 0);
            Assert.IsTrue(archeType.SharedComponentDataIndexes.Length == 0);
        }

        [TestMethod]
        public void CreateEntityArcheType_Destroyed()
        {
            EcsContext.DestroyContext(Context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.ArcheTypeManager.CreateEntityArcheType());
        }
    }
}
