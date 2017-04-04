using System.Text.RegularExpressions;

namespace FileSystem.Internal
{
	public static class Extensions
	{
		private static readonly Regex Expression = new Regex(
			@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])",
			RegexOptions.IgnorePatternWhitespace);

		public static string ToSentence(this string value)
		{
			return Expression.Replace(value, " ");
		}
	}
}
