using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.Misc
{
    [TestClass]
    public class SimpleRealWorldTests : BasePrePostTest
    {
        private EntityBlueprint _spawnBlueprint;
        private EntityFilter _spawnedFilter;
        private EntityQuery _query;
        private List<Entity> _despawnEntities;
        private int _minY;
        private int _maxY;
        private int _spawnPerFrame = 1;
        private int _maxSpawn = 1000;
        private int _runMaxCount = UnitTestConsts.MediumCount;

        [TestMethod]
        public void Test()
        {
            _spawnBlueprint = new EntityBlueprint()
                .SetComponent(new PositionComponent())
                .SetComponent(new OmegaComponent { IsNew = 1 })
                .SetComponent(new SpeedComponent { Speed = 1 });
            _spawnedFilter = Context.Filters
                .WhereAllOf<OmegaComponent>();
            _query = Context.Queries
                .SetFilter(Context.Filters.WhereAllOf<PositionComponent>());
            _despawnEntities = new List<Entity>();
            _minY = 0;
            _maxY = 1000;

            for (var runCount = 0; runCount < _runMaxCount; runCount++)
            {
                // Despawn
                _query.ForEach((int index, Entity entity, in PositionComponent position) =>
                    {
                        if (position.y > _maxY)
                            _despawnEntities.Add(entity);
                    })
                    .Run();
                if (_despawnEntities.Count > 0)
                {
                    for (var i = 0; i < _despawnEntities.Count; i++)
                        Context.Entities.DestroyEntity(_despawnEntities[i]);
                    _despawnEntities.Clear();
                }

                // Spawn
                var availSpawn = Math.Min(_spawnPerFrame, _maxSpawn - Context.Entities.EntityCount());
                if (availSpawn > 0)
                    Context.Entities.CreateEntities(_spawnBlueprint, availSpawn);

                // Move
                var deltaTime = 1;
                _query.ForEach((int index, Entity entity, ref PositionComponent position, in SpeedComponent speed, in OmegaComponent omega) =>
                    {
                        if (omega.IsNew == 1)
                        {
                            position.x = entity.Id;
                            position.y = _minY;
                        }
                        else
                        {
                            position.y += speed.Speed * deltaTime;
                            Assert.IsTrue(position.x == entity.Id,
                                $"Move Entity: {entity}, Position.x: {position.x}, RunCount: {runCount}");
                        }
                    })
                    .Run();

                // Clean
                Context.Entities.UpdateComponents(_spawnedFilter, new OmegaComponent());

                // Draw
                _query.ForEach((int index, Entity entity, in PositionComponent position) =>
                    {
                        Assert.IsTrue(position.x == entity.Id,
                            $"Draw Entity: {entity}, Position.x: {position.x}, RunCount: {runCount}");
                    })
                    .Run();
            }
        }
    }

    public struct PositionComponent : IGeneralComponent
    {
        public int x;
        public int y;
    }

    public struct OmegaComponent : IGeneralComponent
    {
        public byte IsNew;
    }

    public struct SpeedComponent : IGeneralComponent
    {
        public int Speed;
    }
}
