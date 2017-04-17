using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public static class FileSystemExtensions
	{
		public static async Task<IEnumerable<string>> ReadFileLines(this IFileSystem fileSystem, string path)
		{
			using (var stream = await fileSystem.ReadFile(path))
			using (var sr = new StreamReader(stream))
			{
				var lines = new List<string>();

				string line = null;
				while ((line = sr.ReadLine()) != null)
					lines.Add(line);

				return lines;
			}
		}
	}
}