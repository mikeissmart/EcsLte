using EcsLte.Exceptions;
using EcsLte.UnitTest.InterfaceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EcsContextTests
{
    [TestClass]
    public class EcsContext_FilterEntity : BasePrePostTest, IFilterEntityTest
    {
        [TestMethod]
        public void FilterBy()
        {
            var filter1 = _context.FilterBy(Filter.AllOf<TestComponent1>());

            // Created group
            Assert.IsTrue(filter1 != null);
            // Get same group
            var filter2 = _context.FilterBy(Filter.AllOf<TestComponent1>());
            Assert.IsTrue(filter1 == filter2);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.FilterBy(Filter.AllOf<TestComponent1>()));
        }
    }
}