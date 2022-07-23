using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityDuplicate : BasePrePostTest
    {
        private readonly EntityState _orgState = EntityState.Active;
        private readonly EntityState _nonNullState = EntityState.Destroying;

        [TestMethod]
        public void DuplicateEntity()
        {
            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entity = Context.Entities.CreateEntity(
                blueprint,
                EntityState.Active);

            var uniqueArcheType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 })
                .AddUniqueComponentType<TestUniqueComponent1>();
            var uniqueEntity = Context.Entities.CreateEntity(
                uniqueArcheType,
                EntityState.Active);

            Assert_DuplicateEntity(entity, uniqueEntity, false);
            Assert_DuplicateEntity(entity, uniqueEntity, true);

            Assert.ThrowsException<EntityNotExistException>(() =>
                Context.Entities.DuplicateEntity(Entity.Null));

            Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                Context.Entities.DuplicateEntity(uniqueEntity));

            EcsContexts.DestroyContext(Context);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.DuplicateEntity(entity));
        }

        [TestMethod]
        public void DuplicateEntities()
        {
            var entities = CreateTestEntities();

            Assert_DuplicateEntities(entities, false);
            Assert_DuplicateEntities(entities, true);

            EcsContexts.DestroyContext(Context);
            Assert_DuplicateEntities_ContextDestroyed(false);
            Assert_DuplicateEntities_ContextDestroyed(true);
        }

        [TestMethod]
        public void DuplicateEntities_ArcheType()
        {
            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_ArcheType(archeType, entities, false);
            Assert_DuplicateEntities_ArcheType(archeType, entities, true);

            EcsContexts.DestroyContext(Context);
            Assert_DuplicateEntities_ArcheType_ContextDestroyed(archeType, false);
            Assert_DuplicateEntities_ArcheType_ContextDestroyed(archeType, true);
        }

        [TestMethod]
        public void DuplicateEntities_Filter()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Filter(filter, entities, false);
            Assert_DuplicateEntities_Filter(filter, entities, true);

            EcsContexts.DestroyContext(Context);
            Assert_DuplicateEntities_Filter_ContextDestroyed(filter, false);
            Assert_DuplicateEntities_Filter_ContextDestroyed(filter, true);
        }

        [TestMethod]
        public void DuplicateEntities_Tracker()
        {
            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Tracker(tracker, entities, false);
            Assert_DuplicateEntities_Tracker(tracker, entities, true);

            EcsContexts.DestroyContext(Context);
            Assert_DuplicateEntities_Tracker_ContextDestroyed(tracker, false);
            Assert_DuplicateEntities_Tracker_ContextDestroyed(tracker, true);
        }

        [TestMethod]
        public void DuplicateEntities_Query()
        {
            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            var entities = CreateTestEntities();

            Assert_DuplicateEntities_Query(queryFilter, queryFilterTracker, entities, false);
            Assert_DuplicateEntities_Query(queryFilter, queryFilterTracker, entities, true);

            EcsContexts.DestroyContext(Context);
            Assert_DuplicateEntities_Query_ContextDestroyed(queryFilter, queryFilterTracker, false);
            Assert_DuplicateEntities_Query_ContextDestroyed(queryFilter, queryFilterTracker, true);
        }

        private void Assert_DuplicateEntity(Entity orgEntity, Entity uniqueEntity, bool isNull)
        {
            Entity dupEntity;
            if (isNull)
            {
                dupEntity = Context.Entities.DuplicateEntity(orgEntity);

                Assert.ThrowsException<EntityNotExistException>(() =>
                    Context.Entities.DuplicateEntity(Entity.Null));
                Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                    Context.Entities.DuplicateEntity(uniqueEntity));
            }
            else
            {
                dupEntity = Context.Entities.DuplicateEntity(orgEntity, _nonNullState);

                Assert.ThrowsException<EntityNotExistException>(() =>
                    Context.Entities.DuplicateEntity(Entity.Null, _nonNullState));
                Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                    Context.Entities.DuplicateEntity(uniqueEntity, _nonNullState));
            }

            Assert.IsTrue(Context.Entities.GetComponent<TestComponent1>(dupEntity).Prop == orgEntity.Id);
            Assert.IsTrue(Context.Entities.GetSharedComponent<TestSharedComponent1>(dupEntity).Prop == 2);
            Assert.IsTrue(Context.Entities.GetEntityState(dupEntity) == (isNull
                ? _orgState
                : _nonNullState));
        }

        private void Assert_DuplicateEntities(Entity[] orgEntities, bool isNull) => AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x => isNull
                    ? Context.Entities.DuplicateEntities(x)
                    : Context.Entities.DuplicateEntities(x, _nonNullState),
                (x, startingIndex) => isNull
                    ? Context.Entities.DuplicateEntities(x, startingIndex)
                    : Context.Entities.DuplicateEntities(x, startingIndex, _nonNullState),
                (x, startingIndex, count) => isNull
                    ? Context.Entities.DuplicateEntities(x, startingIndex, count)
                    : Context.Entities.DuplicateEntities(x, startingIndex, count, _nonNullState),
                (inSrc, x) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(inSrc, ref x)
                        : Context.Entities.DuplicateEntities(inSrc, ref x, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, x) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x)
                        : Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, _nonNullState);
                    return x;
                },
                (inSrc, x, destStartingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex)
                        : Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex)
                        : Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex, _nonNullState);
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
                        result = AssertEntities(inSrc, startingIndex, outRef, destStartingIndex, destCount,
                            isNull
                                ? _orgState
                                : _nonNullState);
                    }
                    return result;
                });

        private void Assert_DuplicateEntities_ContextDestroyed(bool isNull) => AssertGetInRef_ContextDestroyed<Entity, Entity>(
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(x);
                    else
                        Context.Entities.DuplicateEntities(x, EntityState.Active);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(x, startingIndex, EntityState.Active);
                },
                (x, startingIndex, count) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(x, startingIndex, count);
                    else
                        Context.Entities.DuplicateEntities(x, startingIndex, count, EntityState.Active);
                },
                (inSrc, x) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, ref x);
                    else
                        Context.Entities.DuplicateEntities(inSrc, ref x, EntityState.Active);
                },
                (inSrc, startingIndex, x) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, EntityState.Active);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, EntityState.Active);
                },
                (inSrc, x, destStartingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex);
                    else
                        Context.Entities.DuplicateEntities(inSrc, ref x, destStartingIndex, EntityState.Active);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, ref x, destStartingIndex, EntityState.Active);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex);
                    else
                        Context.Entities.DuplicateEntities(inSrc, startingIndex, count, ref x, destStartingIndex, EntityState.Active);
                });

        private void Assert_DuplicateEntities_ArcheType(EntityArcheType archeType, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? Context.Entities.DuplicateEntities(archeType)
                        : Context.Entities.DuplicateEntities(archeType, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(archeType, ref x)
                        : Context.Entities.DuplicateEntities(archeType, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(archeType, ref x, startingIndex)
                        : Context.Entities.DuplicateEntities(archeType, ref x, startingIndex, _nonNullState);
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
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length,
                            isNull
                                ? _orgState
                                : _nonNullState);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            AssertArcheType_Invalid_Null(
                new Action<EntityArcheType>[]
                {
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x);
                        else
                            Context.Entities.DuplicateEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });
        }

        private void Assert_DuplicateEntities_ArcheType_ContextDestroyed(EntityArcheType archeType, bool isNull) => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(archeType);
                    else
                        Context.Entities.DuplicateEntities(archeType, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(archeType, ref x);
                    else
                        Context.Entities.DuplicateEntities(archeType, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(archeType, ref x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(archeType, ref x, startingIndex, _nonNullState);
                });

        private void Assert_DuplicateEntities_Filter(EntityFilter filter, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return isNull
                        ? Context.Entities.DuplicateEntities(filter)
                        : Context.Entities.DuplicateEntities(filter, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(filter, ref x)
                        : Context.Entities.DuplicateEntities(filter, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(filter, ref x, startingIndex)
                        : Context.Entities.DuplicateEntities(filter, ref x, startingIndex, _nonNullState);
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
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length,
                            isNull
                                ? _orgState
                                : _nonNullState);
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
                        if (isNull)
                            Context.Entities.DuplicateEntities(x);
                        else
                            Context.Entities.DuplicateEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });
        }

        private void Assert_DuplicateEntities_Filter_ContextDestroyed(EntityFilter filter, bool isNull) => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(filter);
                    else
                        Context.Entities.DuplicateEntities(filter, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(filter, ref x);
                    else
                        Context.Entities.DuplicateEntities(filter, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(filter, ref x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(filter, ref x, startingIndex, _nonNullState);
                });

        private void Assert_DuplicateEntities_Tracker(EntityTracker tracker, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return isNull
                        ? Context.Entities.DuplicateEntities(tracker)
                        : Context.Entities.DuplicateEntities(tracker, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(tracker, ref x)
                        : Context.Entities.DuplicateEntities(tracker, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(tracker, ref x, startingIndex)
                        : Context.Entities.DuplicateEntities(tracker, ref x, startingIndex, _nonNullState);
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
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length,
                            isNull
                                ? _orgState
                                : _nonNullState);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var diffContext = EcsContexts.CreateContext($"DiffContext {(isNull ? "1" : "2")}")
                .Tracking.CreateTracker("Tracker");
            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            var emptyEntities = new Entity[0];
            AssertTracker_Destroyed_Null(
                diffContext,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x);
                        else
                            Context.Entities.DuplicateEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });
        }

        private void Assert_DuplicateEntities_Tracker_ContextDestroyed(EntityTracker tracker, bool isNull) => AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(tracker);
                    else
                        Context.Entities.DuplicateEntities(tracker, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(tracker, ref x);
                    else
                        Context.Entities.DuplicateEntities(tracker, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(tracker, ref x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(tracker, ref x, startingIndex, _nonNullState);
                });

        private void Assert_DuplicateEntities_Query(EntityQuery queryFilter, EntityQuery queryFilterTracker, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? Context.Entities.DuplicateEntities(queryFilter)
                        : Context.Entities.DuplicateEntities(queryFilter, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(queryFilter, ref x)
                        : Context.Entities.DuplicateEntities(queryFilter, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(queryFilter, ref x, startingIndex)
                        : Context.Entities.DuplicateEntities(queryFilter, ref x, startingIndex, _nonNullState);
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
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length,
                            isNull
                                ? _orgState
                                : _nonNullState);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? Context.Entities.DuplicateEntities(queryFilterTracker)
                        : Context.Entities.DuplicateEntities(queryFilterTracker, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(queryFilterTracker, ref x)
                        : Context.Entities.DuplicateEntities(queryFilterTracker, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? Context.Entities.DuplicateEntities(queryFilterTracker, ref x, startingIndex)
                        : Context.Entities.DuplicateEntities(queryFilterTracker, ref x, startingIndex, _nonNullState);
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
                        result = AssertEntities(orgEntities, 0, x, startingIndex, orgEntities.Length,
                            isNull
                                ? _orgState
                                : _nonNullState);
                        if (result.Success)
                            Context.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var destroyedTracker = new EntityQuery(
                Context.Tracking.CreateTracker("Destroyed"),
                new EntityFilter().WhereAllOf<TestComponent1>());
            Context.Tracking.RemoveTracker(destroyedTracker.Tracker);
            var emptyEntities = new Entity[0];
            AssertQuery_DiffContext_DestroyedTracker_Null(
                new EntityQuery(EcsContexts.CreateContext($"DiffContext {(isNull ? "1" : "2")}"),
                    new EntityFilter().WhereAllOf<TestComponent1>()),
                destroyedTracker,
                new Action<EntityQuery>[]
                {
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x);
                        else
                            Context.Entities.DuplicateEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0);
                        else
                            Context.Entities.DuplicateEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });
        }

        private void Assert_DuplicateEntities_Query_ContextDestroyed(EntityQuery queryFilter, EntityQuery queryFilterTracker, bool isNull)
        {
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilter);
                    else
                        Context.Entities.DuplicateEntities(queryFilter, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilter, ref x);
                    else
                        Context.Entities.DuplicateEntities(queryFilter, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilter, ref x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(queryFilter, ref x, startingIndex, _nonNullState);
                });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilterTracker);
                    else
                        Context.Entities.DuplicateEntities(queryFilterTracker, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilterTracker, ref x);
                    else
                        Context.Entities.DuplicateEntities(queryFilterTracker, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.DuplicateEntities(queryFilterTracker, ref x, startingIndex);
                    else
                        Context.Entities.DuplicateEntities(queryFilterTracker, ref x, startingIndex, _nonNullState);
                });
        }

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            Entity[] dupEntities, int destStartingIndex, int destCount, EntityState state)
        {
            var result = new TestResult();
            for (var i = destStartingIndex; i < destCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var dupEntity = dupEntities[i + destStartingIndex];

                var orgComponent = Context.Entities.GetComponent<TestComponent1>(orgEntity);
                var dupComponent = Context.Entities.GetComponent<TestComponent1>(dupEntity);
                var dupState = Context.Entities.GetEntityState(dupEntity);

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
                else if (Context.Entities.GetEntityState(dupEntity) != state)
                {
                    result.Success = false;
                    result.Error = $"EntityState OrgEntity: {orgEntity}, DupEntity: {dupEntity}";
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
                entities[i] = Context.Entities.CreateEntity(blueprint, EntityState.Active);
            }

            return entities;
        }
    }
}
