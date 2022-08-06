using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest.EntityManagerTests
{
    [TestClass]
    public class EntityManagerTests_EntityCopy : BasePrePostTest
    {
        private EcsContext _destContext;

        [TestMethod]
        public void CopyEntityTo()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var blueprint = new EntityBlueprint()
                .SetComponent(new TestComponent1 { Prop = 1 })
                .SetSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var orgEntity = Context.Entities.CreateEntity(
                blueprint);

            var copyEntity = _destContext.Entities.CopyEntityTo(Context.Entities, orgEntity);

            Assert.ThrowsException<EntityNotExistException>(() =>
                _destContext.Entities.CopyEntityTo(Context.Entities, Entity.Null));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntityTo(Context.Entities, orgEntity));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntityTo(null, orgEntity));

            Assert.IsTrue(_destContext.Entities.GetComponent<TestComponent1>(copyEntity).Prop == orgEntity.Id);
            Assert.IsTrue(_destContext.Entities.GetSharedComponent<TestSharedComponent1>(copyEntity).Prop == 2);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CopyEntityTo(_destContext.Entities, orgEntity));
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
                Context.Entities.CopyEntityTo(_destContext.Entities, orgEntity));
        }

        [TestMethod]
        public void CopyEntitiesTo()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var entities = CreateTestEntities();

            Assert_CopyEntitiesTo(entities);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntitiesTo_ContextDestroyed();
        }

        [TestMethod]
        public void CopyEntitiesTo_ArcheType()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var archeType = Context.ArcheTypes
                .AddComponentType<TestComponent1>()
                .AddSharedComponent(new TestSharedComponent1 { Prop = 2 });

            var entities = CreateTestEntities();

            Assert_CopyEntitiesTo_ArcheType(archeType, entities);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntitiesTo_ArcheType_ContextDestroyed(archeType);
        }

        [TestMethod]
        public void CopyEntitiesTo_Filter()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var entities = CreateTestEntities();

            Assert_CopyEntitiesTo_Filter(filter, entities);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            Assert_CopyEntitiesTo_Filter_ContextDestroyed(filter);
        }

        [TestMethod]
        public void CopyEntitiesTo_Tracker()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var tracker = Context.Tracking.CreateTracker("Tracker")
                .SetTrackingState<TestComponent1>(TrackingState.Added)
                .StartTracking();

            var entities = CreateTestEntities();

            Assert_CopyEntitiesTo_Tracker(tracker, entities);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            var emptyEntities = new Entity[0];
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(tracker), "Context Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(tracker, ref emptyEntities), "Ref Context Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(tracker, ref emptyEntities, 0),
                "Ref StartingIndex Context Destroyed");
        }

        [TestMethod]
        public void CopyEntitiesTo_Query()
        {
            _destContext = EcsContexts.CreateContext("DestContext");

            var filter = Context.Filters
                .WhereAllOf<TestComponent1>();

            var queryFilterTracker = Context.Queries
                .SetFilter(filter)
                .SetTracker(Context.Tracking.CreateTracker("Tracker1")
                    .SetTrackingState<TestComponent1>(TrackingState.Added)
                    .StartTracking());

            var entities = CreateTestEntities();

            Assert_CopyEntitiesTo_Query(queryFilterTracker, entities);

            EcsContexts.DestroyContext(Context);
            EcsContexts.DestroyContext(_destContext);
            var emptyEntities = new Entity[0];
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(queryFilterTracker), "Context Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(queryFilterTracker, ref emptyEntities), "Ref Context Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(queryFilterTracker, ref emptyEntities, 0),
                "Ref StartingIndex Context Destroyed");
        }

        private void Assert_CopyEntitiesTo(Entity[] orgEntities)
        {
            AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange(
                () => orgEntities,
                () => new[] { Entity.Null, Entity.Null, Entity.Null, Entity.Null },
                x => _destContext.Entities.CopyEntitiesTo(Context.Entities, x),
                (x, startingIndex) =>
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, x, startingIndex),
                (x, startingIndex, count) =>
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, x, startingIndex, count),
                (inSrc, x) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, x) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, ref x);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, count, ref x);
                    return x;
                },
                (inSrc, x, destStartingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex);
                    return (x, count);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex);
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

            var emptyEntities = new Entity[0];
            EntityManager nullEntities = null;

            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0, 1));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, ref emptyEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0, ref emptyEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0, 1, ref emptyEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, ref emptyEntities, 0));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0, ref emptyEntities, 0));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(Context.Entities, orgEntities, 0, 1, ref emptyEntities, 0));

            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0, 1));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, ref emptyEntities));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0, ref emptyEntities));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0, 1, ref emptyEntities));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, ref emptyEntities, 0));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0, ref emptyEntities, 0));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullEntities, orgEntities, 0, 1, ref emptyEntities, 0));
        }

        private void Assert_CopyEntitiesTo_ContextDestroyed()
            => AssertGetInRef_ContextDestroyed<Entity, Entity>(
                x =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, x);
                },
                (x, startingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, x, startingIndex);
                },
                (x, startingIndex, count) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, x, startingIndex, count);
                },
                (inSrc, x) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, ref x);
                },
                (inSrc, startingIndex, x) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, ref x);
                },
                (inSrc, startingIndex, count, x) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, count, ref x);
                },
                (inSrc, x, destStartingIndex) =>
                {
                     _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, ref x, destStartingIndex);
                },
                (inSrc, startingIndex, x, destStartingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, ref x, destStartingIndex);
                },
                (inSrc, startingIndex, count, x, destStartingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(Context.Entities, inSrc, startingIndex, count, ref x, destStartingIndex);
                });

        private void Assert_CopyEntitiesTo_ArcheType(EntityArcheType archeType, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return _destContext.Entities.CopyEntitiesTo(archeType);
                },
                x =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(archeType, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(archeType, ref x, startingIndex);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            EntityArcheType nullable = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable, ref emptyEntities));
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable, ref emptyEntities, 0));
        }

        private void Assert_CopyEntitiesTo_ArcheType_ContextDestroyed(EntityArcheType archeType)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                _destContext.Entities.CopyEntitiesTo(archeType);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    _destContext.Entities.CopyEntitiesTo(archeType);
                },
                x =>
                {
                    _destContext.Entities.CopyEntitiesTo(archeType, ref x);
                },
                (x, startingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(archeType, ref x, startingIndex);
                });
        }

        private void Assert_CopyEntitiesTo_Filter(EntityFilter filter, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return _destContext.Entities.CopyEntitiesTo(filter);
                },
                x =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(filter, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(filter, ref x, startingIndex);
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
                        _destContext.Entities.CopyEntitiesTo(x);
                    },
                    x =>
                    {
                        _destContext.Entities.CopyEntitiesTo(x, ref emptyEntities);
                    },
                    x =>
                    {
                        _destContext.Entities.CopyEntitiesTo(x, ref emptyEntities, 0);
                    }
                });

            EntityManager nullEntities = null;
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                    Context.Entities.CopyEntitiesTo(filter));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(filter, ref emptyEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(filter, ref emptyEntities, 0));
        }

        private void Assert_CopyEntitiesTo_Filter_ContextDestroyed(EntityFilter filter)
        {
            Assert.ThrowsException<EcsContextIsDestroyedException>(() =>
            {
                _destContext.Entities.CopyEntitiesTo(filter);
            });
            AssertGetRef_ContextDestroyed<Entity>(
                () =>
                {
                    _destContext.Entities.CopyEntitiesTo(filter);
                },
                x =>
                {
                    _destContext.Entities.CopyEntitiesTo(filter, ref x);
                },
                (x, startingIndex) =>
                {
                    _destContext.Entities.CopyEntitiesTo(filter, ref x, startingIndex);
                });
        }

        private void Assert_CopyEntitiesTo_Tracker(EntityTracker tracker, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange<Entity>(
                () =>
                {
                    return _destContext.Entities.CopyEntitiesTo(tracker);
                },
                x =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(tracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(tracker, ref x, startingIndex);
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
                        _destContext.Entities.CopyEntitiesTo(x);
                    },
                    x =>
                    {
                        _destContext.Entities.CopyEntitiesTo(x, ref emptyEntities);
                    },
                    x =>
                    {
                        _destContext.Entities.CopyEntitiesTo(x, ref emptyEntities, 0);
                    }
                });

            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(tracker));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(tracker, ref emptyEntities));
            Assert.ThrowsException<EntityCopyToSameContextException>(() =>
                Context.Entities.CopyEntitiesTo(tracker, ref emptyEntities, 0));
        }

        private void Assert_CopyEntitiesTo_Query(EntityQuery queryFilterTracker, Entity[] orgEntities)
        {
            AssertGetRef_Valid_StartingIndex_Null_OutOfRange(
                () =>
                {
                    return _destContext.Entities.CopyEntitiesTo(queryFilterTracker);
                },
                x =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(queryFilterTracker, ref x);
                    return (x, count);
                },
                (x, startingIndex) =>
                {
                    var count = _destContext.Entities.CopyEntitiesTo(queryFilterTracker, ref x, startingIndex);
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
                            _destContext.Entities.DestroyEntities(x, startingIndex);
                    }
                    return result;
                });

            var emptyEntities = new Entity[0];
            EntityQuery nullable = null;
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable), "Null");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable, ref emptyEntities), "Null");
            Assert.ThrowsException<ArgumentNullException>(() =>
                _destContext.Entities.CopyEntitiesTo(nullable, ref emptyEntities, 0), "Null");

            var destroyedTracker = Context.Queries
                .SetTracker(Context.Tracking.CreateTracker("Destroyed"))
                .SetFilter(Context.Filters.WhereAllOf<TestComponent1>());
            Context.Tracking.RemoveTracker(destroyedTracker.Tracker);
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(destroyedTracker), "Tracker Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(destroyedTracker, ref emptyEntities), "Tracker Destroyed");
            Assert.ThrowsException<EntityTrackerIsDestroyedException>(() =>
                _destContext.Entities.CopyEntitiesTo(destroyedTracker, ref emptyEntities, 0), "Tracker Destroyed");
        }

        private TestResult AssertEntities(Entity[] orgEntities, int startingIndex,
            Entity[] copyEntities, int destStartingIndex, int destCount)
        {
            var result = new TestResult();
            for (var i = destStartingIndex; i < destCount; i++)
            {
                var orgEntity = orgEntities[i + startingIndex];
                var copyEntity = copyEntities[i + destStartingIndex];

                var orgComponent = Context.Entities.GetComponent<TestComponent1>(orgEntity);
                var copyComponent = _destContext.Entities.GetComponent<TestComponent1>(copyEntity);

                if (_destContext.Entities.GetComponent<TestComponent1>(copyEntity).Prop != orgEntity.Id)
                {
                    result.Success = false;
                    result.Error = $"GeneralComponent OrgEntity: {orgEntity}, CopyEntityTo: {copyEntity}";
                    break;
                }
                else if (_destContext.Entities.GetSharedComponent<TestSharedComponent1>(copyEntity).Prop != 2)
                {
                    result.Success = false;
                    result.Error = $"SharedComponent OrgEntity: {orgEntity}, CopyEntityTo: {copyEntity}";
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
