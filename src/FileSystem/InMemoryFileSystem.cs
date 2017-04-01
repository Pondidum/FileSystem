using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem
{
	public class InMemoryFileSystem : IFileSystem
	{
		private readonly Dictionary<string, byte[]> _files;
		private readonly HashSet<string> _directories;

		public InMemoryFileSystem()
		{
			_files = new Dictionary<string, byte[]>();
			_directories = new HashSet<string>();
		}

		public Task<bool> FileExists(string path)
		{
			return Task.FromResult(_files.ContainsKey(path));
		}

		public async Task WriteFile(string path, Func<Stream, Task> write)
		{
			using (var ms = new MemoryStream())
			{
				await write(ms);
				_files[path] = ms.ToArray();
			}
		}

		public Task<Stream> ReadFile(string path)
		{
			byte[] contents;

			if (_files.TryGetValue(path, out contents))
				return Task.FromResult((Stream)new MemoryStream(contents));

			throw new FileNotFoundException("Cannot find file", path);
		}

		public Task DeleteFile(string path)
		{
			if (_files.ContainsKey(path))
				_files.Remove(path);

			return Task.CompletedTask;
		}

		public async Task CopyFile(string source, string destination)
		{
			using (var contents = await ReadFile(source))
				await WriteFile(destination, async stream => await contents.CopyToAsync(stream));
		}

		public async Task MoveFile(string source, string destination)
		{
			await CopyFile(source, destination);
			await DeleteFile(source);
		}

		public Task<bool> DirectoryExists(string path)
		{
			return Task.FromResult(_directories.Contains(path));
		}

		public Task CreateDirectory(string path)
		{
			_directories.Add(path);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<string>> ListFiles(string path)
		{
			return Task.FromResult(
				_files.Keys
				.Where(key => key.StartsWith(path)));
		}

		public Task<IEnumerable<string>> ListDirectories(string path)
		{
			return Task.FromResult(
				_files.Keys
				.Union(_directories)
				.Where(key => key.StartsWith(path)));
		}

		public Task DeleteDirectory(string path)
		{
			_directories.RemoveWhere(key => key.StartsWith(path));
			return Task.CompletedTask;
		}
	}
}
