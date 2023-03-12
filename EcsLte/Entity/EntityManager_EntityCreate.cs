using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public Entity CreateEntity()
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            ChangeVersion.IncVersion(ref _globalVersion);

            _cachedArcheType.ConfigsLength = 0;
            _cachedArcheType.SharedDataIndexesLength = 0;

            CheckAndAllocEntity(Context.ArcheTypes.GetArcheTypeData(_cachedArcheType), true,
                out var entity, out var _);

            return entity;
        }

        public Entity CreateEntity(EntityArcheType archeType)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);
            ChangeVersion.IncVersion(ref _globalVersion);

            CheckAndAllocEntity(Context.ArcheTypes.GetArcheTypeData(archeType), true,
                out var entity, out var _);

            return entity;
        }

        public Entity CreateEntity(EntityBlueprint blueprint)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityBlueprint.AssertEntityBlueprint(blueprint);
            ChangeVersion.IncVersion(ref _globalVersion);

            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(blueprint.GetArcheType(Context));
            CheckAndAllocEntity(archeTypeData, false,
                out var entity, out var entityData);

            for (var i = 0; i < blueprint.GeneralComponentDatas.Length; i++)
            {
                blueprint.GeneralComponentDatas[i]
                    .SetComponentData(archeTypeData, GlobalVersion, entityData);
            }
            for (var i = 0; i < blueprint.ManagedComponents.Length; i++)
            {
                blueprint.ManagedComponentDatas[i]
                    .SetComponentData(archeTypeData, GlobalVersion, entityData);
            }

            return entity;
        }

        public Entity[] CreateEntities(int count)
        {
            var entities = new Entity[0];
            CreateEntities(ref entities, count);

            return entities;
        }

        public void CreateEntities(ref Entity[] entities, int count) => CreateEntities(ref entities, 0, count);

        public void CreateEntities(ref Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            Helper.AssertAndResizeArray(ref entities, startingIndex, count);

            if (count > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);

                _cachedArcheType.ConfigsLength = 0;
                _cachedArcheType.SharedDataIndexesLength = 0;

                CheckAndAllocEntities(Context.ArcheTypes.GetArcheTypeData(_cachedArcheType), true,
                    ref entities, startingIndex, count);
            }
        }

        public Entity[] CreateEntities(EntityArcheType archeType, int count)
        {
            var entities = new Entity[0];
            CreateEntities(archeType, ref entities, count);

            return entities;
        }

        public void CreateEntities(EntityArcheType archeType, ref Entity[] entities, int count)
            => CreateEntities(archeType, ref entities, 0, count);

        public void CreateEntities(EntityArcheType archeType, ref Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);
            Helper.AssertAndResizeArray(ref entities, startingIndex, count);

            if (count > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);

                CheckAndAllocEntities(Context.ArcheTypes.GetArcheTypeData(archeType), true,
                    ref entities, startingIndex, count);
            }
        }

        public Entity[] CreateEntities(EntityBlueprint blueprint, int count)
        {
            var entities = new Entity[0];
            CreateEntities(blueprint, ref entities, count);

            return entities;
        }

        public void CreateEntities(EntityBlueprint blueprint, ref Entity[] entities, int count) => CreateEntities(blueprint, ref entities, 0, count);

        public void CreateEntities(EntityBlueprint blueprint, ref Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityBlueprint.AssertEntityBlueprint(blueprint);
            Helper.AssertAndResizeArray(ref entities, startingIndex, count);

            if (count > 0)
            {
                ChangeVersion.IncVersion(ref _globalVersion);

                var archeTypeData = Context.ArcheTypes.GetArcheTypeData(blueprint.GetArcheType(Context));
                var prevEntityIndex = archeTypeData.EntityCount();
                CheckAndAllocEntities(archeTypeData, false,
                    ref entities, startingIndex, count);

                for (var i = 0; i < blueprint.GeneralComponentDatas.Length; i++)
                {
                    blueprint.GeneralComponentDatas[i]
                        .SetComponentDatas(archeTypeData, GlobalVersion, prevEntityIndex, count);
                }
                for (var i = 0; i < blueprint.ManagedComponents.Length; i++)
                {
                    blueprint.ManagedComponentDatas[i]
                        .SetComponentDatas(archeTypeData, GlobalVersion, prevEntityIndex, count);
                }
            }
        }
    }
}
