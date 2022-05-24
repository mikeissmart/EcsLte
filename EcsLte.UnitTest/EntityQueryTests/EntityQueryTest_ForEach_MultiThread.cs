using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcsLte.UnitTest.EntityQueryTests
{
    [TestClass]
    public class EntityQueryTest_ForEach_MultiThread : BasePrePostTest
    {
        private TestComponent1 _component1 = new TestComponent1 { Prop = 1 };
        private TestComponent2 _component2 = new TestComponent2 { Prop = 2 };
        private TestManageComponent1 _component3 = new TestManageComponent1 { Prop = 3 };
        private TestManageComponent2 _component4 = new TestManageComponent2 { Prop = 4 };
        private TestSharedComponent1 _component5 = new TestSharedComponent1 { Prop = 5 };
        private TestSharedComponent2 _component6 = new TestSharedComponent2 { Prop = 6 };
        private TestManageSharedComponent1 _component7 = new TestManageSharedComponent1 { Prop = 7 };
        private TestManageSharedComponent2 _component8 = new TestManageSharedComponent2 { Prop = 8 };
        private readonly int _refChangeAmount = 10;

        [TestMethod]
        public void ForEach_Duplicate()
        {
            var query = CreateQuery_x8();

            Assert.ThrowsException<EntityQueryDuplicateComponentException>(() =>
                query.ForEach(Context, true,
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
            query.ForEach(Context, true, (int index, Entity entity) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
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

            query.ForEach(Context, true, (int index, Entity entity,
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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

            query.ForEach(Context, true, (int index, Entity entity,
                in TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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

        #endregion Write 0

        #region Write 1

        [TestMethod]
        public void ForEach_Read_0_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1) =>
            {
                component1.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_6_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_7_Write_1()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                in TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 1

        #region Write 2

        [TestMethod]
        public void ForEach_Read_0_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;

                Assert.IsTrue(component3.Prop == _component3.Prop,
                    $"Enity.Id {entity.Id}");
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_6_Write_2()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                in TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 2

        #region Write 3

        [TestMethod]
        public void ForEach_Read_0_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                in TestManageComponent2 component4) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_5_Write_3()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                in TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 3

        #region Write 4

        [TestMethod]
        public void ForEach_Read_0_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4) =>
            {
                component1.Prop += _refChangeAmount;
                component2.Prop += _refChangeAmount;
                component3.Prop += _refChangeAmount;
                component4.Prop += _refChangeAmount;
            });

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                in TestSharedComponent1 component5) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_4_Write_4()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                in TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 4

        #region Write 5

        [TestMethod]
        public void ForEach_Read_0_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_3_Write_5()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                in TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 5

        #region Write 6

        [TestMethod]
        public void ForEach_Read_0_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_2_Write_6()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                in TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 6

        #region Write 7

        [TestMethod]
        public void ForEach_Read_0_Write_7()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestManageSharedComponent1 component7) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageSharedComponent1>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        [TestMethod]
        public void ForEach_Read_1_Write_7()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            var isOk = new bool[entities.Length];
            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestManageSharedComponent1 component7,
                in TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageSharedComponent1>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 7

        #region Write 8

        [TestMethod]
        public void ForEach_Read_0_Write_8()
        {
            var entities = CreateEntities_x8();
            var query = CreateQuery_x8();

            query.ForEach(Context, true, (int index, Entity entity,
                ref TestComponent1 component1,
                ref TestComponent2 component2,
                ref TestManageComponent1 component3,
                ref TestManageComponent2 component4,
                ref TestSharedComponent1 component5,
                ref TestSharedComponent2 component6,
                ref TestManageSharedComponent1 component7,
                ref TestManageSharedComponent2 component8) =>
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
                Assert.IsTrue(Context.GetComponent<TestComponent1>(entity).Prop == _component1.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestComponent2>(entity).Prop == _component2.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent1>(entity).Prop == _component3.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageComponent2>(entity).Prop == _component4.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent1>(entity).Prop == _component5.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestSharedComponent2>(entity).Prop == _component6.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageSharedComponent1>(entity).Prop == _component7.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
                Assert.IsTrue(Context.GetComponent<TestManageSharedComponent2>(entity).Prop == _component8.Prop + _refChangeAmount,
                    $"Enity.Id {entities[i].Id}");
            }
        }

        #endregion Write 8

        private Entity[] CreateEntities_x8() => Context.CreateEntities(UnitTestConsts.SmallCount, new EntityBlueprint()
                .AddComponent(_component1)
                .AddComponent(_component2)
                .AddComponent(_component3)
                .AddComponent(_component4)
                .AddComponent(_component5)
                .AddComponent(_component6)
                .AddComponent(_component7)
                .AddComponent(_component8));

        private EntityQuery CreateQuery_x8() => new EntityQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestManageComponent1, TestManageComponent2>()
                .WhereAllOf<TestSharedComponent1, TestSharedComponent2, TestManageSharedComponent1, TestManageSharedComponent2>();
    }
}