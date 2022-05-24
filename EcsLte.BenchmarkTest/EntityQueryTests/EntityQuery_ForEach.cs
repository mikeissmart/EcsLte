using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BenchmarkTest.EntityQueryTests
{
    public class EntityQuery_ForEach
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityQuery _query;

        [ParamsAllValues]
        public ReadWriteType ReadWrite { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            var blueprint = EcsContextSetupCleanup.CreateBlueprint(ReadWrite);
            _entities = _context.CreateEntities(BenchmarkTestConsts.LargeCount, blueprint);
            _query = new EntityQuery()
                .WhereAllOf(blueprint.GetEntityArcheType());
        }

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void ForEach()
        {
            switch (ReadWrite)
            {
                case ReadWriteType.R0W0:
                    _query.ForEach(_context, false,
                        (int index, Entity entity) =>
                        {

                        });
                    break;

                #region Normal

                case ReadWriteType.R0W4_Normal_Bx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1, ref TestComponent2 component2, ref TestComponent3 component3, ref TestComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Normal_Bx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestComponent1 component1, in TestComponent2 component2, in TestComponent3 component3, in TestComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        });
                    break;
                case ReadWriteType.R0W4_Normal_Mx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestManageComponent1 component1, ref TestManageComponent2 component2, ref TestManageComponent3 component3, ref TestManageComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Normal_Mx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestManageComponent1 component1, in TestManageComponent2 component2, in TestManageComponent3 component3, in TestManageComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        });
                    break;

                #endregion

                #region Shared

                case ReadWriteType.R0W4_Shared_Bx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestSharedComponent1 component1, ref TestSharedComponent2 component2, ref TestSharedComponent3 component3, ref TestSharedComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Shared_Bx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestSharedComponent1 component1, in TestSharedComponent2 component2, in TestSharedComponent3 component3, in TestSharedComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        });
                    break;
                case ReadWriteType.R0W4_Shared_Mx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestManageSharedComponent1 component1, ref TestManageSharedComponent2 component2, ref TestManageSharedComponent3 component3, ref TestManageSharedComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Shared_Mx4:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestManageSharedComponent1 component1, in TestManageSharedComponent2 component2, in TestManageSharedComponent3 component3, in TestManageSharedComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        });
                    break;

                #endregion

                #region Both Normal

                case ReadWriteType.R0W4_Normal_Bx2_Mx2:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestComponent1 component1, ref TestComponent2 component2, ref TestManageComponent1 component3, ref TestManageComponent2 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Normal_Bx2_Mx2:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestComponent1 component1, in TestComponent2 component2, in TestManageComponent1 component3, in TestManageComponent2 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        });
                    break;

                #endregion

                #region Both Shared

                case ReadWriteType.R0W4_Shared_Bx2_Mx2:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, ref TestSharedComponent1 component1, ref TestSharedComponent2 component2, ref TestManageSharedComponent1 component3, ref TestManageSharedComponent2 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        });
                    break;
                case ReadWriteType.R4W0_Shared_Bx2_Mx2:
                    _query.ForEach(_context, false,
                        (int index, Entity entity, in TestSharedComponent1 component1, in TestSharedComponent2 component2, in TestManageSharedComponent1 component3, in TestManageSharedComponent2 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
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

            R0W4_Normal_Bx4,
            R4W0_Normal_Bx4,

            R0W4_Normal_Mx4,
            R4W0_Normal_Mx4,

            R0W4_Shared_Bx4,
            R4W0_Shared_Bx4,

            R0W4_Shared_Mx4,
            R4W0_Shared_Mx4,

            R0W4_Normal_Bx2_Mx2,
            R4W0_Normal_Bx2_Mx2,

            R0W4_Shared_Bx2_Mx2,
            R4W0_Shared_Bx2_Mx2,
        }
    }
}
