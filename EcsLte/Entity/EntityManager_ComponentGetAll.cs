using EcsLte.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public IComponent[] GetAllComponents(Entity entity)
        {
            var components = new IComponent[0];
            GetAllComponents(entity, ref components, 0);

            return components;
        }

        public int GetAllComponents(Entity entity,
            ref IComponent[] destComponents)
            => GetAllComponents(entity, ref destComponents, 0);

        public int GetAllComponents(Entity entity,
            ref IComponent[] destComponents, int destStartingIndex)
        {
            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            Helper.AssertAndResizeComponents(ref destComponents,
                destStartingIndex, archeTypeData.ArcheType.ConfigsLength);

            archeTypeData.GetAllEntityComponents(entityData.EntityIndex,
                ref destComponents, destStartingIndex);

            return archeTypeData.ArcheType.ConfigsLength;
        }
    }
}
