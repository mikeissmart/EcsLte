using BenchmarkDotNet.Attributes;
using System;

namespace EcsLte.BenchmarkTest.EcsContextTests
{
    public class EcsContext_UpdateComponent
    {
        private EcsContext _context;
        private Entity[] _entities;
        private EntityBlueprint _blueprint;
        private EntityArcheType _archeType1;
        private EntityArcheType _archeType2;
        private EntityArcheType _archeType3;
        private EntityArcheType _archeType4;
        private EntityQuery _query1;
        private EntityQuery _query2;
        private EntityQuery _query3;
        private EntityQuery _query4;

        private TestComponent1 Component1 = new TestComponent1 { Prop = 2 };
        private TestComponent2 Component2 = new TestComponent2 { Prop = 3 };
        private TestComponent3 Component3 = new TestComponent3 { Prop = 4 };
        private TestComponent4 Component4 = new TestComponent4 { Prop = 5 };
        private TestSharedComponent1 SharedComponent1 = new TestSharedComponent1 { Prop = 6 };
        private TestSharedComponent2 SharedComponent2 = new TestSharedComponent2 { Prop = 7 };
        private TestSharedComponent3 SharedComponent3 = new TestSharedComponent3 { Prop = 8 };
        private TestSharedComponent4 SharedComponent4 = new TestSharedComponent4 { Prop = 9 };

        private TestManageComponent1 ManageComponent1 = new TestManageComponent1 { Prop = 12 };
        private TestManageComponent2 ManageComponent2 = new TestManageComponent2 { Prop = 13 };
        private TestManageComponent3 ManageComponent3 = new TestManageComponent3 { Prop = 14 };
        private TestManageComponent4 ManageComponent4 = new TestManageComponent4 { Prop = 15 };
        private TestManageSharedComponent1 ManageSharedComponent1 = new TestManageSharedComponent1 { Prop = 16 };
        private TestManageSharedComponent2 ManageSharedComponent2 = new TestManageSharedComponent2 { Prop = 17 };
        private TestManageSharedComponent3 ManageSharedComponent3 = new TestManageSharedComponent3 { Prop = 18 };
        private TestManageSharedComponent4 ManageSharedComponent4 = new TestManageSharedComponent4 { Prop = 19 };

        [ParamsAllValues]
        public ComponentArrangement CompArr { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (EcsContexts.HasContext("Test"))
                EcsContexts.DestroyContext(EcsContexts.GetContext("Test"));
            _context = EcsContexts.CreateContext("Test");
            _entities = new Entity[BenchmarkTestConsts.LargeCount];
            _blueprint = EcsContextSetupCleanup.CreateBlueprint(CompArr);

            CreateArcheTypes();
        }

        private EntityArcheType BlueprintUpdateAndArcheType<T>(T component) where T : IComponent =>
            _blueprint.UpdateComponent(component).GetEntityArcheType();

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            if (!_context.IsDestroyed)
                EcsContexts.DestroyContext(_context);
        }

        [IterationSetup]
        public void IterationSetup() => _entities = _context.CreateEntities(_entities.Length, _blueprint);

        [IterationCleanup]
        public void IterationCleanup() => _context.DestroyEntities(_entities);

