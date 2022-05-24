using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContextTest
    {
        [TestMethod]
        public void Default()
        {
            var context = EcsContexts.Default;

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);
        }

        [TestMethod]
        public void GetContext()
            => Assert.IsTrue(EcsContexts.GetContext(EcsContexts.Default.Name) != null);

        [TestMethod]
        public void HasContext()
            => Assert.IsTrue(EcsContexts.HasContext(EcsContexts.Default.Name));

        [TestMethod]
        public void Create()
        {
            var context = EcsContexts.CreateContext("TestCreate");

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);

            EcsContexts.DestroyContext(context);
        }

        [TestMethod]
        public void Create_Duplicate()
        {
            var context = EcsContexts.CreateContext("TestCreate");

            Assert.ThrowsException<EcsContextNameAlreadyExistException>(()
                => EcsContexts.CreateContext("TestCreate"));

            EcsContexts.DestroyContext(context);
        }

        [TestMethod]
        public void Create_null() => Assert.ThrowsException<ArgumentNullException>(()
                                       => EcsContexts.CreateContext(null));

        [TestMethod]
        public void CreateMultiple()
        {
            var context1 = EcsContexts.CreateContext("TestCreateMultiple1");
            var context2 = EcsContexts.CreateContext("TestCreateMultiple2");

            Assert.IsTrue(context1 != null);
            Assert.IsFalse(context1.IsDestroyed);
            Assert.IsTrue(context2 != null);
            Assert.IsFalse(context2.IsDestroyed);
            Assert.IsTrue(context1 != context2);

            EcsContexts.DestroyContext(context1);
            EcsContexts.DestroyContext(context2);
        }

        [TestMethod]
        public void Destroy()
        {
            var context = EcsContexts.CreateContext("TestDestroy");
            EcsContexts.DestroyContext(context);

            Assert.IsTrue(context.IsDestroyed);
            Assert.IsFalse(EcsContexts.HasContext(context.Name));
        }

        [TestMethod]
        public void Destroy_Null() => Assert.ThrowsException<ArgumentNullException>(()
                                        => EcsContexts.DestroyContext(null));

        [TestMethod]
        public void Destroy_Duplicate()
        {
            var context = EcsContexts.CreateContext("TestDestroyAfterDestroy");
            EcsContexts.DestroyContext(context);

            Assert.ThrowsException<EcsContextIsDestroyedException>(()
                => EcsContexts.DestroyContext(context));
        }
    }
}
