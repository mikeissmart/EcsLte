using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandQueueTests
{
    [TestClass]
    public class EntityCommandQueue_EcsContext : BasePrePostTest, IEcsContextTest
    {
        [TestMethod]
        public void CurrentContext()
        {
            // Correct context
            Assert.IsTrue(_context.DefaultCommand.CurrentContext == _context);
        }
    }
}