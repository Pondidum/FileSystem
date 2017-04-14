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

		public async Task DeleteFile(string path)
		{
			await Task.Run(() => File.Delete(path));
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

		public async Task WriteFileMetadata(string path, FileMetadata metadata)
		{
			await Task.Run(() =>
			{

				File.SetLastAccessTime(path, metadata.AccessTime);
				File.SetCreationTime(path, metadata.CreationTime);
				File.SetLastWriteTime(path, metadata.ModificationTime);
				File.SetAttributes(path, metadata.Attributes);
			});
		}

		public async Task CopyFile(string source, string destination)
		{
			await Task.Run(() => File.Copy(source, destination));
		}

		public async Task MoveFile(string source, string destination)
		{
			await Task.Run(() => File.Move(source, destination));
		}

		public Task<bool> DirectoryExists(string path)
		{
			return Task.FromResult(Directory.Exists(path));
		}

		public async Task CreateDirectory(string path)
		{
			await Task.Run(() => Directory.CreateDirectory(path));
		}

		public Task<IEnumerable<string>> ListFiles(string path)
		{
			return Task.FromResult(Directory.EnumerateFiles(path));
		}

		public Task<IEnumerable<string>> ListDirectories(string path)
		{
			return Task.FromResult(Directory.EnumerateDirectories(path));
		}

		public async Task DeleteDirectory(string path)
		{
			await Task.Run(() => Directory.Delete(path, recursive: true));
		}
	}
}
