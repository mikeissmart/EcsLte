using System;

namespace EcsLte
{
	public static class StringExtensions
	{
		public static string AddSuffix(this string str, string suffix) => str.EndsWith(suffix, StringComparison.Ordinal)
				? str
				: str + suffix;

		public static string RemoveComponentSuffix(this string str) => str.RemoveSuffix("Component");

		public static string RemoveSuffix(this string str, string suffix) => str.EndsWith(suffix, StringComparison.Ordinal)
				? str.Substring(0, str.Length - suffix.Length)
				: str;
	}
}