using System;
using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityGroupTests
{
    [TestClass]
    public class EntityGroup_EcsContext : BasePrePostTest, IEcsContextTest
    {
        [TestMethod]
        public void CurrentContext()
        {
            Assert.IsTrue(_context.GroupWith(new TestPrimaryKeyComponent1()).CurrentContext == _context);
        }
    }
}