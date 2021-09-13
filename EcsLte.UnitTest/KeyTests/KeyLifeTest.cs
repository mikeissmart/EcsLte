using System;
using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.KeyTests
{
    [TestClass]
    public class KeyLifeTest : BasePrePostTest
    {
        [TestMethod]
        public void GetPrimaryKey()
        {
            var key = _world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>();

            // Not null
            Assert.IsTrue(key != null);
            // Get same key
            Assert.IsTrue(_world.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>() == key);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.KeyManager.GetPrimaryKey<TestPrimaryKeyComponent1>());
        }

        [TestMethod]
        public void GetSharedKey()
        {
            var key = _world.KeyManager.GetSharedKey<TestSharedKeyComponent1>();

            // Not null
            Assert.IsTrue(key != null);
            // Get same key
            Assert.IsTrue(_world.KeyManager.GetSharedKey<TestSharedKeyComponent1>() == key);
            // World is destroyed
            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _destroyedWorld.KeyManager.GetSharedKey<TestSharedKeyComponent1>());
        }
    }
}