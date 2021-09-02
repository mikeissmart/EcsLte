using EcsLte.Exceptions;
using EcsLte.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.WorldTests
{
    [TestClass]
    public class WorldLife
    {
        [TestMethod]
        public void DefaultWorld()
        {
            var world = World.DefaultWorld;

            Assert.IsTrue(world != null && !world.IsDestroyed);
        }

        [TestMethod]
        public void GetWorld()
        {
            Assert.IsTrue(World.GetWorld(World.DefaultWorld.Name) != null);
        }

        [TestMethod]
        public void HasWorld()
        {
            Assert.IsTrue(World.HasWorld(World.DefaultWorld.Name));
        }

        [TestMethod]
        public void Create()
        {
            var world = World.CreateWorld("TestCreate");

            Assert.IsTrue(world != null && !world.IsDestroyed);

            World.DestroyWorld(world);
        }

        [TestMethod]
        public void Create_Parallel()
        {
            World world = null;
            var errorThrown = false;

            ParallelRunner.RunParallelFor(1,
                index =>
                {
                    try
                    {
                        world = World.CreateWorld("TestCreate: " + index);
                    }
                    catch (WorldCreateOffThreadException)
                    {
                        errorThrown = true;
                    }
                });

            Assert.IsTrue(errorThrown);

            if (world != null && !world.IsDestroyed)
                World.DestroyWorld(world);
        }

        [TestMethod]
        public void CreateMultiple()
        {
            var world1 = World.CreateWorld("TestCreateMultiple1");
            var world2 = World.CreateWorld("TestCreateMultiple2");

            Assert.IsTrue(world1 != null && !world1.IsDestroyed);
            Assert.IsTrue(world2 != null && !world2.IsDestroyed);
            Assert.IsTrue(world1 != world2);

            World.DestroyWorld(world1);
            World.DestroyWorld(world2);
        }

        [TestMethod]
        public void Destroy()
        {
            var world = World.CreateWorld("TestDestroy");
            World.DestroyWorld(world);

            Assert.IsTrue(world != null && world.IsDestroyed);
            Assert.IsFalse(World.HasWorld(world.Name));
        }

        [TestMethod]
        public void Destroy_Parallel()
        {
            var world = World.CreateWorld("TestDestroy");
            var errorThrown = false;

            ParallelRunner.RunParallelFor(1,
                index =>
                {
                    try
                    {
                        World.DestroyWorld(world);
                    }
                    catch (WorldDestroyOffThreadException)
                    {
                        errorThrown = true;
                    }
                });

            Assert.IsTrue(errorThrown);

            if (world != null && !world.IsDestroyed)
                World.DestroyWorld(world);
        }

        [TestMethod]
        public void DestroyAfterDestroy()
        {
            var world = World.CreateWorld("TestDestroyAfterDestroy");
            World.DestroyWorld(world);

            Assert.ThrowsException<WorldIsDestroyedException>(() => World.DestroyWorld(world));
        }
    }
}