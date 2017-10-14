using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem
{
	public static class FileSystemExtensions
	{
		public static async Task<string> ReadFileText(this IFileSystem fileSystem, string path)
		{
			using (var stream = await fileSystem.ReadFile(path))
			using (var reader = new StreamReader(stream))
				return await reader.ReadToEndAsync();
		}

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

		public static async Task AppendFileLines(this IFileSystem fileSystem, string path, params string[] lines)
		{
			if (lines == null || lines.Any() == false)
				return;

			await fileSystem.AppendFile(path, async stream =>
			{
				using (var sw = new StreamWriter(stream))
					foreach (var line in lines)
						await sw.WriteLineAsync(line);
			});
		}

		public static async Task WriteFileText(this IFileSystem fileSystem, string path, string content)
		{
			await fileSystem.WriteFile(path, async stream =>
			{
				using (var sw = new StreamWriter(stream))
					await sw.WriteAsync(content);
			});
		}
	}
}
