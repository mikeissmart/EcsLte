using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTests_ForEach : BasePrePostTest
    {
        [TestMethod]
        public void ForEach_R0W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity) =>
                {
                    wasHit[index]++;
                })
                .Run();

            AssertWasHit(wasHit);
        }

        #region Write 0

        [TestMethod]
        public void ForEach_R1W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
                Context.Entities.UpdateComponent(entities[i], new TestComponent1 { Prop = entities[i].Id });

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1) =>
                {
                    wasHit[index]++;
                    Assert.IsTrue(component1.Prop == entity.Id,
                        $"Entity: {entity}");
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R2W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R3W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R4W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R5W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R6W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R7W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R8W0()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent1 { Prop = entities[i].Id },
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    AssertReadComponent(entity, component1);
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
        }

        #endregion

        #region Write 1

        [TestMethod]
        public void ForEach_R0W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R1W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponent(entities[i],
                    new TestComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R2W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R3W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R4W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R5W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R6W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R7W1()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestComponent2 { Prop = entities[i].Id },
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    AssertReadComponent(entity, component2);
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
        }

        #endregion

        #region Write 2

        [TestMethod]
        public void ForEach_R0W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R1W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateManagedComponent(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R2W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R3W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R4W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R5W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R6W2()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent1 { Prop = entities[i].Id },
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    AssertReadComponent(entity, component3);
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
        }

        #endregion

        #region Write 3

        [TestMethod]
        public void ForEach_R0W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R1W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateManagedComponent(entities[i],
                    new TestManagedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                in TestManagedComponent2 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    AssertReadComponent(entity, component4);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R2W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R3W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R4W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R5W3()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestManagedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                in TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    AssertReadComponent(entity, component4);
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
        }

        #endregion

        #region Write 4

        [TestMethod]
        public void ForEach_R0W4()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R1W4()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateSharedComponent(entities[i],
                    new TestSharedComponent1 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    AssertReadComponent(entity, component5);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R2W4()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R3W4()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R4W4()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent1 { Prop = entities[i].Id },
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    AssertReadComponent(entity, component5);
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
        }

        #endregion

        #region Write 5

        [TestMethod]
        public void ForEach_R0W5()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R1W5()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateSharedComponent(entities[i],
                    new TestSharedComponent2 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    AssertReadComponent(entity, component6);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R2W5()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R3W5()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent2 { Prop = entities[i].Id },
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    AssertReadComponent(entity, component6);
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
        }

        #endregion

        #region Write 6

        [TestMethod]
        public void ForEach_R0W6()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R1W6()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateSharedComponent(entities[i],
                    new TestSharedComponent3 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                    AssertReadComponent(entity, component7);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R2W6()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateComponents(entities[i],
                    new TestSharedComponent3 { Prop = entities[i].Id },
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                    AssertReadComponent(entity, component7);
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
        }

        #endregion

        #region Write 7

        [TestMethod]
        public void ForEach_R0W7()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                    component7.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R1W7()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            for (var i = 0; i < entities.Length; i++)
            {
                Context.Entities.UpdateSharedComponent(entities[i],
                    new TestSharedComponent4 { Prop = entities[i].Id });
            }

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                    component7.Prop++;
                    AssertReadComponent(entity, component8);
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent3>(entities);
        }

        #endregion

        #region Write 8

        [TestMethod]
        public void ForEach_R0W8()
        {
            PrepTest(out var query, out var tracker, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManagedComponent1 component3,
                ref TestManagedComponent2 component4,
                ref TestSharedComponent2 component6,
                ref TestSharedComponent3 component7,
                ref TestSharedComponent4 component8,
                ref TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                    component6.Prop++;
                    component7.Prop++;
                    component8.Prop++;
                })
                .Run();

            AssertWasHit(wasHit);
            AssertTracked(tracker, entities);
            AssertUpdateComponents<TestComponent1>(entities);
            AssertUpdateComponents<TestComponent2>(entities);
            AssertUpdateManagedComponents<TestManagedComponent1>(entities);
            AssertUpdateManagedComponents<TestManagedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent1>(entities);
            AssertUpdateSharedComponents<TestSharedComponent2>(entities);
            AssertUpdateSharedComponents<TestSharedComponent4>(entities);
        }

        #endregion

        private void PrepTest(out EntityQuery query, out EntityTracker tracker, out Entity[] entities, out int[] hitCount)
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddComponentType<TestComponent2>()
                .AddManagedComponentType<TestManagedComponent1>()
                .AddManagedComponentType<TestManagedComponent2>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 0 })
                .AddSharedComponent(new TestSharedComponent2 { Prop = 0 })
                .AddSharedComponent(new TestSharedComponent3 { Prop = 0 })
                .AddSharedComponent(new TestSharedComponent4 { Prop = 0 });
            query = Context.Queries
                .SetFilter(Context.Filters.WhereAnyOf(archeType));
            tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Updated)
                .SetTrackingState<TestComponent2>(TrackingState.Updated)
                .SetTrackingState<TestManagedComponent1>(TrackingState.Updated)
                .SetTrackingState<TestManagedComponent2>(TrackingState.Updated)
                .SetTrackingState<TestSharedComponent1>(TrackingState.Updated)
                .SetTrackingState<TestSharedComponent2>(TrackingState.Updated)
                .SetTrackingState<TestSharedComponent3>(TrackingState.Updated)
                .SetTrackingState<TestSharedComponent4>(TrackingState.Updated)
                .StartTracking();
            entities = Context.Entities
                .CreateEntities(archeType, UnitTestConsts.SmallCount);
            hitCount = new int[UnitTestConsts.SmallCount];
        }

        private void AssertWasHit(int[] wasHit)
        {
            for (var i = 0; i < wasHit.Length; i++)
            {
                Assert.IsTrue(wasHit[i] == 1,
                    $"Invalid hit rate Entity: {i}");
            }
        }

        private void AssertReadComponent(Entity entity, ITestComponent component) => Assert.IsTrue(component.Prop == entity.Id,
                $"Entity: {entity}");

        private void AssertTracked(EntityTracker tracker, Entity[] entities)
        {
            var trackedEntities = Context.Entities.GetEntities(tracker);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(trackedEntities.Any(x => x == entities[i]),
                    $"Not Tracked Entity: {entities[i]}");
            }
        }

        private void AssertUpdateComponents<TComponent>(Entity[] entities)
            where TComponent : unmanaged, IGeneralComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetComponent<TComponent>(entities[i]).Prop == 1,
                    $"Invalid Component.Prop Entity: {entities[i]}, {typeof(TComponent).Name}");
            }
        }

        private void AssertUpdateManagedComponents<TComponent>(Entity[] entities)
            where TComponent : IManagedComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetManagedComponent<TComponent>(entities[i]).Prop == 1,
                    $"Invalid Component.Prop Entity: {entities[i]}, {typeof(TComponent).Name}");
            }
        }

        private void AssertUpdateSharedComponents<TComponent>(Entity[] entities)
            where TComponent : unmanaged, ISharedComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetSharedComponent<TComponent>(entities[i]).Prop == 1,
                    $"Invalid SharedComponent.Prop Entity: {entities[i]}, {typeof(TComponent).Name}");
            }
        }
    }
}
