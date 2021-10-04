using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityGroupTests
{
    [TestClass]
    public class EntityGroup_EntityGroup : BasePrePostTest
    {
        [TestMethod]
        public void FilterBy()
        {
            var component = new TestSharedComponent1 { Prop = 1 };
            var group = _context.GroupWith(component);

            // Correct filterGroup
            Assert.IsTrue(group != null);
            Assert.IsTrue(_context.GroupWith(component) == group);
            // Different component gets different entity
            var compoennt2 = new TestSharedComponent1 { Prop = 2 };
            Assert.IsFalse(_context.GroupWith(compoennt2) == group);
            // EcsContext is destroyed
            EcsContexts.DestroyContext(_context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                _context.GroupWith(component));
        }
    }
}