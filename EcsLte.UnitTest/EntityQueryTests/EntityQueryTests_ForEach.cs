using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTests_ForEach : BasePrePostTest
    {
        [TestMethod]
        public void ForEach_R0W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        #region Write 0

        [TestMethod]
        public void ForEach_R1W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R2W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R3W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R4W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R5W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R6W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R7W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        [TestMethod]
        public void ForEach_R8W0()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                },
                false);

            AssertWasHit(wasHit);
        }

        #endregion

        #region Write 1

        [TestMethod]
        public void ForEach_R0W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R1W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R2W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R3W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R4W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R5W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R6W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R7W1()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
        }

        #endregion

        #region Write 2

        [TestMethod]
        public void ForEach_R0W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R1W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R2W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R3W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R4W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R5W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R6W2()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
        }

        #endregion

        #region Write 3

        [TestMethod]
        public void ForEach_R0W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R1W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R2W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R3W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R4W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R5W3()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7,
                in TestSharedComponent4 component8) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
        }

        #endregion

        #region Write 4

        [TestMethod]
        public void ForEach_R0W4()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
        }

        [TestMethod]
        public void ForEach_R1W4()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
        }

        [TestMethod]
        public void ForEach_R2W4()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
        }

        [TestMethod]
        public void ForEach_R3W4()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestSharedComponent3 component7) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
        }

        [TestMethod]
        public void ForEach_R4W4()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
        }

        #endregion

        #region Write 5

        [TestMethod]
        public void ForEach_R0W5()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestSharedComponent1 component5) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R1W5()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
                {
                    wasHit[index]++;
                    component1.Prop++;
                    component2.Prop++;
                    component3.Prop++;
                    component4.Prop++;
                    component5.Prop++;
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R2W5()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
        }

        [TestMethod]
        public void ForEach_R3W5()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
        }

        #endregion

        #region Write 6

        [TestMethod]
        public void ForEach_R0W6()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R1W6()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
        }

        [TestMethod]
        public void ForEach_R2W6()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
        }

        #endregion

        #region Write 7

        [TestMethod]
        public void ForEach_R0W7()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
            AssertSharedComponents<TestSharedComponent3>(entities);
        }

        [TestMethod]
        public void ForEach_R1W7()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
            AssertSharedComponents<TestSharedComponent3>(entities);
        }

        #endregion

        #region Write 8

        [TestMethod]
        public void ForEach_R0W8()
        {
            PrepTest(out var query, out var entities, out var wasHit);

            query.ForEach(
                (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestSharedComponent3 component7,
                ref TestSharedComponent4 component8) =>
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
                },
                false);

            AssertWasHit(wasHit);
            AssertComponents<TestComponent1>(entities);
            AssertComponents<TestComponent2>(entities);
            AssertComponents<TestComponent3>(entities);
            AssertComponents<TestComponent4>(entities);
            AssertSharedComponents<TestSharedComponent1>(entities);
            AssertSharedComponents<TestSharedComponent2>(entities);
            AssertSharedComponents<TestSharedComponent3>(entities);
            AssertSharedComponents<TestSharedComponent4>(entities);
        }

        #endregion

        private void AssertWasHit(int[] wasHit)
        {
            for (var i = 0; i < wasHit.Length; i++)
            {
                Assert.IsTrue(wasHit[i] == 1,
                    $"Invalid hit rate Entity: {i}");
            }
        }

        private void AssertComponents<TComponent>(Entity[] entities)
            where TComponent : unmanaged, IGeneralComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetComponent<TComponent>(entities[i]).Prop == 1,
                    $"Invalid Component.Prop Entity: {entities[i]}, {typeof(TComponent).Name}");
            }
        }

        private void AssertSharedComponents<TComponent>(Entity[] entities)
            where TComponent : unmanaged, ISharedComponent, ITestComponent
        {
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(Context.Entities.GetSharedComponent<TComponent>(entities[i]).Prop == 1,
                    $"Invalid SharedComponent.Prop Entity: {entities[i]}, {typeof(TComponent).Name}");
            }
        }

        private void PrepTest(out EntityQuery query, out Entity[] entities, out int[] wasHit)
        {
            query = new EntityQuery(
                Context,
                new EntityFilter()
                    .WhereAllOf<TestComponent1>()
                    .WhereAllOf<TestComponent2>()
                    .WhereAllOf<TestComponent3>()
                    .WhereAllOf<TestComponent4>()
                    .WhereAllOf<TestSharedComponent1>()
                    .WhereAllOf<TestSharedComponent2>()
                    .WhereAllOf<TestSharedComponent3>()
                    .WhereAllOf<TestSharedComponent4>());
            entities = Context.Entities.CreateEntities(
                new EntityBlueprint()
                    .SetComponent(new TestComponent1 { Prop = 0 })
                    .SetComponent(new TestComponent2 { Prop = 0 })
                    .SetComponent(new TestComponent3 { Prop = 0 })
                    .SetComponent(new TestComponent4 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent1 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent2 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent3 { Prop = 0 })
                    .SetSharedComponent(new TestSharedComponent4 { Prop = 0 }),
                EntityState.Active,
                UnitTestConsts.SmallCount);
            wasHit = new int[entities.Length];
        }
    }
}
