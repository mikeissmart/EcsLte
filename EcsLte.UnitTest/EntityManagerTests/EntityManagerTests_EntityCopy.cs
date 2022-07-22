using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityCopy : BasePrePostTest
    {
        private EntityState _orgState = EntityState.Active;
        private EntityState _nonNullState = EntityState.Destroying;
        private EcsContext _destContext;

        [TestMethod]
        public void CopyEntity()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

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
            var uniqueEntity = Context.Entities.CreateEntity(uniqueArcheType, EntityState.Active);
            _destContext.Entities.CreateEntity(uniqueArcheType, EntityState.Active);

            Assert_CopyEntity(entity, uniqueEntity, false);
            Assert_CopyEntity(entity, uniqueEntity, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntity_ContextDestroyed(entity);
        }

        [TestMethod]
        public void CopyEntities()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var entities = CreateTestEntities();

            Assert_CopyEntities(entities, false);
            Assert_CopyEntities(entities, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntities_ContextDestroyed(false);
            Assert_CopyEntities_ContextDestroyed(true);
        }

        [TestMethod]
        public void CopyEntities_ArcheType()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var archeType = new EntityArcheType()
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_CopyEntities_ArcheType(archeType, entities, false);
            Assert_CopyEntities_ArcheType(archeType, entities, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntities_ArcheType_ContextDestroyed(archeType, false);
            Assert_CopyEntities_ArcheType_ContextDestroyed(archeType, true);
        }

        [TestMethod]
        public void CopyEntities_Filter()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Filter(filter, entities, false);
            Assert_CopyEntities_Filter(filter, entities, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntities_Filter_ContextDestroyed(filter, false);
            Assert_CopyEntities_Filter_ContextDestroyed(filter, true);
        }

        [TestMethod]
        public void CopyEntities_Tracker()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var tracker = Context.Tracking.CreateTracker("Tracker");
            tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            tracker.StartTracking();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Tracker(tracker, entities, false);
            Assert_CopyEntities_Tracker(tracker, entities, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntities_Tracker_ContextDestroyed(tracker, false);
            Assert_CopyEntities_Tracker_ContextDestroyed(tracker, true);
        }

        [TestMethod]
        public void CopyEntities_Query()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var filter = new EntityFilter()
                .WhereAllOf<TestComponent1>();

            var queryFilter = new EntityQuery(Context, filter);

            var queryFilterTracker = new EntityQuery(Context.Tracking.CreateTracker("Tracker1"),
                filter);
            queryFilterTracker.Tracker.SetComponentState<TestComponent1>(EntityTrackerState.Added);
            queryFilterTracker.Tracker.StartTracking();

            var entities = CreateTestEntities();

            Assert_CopyEntities_Query(queryFilter, queryFilterTracker, entities, false);
            Assert_CopyEntities_Query(queryFilter, queryFilterTracker, entities, true);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntities_Query_ContextDestroyed(queryFilter, queryFilterTracker, false);
            Assert_CopyEntities_Query_ContextDestroyed(queryFilter, queryFilterTracker, true);
        }

        private void Assert_CopyEntity(Entity orgEntity, Entity uniqueEntity, bool isNull)
        {
            Entity copyEntity;
            if (isNull)
            {
                copyEntity = _destContext.Entities.CopyEntity(Context.Entities, orgEntity);

                Assert.ThrowsException<EntityNotExistException>(() =>
                    _destContext.Entities.CopyEntity(Context.Entities, Entity.Null));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntity(Context.Entities, orgEntity));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntity(null, orgEntity));
                Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                    _destContext.Entities.CopyEntity(Context.Entities, uniqueEntity));
            }
            else
            {
                copyEntity = _destContext.Entities.CopyEntity(Context.Entities, orgEntity, _nonNullState);

                Assert.ThrowsException<EntityNotExistException>(() =>
                    _destContext.Entities.CopyEntity(Context.Entities, Entity.Null, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntity(Context.Entities, orgEntity, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntity(null, orgEntity, _nonNullState));
                Assert.ThrowsException<EntityUniqueComponentExistsException>(() =>
                    _destContext.Entities.CopyEntity(Context.Entities, uniqueEntity, _nonNullState));
            }

            Assert.IsTrue(_destContext.Entities.GetComponent<TestComponent1>(copyEntity).Prop == orgEntity.Id);
            Assert.IsTrue(_destContext.Entities.GetSharedComponent<TestSharedComponent1>(copyEntity).Prop == 2);
            Assert.IsTrue(_destContext.Entities.GetEntityState(copyEntity) == (isNull
                ? _orgState
                : _nonNullState));
        }

        private void Assert_CopyEntity_ContextDestroyed(Entity entity)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CopyEntity(_destContext.Entities, entity));
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CopyEntity(_destContext.Entities, entity, _nonNullState));
        }

        private void Assert_CopyEntities(Entity[] orgEntities, bool isNull)
        {
            AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x => isNull
                    ? _destContext.Entities.CopyEntities(Context.Entities, x)
                    : _destContext.Entities.CopyEntities(Context.Entities, x, _nonNullState),
                (x, startingIndex) => isNull
                    ? _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex)
                    : _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, _nonNullState),
                (x, startingIndex, count) => isNull
                    ? _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, count)
                    : _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, count, _nonNullState),
                (inSrc, x) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x)
                        : _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, x) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x)
                        : _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, _nonNullState);
                    return x;
                },
                (inSrc, x, destStartingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, destStartingIndex)
                        : _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, destStartingIndex, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex)
                        : _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex, _nonNullState);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex, _nonNullState);
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

            var emptyEntities = new Entity[0];
            EntityManager nullEntities = null;
            if (isNull)
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, ref emptyEntities, 0));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, ref emptyEntities, 0));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1, ref emptyEntities, 0));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, ref emptyEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, ref emptyEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1, ref emptyEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, ref emptyEntities, 0));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, ref emptyEntities, 0));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1, ref emptyEntities, 0));
            }
            else
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, ref emptyEntities, 0, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, ref emptyEntities, 0, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, orgEntities, 0, 1, ref emptyEntities, 0, _nonNullState));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, ref emptyEntities, 0, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, ref emptyEntities, 0, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, orgEntities, 0, 1, ref emptyEntities, 0, _nonNullState));
            }
        }

        private void Assert_CopyEntities_ContextDestroyed(bool isNull)
        {
            AssertGetInRef_ContextDestroyed<Entity, Entity>(
                x =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, x, EntityState.Active);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, EntityState.Active);
                },
                (x, startingIndex, count) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, count);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, x, startingIndex, count, EntityState.Active);
                },
                (inSrc, x) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, EntityState.Active);
                },
                (inSrc, startingIndex, x) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, EntityState.Active);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, EntityState.Active);
                },
                (inSrc, x, destStartingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, destStartingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, ref x, destStartingIndex, EntityState.Active);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex, EntityState.Active);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex, EntityState.Active);
                });
        }

        private void Assert_CopyEntities_ArcheType(EntityArcheType archeType, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, archeType)
                        : _destContext.Entities.CopyEntities(Context.Entities, archeType, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x)
                        : _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, startingIndex)
                        : _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, startingIndex, _nonNullState);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
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
                            _destContext.Entities.CopyEntities(Context.Entities, x);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, 0);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, 0, _nonNullState);
                    }
                });

            EntityManager nullEntities = null;
            if (isNull)
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType, ref emptyEntities, 0));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType, ref emptyEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType, ref emptyEntities, 0));
            }
            else
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, archeType, ref emptyEntities, 0, _nonNullState));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, archeType, ref emptyEntities, 0, _nonNullState));
            }
        }

        private void Assert_CopyEntities_ArcheType_ContextDestroyed(EntityArcheType archeType, bool isNull)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                if (isNull)
                    _destContext.Entities.CopyEntities(Context.Entities, archeType);
                else
                    _destContext.Entities.CopyEntities(Context.Entities, archeType, _nonNullState);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, archeType);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, archeType, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, startingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, archeType, ref x, startingIndex, _nonNullState);
                });
        }

        private void Assert_CopyEntities_Filter(EntityFilter filter, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, filter)
                        : _destContext.Entities.CopyEntities(Context.Entities, filter, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, filter, ref x)
                        : _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, startingIndex)
                        : _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, startingIndex, _nonNullState);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
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
                            _destContext.Entities.CopyEntities(Context.Entities, x);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, 0);
                        else
                            _destContext.Entities.CopyEntities(Context.Entities, x, ref emptyEntities, 0, _nonNullState);
                    }
                });

            EntityManager nullEntities = null;
            if (isNull)
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter, ref emptyEntities, 0));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter, ref emptyEntities));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter, ref emptyEntities, 0));
            }
            else
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(Context.Entities, filter, ref emptyEntities, 0, _nonNullState));

                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<ArgumentNullException>(() =>
                    _destContext.Entities.CopyEntities(nullEntities, filter, ref emptyEntities, 0, _nonNullState));
            }
        }

        private void Assert_CopyEntities_Filter_ContextDestroyed(EntityFilter filter, bool isNull)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                if (isNull)
                    _destContext.Entities.CopyEntities(Context.Entities, filter);
                else
                    _destContext.Entities.CopyEntities(Context.Entities, filter, _nonNullState);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, filter);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, filter, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, filter, ref x);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, startingIndex);
                    else
                        _destContext.Entities.CopyEntities(Context.Entities, filter, ref x, startingIndex, _nonNullState);
                });
        }

        private void Assert_CopyEntities_Tracker(EntityTracker tracker, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return isNull
                        ? _destContext.Entities.CopyEntities(tracker)
                        : _destContext.Entities.CopyEntities(tracker, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(tracker, ref x)
                        : _destContext.Entities.CopyEntities(tracker, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(tracker, ref x, startingIndex)
                        : _destContext.Entities.CopyEntities(tracker, ref x, startingIndex, _nonNullState);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var destroyedTracker = Context.Tracking.CreateTracker("Destroyed");
            Context.Tracking.RemoveTracker(destroyedTracker);
            var emptyEntities = new Entity[0];
            AssertTracker_Destroyed_Null(
                null,
                destroyedTracker,
                new Action<EntityTracker>[]
                {
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x);
                        else
                            _destContext.Entities.CopyEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x, ref emptyEntities);
                        else
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, 0);
                        else
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });

            if (isNull)
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker, ref emptyEntities, 0));
            }
            else
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(tracker, ref emptyEntities, 0, _nonNullState));
            }
        }

        private void Assert_CopyEntities_Tracker_ContextDestroyed(EntityTracker tracker, bool isNull)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                if (isNull)
                    Context.Entities.CopyEntities(tracker);
                else
                    Context.Entities.CopyEntities(tracker, _nonNullState);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        Context.Entities.CopyEntities(tracker);
                    else
                        Context.Entities.CopyEntities(tracker, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        Context.Entities.CopyEntities(tracker, ref x);
                    else
                        Context.Entities.CopyEntities(tracker, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        Context.Entities.CopyEntities(tracker, ref x, startingIndex);
                    else
                        Context.Entities.CopyEntities(tracker, ref x, startingIndex, _nonNullState);
                });
        }

        private void Assert_CopyEntities_Query(EntityQuery queryFilter, EntityQuery queryFilterTracker, Entity[] orgEntities, bool isNull)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? _destContext.Entities.CopyEntities(queryFilter)
                        : _destContext.Entities.CopyEntities(queryFilter, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(queryFilter, ref x)
                        : _destContext.Entities.CopyEntities(queryFilter, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(queryFilter, ref x, startingIndex)
                        : _destContext.Entities.CopyEntities(queryFilter, ref x, startingIndex, _nonNullState);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return isNull
                        ? _destContext.Entities.CopyEntities(queryFilterTracker)
                        : _destContext.Entities.CopyEntities(queryFilterTracker, _nonNullState);
                },
                x =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(queryFilterTracker, ref x)
                        : _destContext.Entities.CopyEntities(queryFilterTracker, ref x, _nonNullState);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = isNull
                        ? _destContext.Entities.CopyEntities(queryFilterTracker, ref x, startingIndex)
                        : _destContext.Entities.CopyEntities(queryFilterTracker, ref x, startingIndex, _nonNullState);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var destroyedTracker = new EntityQuery(
                Context.Tracking.CreateTracker("Destroyed"),
                new EntityFilter().WhereAllOf<TestComponent1>());
            Context.Tracking.RemoveTracker(destroyedTracker.Tracker);
            var emptyEntities = new Entity[0];
            AssertQuery_DiffContext_DestroyedTracker_Null(
                null,
                destroyedTracker,
                new Action<EntityQuery>[]
                {
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x);
                        else
                            _destContext.Entities.CopyEntities(x, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x, ref emptyEntities);
                        else
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, _nonNullState);
                    },
                    x =>
                    {
                        if (isNull)
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, 0);
                        else
                            _destContext.Entities.CopyEntities(x, ref emptyEntities, 0, _nonNullState);
                    }
                });

            if (isNull)
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter, ref emptyEntities));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter, ref emptyEntities, 0));
            }
            else
            {
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter, ref emptyEntities, _nonNullState));
                Assert.ThrowsException<EntityCopySameContextException>(() =>
                    Context.Entities.CopyEntities(queryFilter, ref emptyEntities, 0, _nonNullState));
            }
        }

        private void Assert_CopyEntities_Query_ContextDestroyed(EntityQuery queryFilter, EntityQuery queryFilterTracker, bool isNull)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                if (isNull)
                    _destContext.Entities.CopyEntities(queryFilter);
                else
                    _destContext.Entities.CopyEntities(queryFilter, _nonNullState);
            });
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                if (isNull)
                    _destContext.Entities.CopyEntities(queryFilterTracker);
                else
                    _destContext.Entities.CopyEntities(queryFilterTracker, _nonNullState);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilter);
                    else
                        _destContext.Entities.CopyEntities(queryFilter, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilter, ref x);
                    else
                        _destContext.Entities.CopyEntities(queryFilter, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilter, ref x, startingIndex);
                    else
                        _destContext.Entities.CopyEntities(queryFilter, ref x, startingIndex, _nonNullState);
                });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilterTracker);
                    else
                        _destContext.Entities.CopyEntities(queryFilterTracker, _nonNullState);
                },
                x =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilterTracker, ref x);
                    else
                        _destContext.Entities.CopyEntities(queryFilterTracker, ref x, _nonNullState);
                },
                (x, startingIndex) =>
                {
                    if (isNull)
                        _destContext.Entities.CopyEntities(queryFilterTracker, ref x, startingIndex);
                    else
                        _destContext.Entities.CopyEntities(queryFilterTracker, ref x, startingIndex, _nonNullState);
                });
        }

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            Entity[] copyEntities, int destStartingIndex, int destCount, EntityState state)
        {
            var result = new TestResult();
            for (var i = destStartingIndex; i < destCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var copyEntity = copyEntities[i + destStartingIndex];

                var orgComponent = Context.Entities.GetComponent<TestComponent1>(orgEntity);
                var copyComponent = _destContext.Entities.GetComponent<TestComponent1>(copyEntity);
                var copyState = _destContext.Entities.GetEntityState(copyEntity);

                if (_destContext.Entities.GetComponent<TestComponent1>(copyEntity).Prop != orgEntity.Id)
                {
                    result.Success = false;
                    result.Error = $"GeneralComponent OrgEntity: {orgEntity}, CopyEntity: {copyEntity}";
                    break;
                }
                else if (_destContext.Entities.GetSharedComponent<TestSharedComponent1>(copyEntity).Prop != 2)
                {
                    result.Success = false;
                    result.Error = $"SharedComponent OrgEntity: {orgEntity}, CopyEntity: {copyEntity}";
                    break;
                }
                else if (_destContext.Entities.GetEntityState(copyEntity) != state)
                {
                    result.Success = false;
                    result.Error = $"EntityState OrgEntity: {orgEntity}, CopyEntity: {copyEntity}";
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
