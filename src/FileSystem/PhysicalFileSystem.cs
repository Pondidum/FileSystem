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

		public async Task WriteFile(string path, Stream contents)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
				await contents.CopyToAsync(fs);
		}

		public Task<Stream> ReadFile(string path)
		{
			return Task.FromResult((Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		public async Task DeleteFile(string path)
		{
			File.Delete(path);
			await Task.Yield();
		}

		public async Task CopyFile(string source, string destination)
		{
			File.Copy(source, destination);
			await Task.Yield();
		}

		public Task<bool> DirectoryExists(string path)
		{
			return Task.FromResult(Directory.Exists(path));
		}

		public async Task CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
			await Task.Yield();
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
			Directory.Delete(path, recursive: true);
			await Task.Yield();
		}
	}
}
