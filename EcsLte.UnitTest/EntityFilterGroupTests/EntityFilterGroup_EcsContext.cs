using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityFilterGroupTests
{
    [TestClass]
    public class EntityFilterGroup_EcsContext : BasePrePostTest, IEcsContextTest
    {
        [TestMethod]
        public void CurrentContext()
        {
            // Correct context
            Assert.IsTrue(_context.FilterByGroupWith(
                Filter.AllOf<TestSharedComponent1>(), new TestSharedComponent1 { Prop = 1 })
                .CurrentContext == _context);
        }
    }
}