using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcsLte
{
	internal struct ComponentArcheType : IEquatable<ComponentArcheType>
	{
		public ComponentConfig[] ComponentConfigs { get; set; }
		public ShareComponentDataIndex[] ShareComponentDataIndexes { get; set; }

		public static ComponentArcheType AppendComponent(ComponentArcheType archeType, ComponentConfig config)
		{
			if (archeType.ComponentConfigs == null)
			{
				return new ComponentArcheType
				{
					ComponentConfigs = new[] { config },
					ShareComponentDataIndexes = new ShareComponentDataIndex[0]
				};
			}

			var newArcheType = new ComponentArcheType
			{
				ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
				ShareComponentDataIndexes = new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length]
			};

			Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
			Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

			newArcheType.ComponentConfigs[archeType.ComponentConfigs.Length] = config;
			Array.Sort(newArcheType.ComponentConfigs);

			return newArcheType;
		}

		public static ComponentArcheType AppendComponent(ComponentArcheType archeType, ComponentConfig config, int sharedComponentDataIndex)
		{
			if (archeType.ComponentConfigs == null)
			{
				return new ComponentArcheType
				{
					ComponentConfigs = new[] { config },
					ShareComponentDataIndexes = new ShareComponentDataIndex[0]
				};
			}

			var newArcheType = new ComponentArcheType
			{
				ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length + 1],
				ShareComponentDataIndexes = new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length + 1]
			};

			Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
			Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

			newArcheType.ComponentConfigs[archeType.ComponentConfigs.Length] = config;
			newArcheType.ShareComponentDataIndexes[archeType.ShareComponentDataIndexes.Length] = new ShareComponentDataIndex
			{
				SharedIndex = config.SharedIndex,
				SharedDataIndex = sharedComponentDataIndex
			};
			Array.Sort(newArcheType.ComponentConfigs);
			Array.Sort(newArcheType.ShareComponentDataIndexes);

			return newArcheType;
		}

		public static ComponentArcheType ReplaceSharedComponent(ComponentArcheType archeType, ComponentConfig config, int sharedComponentDataIndex)
		{
			var newArcheType = new ComponentArcheType
			{
				ComponentConfigs = new ComponentConfig[archeType.ComponentConfigs.Length],
				ShareComponentDataIndexes = new ShareComponentDataIndex[archeType.ShareComponentDataIndexes.Length]
			};

			Array.Copy(archeType.ComponentConfigs, newArcheType.ComponentConfigs, archeType.ComponentConfigs.Length);
			Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

			for (int i = 0; i < newArcheType.ShareComponentDataIndexes.Length; i++)
			{
				if (newArcheType.ShareComponentDataIndexes[i].SharedIndex == config.SharedIndex)
				{
					newArcheType.ShareComponentDataIndexes[i] = new ShareComponentDataIndex
					{
						SharedIndex = config.SharedIndex,
						SharedDataIndex = sharedComponentDataIndex
					};
					break;
				}
			}

			return newArcheType;
		}

		public static ComponentArcheType RemoveComponent(ComponentArcheType archeType, ComponentConfig config)
		{
			var newArcheType = new ComponentArcheType
			{
				ComponentConfigs = archeType.ComponentConfigs
					.Where(x => x != config)
					.ToArray()
			};

			if (config.IsShared)
			{
				newArcheType.ShareComponentDataIndexes = archeType.ShareComponentDataIndexes
					.Where(x => x.SharedIndex != config.SharedIndex)
					.ToArray();
			}
			else
				Array.Copy(archeType.ShareComponentDataIndexes, newArcheType.ShareComponentDataIndexes, archeType.ShareComponentDataIndexes.Length);

			return newArcheType;
		}

		public static bool operator !=(ComponentArcheType lhs, ComponentArcheType rhs) => !(lhs == rhs);

		public static bool operator ==(ComponentArcheType lhs, ComponentArcheType rhs)
		{
			if ((lhs.ComponentConfigs == null && rhs.ComponentConfigs != null) ||
				(lhs.ComponentConfigs != null && rhs.ComponentConfigs == null))
				return false;
			if (lhs.ComponentConfigs == null && rhs.ComponentConfigs == null)
				return true;
			if (lhs.ComponentConfigs.Length != rhs.ComponentConfigs.Length)
				return false;
			for (int i = 0; i < lhs.ComponentConfigs.Length; i++)
			{
				if (lhs.ComponentConfigs[i] != rhs.ComponentConfigs[i])
					return false;
			}

			if ((lhs.ShareComponentDataIndexes == null && rhs.ShareComponentDataIndexes != null) ||
				(lhs.ShareComponentDataIndexes != null && rhs.ShareComponentDataIndexes == null))
				return false;
			if (lhs.ShareComponentDataIndexes == null && rhs.ShareComponentDataIndexes == null)
				return true;
			if (lhs.ShareComponentDataIndexes.Length != rhs.ShareComponentDataIndexes.Length)
				return false;
			for (int i = 0; i < lhs.ShareComponentDataIndexes.Length; i++)
			{
				if (lhs.ShareComponentDataIndexes[i] != rhs.ShareComponentDataIndexes[i])
					return false;
			}

			return true;
		}

		public bool IsEmpty()
			=> ComponentConfigs == null || ComponentConfigs.Length == 0;

		public bool Equals(ComponentArcheType other)
			=> this == other;

		public override bool Equals(object other)
			=> other is ComponentArcheType obj && this == obj;

		public override int GetHashCode()
		{
			var hashCode = 1922561553;
			hashCode = hashCode * -1521134295 + (ComponentConfigs != null
				? EqualityComparer<ComponentConfig[]>.Default.GetHashCode(ComponentConfigs)
				: 0);
			hashCode = hashCode * -1521134295 + (ShareComponentDataIndexes != null
				? EqualityComparer<ShareComponentDataIndex[]>.Default.GetHashCode(ShareComponentDataIndexes)
				: 0);
			return hashCode;
		}
	}
}
