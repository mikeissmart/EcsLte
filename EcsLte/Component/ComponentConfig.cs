using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
	public struct ComponentConfig : IEquatable<ComponentConfig>, IComparable<ComponentConfig>
	{
		public int ComponentIndex { get; set; }
		public int RecordableIndex { get; set; }
		public int UniqueIndex { get; set; }
		public int SharedIndex { get; set; }
		public int UnmanagedInBytesSize { get; set; }
		public bool IsRecordable { get; set; }
		public bool IsUnique { get; set; }
		public bool IsShared { get; set; }
		public bool IsBlittable { get; set; }

		public static bool operator !=(ComponentConfig lhs, ComponentConfig rhs)
			=> !(lhs == rhs);

		public static bool operator ==(ComponentConfig lhs, ComponentConfig rhs)
			=> lhs.ComponentIndex == rhs.ComponentIndex;

		public int CompareTo(ComponentConfig other)
			=> ComponentIndex.CompareTo(other.ComponentIndex);

		public bool Equals(ComponentConfig other)
			=> this == other;

		public override bool Equals(object other)
			=> other is ComponentConfig obj && this == obj;

		public override int GetHashCode() => ComponentIndex.GetHashCode();
	}

	public class ComponentConfig<TComponent> where TComponent : IComponent
	{
		private static ComponentConfig _config;
		private static bool _hasConfig;

		internal static ComponentConfig Config
		{
			get
			{
				if (!_hasConfig)
				{
					_config = ComponentConfigs.Instance.GetConfig(typeof(TComponent));
					_hasConfig = true;
				}

				return _config;
			}
		}
	}
}
