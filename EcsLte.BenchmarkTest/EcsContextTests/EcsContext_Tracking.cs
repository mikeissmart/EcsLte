using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_Tracking
    {
        private EcsContext _context;
        private Entity[] _entities;
        private Entity[] _trackedEntities;
        private EntityTracker _tracker;
        private EntityArcheType _archeType;

        private TestComponent1[] _comp1s = new TestComponent1[BenchmarkTestConsts.LargeCount];
        private TestComponent2[] _comp2s = new TestComponent2[BenchmarkTestConsts.LargeCount];
        private TestComponent3[] _comp3s = new TestComponent3[BenchmarkTestConsts.LargeCount];
        private TestComponent4[] _comp4s = new TestComponent4[BenchmarkTestConsts.LargeCount];

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.Instance.HasContext("Test"))
                EcsContexts.Instance.DestroyContext(EcsContexts.Instance.GetContext("Test"));
            _context = EcsContexts.Instance.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _tracker = _context.Tracking
                .SetTrackingComponent<TestComponent1>(true)
                .SetTrackingComponent<TestComponent2>(true)
                .SetTrackingComponent<TestComponent3>(true)
                .SetTrackingComponent<TestComponent4>(true)
                .SetTrackingComponent<TestManagedComponent1>(true)
                .SetTrackingComponent<TestManagedComponent2>(true)
                .SetTrackingComponent<TestManagedComponent3>(true)
                .SetTrackingComponent<TestManagedComponent4>(true)
                .SetTrackingComponent<TestSharedComponent1>(true)
                .SetTrackingComponent<TestSharedComponent2>(true)
                .SetTrackingComponent<TestSharedComponent3>(true)
                .SetTrackingComponent<TestSharedComponent4>(true)
                .SetTrackingMode(EntityTrackerMode.AnyChanges);
            _trackedEntities = new Entity[BenchmarkTestConsts.LargeCount];
        }

        #region Create Reuse

        [IterationCleanup(Targets = new[]
        {
            nameof(CreateEntities_Reuse_NoTracker),
            nameof(CreateEntities_Reuse_WithTracker)
        })]
        public void IterationCleanup_Create_Reuse()
        {
            _context.Entities.DestroyEntities(_entities);
        }

        [Benchmark]
        public void CreateEntities_Reuse_NoTracker()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_x4);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);
        }

        [Benchmark]
        public void CreateEntities_Reuse_WithTracker()
        {
            _tracker.SetChangeVersion(_context.Entities.GlobalVersion);
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_x4);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);

            _context.Entities.GetEntities(_tracker, ref _trackedEntities);
        }

        #endregion Create Reuse

        #region Update Normal

        [IterationSetup(Targets = new[]
        {
            nameof(Update_Normal_x4_NoTracker),
            nameof(Update_Normal_x4_WithTracker)
        })]
        public void IterationSetup_Update_Normal_x4()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_x4);
            _archeType = blueprint.GetArcheType(_context);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);
        }

        [IterationCleanup(Targets = new[]
        {
            nameof(Update_Normal_x4_NoTracker),
            nameof(Update_Normal_x4_WithTracker)
        })]
        public void IterationCleanup_Update_Normal_x4()
        {
            _context.Entities.DestroyEntities(_entities);
        }

        [Benchmark]
        public void Update_Normal_x4_NoTracker()
        {
            _context.Entities.UpdateComponents(_archeType, new TestComponent1 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent2 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent3 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent4 { Prop = 1 });
        }

        [Benchmark]
        public void Update_Normal_x4_WithTracker()
        {
            _context.Entities.UpdateComponents(_archeType, new TestComponent1 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent2 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent3 { Prop = 1 });
            _context.Entities.UpdateComponents(_archeType, new TestComponent4 { Prop = 1 });

            _context.Entities.GetEntities(_tracker, ref _trackedEntities);
        }

        #endregion Update

        #region Update Managed

        [IterationSetup(Targets = new[]
        {
            nameof(Update_Managed_x4_NoTracker),
            nameof(Update_Managed_x4_WithTracker)
        })]
        public void IterationSetup_Update_Managed_x4()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Managed_x4);
            _archeType = blueprint.GetArcheType(_context);
            _entities = _context.Entities.CreateEntities(blueprint, _entities.Length);
        }

        [IterationCleanup(Targets = new[]
        {
            nameof(Update_Managed_x4_NoTracker),
            nameof(Update_Managed_x4_WithTracker)
        })]
        public void IterationCleanup_Update_Managed_x4()
        {
            _context.Entities.DestroyEntities(_entities);
        }

        [Benchmark]
        public void Update_Managed_x4_NoTracker()
        {
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent1 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent2 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent3 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent4 { Prop = 1 });
        }

        [Benchmark]
        public void Update_Managed_x4_WithTracker()
        {
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent1 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent2 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent3 { Prop = 1 });
            _context.Entities.UpdateManagedComponents(_archeType, new TestManagedComponent4 { Prop = 1 });

            _context.Entities.GetEntities(_tracker, ref _trackedEntities);
        }

        #endregion Update
    }
}
