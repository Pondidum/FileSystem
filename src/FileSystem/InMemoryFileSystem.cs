using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem
{
	public class InMemoryFileSystem : IFileSystem
	{
		private class PathComparer : IEqualityComparer<string>
		{
			public bool Equals(string x, string y)
			{
				if (x == null || y == null)
					return false;

				return GetHashCode(x) == GetHashCode(y);
			}

			public int GetHashCode(string obj)
			{
				return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Replace('/', '\\'));
			}
		}

		private readonly Dictionary<string, byte[]> _files;
		private readonly HashSet<string> _directories;

		public InMemoryFileSystem()
		{
			var comparer = new PathComparer();
			_files = new Dictionary<string, byte[]>(comparer);
			_directories = new HashSet<string>(comparer);
		}

		public Task<bool> FileExists(string path)
		{
			return Task.FromResult(_files.ContainsKey(path));
		}

		public async Task WriteFile(string path, Func<Stream, Task> write)
		{
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			using (var ms = new MemoryStream())
			{
				await write(ms);
				_files[path] = ms.ToArray();
			}
		}

		public async Task AppendFile(string path, Func<Stream, Task> write)
		{
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			byte[] contents;

			if (_files.TryGetValue(path, out contents) == false)
				contents = Array.Empty<byte>();

			using (var ms = new MemoryStream())
			{
				await ms.WriteAsync(contents, 0, contents.Length);
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
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

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
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			return Task.FromResult(
				_files.Keys
				.Where(key => key.StartsWith(path)));
		}

		public Task<IEnumerable<string>> ListDirectories(string path)
		{
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			return Task.FromResult(
				_directories
				.Where(key => key != path && key.StartsWith(path)));
		}

		public Task DeleteDirectory(string path)
		{
			ThrowIfNoDirectory(path);

			_directories.RemoveWhere(key => key.StartsWith(path));
			return Task.CompletedTask;
		}

		private void ThrowIfNoDirectory(string path)
		{
			if (_directories.Contains(path) == false)
				throw new DirectoryNotFoundException($"Unable to find directory '{path}'\n\nKnown Directories:\n{string.Join("\n", _directories)}");
		}
	}
}
