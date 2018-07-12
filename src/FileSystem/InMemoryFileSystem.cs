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

		private class FileData : FileMetadata
		{
			public byte[] Content { get; private set; }

			public FileData()
			{
				Content = new byte[0];
				CreationTime = DateTime.Now;
			}

			public FileData Write(byte[] bytes)
			{
				Content = bytes;
				ModificationTime = DateTime.Now;
				AccessTime = DateTime.Now;

				return this;
			}
		}

		private readonly Dictionary<string, FileData> _files;
		private readonly HashSet<string> _directories;

		public InMemoryFileSystem()
		{
			var comparer = new PathComparer();
			_files = new Dictionary<string, FileData>(comparer);
			_directories = new HashSet<string>(comparer);
		}

		public void Reset()
		{
			_directories.Clear();
			_files.Clear();
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
				_files[path] = new FileData().Write(ms.ToArray());
			}
		}

		public async Task AppendFile(string path, Func<Stream, Task> write)
		{
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			FileData file;

			if (_files.TryGetValue(path, out file) == false)
				file = new FileData();

			using (var ms = new MemoryStream())
			{
				await ms.WriteAsync(file.Content, 0, file.Content.Length);
				await write(ms);

				file.Write(ms.ToArray());

				_files[path] = file;
			}
		}

		public Task<Stream> ReadFile(string path)
		{
			FileData file;

			if (_files.TryGetValue(path, out file))
				return Task.FromResult((Stream)new MemoryStream(file.Content));

			throw new FileNotFoundException("Cannot find file", path);
		}

		public Task DeleteFile(string path)
		{
			ThrowIfNoDirectory(Path.GetDirectoryName(path));

			if (_files.ContainsKey(path))
				_files.Remove(path);

			return Task.FromResult(true);
		}

		public Task<FileMetadata> ReadFileMetadata(string path)
		{
			FileData file;

			if (_files.TryGetValue(path, out file))
				return Task.FromResult((FileMetadata)file);

			throw new FileNotFoundException("Cannot find file", path);
		}

		public Task WriteFileMetadata(string path, FileMetadata metadata)
		{
			FileData file;

			if (_files.TryGetValue(path, out file) == false)
				throw new FileNotFoundException("Cannot find file", path);

			file.AccessTime = metadata.AccessTime;
			file.CreationTime = metadata.CreationTime;
			file.ModificationTime = metadata.ModificationTime;
			file.Attributes = metadata.Attributes;

			return Task.FromResult(true);
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
			return Task.FromResult(true);
		}

		public Task<IEnumerable<string>> ListFiles(string path)
		{
			ThrowIfNoDirectory(path);

			return Task.FromResult(
				_files.Keys
				.Where(key => key.StartsWith(path)));
		}

		public Task<IEnumerable<string>> ListDirectories(string path)
		{
			ThrowIfNoDirectory(path);

			return Task.FromResult(
				_directories
				.Where(key => key != path && key.StartsWith(path)));
		}

		public Task DeleteDirectory(string path)
		{
			ThrowIfNoDirectory(path);

			_directories.RemoveWhere(key => key.StartsWith(path));

			var toRemove = _files.Keys.Where(f => f.StartsWith(path)).ToArray();
			foreach (var file in toRemove)
				_files.Remove(file);

			return Task.FromResult(true);
		}

		private void ThrowIfNoDirectory(string path)
		{
		    if (string.IsNullOrWhiteSpace(path))
		        return;

			if (_directories.Contains(path) == false)
				throw new DirectoryNotFoundException($"Unable to find directory '{path}'\n\nKnown Directories:\n{string.Join("\n", _directories)}");
		}
	}
}
