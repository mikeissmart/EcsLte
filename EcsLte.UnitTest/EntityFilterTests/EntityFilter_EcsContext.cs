using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterTests
{
    [TestClass]
    public class EntityFilter_EcsContext : BasePrePostTest, IEcsContextTest
    {
        [TestMethod]
        public void CurrentContext()
        {
            // Correct context
            Assert.IsTrue(_context.FilterBy(Filter.AllOf<TestComponent1>()).CurrentContext == _context);
        }
    }
}