using EcsLte.Exceptions;
using EcsLte.Utilities;
using System;
using System.Collections.Generic;

namespace EcsLte
{
    public class EntityFilter : IEquatable<EntityFilter>
    {
        private Data _data;

        public EcsContext Context => _data.Context;
        public ComponentConfig[] AllOfConfigs => _data.AllOfConfigs;
        public ComponentConfig[] AnyOfConfigs => _data.AnyOfConfigs;
        public ComponentConfig[] NoneOfConfigs => _data.NoneOfConfigs;
        public ISharedComponent[] FilterByComponents => _data.FilterByComponents;
        internal SharedDataIndex[] SharedDataIndexes => _data.SharedDataIndexes;
        internal ISharedComponentData[] FilterComponentDatas => _data.FilterComponentDatas;
        internal int ConfigCount => _data.AllOfConfigs.Length;
        internal ArcheTypeData[] ArcheTypeDatas
        {
            get => _data.ArcheTypeDatas;
            set => _data.ArcheTypeDatas = value;
        }
        internal ChangeVersion ArcheTypeDataVersion
        {
            get => _data.ArcheTypeDataVersion;
            set => _data.ArcheTypeDataVersion = value;
        }

        internal EntityFilter(EcsContext context) => _data = new Data(context);

        internal EntityFilter(EntityFilter filter) => _data = new Data(filter._data)
        {
            ArcheTypeDatas = filter._data.ArcheTypeDatas,
            ArcheTypeDataVersion = filter._data.ArcheTypeDataVersion,
            HashCode = filter._data.HashCode,
        };

        #region WhereOfHas

        public bool HasWhereOf<TComponent>()
            where TComponent : IComponent
            => HasWhereOf(ComponentConfig<TComponent>.Config);

        public bool HasWhereOf(ComponentConfig config)
            => HasWhereAllOf(config) ||
                HasWhereAnyOf(config) ||
                HasWhereNoneOf(config);

        public bool HasWhereAllOf<TComponent>()
            where TComponent : IComponent
            => HasWhereAllOf(ComponentConfig<TComponent>.Config);

