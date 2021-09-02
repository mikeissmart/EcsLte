using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandLife : BasePrePostTest
    {
        [TestMethod]
        public void Create()
        {
            var entityCmd = _world.EntityManager.CreateOrGetEntityCommand("Test");

            Assert.IsTrue(entityCmd == _world.EntityManager.CreateOrGetEntityCommand("Test"));
        }

        [TestMethod]
        public void CreateAfterWorldDestroy()
        {
            World.DestroyWorld(_world);

            Assert.ThrowsException<WorldIsDestroyedException>(() =>
                _world.EntityManager.CreateOrGetEntityCommand("Test"));
        }

        [TestMethod]
        public void Create_Parallel()
        {
            var entityCmd = _world.EntityManager.CreateOrGetEntityCommand("Test");
            var errorThrown = false;

            ParallelRunner.RunParallelFor(1,
                index =>
                {
                    try
                    {
                        _world.EntityManager.CreateOrGetEntityCommand("Test");
                    }
                    catch (EntityCommandPlaybackOffThreadException)
                    {
                        errorThrown = true;
                    }
                });

            Assert.IsTrue(errorThrown);
        }
    }
}