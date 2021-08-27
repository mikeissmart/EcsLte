using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandLife
    {
        [TestInitialize]
        public void PreTest()
        {
            if (!World.DefaultWorld.IsDestroyed)
                World.DestroyWorld(World.DefaultWorld);
            World.DefaultWorld = World.CreateWorld("DefaultWorld");
        }

        [TestMethod]
        public void Create()
        {
            var world = World.DefaultWorld;
            var entityCmd = world.EntityManager.CreateOrGetEntityCommand("Test");

            Assert.IsTrue(entityCmd == world.EntityManager.CreateOrGetEntityCommand("Test"));
        }

        [TestMethod]
        public void CreateAfterWorldDestroy()
        {
            var world = World.DefaultWorld;
            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => world.EntityManager.CreateOrGetEntityCommand("Test"));
        }

        [TestMethod]
        public void Create_Parallel()
        {
            var world = World.DefaultWorld;
            var entityCmd = world.EntityManager.CreateOrGetEntityCommand("Test");
            bool errorThrown = false;

            ParallelRunner.RunParallelFor(1,
                index =>
                {
                    try
                    {
                        world.EntityManager.CreateOrGetEntityCommand("Test");
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