        public bool HasWhereAllOf(ComponentConfig config)
        {
            Context.AssertContext();

            for (var i = 0; i < _data.AllOfConfigs.Length; i++)
            {
                if (_data.AllOfConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasWhereAnyOf<TComponent>()
            where TComponent : IComponent
            => HasWhereAnyOf(ComponentConfig<TComponent>.Config);

        public bool HasWhereAnyOf(ComponentConfig config)
        {
            Context.AssertContext();

            for (var i = 0; i < _data.AnyOfConfigs.Length; i++)
            {
                if (_data.AnyOfConfigs[i] == config)
                    return true;
            }

            return false;
        }

        public bool HasWhereNoneOf<TComponent>()
            where TComponent : IComponent
            => HasWhereNoneOf(ComponentConfig<TComponent>.Config);

        public bool HasWhereNoneOf(ComponentConfig config)
        {
            Context.AssertContext();

            for (var i = 0; i < _data.NoneOfConfigs.Length; i++)
            {
                if (_data.NoneOfConfigs[i] == config)
                    return true;
            }

            return false;
        }

        #endregion

        #region WhereOf

        public EntityFilter WhereAllOf<TComponent>()
            where TComponent : IComponent
            => WhereAllOf(ComponentConfig<TComponent>.Config);

        public EntityFilter WhereAllOf(ComponentConfig config)
        {
            Context.AssertContext();

            if (!HasWhereAllOf(config))
            {
                AssertHasWhereAnyOf(config);
                AssertHasWhereNoneOf(config);

                _data = new Data(_data)
                {
                    AllOfConfigs = Helper.CopyInsertSort(_data.AllOfConfigs, config)
                };
            }

            return this;
        }

        public EntityFilter WhereAllOf(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var configs = new HashSet<ComponentConfig>(_data.AllOfConfigs);
            for (var i = 0; i < archeType.AllConfigs.Length; i++)
            {
                var config = archeType.AllConfigs[i];
                AssertHasWhereAnyOf(config);
                AssertHasWhereNoneOf(config);

                configs.Add(config);
            }

            if (configs.Count != _data.AllOfConfigs.Length)
            {
                _data = new Data(_data)
                {
                    AllOfConfigs = new ComponentConfig[configs.Count]
                };
                configs.CopyTo(_data.AllOfConfigs);
                Array.Sort(_data.AllOfConfigs);
            }

            return this;
        }

        internal void WhereAllOf(EntityTracker tracker)
        {
            _data = new Data(_data)
            {
                AllOfConfigs = new ComponentConfig[tracker.TrackingGeneralConfigs.Count +
                    tracker.TrackingManagedConfigs.Count +
                    tracker.TrackingSharedConfigs.Count]
            };
            tracker.TrackingGeneralConfigs.CopyTo(_data.AllOfConfigs, 0);
            tracker.TrackingManagedConfigs.CopyTo(_data.AllOfConfigs, tracker.TrackingGeneralConfigs.Count);
            tracker.TrackingSharedConfigs.CopyTo(_data.AllOfConfigs, tracker.TrackingGeneralConfigs.Count +
                tracker.TrackingManagedConfigs.Count);

            Array.Sort(_data.AllOfConfigs);
        }

        internal void RemoveAnyOf(ComponentConfig config)
        {
            var anyOfIndex = -1;
            for (var i = 0; i < _data.AnyOfConfigs.Length; i++)
            {
                if (_data.AnyOfConfigs[i] == config)
                {
                    anyOfIndex = i;
                    break;
                }
            }

            if (anyOfIndex == -1)
                return;

            var data = new Data(_data);
            if (data.AnyOfConfigs.Length == 1)
                data.AnyOfConfigs = new ComponentConfig[0];
            else
            {
                data.AnyOfConfigs = new ComponentConfig[data.AnyOfConfigs.Length - 1];
                if (anyOfIndex > 0)
                    Helper.ArrayCopy(_data.AnyOfConfigs, 0, data.AnyOfConfigs, 0, anyOfIndex);
                if (anyOfIndex != _data.AnyOfConfigs.Length - 1)
                    Helper.ArrayCopy(_data.AnyOfConfigs, anyOfIndex + 1, data.AnyOfConfigs, anyOfIndex, (_data.AnyOfConfigs.Length - 1) - anyOfIndex);
            }

            _data = data;
        }

        public EntityFilter WhereAnyOf<TComponent>()
            where TComponent : IComponent
            => WhereAnyOf(ComponentConfig<TComponent>.Config);

        public EntityFilter WhereAnyOf(ComponentConfig config)
        {
            Context.AssertContext();

            if (!HasWhereAnyOf(config))
            {
                AssertHasWhereAllOf(config);
                AssertHasWhereNoneOf(config);

                _data = new Data(_data)
                {
                    AnyOfConfigs = Helper.CopyInsertSort(_data.AnyOfConfigs, config)
                };
            }

            return this;
        }

        public EntityFilter WhereAnyOf(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var configs = new HashSet<ComponentConfig>(_data.AnyOfConfigs);
            for (var i = 0; i < archeType.AllConfigs.Length; i++)
            {
                var config = archeType.AllConfigs[i];
                AssertHasWhereAllOf(config);
                AssertHasWhereNoneOf(config);

                configs.Add(config);
            }

            if (configs.Count != _data.AnyOfConfigs.Length)
            {
                _data = new Data(_data)
                {
                    AnyOfConfigs = new ComponentConfig[configs.Count]
                };
                configs.CopyTo(_data.AnyOfConfigs);
                Array.Sort(_data.AnyOfConfigs);
            }

            return this;
        }

        internal void WhereAnyOf(EntityTracker tracker)
        {
            _data = new Data(_data)
            {
                AnyOfConfigs = new ComponentConfig[tracker.TrackingGeneralConfigs.Count +
                    tracker.TrackingManagedConfigs.Count +
                    tracker.TrackingSharedConfigs.Count]
            };
            tracker.TrackingGeneralConfigs.CopyTo(_data.AnyOfConfigs, 0);
            tracker.TrackingManagedConfigs.CopyTo(_data.AnyOfConfigs, tracker.TrackingGeneralConfigs.Count);
            tracker.TrackingSharedConfigs.CopyTo(_data.AnyOfConfigs, tracker.TrackingGeneralConfigs.Count +
                tracker.TrackingManagedConfigs.Count);

            Array.Sort(_data.AnyOfConfigs);
        }

        public EntityFilter WhereNoneOf<TComponent>()
            where TComponent : IComponent
            => WhereNoneOf(ComponentConfig<TComponent>.Config);

        public EntityFilter WhereNoneOf(ComponentConfig config)
        {
            Context.AssertContext();

            if (!HasWhereNoneOf(config))
            {
                AssertHasWhereAllOf(config);
                AssertHasWhereAnyOf(config);

                _data = new Data(_data)
                {
                    NoneOfConfigs = Helper.CopyInsertSort(_data.NoneOfConfigs, config)
                };
            }

            return this;
        }

        public EntityFilter WhereNoneOf(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var configs = new HashSet<ComponentConfig>(_data.NoneOfConfigs);
            for (var i = 0; i < archeType.AllConfigs.Length; i++)
            {
                var config = archeType.AllConfigs[i];
                AssertHasWhereAllOf(config);
                AssertHasWhereAnyOf(config);

                configs.Add(config);
            }

            if (configs.Count != _data.NoneOfConfigs.Length)
            {
                _data = new Data(_data)
                {
                    NoneOfConfigs = new ComponentConfig[configs.Count]
                };
                configs.CopyTo(_data.NoneOfConfigs);
                Array.Sort(_data.NoneOfConfigs);
            }

            return this;
        }

        #endregion

        #region FilterByHas

        public bool HasFilterBy<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.FilterComponentDatas.Length; i++)
            {
                if (_data.FilterComponentDatas[i].Config == config)
                    return true;
            }

            return false;
        }

        #endregion

        #region FilterBy

        public TComponent GetFilterBy<TComponent>()
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var config = ComponentConfig<TComponent>.Config;
            for (var i = 0; i < _data.FilterComponentDatas.Length; i++)
            {
                if (_data.FilterComponentDatas[i].Config == config)
                    return (TComponent)_data.FilterComponentDatas[i].Component;
            }

            throw new EntityFilterNotFilterByException(config.ComponentType);
        }

