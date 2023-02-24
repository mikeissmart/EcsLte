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
            var context = EcsContexts.Instance.CreateContext("Test");

            Assert.IsTrue(EcsContexts.Instance.HasContext(context.Name));
            Assert.IsFalse(EcsContexts.Instance.HasContext("Missing"));

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.Instance.HasContext(null));
        }

        [TestMethod]
        public void GetAllContexts()
        {
            var preCount = EcsContexts.Instance.GetAllContexts().Length;

            EcsContexts.Instance.CreateContext("Test");

            Assert.IsTrue(EcsContexts.Instance.GetAllContexts().Length == preCount + 1);
        }

        [TestMethod]
        public void GetContext()
        {
            var context = EcsContexts.Instance.CreateContext("Test");

            Assert.IsTrue(EcsContexts.Instance.GetContext(context.Name) == context);

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.Instance.GetContext(null));
        }

        [TestMethod]
        public void CreateContext()
        {
            var context = EcsContexts.Instance.CreateContext("TestCreate");

            Assert.IsTrue(context != null);
            Assert.IsFalse(context.IsDestroyed);

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.Instance.CreateContext(null));

            Assert.ThrowsException<EcsContextAlreadyExistException>(()
                => EcsContexts.Instance.CreateContext("TestCreate"));
        }

        [TestMethod]
        public void DestroyContext()
        {
            var context = EcsContexts.Instance.CreateContext("TestDestroy");
            EcsContexts.Instance.DestroyContext(context);

            Assert.IsTrue(context.IsDestroyed);
            Assert.IsFalse(EcsContexts.Instance.HasContext(context.Name));

            Assert.ThrowsException<ArgumentNullException>(()
                => EcsContexts.Instance.DestroyContext(null));

            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                EcsContexts.Instance.DestroyContext(context));
        }
    }
}