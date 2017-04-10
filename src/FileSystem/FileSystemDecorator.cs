using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public class FileSystemDecorator : IFileSystem
	{
		private readonly IFileSystem _inner;

		public FileSystemDecorator(IFileSystem inner)
		{
			_inner = inner;
		}

		public virtual async Task<bool> FileExists(string path)
		{
			return await _inner.FileExists(path);
		}

		public virtual async Task WriteFile(string path, Func<Stream, Task> write)
		{
			await _inner.WriteFile(path, write);
		}

		public virtual async Task AppendFile(string path, Func<Stream, Task> write)
		{
			await _inner.AppendFile(path, write);
		}

		public virtual async Task<Stream> ReadFile(string path)
		{
			return await _inner.ReadFile(path);
		}

		public virtual async Task DeleteFile(string path)
		{
			await _inner.DeleteFile(path);
		}

		public async Task<FileMetadata> ReadFileMetadata(string path)
		{
			return await _inner.ReadFileMetadata(path);
		}

		public virtual async Task CopyFile(string source, string destination)
		{
			await _inner.CopyFile(source, destination);
		}

		public virtual async Task MoveFile(string source, string destination)
		{
			await _inner.MoveFile(source, destination);
		}

		public virtual async Task<bool> DirectoryExists(string path)
		{
			return await _inner.DirectoryExists(path);
		}

		public virtual async Task CreateDirectory(string path)
		{
			await _inner.CreateDirectory(path);
		}

		public virtual async Task<IEnumerable<string>> ListFiles(string path)
		{
			return await _inner.ListFiles(path);
		}

		public virtual async Task<IEnumerable<string>> ListDirectories(string path)
		{
			return await _inner.ListDirectories(path);
		}

		public virtual async Task DeleteDirectory(string path)
		{
			await _inner.DeleteDirectory(path);
		}
	}
}
