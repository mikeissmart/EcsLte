using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest_ForEach : BasePrePostTest
    {
        private TestComponent1 _component1 = new TestComponent1 { Prop = 1 };
        private TestComponent2 _component2 = new TestComponent2 { Prop = 2 };
        private TestComponent3 _component3 = new TestComponent3 { Prop = 3 };
        private TestComponent4 _component4 = new TestComponent4 { Prop = 4 };
        private TestComponent5 _component5 = new TestComponent5 { Prop = 5 };
        private TestComponent6 _component6 = new TestComponent6 { Prop = 6 };
        private TestComponent7 _component7 = new TestComponent7 { Prop = 7 };
        private TestComponent8 _component8 = new TestComponent8 { Prop = 8 };
        private readonly int _refChangeAmount = 10;

        [TestMethod]
        public void ForEach_Duplicate()
        {
            var query = CreateQuery_x8();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.ForEach(false,
                    (int index, Entity entity, in TestComponent1 component1, in TestComponent1 component2) =>
                    {

                    }));
        }

        #region Write 0

        [TestMethod]
        public void ForEach_Read_0_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new int[entities.Length];
            query.ForEach(false, (int index, Entity entity) =>
            {
                isOk[index] += 1;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                Assert.IsTrue(isOk[i] == 1,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_2_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_3_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_4_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_5_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_6_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_7_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        [TestMethod]
        public void ForEach_Read_8_Write_0()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                Assert.IsTrue(component1.Prop == _component1.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });
        }

        #endregion

        #region Write 1

        [TestMethod]
        public void ForEach_Read_0_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1) =>
            {
                component1.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_6_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_7_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;

                Assert.IsTrue(component2.Prop == _component2.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 2

        [TestMethod]
        public void ForEach_Read_0_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_6_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 3

        [TestMethod]
        public void ForEach_Read_0_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;

                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;

                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;

                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;

                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                in TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;

                Assert.IsTrue(component4.Prop == _component4.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 4

        [TestMethod]
        public void ForEach_Read_0_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestComponent5 component5) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;

                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;

                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;

                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                in TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;

                Assert.IsTrue(component5.Prop == _component5.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 5

        [TestMethod]
        public void ForEach_Read_0_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                in TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;

                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;

                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                in TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;

                Assert.IsTrue(component6.Prop == _component6.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 6

        [TestMethod]
        public void ForEach_Read_0_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6,
                in TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;

                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6,
                in TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;

                Assert.IsTrue(component7.Prop == _component7.Prop,
                    $"Enity.Id {entity.Id}");
                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 7

        [TestMethod]
        public void ForEach_Read_0_Write_7()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6,
                ref TestComponent7 component7) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;
                component7.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent7>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_7()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6,
                ref TestComponent7 component7,
                in TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;
                component7.Prop += _refChangeAmount;

                Assert.IsTrue(component8.Prop == _component8.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent7>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        #region Write 8

        [TestMethod]
        public void ForEach_Read_0_Write_8()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(false, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestComponent3 component3,
                ref TestComponent4 component4,
                ref TestComponent5 component5,
                ref TestComponent6 component6,
                ref TestComponent7 component7,
                ref TestComponent8 component8) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
                component5.Prop += _refChangeAmount;
                component6.Prop += _refChangeAmount;
                component7.Prop += _refChangeAmount;
                component8.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent3>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent4>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent5>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent6>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent7>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.EntityManager.GetComponent<TestComponent8>(entity).Prop == _component8.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion

        private Entity[] CreateEntities_x8() => Context.EntityManager.CreateEntities(UnitTestConsts.SmallCount, new EntityBlueprint()
                .AddComponent(_component1)
                .AddComponent(_component2)
                .AddComponent(_component3)
                .AddComponent(_component4)
                .AddComponent(_component5)
                .AddComponent(_component6)
                .AddComponent(_component7)
                .AddComponent(_component8));

        private EntityQuery CreateQuery_x8() => Context.QueryManager.CreateQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestComponent3, TestComponent4>()
                .WhereAllOf<TestComponent5, TestComponent6, TestComponent7, TestComponent8>();
    }
}
