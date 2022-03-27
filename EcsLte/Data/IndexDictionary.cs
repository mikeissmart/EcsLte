using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data
{
	internal interface IIndexDictionary
	{
		int GetIndexObj(object value);
		void Clear();
	}

	internal static class IndexDictionary
    {
		public static IIndexDictionary[] CreateSharedComponentIndexDictionaries()
        {
			var sharedIndexes = new IIndexDictionary[ComponentConfigs.Instance.SharedComponentCount];
			var indexDicType = typeof(IndexDictionary<>);
			for (int i = 0; i < sharedIndexes.Length; i++)
			{
				var sharedType = ComponentConfigs.Instance.AllSharedTypes[i];
				sharedIndexes[i] = (IIndexDictionary)Activator
					.CreateInstance(indexDicType.MakeGenericType(sharedType));
			}

			return sharedIndexes;
		}
	}

	internal class IndexDictionary<TValue> : IIndexDictionary
	{
		private readonly Dictionary<TValue, int> _indexes;
		private int _nextIndex;

		public IndexDictionary()
		{
			_indexes = new Dictionary<TValue, int>();
			_nextIndex = 1;
		}

		public int GetIndex(TValue value)
		{
			if (!_indexes.TryGetValue(value, out int index))
			{
				index = _nextIndex++;
				_indexes.Add(value, index);
			}
			return index;
		}

		public void RemoveValue(TValue value)
		{
			_indexes.Remove(value);
		}

		public int GetIndexObj(object value)
		{
			if (value is TValue val)
				return GetIndex(val);
			throw new InvalidCastException("value");
		}

		public void RemoveObj(object value)
		{
			if (value is TValue val)
				RemoveValue(val);
			else
				throw new InvalidCastException("value");
		}

		public void Clear()
		{
			_indexes.Clear();
			_nextIndex = 1;
		}
	}
}
