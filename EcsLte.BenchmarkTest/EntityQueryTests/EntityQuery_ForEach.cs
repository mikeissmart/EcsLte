using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BenchmarkTest.EntityQueryTests
{
    public class EntityQuery_ForEach
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityQuery _entityQuery;

        [ParamsAllValues]
        public ReadWriteType ReadWrite { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContext.HasContext("Test"))
                EcsContext.DestroyContext(EcsContext.GetContext("Test"));
            _context = EcsContext.CreateContext("Test");
            _entities = _context.EntityManager.CreateEntities(BenchmarkTestConsts.LargeCount,
                EcsContextSetupCleanup.CreateBlueprint(ComponentArrangement.Normal_x2_Shared_x2));
            _entityQuery = new EntityQuery()
                .WhereAllOf<TestComponent1, TestComponent2, TestSharedComponent1, TestSharedComponent2>();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContext.DestroyContext(_context);
        }

        [Benchmark]
        public void ForEach()
        {
            switch (ReadWrite)
            {
                case ReadWriteType.R0W0:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity) =>
                        {

                        });
                    break;

                #region Read 0

                case ReadWriteType.R0W1_Normal:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1) =>
                        {
                            component1.Prop++;
                        });
                    break;
                case ReadWriteType.R0W2_Normal:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1, ref TestComponent2 component2) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                        });
                    break;
                case ReadWriteType.R0W1_Shared:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1) =>
                        {
                            component1.Prop++;
                        });
                    break;
                case ReadWriteType.R0W2_Shared:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestSharedComponent1 component1, ref TestSharedComponent2 component2) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                        });
                    break;

                #endregion

                #region Read 1

                case ReadWriteType.R1W0_Normal:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, in TestComponent1 component1) =>
                        {
                            var prop = component1.Prop + 1;
                        });
                    break;
                case ReadWriteType.R1W1_Normal:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1, in TestComponent2 component2) =>
                        {
                            component1.Prop++;
                            var prop = component2.Prop + 1;
                        });
                    break;
                case ReadWriteType.R1W0_Shared:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, in TestComponent1 component1) =>
                        {
                            var prop = component1.Prop + 1;
                        });
                    break;
                case ReadWriteType.R1W1_Shared:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, ref TestSharedComponent1 component1, in TestSharedComponent2 component2) =>
                        {
                            component1.Prop++;
                            var prop = component2.Prop + 1;
                        });
                    break;

                #endregion

                #region Read 2

                case ReadWriteType.R2W0_Normal:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, in TestComponent1 component1, in TestComponent2 component2) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                        });
                    break;
                case ReadWriteType.R2W0_Shared:
                    _entityQuery.ForEach(_context, false,
                        (int index, Entity entity, in TestSharedComponent1 component1, in TestSharedComponent2 component2) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                        });
                    break;

                #endregion

                default:
                    throw new InvalidOperationException();
            }
        }

        public enum ReadWriteType
        {
            R0W0,

            R0W1_Normal,
            R0W2_Normal,
            R0W1_Shared,
            R0W2_Shared,

            R1W0_Normal,
            R1W1_Normal,
            R1W0_Shared,
            R1W1_Shared,

            R2W0_Normal,
            R2W0_Shared,
        }
    }
}
