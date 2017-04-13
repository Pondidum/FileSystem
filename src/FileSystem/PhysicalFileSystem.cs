using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public class PhysicalFileSystem : IFileSystem
	{
		public Task<bool> FileExists(string path)
		{
			return Task.FromResult(File.Exists(path));
		}

		public async Task WriteFile(string path, Func<Stream, Task> write)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
				await write(fs);
		}

		public async Task AppendFile(string path, Func<Stream, Task> write)
		{
			using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write))
				await write(fs);
		}

		public Task<Stream> ReadFile(string path)
		{
			return Task.FromResult((Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		public Task DeleteFile(string path)
		{
			File.Delete(path);
			return Task.CompletedTask;
		}

		public Task<FileMetadata> ReadFileMetadata(string path)
		{
			return Task.FromResult(new FileMetadata
			{
				CreationTime = File.GetCreationTime(path),
				ModificationTime = File.GetLastWriteTime(path),
				AccessTime = File.GetLastAccessTime(path),
				Attributes = File.GetAttributes(path)
			});
		}

		public Task WriteFileMetadata(string path, FileMetadata metadata)
		{
			File.SetLastAccessTime(path, metadata.AccessTime);
			File.SetCreationTime(path, metadata.CreationTime);
			File.SetLastWriteTime(path, metadata.ModificationTime);
			File.SetAttributes(path, metadata.Attributes);

			return Task.CompletedTask;
		}

		public Task CopyFile(string source, string destination)
		{
			File.Copy(source, destination);
			return Task.CompletedTask;
		}

		public Task MoveFile(string source, string destination)
		{
			File.Move(source, destination);
			return Task.CompletedTask;
		}

		public Task<bool> DirectoryExists(string path)
		{
			return Task.FromResult(Directory.Exists(path));
		}

		public Task CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<string>> ListFiles(string path)
		{
			return Task.FromResult(Directory.EnumerateFiles(path));
		}

		public Task<IEnumerable<string>> ListDirectories(string path)
		{
			return Task.FromResult(Directory.EnumerateDirectories(path));
		}

		public Task DeleteDirectory(string path)
		{
			Directory.Delete(path, recursive: true);
			return Task.CompletedTask;
		}
	}
}
