using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest : BasePrePostTest
    {
        [TestMethod]
        public void Equals() =>
            AssertClassEquals(
                new EntityQuery()
                    .WhereAllOf<TestComponent1>(),
                new EntityQuery()
                    .WhereAllOf<TestComponent1>(),
                new EntityQuery()
                    .WhereAllOf<TestComponent2>(),
                null);
    }
}