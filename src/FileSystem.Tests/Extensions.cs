using System.IO;
using System.Text;

namespace FileSystem.Tests
{
	public static class Extensions
	{
		public static Stream ToStream(this string text)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(text));
		}

		public static string ReadAsString(this Stream stream)
		{
			using (var sr = new StreamReader(stream))
				return sr.ReadToEnd();
		}
	}
}
