using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest
{
    public class BasePrePostTest
    {
        protected World _destroyedWorld;
        protected World _world;

        [TestInitialize]
        public void PreTest()
        {
            _world = World.CreateWorld($"TestWorld {World.Worlds.Length}");
            _destroyedWorld = World.CreateWorld($"DestroyedTestWorld {World.Worlds.Length}");
            World.DestroyWorld(_destroyedWorld);
        }

        [TestCleanup]
        public void PostTest()
        {
            if (!_world.IsDestroyed)
                World.DestroyWorld(_world);
        }
    }
}