        [Benchmark]
        public void UpdateComponent_Entity()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, Component1);
                        _context.UpdateComponent(entity, Component2);
                        _context.UpdateComponent(entity, Component3);
                        _context.UpdateComponent(entity, Component4);
                    }
                    break;

                case ComponentArrangement.Shared_Bx4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, SharedComponent1);
                        _context.UpdateComponent(entity, SharedComponent2);
                        _context.UpdateComponent(entity, SharedComponent3);
                        _context.UpdateComponent(entity, SharedComponent4);
                    }
                    break;

                case ComponentArrangement.Normal_Mx4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, ManageComponent1);
                        _context.UpdateComponent(entity, ManageComponent2);
                        _context.UpdateComponent(entity, ManageComponent3);
                        _context.UpdateComponent(entity, ManageComponent4);
                    }
                    break;

                case ComponentArrangement.Shared_Mx4:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, ManageSharedComponent1);
                        _context.UpdateComponent(entity, ManageSharedComponent2);
                        _context.UpdateComponent(entity, ManageSharedComponent3);
                        _context.UpdateComponent(entity, ManageSharedComponent4);
                    }
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, Component1);
                        _context.UpdateComponent(entity, Component2);
                        _context.UpdateComponent(entity, ManageComponent1);
                        _context.UpdateComponent(entity, ManageComponent2);
                    }
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    for (var i = 0; i < _entities.Length; i++)
                    {
                        var entity = _entities[i];
                        _context.UpdateComponent(entity, SharedComponent1);
                        _context.UpdateComponent(entity, SharedComponent2);
                        _context.UpdateComponent(entity, ManageSharedComponent1);
                        _context.UpdateComponent(entity, ManageSharedComponent2);
                    }
                    break;
            }
        }

        [Benchmark]
        public void UpdateComponents_Entities()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    _context.UpdateComponents(_entities, Component1);
                    _context.UpdateComponents(_entities, Component2);
                    _context.UpdateComponents(_entities, Component3);
                    _context.UpdateComponents(_entities, Component4);
                    break;

                case ComponentArrangement.Shared_Bx4:
                    _context.UpdateComponents(_entities, SharedComponent1);
                    _context.UpdateComponents(_entities, SharedComponent2);
                    _context.UpdateComponents(_entities, SharedComponent3);
                    _context.UpdateComponents(_entities, SharedComponent4);
                    break;

                case ComponentArrangement.Normal_Mx4:
                    _context.UpdateComponents(_entities, ManageComponent1);
                    _context.UpdateComponents(_entities, ManageComponent2);
                    _context.UpdateComponents(_entities, ManageComponent3);
                    _context.UpdateComponents(_entities, ManageComponent4);
                    break;

                case ComponentArrangement.Shared_Mx4:
                    _context.UpdateComponents(_entities, ManageSharedComponent1);
                    _context.UpdateComponents(_entities, ManageSharedComponent2);
                    _context.UpdateComponents(_entities, ManageSharedComponent3);
                    _context.UpdateComponents(_entities, ManageSharedComponent4);
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    _context.UpdateComponents(_entities, Component1);
                    _context.UpdateComponents(_entities, Component2);
                    _context.UpdateComponents(_entities, ManageComponent1);
                    _context.UpdateComponents(_entities, ManageComponent2);
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    _context.UpdateComponents(_entities, SharedComponent1);
                    _context.UpdateComponents(_entities, SharedComponent2);
                    _context.UpdateComponents(_entities, ManageSharedComponent1);
                    _context.UpdateComponents(_entities, ManageSharedComponent2);
                    break;
            }
        }

        [Benchmark]
        public void UpdateComponents_EntityArcheType()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    _context.UpdateComponents(_archeType1, Component1);
                    _context.UpdateComponents(_archeType2, Component2);
                    _context.UpdateComponents(_archeType3, Component3);
                    _context.UpdateComponents(_archeType4, Component4);
                    break;

                case ComponentArrangement.Shared_Bx4:
                    _context.UpdateComponents(_archeType1, SharedComponent1);
                    _context.UpdateComponents(_archeType2, SharedComponent2);
                    _context.UpdateComponents(_archeType3, SharedComponent3);
                    _context.UpdateComponents(_archeType4, SharedComponent4);
                    break;

                case ComponentArrangement.Normal_Mx4:
                    _context.UpdateComponents(_archeType1, ManageComponent1);
                    _context.UpdateComponents(_archeType2, ManageComponent2);
                    _context.UpdateComponents(_archeType3, ManageComponent3);
                    _context.UpdateComponents(_archeType4, ManageComponent4);
                    break;

                case ComponentArrangement.Shared_Mx4:
                    _context.UpdateComponents(_archeType1, ManageSharedComponent1);
                    _context.UpdateComponents(_archeType2, ManageSharedComponent2);
                    _context.UpdateComponents(_archeType3, ManageSharedComponent3);
                    _context.UpdateComponents(_archeType4, ManageSharedComponent4);
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    _context.UpdateComponents(_archeType1, Component1);
                    _context.UpdateComponents(_archeType2, Component2);
                    _context.UpdateComponents(_archeType3, ManageComponent1);
                    _context.UpdateComponents(_archeType4, ManageComponent2);
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    _context.UpdateComponents(_archeType1, SharedComponent1);
                    _context.UpdateComponents(_archeType2, SharedComponent2);
                    _context.UpdateComponents(_archeType3, ManageSharedComponent1);
                    _context.UpdateComponents(_archeType4, ManageSharedComponent2);
                    break;
            }
        }

        [Benchmark]
        public void UpdateComponents_EntityQuery()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    _context.UpdateComponents(_query1, Component1);
                    _context.UpdateComponents(_query2, Component2);
                    _context.UpdateComponents(_query3, Component3);
                    _context.UpdateComponents(_query4, Component4);
                    break;

                case ComponentArrangement.Shared_Bx4:
                    _context.UpdateComponents(_query1, SharedComponent1);
                    _context.UpdateComponents(_query2, SharedComponent2);
                    _context.UpdateComponents(_query3, SharedComponent3);
                    _context.UpdateComponents(_query4, SharedComponent4);
                    break;

                case ComponentArrangement.Normal_Mx4:
                    _context.UpdateComponents(_query1, ManageComponent1);
                    _context.UpdateComponents(_query2, ManageComponent2);
                    _context.UpdateComponents(_query3, ManageComponent3);
                    _context.UpdateComponents(_query4, ManageComponent4);
                    break;

                case ComponentArrangement.Shared_Mx4:
                    _context.UpdateComponents(_query1, ManageSharedComponent1);
                    _context.UpdateComponents(_query2, ManageSharedComponent2);
                    _context.UpdateComponents(_query3, ManageSharedComponent3);
                    _context.UpdateComponents(_query4, ManageSharedComponent4);
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    _context.UpdateComponents(_query1, Component1);
                    _context.UpdateComponents(_query2, Component2);
                    _context.UpdateComponents(_query3, ManageComponent1);
                    _context.UpdateComponents(_query4, ManageComponent2);
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    _context.UpdateComponents(_query1, SharedComponent1);
                    _context.UpdateComponents(_query2, SharedComponent2);
                    _context.UpdateComponents(_query3, ManageSharedComponent1);
                    _context.UpdateComponents(_query4, ManageSharedComponent2);
                    break;
            }
        }

        private void CreateArcheTypes()
        {
            switch (CompArr)
            {
                case ComponentArrangement.Normal_Bx4:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(Component1);
                    _archeType3 = BlueprintUpdateAndArcheType(Component2);
                    _archeType4 = BlueprintUpdateAndArcheType(Component3);
                    break;

                case ComponentArrangement.Shared_Bx4:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(SharedComponent1);
                    _archeType3 = BlueprintUpdateAndArcheType(SharedComponent2);
                    _archeType4 = BlueprintUpdateAndArcheType(SharedComponent3);
                    break;

                case ComponentArrangement.Normal_Mx4:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(ManageComponent1);
                    _archeType3 = BlueprintUpdateAndArcheType(ManageComponent2);
                    _archeType4 = BlueprintUpdateAndArcheType(ManageComponent3);
                    break;

                case ComponentArrangement.Shared_Mx4:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(ManageSharedComponent1);
                    _archeType3 = BlueprintUpdateAndArcheType(ManageSharedComponent2);
                    _archeType4 = BlueprintUpdateAndArcheType(ManageSharedComponent3);
                    break;

                case ComponentArrangement.Normal_Bx2_Mx2:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(Component1);
                    _archeType3 = BlueprintUpdateAndArcheType(Component2);
                    _archeType4 = BlueprintUpdateAndArcheType(ManageComponent1);
                    break;

                case ComponentArrangement.Shared_Bx2_Mx2:
                    _archeType1 = _blueprint.GetEntityArcheType();
                    _archeType2 = BlueprintUpdateAndArcheType(SharedComponent1);
                    _archeType3 = BlueprintUpdateAndArcheType(SharedComponent2);
                    _archeType4 = BlueprintUpdateAndArcheType(ManageSharedComponent1);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            _query1 = new EntityQuery().WhereAllOf(_archeType1);
            _query2 = new EntityQuery().WhereAllOf(_archeType2);
            _query3 = new EntityQuery().WhereAllOf(_archeType3);
            _query4 = new EntityQuery().WhereAllOf(_archeType4);
        }
    }
}