        public EntityFilter FilterBy<TComponent>(TComponent component)
            where TComponent : unmanaged, ISharedComponent
        {
            Context.AssertContext();

            var componentData = new SharedComponentData<TComponent>(component);

            AssertHasWhereAnyOf(componentData.Config);
            AssertHasWhereNoneOf(componentData.Config);

            var data = new Data(_data);

            if (HasFilterBy<TComponent>())
            {
                data.AllOfConfigs = new ComponentConfig[_data.AllOfConfigs.Length];
                Helper.ArrayCopy(_data.AllOfConfigs, data.AllOfConfigs, data.AllOfConfigs.Length);

                data.SharedDataIndexes = new SharedDataIndex[_data.SharedDataIndexes.Length];
                Helper.ArrayCopy(_data.SharedDataIndexes, data.SharedDataIndexes, data.SharedDataIndexes.Length);

                data.FilterByComponents = new ISharedComponent[_data.FilterByComponents.Length];
                Helper.ArrayCopy(_data.FilterByComponents, data.FilterByComponents, data.FilterByComponents.Length);

                data.FilterComponentDatas = new ISharedComponentData[_data.FilterComponentDatas.Length];
                Helper.ArrayCopy(_data.FilterComponentDatas, data.FilterComponentDatas, data.FilterComponentDatas.Length);

                for (var i = 0; i < data.FilterComponentDatas.Length; i++)
                {
                    if (data.FilterComponentDatas[i].Config == componentData.Config)
                    {
                        data.FilterByComponents[i] = component;
                        data.SharedDataIndexes[i] = componentData
                            .GetSharedComponentDataIndex(Context.SharedComponentDics);
                        data.FilterComponentDatas[i] = componentData;
                        break;
                    }
                }
            }
            else
            {
                data.AllOfConfigs = Helper.CopyInsertSort(_data.AllOfConfigs, componentData.Config);
                data.FilterComponentDatas = Helper.CopyInsertSort(_data.FilterComponentDatas, componentData);

                data.SharedDataIndexes = new SharedDataIndex[data.FilterComponentDatas.Length];
                data.FilterByComponents = new ISharedComponent[data.FilterComponentDatas.Length];
                for (var i = 0; i < data.FilterComponentDatas.Length; i++)
                {
                    data.SharedDataIndexes[i] = data.FilterComponentDatas[i]
                        .GetSharedComponentDataIndex(Context.SharedComponentDics);
                    data.FilterByComponents[i] = data.FilterComponentDatas[i].Component;
                }
            }

            _data = data;

            return this;
        }

