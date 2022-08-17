using EcsLte.Utilities;

namespace EcsLte
{
    public unsafe partial class EntityManager
    {
        public void GetComponents<T1, T2>(Entity entity,
            out T1 component1, out T2 component2)
            where T1 : IComponent
            where T2 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3, T4>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
            component4 = config4.Adapter.GetComponent<T4>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3, T4, T5>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
            component4 = config4.Adapter.GetComponent<T4>(entityData.EntityIndex, archeTypeData);
            component5 = config5.Adapter.GetComponent<T5>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
            component4 = config4.Adapter.GetComponent<T4>(entityData.EntityIndex, archeTypeData);
            component5 = config5.Adapter.GetComponent<T5>(entityData.EntityIndex, archeTypeData);
            component6 = config6.Adapter.GetComponent<T6>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6, T7>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6, out T7 component7)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);
            AssertNotHaveComponent(config7, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
            component4 = config4.Adapter.GetComponent<T4>(entityData.EntityIndex, archeTypeData);
            component5 = config5.Adapter.GetComponent<T5>(entityData.EntityIndex, archeTypeData);
            component6 = config6.Adapter.GetComponent<T6>(entityData.EntityIndex, archeTypeData);
            component7 = config7.Adapter.GetComponent<T7>(entityData.EntityIndex, archeTypeData);
        }

        public void GetComponents<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity,
            out T1 component1, out T2 component2, out T3 component3, out T4 component4,
            out T5 component5, out T6 component6, out T7 component7, out T8 component8)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
            where T5 : IComponent
            where T6 : IComponent
            where T7 : IComponent
            where T8 : IComponent
        {
            var config1 = ComponentConfig<T1>.Config;
            var config2 = ComponentConfig<T2>.Config;
            var config3 = ComponentConfig<T3>.Config;
            var config4 = ComponentConfig<T4>.Config;
            var config5 = ComponentConfig<T5>.Config;
            var config6 = ComponentConfig<T6>.Config;
            var config7 = ComponentConfig<T7>.Config;
            var config8 = ComponentConfig<T8>.Config;

            Helper.AssertDuplicateConfigs(
                config1,
                config2,
                config3,
                config4,
                config5,
                config6,
                config7,
                config8);

            Context.AssertContext();
            AssertNotExistEntity(entity,
                out var entityData, out var archeTypeData);

            AssertNotHaveComponent(config1, archeTypeData);
            AssertNotHaveComponent(config2, archeTypeData);
            AssertNotHaveComponent(config3, archeTypeData);
            AssertNotHaveComponent(config4, archeTypeData);
            AssertNotHaveComponent(config5, archeTypeData);
            AssertNotHaveComponent(config6, archeTypeData);
            AssertNotHaveComponent(config7, archeTypeData);
            AssertNotHaveComponent(config8, archeTypeData);

            component1 = config1.Adapter.GetComponent<T1>(entityData.EntityIndex, archeTypeData);
            component2 = config2.Adapter.GetComponent<T2>(entityData.EntityIndex, archeTypeData);
            component3 = config3.Adapter.GetComponent<T3>(entityData.EntityIndex, archeTypeData);
            component4 = config4.Adapter.GetComponent<T4>(entityData.EntityIndex, archeTypeData);
            component5 = config5.Adapter.GetComponent<T5>(entityData.EntityIndex, archeTypeData);
            component6 = config6.Adapter.GetComponent<T6>(entityData.EntityIndex, archeTypeData);
            component7 = config7.Adapter.GetComponent<T7>(entityData.EntityIndex, archeTypeData);
            component8 = config8.Adapter.GetComponent<T8>(entityData.EntityIndex, archeTypeData);
        }

        public TComponent[] GetComponents<TComponent>(EntityArcheType archeType)
            where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(archeType, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponents(archeType, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertNotHaveComponent(config, archeTypeData);

            Helper.AssertAndResizeArray(ref destComponents, destStartingIndex, archeTypeData.EntityCount);

            archeTypeData.GetComponents(ref destComponents, destStartingIndex, config);
            return archeTypeData.EntityCount;
        }

        public TComponent[] GetComponents<TComponent>(EntityFilter filter)
            where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(filter, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponents(filter, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var componentIndex = destStartingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.EntityCount > 0 &&
                    archeTypeData.HasConfig(config))
                {
                    Helper.ResizeRefArray(ref destComponents, componentIndex, archeTypeData.EntityCount);
                    archeTypeData.GetComponents(ref destComponents, destStartingIndex, config);
                    componentIndex += archeTypeData.EntityCount;
                }
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetComponents<TComponent>(EntityTracker tracker)
            where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(tracker, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponents(tracker, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var trackedArcheTypeDatas = tracker.CachedArcheTypeDatas;
            var componentIndex = destStartingIndex;
            for (var i = 0; i < trackedArcheTypeDatas.Length; i++)
            {
                componentIndex += InternalGetComponentsTracker(tracker, trackedArcheTypeDatas[i],
                    ref destComponents, componentIndex);
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetComponents<TComponent>(EntityQuery query)
            where TComponent : unmanaged, IGeneralComponent
        {
            var components = new TComponent[0];
            GetComponents(query, ref components, 0);

            return components;
        }

        public int GetComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, IGeneralComponent
            => GetComponents(query, ref destComponents, 0);

        public int GetComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, IGeneralComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                return GetComponents(query.Filter, ref destComponents, destStartingIndex);
            else if (query.Filter == null && query.Tracker != null)
                return GetComponents(query.Tracker, ref destComponents, destStartingIndex);
            else if (query.Filter != null && query.Tracker != null)
            {
                Helper.AssertArray(destComponents, destStartingIndex);

                var filteredArcheTypeDatas = query.Filter.ArcheTypeDatas;
                var componentIndex = destStartingIndex;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    componentIndex += InternalGetComponentsTracker(query.Tracker, filteredArcheTypeDatas[i],
                        ref destComponents, componentIndex);
                }

                return componentIndex - destStartingIndex;
            }

            return 0;
        }

        public TComponent[] GetManagedComponents<TComponent>(EntityArcheType archeType)
            where TComponent : IManagedComponent
        {
            var components = new TComponent[0];
            GetManagedComponents(archeType, ref components, 0);

            return components;
        }

        public int GetManagedComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents)
            where TComponent : IManagedComponent
            => GetManagedComponents(archeType, ref destComponents, 0);

        public int GetManagedComponents<TComponent>(EntityArcheType archeType,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeData = Context.ArcheTypes.GetArcheTypeData(archeType);

            AssertNotHaveComponent(config, archeTypeData);

            Helper.AssertAndResizeArray(ref destComponents, destStartingIndex, archeTypeData.EntityCount);

            archeTypeData.GetManagedComponents(ref destComponents, destStartingIndex, config);
            return archeTypeData.EntityCount;
        }

        public TComponent[] GetManagedComponents<TComponent>(EntityFilter filter)
            where TComponent : IManagedComponent
        {
            var components = new TComponent[0];
            GetManagedComponents(filter, ref components, 0);

            return components;
        }

        public int GetManagedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents)
            where TComponent : IManagedComponent
            => GetManagedComponents(filter, ref destComponents, 0);

        public int GetManagedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var componentIndex = destStartingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.EntityCount > 0 &&
                    archeTypeData.HasConfig(config))
                {
                    Helper.ResizeRefArray(ref destComponents, componentIndex, archeTypeData.EntityCount);
                    archeTypeData.GetManagedComponents(ref destComponents, destStartingIndex, config);
                    componentIndex += archeTypeData.EntityCount;
                }
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetManagedComponents<TComponent>(EntityTracker tracker)
            where TComponent : IManagedComponent
        {
            var components = new TComponent[0];
            GetManagedComponents(tracker, ref components, 0);

            return components;
        }

        public int GetManagedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents)
            where TComponent : IManagedComponent
            => GetManagedComponents(tracker, ref destComponents, 0);

        public int GetManagedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var trackedArcheTypeDatas = tracker.CachedArcheTypeDatas;
            var componentIndex = destStartingIndex;
            for (var i = 0; i < trackedArcheTypeDatas.Length; i++)
            {
                componentIndex += InternalGetManagedComponentsTracker(tracker, trackedArcheTypeDatas[i],
                    ref destComponents, componentIndex);
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetManagedComponents<TComponent>(EntityQuery query)
            where TComponent : IManagedComponent
        {
            var components = new TComponent[0];
            GetManagedComponents(query, ref components, 0);

            return components;
        }

        public int GetManagedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents)
            where TComponent : IManagedComponent
            => GetManagedComponents(query, ref destComponents, 0);

        public int GetManagedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : IManagedComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                return GetManagedComponents(query.Filter, ref destComponents, destStartingIndex);
            else if (query.Filter == null && query.Tracker != null)
                return GetManagedComponents(query.Tracker, ref destComponents, destStartingIndex);
            else if (query.Filter != null && query.Tracker != null)
            {
                Helper.AssertArray(destComponents, destStartingIndex);

                var filteredArcheTypeDatas = query.Filter.ArcheTypeDatas;
                var componentIndex = destStartingIndex;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    componentIndex += InternalGetManagedComponentsTracker(query.Tracker, filteredArcheTypeDatas[i],
                        ref destComponents, componentIndex);
                }

                return componentIndex - destStartingIndex;
            }

            return 0;
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityFilter filter)
            where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(filter, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent
            => GetSharedComponents(filter, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityFilter filter,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityFilter.AssertEntityFilter(filter, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var config = ComponentConfig<TComponent>.Config;
            var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(filter);
            var componentIndex = destStartingIndex;
            for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
            {
                var archeTypeData = filteredArcheTypeDatas[i];
                if (archeTypeData.HasConfig(config))
                {
                    Helper.ResizeRefArray(ref destComponents, componentIndex, 1);
                    destComponents[componentIndex++] = archeTypeData.GetSharedComponent<TComponent>(config);
                }
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityTracker tracker)
            where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(tracker, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent
            => GetSharedComponents(tracker, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityTracker tracker,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityTracker.AssertEntityTracker(tracker, Context);
            Helper.AssertArray(destComponents, destStartingIndex);

            var config = ComponentConfig<TComponent>.Config;
            var archeTypeDatas = tracker.CachedArcheTypeDatas;
            var componentIndex = destStartingIndex;
            for (var i = 0; i < archeTypeDatas.Length; i++)
            {
                var archeTypeData = archeTypeDatas[i];
                if (archeTypeData.HasConfig(config))
                {
                    Helper.ResizeRefArray(ref destComponents, componentIndex, 1);
                    destComponents[componentIndex++] = archeTypeData.GetSharedComponent<TComponent>(config);
                }
            }

            return componentIndex - destStartingIndex;
        }

        public TComponent[] GetSharedComponents<TComponent>(EntityQuery query)
            where TComponent : unmanaged, ISharedComponent
        {
            var components = new TComponent[0];
            GetSharedComponents(query, ref components, 0);

            return components;
        }

        public int GetSharedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents)
            where TComponent : unmanaged, ISharedComponent
            => GetSharedComponents(query, ref destComponents, 0);

        public int GetSharedComponents<TComponent>(EntityQuery query,
            ref TComponent[] destComponents, int destStartingIndex)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();
            EntityQuery.AssertEntityQuery(query, Context);

            if (query.Filter != null && query.Tracker == null)
                return GetSharedComponents(query.Filter, ref destComponents, destStartingIndex);
            else if (query.Filter == null && query.Tracker != null)
                return GetSharedComponents(query.Tracker, ref destComponents, destStartingIndex);
            else if (query.Filter != null && query.Tracker != null)
            {
                Helper.AssertArray(destComponents, destStartingIndex);

                var config = ComponentConfig<TComponent>.Config;
                var filteredArcheTypeDatas = Context.ArcheTypes.GetArcheTypeDatas(query.Filter);
                var componentIndex = destStartingIndex;
                for (var i = 0; i < filteredArcheTypeDatas.Length; i++)
                {
                    var archeTypeData = filteredArcheTypeDatas[i];
                    if (query.Tracker.HasArcheTypeData(archeTypeData) &&
                        archeTypeData.HasConfig(config))
                    {
                        Helper.ResizeRefArray(ref destComponents, componentIndex, 1);
                        destComponents[componentIndex++] = archeTypeData.GetSharedComponent<TComponent>(config);
                    }
                }

                return componentIndex - destStartingIndex;
            }

            return 0;
        }
    }
}
