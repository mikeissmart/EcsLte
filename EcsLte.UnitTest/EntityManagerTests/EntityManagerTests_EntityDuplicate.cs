using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityDuplicate : BasePrePostTest
    {
        [TestMethod]
        public void DuplicateEntity()
        {
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entity = Context.Entities.CreateEntity(
                blueprint);

            Assert_DuplicateEntity(entity);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.DuplicateEntity(Entity.Null));

            EcsContexts.Instance.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DuplicateEntity(entity));
        }

        [TestMethod]
        public void DuplicateEntities()
        {
            var entities = CreateTestEntities();

            Assert_DuplicateEntities(entities);

            EcsContexts.Instance.DestroyContext(Context);
            Assert_DuplicateEntities_ContextDestroyed();
        }

        [TestMethod]
        public void DuplicateEntities_ArcheType()
        {
            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_ArcheType(archeType, entities);

            EcsContexts.Instance.DestroyContext(Context);
            Assert_DuplicateEntities_ArcheType_ContextDestroyed(archeType);
        }

        [TestMethod]
        public void DuplicateEntities_Filter()
        {
            var filter = Context.Filters
                    .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Filter(filter, entities);

            EcsContexts.Instance.DestroyContext(Context);
            Assert_DuplicateEntities_Filter_ContextDestroyed(filter);
        }

        [TestMethod]
        public void DuplicateEntities_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker1")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Tracker(tracker, entities);

            EcsContexts.Instance.DestroyContext(Context);
            Assert_DuplicateEntities_Tracker_ContextDestroyed(tracker);
        }

        [TestMethod]
        public void DuplicateEntities_Query()
        {
            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var query = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking.CreateTracker("Tracker1")
                    .SetTrackingState<TestComponent1>(TrackingState.Added)
                    .StartTracking());

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Query(query, entities);

            EcsContexts.Instance.DestroyContext(Context);
            Assert_DuplicateEntities_Query_ContextDestroyed(query);
        }

        private void Assert_DuplicateEntity(Entity orgEntity)
        {
            var dupEntity = Context.Entities.DuplicateEntity(orgEntity);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.DuplicateEntity(Entity.Null));

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(dupEntity).Prop == orgEntity.Id);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(dupEntity).Prop == 2);
        }

        private void Assert_DuplicateEntities(Entity[] orgEntities)
            => AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x => Context.Entities.DuplicateEntities(x),
                (x, startingIndex) => Context.Entities.DuplicateEntities(x, startingIndex),
                (x, startingIndex, count) => Context.Entities.DuplicateEntities(x, startingIndex, count),
                (inSrc, x) =>
                {
                    var count = Context.Entities.DuplicateEntities(inSrc, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, x) =>
                {
                    var count = Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x);
                    return x;
                },
                (inSrc, x, destStartingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex);
                    return x;
                },
                (inSrc, startingIndex, outRef, destStartingIndex, destCount) =>
                {
                    var result = new TestResult();
                    if (outRef.Length - destStartingIndex != destCount)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {outRef.Length}, Dest Count: {destCount}";
                    }
                    else
                    {
                        result = AssertEntities(inSrc, startingIndex, outRef, destStartingIndex, destCount);
                    }
                    return result;
                });

        private void Assert_DuplicateEntities_ContextDestroyed()
            => AssertGetInRef_ContextDestroyed<Entity, Entity>(
                x =>
                {
                    Context.Entities.DuplicateEntities(x);
                },
                (x, startingIndex) =>
                {
                    Context.Entities.DuplicateEntities(x, startingIndex);
                },
                (x, startingIndex, count) =>
                {
                    Context.Entities.DuplicateEntities(x, startingIndex, count);
                },
                (inSrc, x) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, ref x);
                },
                (inSrc, startingIndex, x) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x);
                },
                (inSrc, x, destStartingIndex) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex);
                });

        private void Assert_DuplicateEntities_ArcheType(EntityArcheType archeType, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return Context.Entities.DuplicateEntities(archeType);
                },
                x =>
                {
                    var count = Context.Entities.DuplicateEntities(archeType, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(archeType, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            AssertArcheType_DiffContext_Null(
                new Action<EntityArcheType>[]
                {
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                    }
                });
        }

        private void Assert_DuplicateEntities_ArcheType_ContextDestroyed(EntityArcheType archeType)
            => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    Context.Entities.DuplicateEntities(archeType);
                },
                x =>
                {
                    Context.Entities.DuplicateEntities(archeType, ref x);
                },
                (x, startingIndex) =>
                {
                    Context.Entities.DuplicateEntities(archeType, ref x, startingIndex);
                });

        private void Assert_DuplicateEntities_Filter(EntityFilter filter, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return Context.Entities.DuplicateEntities(filter);
                },
                x =>
                {
                    var count = Context.Entities.DuplicateEntities(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(filter, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            AssertFilter_Null(
                new Action<EntityFilter>[]
                {
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                    }
                });
        }

        private void Assert_DuplicateEntities_Filter_ContextDestroyed(EntityFilter filter)
            => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    Context.Entities.DuplicateEntities(filter);
                },
                x =>
                {
                    Context.Entities.DuplicateEntities(filter, ref x);
                },
                (x, startingIndex) =>
                {
                    Context.Entities.DuplicateEntities(filter, ref x, startingIndex);
                });

        private void Assert_DuplicateEntities_Tracker(EntityTracker tracker, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return Context.Entities.DuplicateEntities(tracker);
                },
                x =>
                {
                    var count = Context.Entities.DuplicateEntities(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(tracker, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            AssertTracker_Destroyed_Null(
                EcsContexts.Instance.CreateContext("DiffContext")
                    .Tracking.CreateTracker("Tracker"),
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                    }
                });
        }

        private void Assert_DuplicateEntities_Tracker_ContextDestroyed(EntityTracker tracker)
            => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    Context.Entities.DuplicateEntities(tracker);
                },
                x =>
                {
                    Context.Entities.DuplicateEntities(tracker, ref x);
                },
                (x, startingIndex) =>
                {
                    Context.Entities.DuplicateEntities(tracker, ref x, startingIndex);
                });

        private void Assert_DuplicateEntities_Query(EntityQuery query, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return Context.Entities.DuplicateEntities(query);
                },
                x =>
                {
                    var count = Context.Entities.DuplicateEntities(query, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = Context.Entities.DuplicateEntities(query, ref x, startingIndex);
                    return (x, count);
                },
                (x, startingIndex, count) =>
                {
                    var result = new TestResult();
                    if (x.Length != startingIndex + count)
                    {
                        result.Success = false;
                        result.Error = $"Ref Length: {x.Length}, StartingIndex: {startingIndex}, Count: {count}";
                    }
                    else
                    {
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            Context.Tracking.RemoveTracker(query.Tracker);
            AssertQuery_DiffContext_DestroyedTracker_Null(
                query,
                new Action<EntityQuery>[]
                {
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities);
                    },
                    x =>
                    {
                        Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                    }
                });
        }

        private void Assert_DuplicateEntities_Query_ContextDestroyed(EntityQuery query) => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    Context.Entities.DuplicateEntities(query);
                },
                x =>
                {
                    Context.Entities.DuplicateEntities(query, ref x);
                },
                (x, startingIndex) =>
                {
                    Context.Entities.DuplicateEntities(query, ref x, startingIndex);
                });

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            Entity[] dupEntities, int destStartingIndex, int destCount)
        {
            var result = new TestResult();
            for (var i = destStartingIndex; i < destCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var dupEntity = dupEntities[i + destStartingIndex];

                var orgComponent = Context.Entities.GetComponent<TestComponent1>(orgEntity);
                var dupComponent = Context.Entities.GetComponent<TestComponent1>(dupEntity);

                if (Context.Entities.GetComponent<TestComponent1>(dupEntity).Prop != orgEntity.Id)
                {
                    result.Success = false;
                    result.Error = $"GeneralComponent OrgEntity: {orgEntity}, DupEntity: {dupEntity}";
                    break;
                }
                else if (Context.Entities.GetSharedComponent<TestSharedComponent1>(dupEntity).Prop != 2)
                {
                    result.Success = false;
                    result.Error = $"SharedComponent OrgEntity: {orgEntity}, DupEntity: {dupEntity}";
                    break;
                }
            }
            return result;
        }

        private Entity[] CreateTestEntities()
        {
            var entities = new Entity[UnitTestConsts.SmallCount];
            var blueprint = new EntityBlueprint()
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            for (var i = 0; i < UnitTestConsts.SmallCount; i++)
            {
                blueprint.SetComponent(
                    new TestComponent1 { Prop = Context.Entities.EntityCount() + 1 });
                entities[i] = Context.Entities.CreateEntity(blueprint);
            }

            return entities;
        }
    }
}