        public EntityFilter FilterBy(EntityArcheType archeType)
        {
            Context.AssertContext();
            EntityArcheType.AssertEntityArcheType(archeType, Context);

            var configs = new HashSet<ComponentConfig>(_data.AllOfConfigs);
            var filterDatas = new List<ISharedComponentData>(_data.FilterComponentDatas);
            for (var i = 0; i < archeType.AllConfigs.Length; i++)
            {
                var sharedData = archeType.SharedComponentDatas[i];
                AssertHasWhereAnyOf(sharedData.Config);
                AssertHasWhereNoneOf(sharedData.Config);

                configs.Add(sharedData.Config);

                var replaced = false;
                for (var j = 0; j < _data.FilterComponentDatas.Length; j++)
                {
                    if (filterDatas[j].Config == sharedData.Config)
                    {
                        filterDatas[j] = sharedData;
                        replaced = true;
                        break;
                    }
                }

                if (!replaced)
                    filterDatas.Add(sharedData);
            }
            filterDatas.Sort();

            var data = new Data(_data)
            {
                AllOfConfigs = new ComponentConfig[configs.Count]
            };
            configs.CopyTo(data.AllOfConfigs);

            data.FilterComponentDatas = filterDatas.ToArray();
            data.FilterByComponents = new ISharedComponent[data.FilterComponentDatas.Length];
            for (var i = 0; i < data.FilterComponentDatas.Length; i++)
                data.FilterByComponents[i] = data.FilterComponentDatas[i].Component;

            _data = data;

            return this;
        }

        #endregion

        #region Equals

        public static bool operator !=(EntityFilter lhs, EntityFilter rhs)
            => !(lhs == rhs);

        public static bool operator ==(EntityFilter lhs, EntityFilter rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;

            if (lhs._data.Context != rhs._data.Context ||
                lhs._data.AllOfConfigs.Length != rhs._data.AllOfConfigs.Length ||
                lhs._data.AnyOfConfigs.Length != rhs._data.AnyOfConfigs.Length ||
                lhs._data.NoneOfConfigs.Length != rhs._data.NoneOfConfigs.Length ||
                lhs._data.FilterComponentDatas.Length != rhs._data.FilterComponentDatas.Length)
            {
                return false;
            }

            for (var i = 0; i < lhs._data.FilterComponentDatas.Length; i++)
            {
                if (!lhs._data.FilterComponentDatas[i].Equals(rhs.FilterComponentDatas[i]))
                    return false;
            }
            for (var i = 0; i < lhs._data.AllOfConfigs.Length; i++)
            {
                if (lhs._data.AllOfConfigs[i] != rhs._data.AllOfConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs._data.AnyOfConfigs.Length; i++)
            {
                if (lhs._data.AnyOfConfigs[i] != rhs._data.AnyOfConfigs[i])
                    return false;
            }
            for (var i = 0; i < lhs._data.NoneOfConfigs.Length; i++)
            {
                if (lhs._data.NoneOfConfigs[i] != rhs._data.NoneOfConfigs[i])
                    return false;
            }

            return true;
        }

        public bool Equals(EntityFilter other)
            => this == other;

        public override bool Equals(object other)
            => other is EntityFilter obj && this == obj;

        public override int GetHashCode()
        {
            if (_data.HashCode == 0)
            {
                var hashCode = HashCodeHelper.StartHashCode()
                    .AppendHashCode(_data.AllOfConfigs.Length)
                    .AppendHashCode(_data.AnyOfConfigs.Length)
                    .AppendHashCode(_data.NoneOfConfigs.Length)
                    .AppendHashCode(_data.FilterComponentDatas.Length);
                for (var i = 0; i < _data.AllOfConfigs.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.AllOfConfigs[i]);
                for (var i = 0; i < _data.AnyOfConfigs.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.AnyOfConfigs[i]);
                for (var i = 0; i < _data.NoneOfConfigs.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.NoneOfConfigs[i]);
                for (var i = 0; i < _data.FilterComponentDatas.Length; i++)
                    hashCode = hashCode.AppendHashCode(_data.FilterComponentDatas[i]);
                _data.HashCode = hashCode.HashCode;
            }

            return _data.HashCode;
        }

        #endregion

        internal bool IsFiltered(ArcheType archeType)
            => IsFilteredAllConfigs(archeType) &&
                IsFilteredAnyConfigs(archeType) &&
                IsFilteredNoneConfigs(archeType) &&
                IsFilteredFilterBy(archeType);

        private unsafe bool IsFilteredAllConfigs(ArcheType archeType)
        {
            for (var i = 0; i < _data.AllOfConfigs.Length; i++)
            {
                if (!archeType.HasConfig(_data.AllOfConfigs[i]))
                    return false;
            }

            return true;
        }

        private unsafe bool IsFilteredAnyConfigs(ArcheType archeType)
        {
            for (var i = 0; i < _data.AnyOfConfigs.Length; i++)
            {
                if (archeType.HasConfig(_data.AnyOfConfigs[i]))
                    return true;
            }

            return false || _data.AnyOfConfigs.Length == 0;
        }

        private unsafe bool IsFilteredNoneConfigs(ArcheType archeType)
        {
            for (var i = 0; i < _data.NoneOfConfigs.Length; i++)
            {
                if (archeType.HasConfig(_data.NoneOfConfigs[i]))
                    return false;
            }

            return true;
        }


        private unsafe bool IsFilteredFilterBy(ArcheType archeType)
        {
            for (var i = 0; i < _data.SharedDataIndexes.Length; i++)
            {
                if (!archeType.HasSharedDataIndex(_data.SharedDataIndexes[i]))
                    return false;
            }

            return true;
        }

        #region Assert

        internal static void AssertEntityFilter(EntityFilter filter, EcsContext context)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (filter.Context != context)
                throw new EcsContextNotSameException(filter.Context, context);
        }

