using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContextTest
    {
        [TestMethod]
        public void Default()
        {
            var context = EcsContext.Default;

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);
        }

        [TestMethod]
        public void GetContext()
            => Assert.IsTrue(EcsContext.GetContext(EcsContext.Default.Name) != null);

        [TestMethod]
        public void HasContext()
            => Assert.IsTrue(EcsContext.HasContext(EcsContext.Default.Name));

        [TestMethod]
        public void Create()
        {
            var context = EcsContext.CreateContext("TestCreate");

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);

            EcsContext.DestroyContext(context);
        }

        [TestMethod]
        public void CreateMultiple()
        {
            var context1 = EcsContext.CreateContext("TestCreateMultiple1");
            var context2 = EcsContext.CreateContext("TestCreateMultiple2");

            Assert.IsTrue(context1 != null);
            Assert.IsFalse(context1.IsDestroyed);
            Assert.IsTrue(context2 != null);
            Assert.IsFalse(context2.IsDestroyed);
            Assert.IsTrue(context1 != context2);

            EcsContext.DestroyContext(context1);
            EcsContext.DestroyContext(context2);
        }

        [TestMethod]
        public void Destroy()
        {
            var context = EcsContext.CreateContext("TestDestroy");
            EcsContext.DestroyContext(context);

            Assert.IsTrue(context.IsDestroyed);
            Assert.IsFalse(EcsContext.HasContext(context.Name));
        }

        [TestMethod]
        public void DestroyAfterDestroy()
        {
            var context = EcsContext.CreateContext("TestDestroyAfterDestroy");
            EcsContext.DestroyContext(context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(()
                => EcsContext.DestroyContext(context));
        }
    }
}
