using System;

namespace EcsLte.Exceptions
{
	public class EcsLteException : Exception
	{
		public EcsLteException(string message, string hint)
			: base(hint != null ? message + Environment.NewLine + hint : message)
		{
		}
	}
}