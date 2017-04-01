using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Tests
{
	public static class Extensions
	{
		public static Stream ToStream(this string text)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(text));
		}

		public static async Task WriteTo(this string text, Stream stream)
		{
			using (var sw = new StreamWriter(stream))
				await sw.WriteAsync(text);
		}

		public static string ReadAsString(this Stream stream)
		{
			using (var sr = new StreamReader(stream))
				return sr.ReadToEnd();
		}
	}
}
