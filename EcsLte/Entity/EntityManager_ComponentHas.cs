using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public bool HasComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var _, out var archeTypeData);

            return archeTypeData.ArcheType.HasConfig(ComponentConfig<TComponent>.Config);
        }

        public bool HasManagedComponent<TComponent>(Entity entity)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var _, out var archeTypeData);

            return archeTypeData.ArcheType.HasConfig(ComponentConfig<TComponent>.Config);
        }

        public bool HasSharedComponent<TComponent>(Entity entity)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var _, out var archeTypeData);

            return archeTypeData.ArcheType.HasConfig(ComponentConfig<TComponent>.Config);
        }
    }
}
