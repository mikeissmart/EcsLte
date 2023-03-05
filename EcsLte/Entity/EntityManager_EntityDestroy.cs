using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void DestroyEntity(Entity entity)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            AssertNotExistEntity(entity, out var _, out var archeTypeData);
            ChangeVersion.IncVersion(ref _globalVersion);

            DeallocEntity(entity, archeTypeData);
        }

        public void DestroyEntities(in Entity[] entities)
            => DestroyEntities(entities, 0, entities?.Length ?? 0);

        public void DestroyEntities(in Entity[] entities, int startingIndex)
            => DestroyEntities(entities, startingIndex, (entities?.Length ?? 0) - startingIndex);

        public void DestroyEntities(in Entity[] entities, int startingIndex, int count)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            Helper.AssertArray(entities, startingIndex, count);

            for (var i = 0; i < count; i++, startingIndex++)
            {
                var entity = entities[startingIndex];
                AssertNotExistEntity(entity, out var _, out var archeTypeData);

                DeallocEntity(entity, archeTypeData);
            }

            if (count > 0)
                ChangeVersion.IncVersion(ref _globalVersion);
        }

        public void DestroyEntities(EntityArcheType archeType)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);
            if (archeTypeData.EntityCount > 0)
                ChangeVersion.IncVersion(ref _globalVersion);

            DealloArcheTypeDataEntities(archeTypeData);
        }

        public void DestroyEntities(EntityFilter filter)
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();
            EntityFilter.AssertEntityFilter(filter, Context);

            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                if (filteredArcheTypeDatas[i].EntityCount > 0)
                {
                    ChangeVersion.IncVersion(ref _globalVersion);
                    break;
                }
            }

            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                DealloArcheTypeDataEntities(filteredArcheTypeDatas[i]);
        }

        public void DestroyAllEntities()
        {
            Context.AssertContext();
            Context.AssertStructualChangeAvailable();

            if (_entitiesCount > 0)
                ChangeVersion.IncVersion(ref _globalVersion);

            _reusableEntitiesCount += Context.ArcheTypes.GetAndClearAllEntities(ref _reusableEntities, _reusableEntitiesCount);
            MemoryHelper.Clear(_entityDatas, _entitiesCount);
        }
    }
}
