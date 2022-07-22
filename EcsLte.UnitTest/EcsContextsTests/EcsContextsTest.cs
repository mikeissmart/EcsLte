using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContextsTest : BasePrePostTest
    {
        [TestMethod]
        public void HasContext()
        {
            var context = EcsContexts.CreateContext("Test");

            Assert.IsTrue(EcsContexts.HasContext(context.Name));
            Assert.IsFalse(EcsContexts.HasContext("Missing"));

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.HasContext(null));
        }

        [TestMethod]
        public void GetAllContexts()
        {
            var preCount = EcsContexts.GetAllContexts().Length;

            EcsContexts.CreateContext("Test");

            Assert.IsTrue(EcsContexts.GetAllContexts().Length == preCount + 1);
        }

        [TestMethod]
        public void GetContext()
        {
            var context = EcsContexts.CreateContext("Test");

            Assert.IsTrue(EcsContexts.GetContext(context.Name) == context);

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.GetContext(null));
        }

        [TestMethod]
        public void CreateContext()
        {
            var context = EcsContexts.CreateContext("TestCreate");

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.CreateContext(null));

            Assert.ThrowsException<EcsContextAlreadyExistException>(()
                => EcsContexts.CreateContext("TestCreate"));
        }

        [TestMethod]
        public void DestroyContext()
        {
            var context = EcsContexts.CreateContext("TestDestroy");
            EcsContexts.DestroyContext(context);

            Assert.IsTrue(context.IsDestroyed);
            Assert.IsFalse(EcsContexts.HasContext(context.Name));

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.DestroyContext(null));

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                EcsContexts.DestroyContext(context));
        }
    }
}