using EcsLte.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EcsLte.UnitTest
{
    public abstract class BasePrePostTest
    {
        public EcsContext Context { get; set; }

        [TestInitialize]
        public void PreTest() => Context = EcsContexts.Instance.CreateContext("UnitTest");

        [TestCleanup]
        public void PostTest()
        {
            foreach (var context in EcsContexts.Instance.GetAllContexts())
                EcsContexts.Instance.DestroyContext(context);
            Context = null;
        }

        protected void AssertClassEquals<T>(T same1, T same2, T different, T nullable) where T : IEquatable<T>
        {
            Assert.IsTrue(same1.GetHashCode() != 0);
            Assert.IsTrue(same1.GetHashCode() == same2.GetHashCode());
            Assert.IsFalse(same1.GetHashCode() == different.GetHashCode());

            Assert.IsTrue(same1.Equals(same2));
            Assert.IsFalse(different.Equals(same1));
            Assert.IsFalse(same1.Equals(nullable));
        }

        /*protected void AssertEntity_Valid_EntityNull_ContextDestroyed(EcsContext destroyContext,
            Entity valid,
            Func<Entity, TestResult> assertAction)
        {
            TestResult result;

            result = assertAction(valid);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            Assert.ThrowsException<EntityDoesNotExistException>(() => assertAction(Entity.Null),
                "Entity.Null");

            EcsContexts.Instance.DestroyContext(destroyContext);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertAction(valid),
                "Context Destroyed");
        }*/

        /*protected void AssertGetIn_Valid_EntityNull_StartingIndex_Null_OutOfRange_ContextDestroyed<TSrc,TOut>(
            EcsContext destroyContext,
            Func<TSrc[]> getValid,
            Func<TSrc[], TOut[]> assertAction,
            Func<TSrc[], int, TOut[]> assertStartingIndexAction,
            Func<TSrc[], int, int, TOut[]> assertStartingIndexCountAction,
            Func<TSrc[], int, int, TOut[], TestResult> checkAction)
        {
            TestResult result;
            TSrc[] nullable = null;

            Assert.ThrowsException<ArgumentNullException>(() => assertAction(nullable),
                "Null");
            Assert.ThrowsException<ArgumentNullException>(() => assertStartingIndexAction(nullable, 0),
                "Null StartingIndex");
            Assert.ThrowsException<ArgumentNullException>(() => assertStartingIndexCountAction(nullable, 0, 1),
                "Null StartingIndex Count");

            var srcValid = getValid();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertStartingIndexAction(srcValid, -1),
                "Neg StartingIndex");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertStartingIndexCountAction(srcValid, 0, 0),
                "Zero Count");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertStartingIndexCountAction(srcValid, 0, srcValid.Length + 1),
                "Count Greater Than Length");

            result = checkAction(srcValid, 0, srcValid.Length, assertAction(srcValid));
            Assert.IsTrue(result.Success, $"Valid : {result.Error}");

            srcValid = getValid();
            result = checkAction(srcValid, 5, srcValid.Length - 5, assertStartingIndexAction(srcValid, 5));
            Assert.IsTrue(result.Success, $"StartingIndex: {result.Error}");

            srcValid = getValid();
            result = checkAction(srcValid, 5, srcValid.Length - 10, assertStartingIndexCountAction(srcValid, 5, srcValid.Length - 10));
            Assert.IsTrue(result.Success, $"StartingIndex Count: {result.Error}");

            srcValid = getValid();
            EcsContexts.Instance.DestroyContext(destroyContext);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertAction(srcValid),
                "Valid Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertStartingIndexAction(srcValid, 0),
                "Valid StartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertStartingIndexCountAction(srcValid, 0, 1),
                "Valid StartingIndex CountContext Destroyed");
        }*/

        /*protected void AssertGetRef_Valid_EntityNull_StartingIndex_Null_OutOfRange_ContextDestroyed<TSrc, TOut>(
            EcsContext destroyContext,
            Func<TSrc[]> getValid,
            Func<TSrc[]> getInvalid,
            Func<TSrc[], TOut[]> assertAction,
            Func<TSrc[], TOut[], TOut[]> assertRefAction,
            Func<TSrc[], TOut[], int, TOut[]> assertRefStartingIndexAction,
            Func<TSrc[], TOut[], int, int, TOut[]> assertRefStartingIndexCountAction,
            Func<TSrc[], int, int, TOut[], TestResult> checkAction)
        {
            TestResult result;
            TSrc[] nullable = null;
            TOut[] errorRef = new TOut[0];

            Assert.ThrowsException<ArgumentNullException>(() => assertRefAction(nullable, errorRef),
                "Null Ref");
            Assert.ThrowsException<ArgumentNullException>(() => assertRefStartingIndexAction(nullable, errorRef, 0),
                "Null Ref StartingIndex");
            Assert.ThrowsException<ArgumentNullException>(() => assertRefStartingIndexCountAction(nullable, errorRef, 0, 1),
                "Null Ref StartingIndex Count");

            var srcValid = getValid();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertRefStartingIndexAction(srcValid, errorRef, -1),
                "Neg StartingIndex");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertRefStartingIndexCountAction(srcValid, errorRef, 0, 0),
                "Zero Count");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => assertRefStartingIndexCountAction(srcValid, errorRef, 0, srcValid.Length + 1),
                "Count Greater Than Length");

            var validRef = new TOut[0];
            validRef = assertRefAction(srcValid, validRef);
            result = checkAction(srcValid, 0, srcValid.Length, validRef);
            Assert.IsTrue(result.Success, $"Valid : {result.Error}");

            srcValid = getValid();
            validRef = new TOut[0];
            validRef = assertRefStartingIndexAction(srcValid, validRef, 5);
            result = checkAction(srcValid, 5, srcValid.Length - 5, validRef);
            Assert.IsTrue(result.Success, $"StartingIndex: {result.Error}");

            srcValid = getValid();
            validRef = new TOut[0];
            validRef = assertRefStartingIndexCountAction(srcValid, validRef, 5, srcValid.Length - 10);
            result = checkAction(srcValid, 5, srcValid.Length - 10, validRef);
            Assert.IsTrue(result.Success, $"StartingIndex Count: {result.Error}");

            srcValid = getValid();
            validRef = new TOut[0];
            EcsContexts.Instance.DestroyContext(destroyContext);
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertAction(srcValid),
                "Valid Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertRefAction(srcValid, validRef),
                "Valid Ref Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertRefStartingIndexAction(srcValid, validRef, 0),
                "Valid Ref StartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => assertRefStartingIndexCountAction(srcValid, validRef, 0, 1),
                "Valid Ref StartingIndex CountContext Destroyed");
        }*/

        /*protected void AssertRef_Valid_StartingIndex_Null_OutOfRange(
            Func<Entity[], TestResult> assertAction,
            Func<Entity[], int, TestResult> assertStartingIndexAction)
        {
            TestResult result;

            var entities = new Entity[0];
            var startingIndex = new Entity[5];
            Entity[] nullable = null;

            result = assertAction(entities);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");

            result = assertStartingIndexAction(startingIndex, 5);
            Assert.IsTrue(result.Success, $"StartingIndex: {result.Error}");

            Assert.ThrowsException<ArgumentNullException>(() => assertAction(nullable),
                "Null");
        }*/

        protected void AssertEntity_Valid_EntityNull<TOut>(
            Entity valid,
            Func<Entity, TOut> entityAction,
            Func<Entity, TOut, TestResult> assertAction)
        {
            var invalid = Entity.Null;

            Assert.ThrowsException<EntityNotExistException>(() => entityAction(invalid),
                "Entity.Null");

            var tResult = entityAction(valid);
            var result = assertAction(valid, tResult);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");
        }

        protected void AssertGetInRef_Valid_Invalid_StartingIndex_Null_OutOfRange<TIn, TRef>(
            Func<TIn[]> inSrcAction,
            Func<ChangeVersion> inDestChangeVersion,
            Func<TIn[]> invalidInSrcAction,
            Func<TIn[], TRef[]> inAction,
            Func<TIn[], int, TRef[]> inStartingIndexAction,
            Func<TIn[], int, int, TRef[]> inStartingIndexCountAction,
            Func<TIn[], TRef[], (TRef[], int)> inRefAction,
            Func<TIn[], int, TRef[], (TRef[], int)> inStartingIndexRefAction,
            Func<TIn[], int, int, TRef[], TRef[]> inStartingIndexCountRefAction,
            Func<TIn[], TRef[], int, (TRef[], int)> inRefDestStartingIndexAction,
            Func<TIn[], int, TRef[], int, (TRef[], int)> inStartingIndexRefDestStartingIndexAction,
            Func<TIn[], int, int, TRef[], int, TRef[]> inStartingIndexCountRefDestStartingIndexAction,
            // in, startingIndex, ref, destStartingIndex, count
            Func<TIn[], int, TRef[], int, int, TestResult> assertAction)
        {
            TIn[] nullIn = null;
            var emptyRef = new TRef[0];
            var inSrc = inSrcAction();
            TRef[] nullRef = null;
            var prevChange = inDestChangeVersion();

            Assert.ThrowsException<ArgumentNullException>(() => inAction(nullIn),
                "In Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexAction(nullIn, 0),
                "In Null StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexAction(inSrc, -1),
                "In Neg StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexAction(inSrc, inSrc.Length + 1),
                "In OutOfRange StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountAction(nullIn, 0, 1),
                "In Null StartingIndex Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(inSrc, -1, 1),
                "In StartingIndex Neg Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(inSrc, 0, -1),
                "In StartingIndex Count Neg");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(inSrc, inSrc.Length + 1, 1),
                "In StartingIndex OutOfRange Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(inSrc, 0, inSrc.Length + 1),
                "In StartingIndex Count OutOfRange");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inRefAction(nullIn, emptyRef),
                "In Null Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inRefAction(inSrc, nullRef),
                "In Ref Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexRefAction(nullIn, 0, emptyRef),
                "In Null StartingIndex Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexRefAction(inSrc, 0, nullRef),
                "In StartingIndex Ref Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefAction(inSrc, -1, emptyRef),
                "In StartingIndex Neg Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefAction(inSrc, inSrc.Length + 1, emptyRef),
                "In StartingIndex OutOfRange Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefAction(nullIn, 0, 1, emptyRef),
                "In Null StartingIndex Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefAction(inSrc, 0, 1, nullRef),
                "In StartingIndex Count Ref Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, -1, 1, emptyRef),
                "In StartingIndex Neg Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, 0, -1, emptyRef),
                "In StartingIndex Count Neg Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, inSrc.Length + 1, 0, emptyRef),
                "In StartingIndex OutOfRange Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, 0, inSrc.Length + 1, emptyRef),
                "In StartingIndex Count OutOfRange Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inRefDestStartingIndexAction(nullIn, emptyRef, 0),
                "In Null Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inRefDestStartingIndexAction(inSrc, nullRef, 0),
                "In Ref Null DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inRefDestStartingIndexAction(inSrc, emptyRef, -1),
                "In Ref DestStartingIndex Neg");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inRefDestStartingIndexAction(inSrc, emptyRef, 1),
                "In Ref DestStartingIndex OutOfRange");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexRefDestStartingIndexAction(nullIn, 0, emptyRef, 0),
                "In Null StartingIndex Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexRefDestStartingIndexAction(inSrc, 0, nullRef, 0),
                "In StartingIndex Ref Null DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefDestStartingIndexAction(inSrc, -1, emptyRef, 0),
                "In Neg StartingIndex Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefDestStartingIndexAction(inSrc, 0, emptyRef, -1),
                "In StartingIndex Ref DestStartingIndex Neg");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefDestStartingIndexAction(inSrc, inSrc.Length + 1, emptyRef, 0),
                "In StartingIndex OutOfRange Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexRefDestStartingIndexAction(inSrc, 0, emptyRef, 1),
                "In StartingIndex Ref DestStartingIndex OutOfRange");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefAction(nullIn, 0, 1, emptyRef),
                "In Null StartingIndex Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefAction(inSrc, 0, 1, nullRef),
                "In StartingIndex Count Ref Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, -1, 1, emptyRef),
                "In StartingIndex Neg Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, 0, -1, emptyRef),
                "In StartingIndex Count Neg Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefAction(inSrc, inSrc.Length + 1, 1, emptyRef),
                "In StartingIndex OutOfRange Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefDestStartingIndexAction(nullIn, 0, 1, emptyRef, 0),
                "In Null StartingIndex Count Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, 0, 1, nullRef, 0),
                "In StartingIndex Count Ref Null DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, -1, 1, emptyRef, 0),
                "In StartingIndex Neg Count Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, 0, -1, emptyRef, 0),
                "In StartingIndex Count Neg Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, 0, 1, emptyRef, -1),
                "In StartingIndex Count Ref DestStartingIndex Neg");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, inSrc.Length + 1, 1, emptyRef, 0),
                "In StartingIndex OutOfRange Count Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountRefDestStartingIndexAction(inSrc, 0, 1, emptyRef, 1),
                "In StartingIndex Count Ref DestStartingIndex OutOfRange");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            var inInvalid = invalidInSrcAction();
            Assert.ThrowsException<EntityNotExistException>(() => inAction(inInvalid),
                "In Invalid");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexAction(inInvalid, 0),
                "In Invalid StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexCountAction(inInvalid, 0, 1),
                "In Invalid StartingIndex Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inRefAction(inInvalid, emptyRef),
                "In Invalid Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexRefAction(inInvalid, 0, emptyRef),
                "In Invalid StartingIndex Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexCountRefAction(inInvalid, 0, 1, emptyRef),
                "In Invalid StartingIndex Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inRefDestStartingIndexAction(inInvalid, emptyRef, 0),
                "In Invalid Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexRefDestStartingIndexAction(inInvalid, 0, emptyRef, 0),
                "In Invalid StartingIndex Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexCountRefAction(inInvalid, 0, 1, emptyRef),
                "In Invalid StartingIndex Count Ref");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexCountRefDestStartingIndexAction(inInvalid, 0, 1, emptyRef, 0),
                "In Invalid StartingIndex Count Ref DestStartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            var startingIndex = 5;
            prevChange = inDestChangeVersion();
            var tResult = inAction(inSrc);
            var postChange = inDestChangeVersion();
            var result = assertAction(inSrc, 0, tResult, 0, inSrc.Length);
            Assert.IsTrue(result.Success, $"In: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tResult = inStartingIndexAction(inSrc, startingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResult, 0, inSrc.Length - startingIndex);
            Assert.IsTrue(result.Success, $"In StartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tResult = inStartingIndexCountAction(inSrc, startingIndex, inSrc.Length - (startingIndex * 2));
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResult, 0, inSrc.Length - (startingIndex * 2));
            Assert.IsTrue(result.Success, $"In StartingIndex Count: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex Count");

            var tRef = new TRef[0];
            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            var tResultCount = inRefAction(inSrc, tRef);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, 0, tResultCount.Item1, 0, tResultCount.Item2);
            Assert.IsTrue(result.Success, $"In Ref: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In Ref");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tRef = new TRef[0];
            tResultCount = inStartingIndexRefAction(inSrc, startingIndex, tRef);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResultCount.Item1, 0, tResultCount.Item2);
            Assert.IsTrue(result.Success, $"In StartingIndex Ref: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex Ref");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tRef = new TRef[0];
            tResult = inStartingIndexCountRefAction(inSrc, startingIndex, inSrc.Length - (startingIndex * 2), tRef);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResult, 0, inSrc.Length - (startingIndex * 2));
            Assert.IsTrue(result.Success, $"In StartingIndex Count Ref: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex Count Ref");

            var destStartingIndex = 5;
            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tRef = new TRef[destStartingIndex];
            tResultCount = inRefDestStartingIndexAction(inSrc, tRef, destStartingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, 0, tResultCount.Item1, destStartingIndex, tResultCount.Item2);
            Assert.IsTrue(result.Success, $"In Ref DestStartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In Ref DestStartingIndex");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tRef = new TRef[destStartingIndex];
            tResultCount = inStartingIndexRefDestStartingIndexAction(inSrc, startingIndex, tRef, destStartingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResultCount.Item1, destStartingIndex, tResultCount.Item2);
            Assert.IsTrue(result.Success, $"In StartingIndex Ref DestStartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex Ref DestStartingIndex");

            inSrc = inSrcAction();
            prevChange = inDestChangeVersion();
            tRef = new TRef[destStartingIndex];
            tResult = inStartingIndexCountRefDestStartingIndexAction(inSrc, startingIndex, inSrc.Length - (startingIndex * 2), tRef, destStartingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(inSrc, startingIndex, tResult, destStartingIndex, inSrc.Length - (startingIndex * 2));
            Assert.IsTrue(result.Success, $"In StartingIndex Count Ref DestStartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "In StartingIndex Count Ref DestStartingIndex");
        }

        protected void AssertGetInRef_ContextDestroyed<TIn, TRef>(
            Action<TIn[]> inAction,
            Action<TIn[], int> inStartingIndexAction,
            Action<TIn[], int, int> inStartingIndexCountAction,
            Action<TIn[], TRef[]> inRefAction,
            Action<TIn[], int, TRef[]> inStartingIndexRefAction,
            Action<TIn[], int, int, TRef[]> inStartingIndexCountRefAction,
            Action<TIn[], TRef[], int> inRefDestStartingIndexAction,
            Action<TIn[], int, TRef[], int> inStartingIndexRefDestStartingIndexAction,
            Action<TIn[], int, int, TRef[], int> inStartingIndexCountRefDestStartingIndexAction)
        {
            var tIn = new TIn[0];
            var tRef = new TRef[0];

            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inAction(tIn),
                "In Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexAction(tIn, 0),
                "In StartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexCountAction(tIn, 0, 1),
                "In StartingIndex Count Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inRefAction(tIn, tRef),
                "In Ref Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexRefAction(tIn, 0, tRef),
                "In StartingIndex Ref Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexCountRefAction(tIn, 0, 1, tRef),
                "In StartingIndex Count Ref Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inRefDestStartingIndexAction(tIn, tRef, 0),
                "In Ref DestStartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexRefDestStartingIndexAction(tIn, 0, tRef, 0),
                "In StartingIndex Ref DestStartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexCountRefDestStartingIndexAction(tIn, 0, 1, tRef, 0),
                "In StartingIndex Count Ref DestStartingIndex Context Destroyed");
        }

        protected void AssertGetIn_Valid_Invalid_StartingIndex_Null_OutOfRange<TIn, TOut>(
            Func<TIn[]> srcAction,
            Func<ChangeVersion> inDestChangeVersion,
            Func<TIn[]> invalidSrcAction,
            Func<TIn[], TOut[]> inAction,
            Func<TIn[], int, TOut[]> inStartingIndexAction,
            Func<TIn[], int, int, TOut[]> inStartingIndexCountAction,
            // in, startingIndex, count, out
            Func<TIn[], int, int, TOut[], TestResult> assertAction)
        {
            var startingIndex = 5;
            TIn[] nullable = null;
            var prevChange = inDestChangeVersion();

            Assert.ThrowsException<ArgumentNullException>(() => inAction(nullable),
                "In Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexAction(nullable, 0),
                "In Null StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => inStartingIndexCountAction(nullable, 0, 0),
                "In Null StartingIndex Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            var src = srcAction();
            prevChange = inDestChangeVersion();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexAction(src, -1),
                "In Neg StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(src, 0, -1),
                "In StartingIndex Neg Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(src, -1, -1),
                "In Neg StartingIndex Neg Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexAction(src, src.Length + 1),
                "In OutOfRange StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(src, 0, src.Length + 1),
                "In StartingIndex OutOfRange Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => inStartingIndexCountAction(src, src.Length + 1, src.Length + 1),
                "In OutOfRange StartingIndex OutOfRange Count");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            if (invalidSrcAction != null)
            {
                var invalidSrc = invalidSrcAction();
                Assert.ThrowsException<EntityNotExistException>(() => inAction(invalidSrc),
                    "In Invalid");
                Assert.AreEqual(prevChange, inDestChangeVersion());
                Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexAction(invalidSrc, 0),
                    "In Invalid StartingIndex");
                Assert.AreEqual(prevChange, inDestChangeVersion());
                Assert.ThrowsException<EntityNotExistException>(() => inStartingIndexCountAction(invalidSrc, 0, 1),
                    "In Invalid StartingIndex Count");
                Assert.AreEqual(prevChange, inDestChangeVersion());
            }

            prevChange = inDestChangeVersion();
            var tResult = inAction(src);
            var postChange = inDestChangeVersion();
            var result = assertAction(src, 0, src.Length, tResult);
            Assert.IsTrue(result.Success, $"Valid: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "Valid");

            src = srcAction();
            prevChange = inDestChangeVersion();
            tResult = inStartingIndexAction(src, startingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(src, startingIndex, src.Length - startingIndex, tResult);
            Assert.IsTrue(result.Success, $"StartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "StartingIndex");

            src = srcAction();
            prevChange = inDestChangeVersion();
            tResult = inStartingIndexCountAction(src, startingIndex, src.Length - (startingIndex * 2));
            postChange = inDestChangeVersion();
            result = assertAction(src, startingIndex, src.Length - (startingIndex * 2), tResult);
            Assert.IsTrue(result.Success, $"Count: {result.Error}");
            Assert.AreEqual(prevChange.Version + 1, postChange.Version, "Count");
        }

        protected void AssertGetIn_ContextDestroyed<TIn>(
            Action<TIn[]> inAction,
            Action<TIn[], int> inStartingIndexAction,
            Action<TIn[], int, int> inStartingIndexCountAction)
        {
            var tIn = new TIn[0];

            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inAction(tIn),
                "In Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexAction(tIn, 0),
                "In StartingIndex Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => inStartingIndexCountAction(tIn, 0, 0),
                "In StartingIndex Count Context Destroyed");
        }

        protected void AssertGetRef_Valid_StartingIndex_Null_OutOfRange<TRef>(
            bool incVersionAfterAction,
            Func<ChangeVersion> inDestChangeVersion,
            Func<TRef[]> action,
            Func<TRef[], (TRef[], int)> refAction,
            Func<TRef[], int, (TRef[], int)> refStartingIndexAction,
            Func<TRef[], int, int, TestResult> assertAction)
        {
            var startingIndex = 5;
            TRef[] nullable = null;
            var prevChange = inDestChangeVersion();

            Assert.ThrowsException<ArgumentNullException>(() => refAction(nullable),
                "Ref Null");
            Assert.AreEqual(prevChange, inDestChangeVersion());
            Assert.ThrowsException<ArgumentNullException>(() => refStartingIndexAction(nullable, 0),
                "Ref Null StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            var tRef = new TRef[0];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => refStartingIndexAction(tRef, -1),
                "Ref Neg StartingIndex");
            Assert.AreEqual(prevChange, inDestChangeVersion());

            prevChange = inDestChangeVersion();
            tRef = action();
            var postChange = inDestChangeVersion();
            var result = assertAction(tRef, 0, tRef.Length);
            Assert.IsTrue(result.Success, $"Ref: {result.Error}");
            Assert.AreEqual(prevChange.Version + (incVersionAfterAction ? 1ul : 0), postChange.Version, "Ref");

            prevChange = inDestChangeVersion();
            var tResult = refAction(tRef);
            postChange = inDestChangeVersion();
            result = assertAction(tResult.Item1, 0, tResult.Item2);
            Assert.IsTrue(result.Success, $"Ref: {result.Error}");
            Assert.AreEqual(prevChange.Version + (incVersionAfterAction ? 1ul : 0), postChange.Version, "Ref");

            prevChange = inDestChangeVersion();
            tRef = new TRef[startingIndex];
            tResult = refStartingIndexAction(tRef, startingIndex);
            postChange = inDestChangeVersion();
            result = assertAction(tResult.Item1, startingIndex, tResult.Item2);
            Assert.IsTrue(result.Success, $"Ref StartingIndex: {result.Error}");
            Assert.AreEqual(prevChange.Version + (incVersionAfterAction ? 1ul : 0), postChange.Version, "Ref StartingIndex");
        }

        protected void AssertGetRef_ContextDestroyed<TRef>(
            Action action,
            Action<TRef[]> refAction,
            Action<TRef[], int> refStartingIndexAction)
        {
            var tRef = new TRef[0];

            Assert.ThrowsException<EcsContextIsDestroyedException>(() => action(),
                "Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => refAction(tRef),
                "Ref Context Destroyed");
            Assert.ThrowsException<EcsContextIsDestroyedException>(() => refStartingIndexAction(tRef, 0),
                "Ref StartingIndex Context Destroyed");
        }

        protected void AssertArcheType_DiffContext_Null(
            params Action<EntityArcheType>[] assertActions)
        {
            var diffArcheType = EcsContexts.Instance.CreateContext(DateTime.Now.Ticks.ToString())
                .ArcheTypes
                .AddComponentType<TestComponent1>();
            EntityArcheType nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<EcsContextNotSameException>(() => action(diffArcheType),
                    "Context not same");

                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");
            }
        }

        protected void AssertFilter_Null(
            params Action<EntityFilter>[] assertActions)
        {
            EntityFilter nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");
            }
        }

        protected void AssertTracker_Different_Null(
            EntityTracker diffContext,
            params Action<EntityTracker>[] assertActions)
        {
            EntityTracker nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");

                if (diffContext != null)
                {
                    Assert.ThrowsException<EcsContextNotSameException>(() => action(diffContext),
                        "Different Context");
                }
            }
        }

        protected void AssertQuery_DiffContext_Null(
            params Action<EntityQuery>[] assertActions)
        {
            var diffContext = EcsContexts.Instance.CreateContext(DateTime.Now.Ticks.ToString());
            var diffQuery = diffContext.Queries
                .SetFilter(diffContext.Filters
                    .WhereAllOf<TestComponent1>())
                .SetTracker(diffContext.Tracking.SetTrackingMode(EntityTrackerMode.AnyChanges));
            EntityQuery nullable = null;

            foreach (var action in assertActions)
            {
                Assert.ThrowsException<ArgumentNullException>(() => action(nullable),
                    "Null");

                if (diffContext != null)
                {
                    Assert.ThrowsException<EcsContextNotSameException>(() => action(diffQuery),
                        "Different Context");
                }
            }
        }
    }
}