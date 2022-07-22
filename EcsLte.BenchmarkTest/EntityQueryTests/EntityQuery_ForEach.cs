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
            _entities = _context.Entities.CreateEntities(blueprint, EntityState.Active, BenchmarkTestConsts.LargeCount);
            _query = new EntityQuery(
                _context,
                new EntityFilter()
                    .WhereAllOf(blueprint.GetArcheType()));
        }

        [IterationCleanup]
        public void IterationCleanup() => _context.Entities.DestroyEntities(_entities);

        [Benchmark]
        public void ForEach()
        {
            switch (ReadWrite)
            {
                case ReadWriteType.R0W0:
                    _query.ForEach(
                        (int index, Entity entity) =>
                        {
                        }, false);
                    break;

                #region Normal

                case ReadWriteType.R0W4_Normal_x4:
                    _query.ForEach(
                        (int index, Entity entity, ref TestComponent1 component1, ref TestComponent2 component2, ref TestComponent3 component3, ref TestComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        }, false);
                    break;

                case ReadWriteType.R4W0_Normal_x4:
                    _query.ForEach(
                        (int index, Entity entity, in TestComponent1 component1, in TestComponent2 component2, in TestComponent3 component3, in TestComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        }, false);
                    break;

                #endregion Normal

                #region Shared

                case ReadWriteType.R0W4_Shared_x4:
                    _query.ForEach(
                        (int index, Entity entity, ref TestSharedComponent1 component1, ref TestSharedComponent2 component2, ref TestSharedComponent3 component3, ref TestSharedComponent4 component4) =>
                        {
                            component1.Prop++;
                            component2.Prop++;
                            component3.Prop++;
                            component4.Prop++;
                        }, false);
                    break;

                case ReadWriteType.R4W0_Shared_x4:
                    _query.ForEach(
                        (int index, Entity entity, in TestSharedComponent1 component1, in TestSharedComponent2 component2, in TestSharedComponent3 component3, in TestSharedComponent4 component4) =>
                        {
                            var prop1 = component1.Prop + 1;
                            var prop2 = component2.Prop + 1;
                            var prop3 = component3.Prop + 1;
                            var prop4 = component4.Prop + 1;
                        }, false);
                    break;

                #endregion Shared

                default:
                    throw new InvalidOperationException();
            }
        }

        public enum ReadWriteType
        {
            R0W0,

            R0W4_Normal_x4,
            R4W0_Normal_x4,

            R0W4_Shared_x4,
            R4W0_Shared_x4,
        }
    }
}