        private void AssertHasWhereAllOf(ComponentConfig config)
        {
            for (var i = 0; i < _data.AllOfConfigs.Length; i++)
            {
                if (_data.AllOfConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereAllOfException(config.ComponentType);
            }
        }

        private void AssertHasWhereAnyOf(ComponentConfig config)
        {
            for (var i = 0; i < _data.AnyOfConfigs.Length; i++)
            {
                if (_data.AnyOfConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereAnyOfException(config.ComponentType);
            }
        }

        private void AssertHasWhereNoneOf(ComponentConfig config)
        {
            for (var i = 0; i < _data.NoneOfConfigs.Length; i++)
            {
                if (_data.NoneOfConfigs[i] == config)
                    throw new EntityFilterAlreadyHasWhereNoneOfException(config.ComponentType);
            }
        }

        #endregion

        private class Data
        {
            public ArcheTypeData[] ArcheTypeDatas;
            public ChangeVersion ArcheTypeDataVersion;
            public int HashCode;
            public EcsContext Context;
            public ComponentConfig[] AllOfConfigs;
            public ComponentConfig[] AnyOfConfigs;
            public ComponentConfig[] NoneOfConfigs;
            public SharedDataIndex[] SharedDataIndexes;
            public ISharedComponent[] FilterByComponents;
            public ISharedComponentData[] FilterComponentDatas;

            public Data(EcsContext context)
            {
                ArcheTypeDatas = new ArcheTypeData[0];
                //ArcheTypeDataVersion
                //HashCode
                Context = context;
                AllOfConfigs = new ComponentConfig[0];
                AnyOfConfigs = new ComponentConfig[0];
                NoneOfConfigs = new ComponentConfig[0];
                SharedDataIndexes = new SharedDataIndex[0];
                FilterByComponents = new ISharedComponent[0];
                FilterComponentDatas = new ISharedComponentData[0];
            }

            public Data(Data data)
            {
                ArcheTypeDatas = new ArcheTypeData[0];
                //ArcheTypeDataVersion
                //HashCode
                Context = data.Context;
                AllOfConfigs = data.AllOfConfigs;
                AnyOfConfigs = data.AnyOfConfigs;
                NoneOfConfigs = data.NoneOfConfigs;
                SharedDataIndexes = data.SharedDataIndexes;
                FilterByComponents = data.FilterByComponents;
                FilterComponentDatas = data.FilterComponentDatas;
            }
        }
    }
}
