using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityCommandPlayback
{
    [TestClass]
    public class EntityCommandEntityComponent : BasePrePostTest
    {
        [TestMethod]
        public void AddComponent()
        {
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.DefaultEntityCommandPlayback.AddComponent(entity, new TestComponent1());
            Assert.IsFalse(_world.EntityManager.HasComponent<TestComponent1>(entity));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(_world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void RemoveComponent()
        {
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.AddComponent(entity, new TestComponent1());
            _world.EntityManager.DefaultEntityCommandPlayback.RemoveComponent<TestComponent1>(entity);
            Assert.IsTrue(_world.EntityManager.HasComponent<TestComponent1>(entity));

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsFalse(_world.EntityManager.HasComponent<TestComponent1>(entity));
        }

        [TestMethod]
        public void ReplaceComponent()
        {
            var entity = _world.EntityManager.CreateEntity();

            _world.EntityManager.AddComponent(entity, new TestComponent1());
            _world.EntityManager.DefaultEntityCommandPlayback.ReplaceComponent(entity, new TestComponent1 {Prop = 1});
            Assert.IsTrue(_world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 0);

            _world.EntityManager.DefaultEntityCommandPlayback.RunCommands();
            Assert.IsTrue(_world.EntityManager.GetComponent<TestComponent1>(entity).Prop == 1);
        }
    }
}