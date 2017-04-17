using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public static class FileSystemExtensions
	{
		public static async Task<IEnumerable<string>> ReadFileLines(this IFileSystem fileSystem, string path)
		{
			return ReadLines(await fileSystem.ReadFile(path));
		}

		private static IEnumerable<string> ReadLines(Stream stream)
		{
			using (stream)
			using (var sr = new StreamReader(stream))
			{
				string line = null;
				while ((line = sr.ReadLine()) != null)
					yield return line;
			}
		}
	}
}