using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityKeyTests
{
    [TestClass]
    public class EntityKey_EcsContext : BasePrePostTest, IEcsContextTest
    {
        [TestMethod]
        public void CurrentContext()
        {
            Assert.IsTrue(_context.WithKey(new TestPrimaryKeyComponent1()).CurrentContext == _context);
        }
    }
}