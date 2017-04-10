using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public interface IFileSystem
	{
		Task<bool> FileExists(string path);
		Task WriteFile(string path, Func<Stream, Task> write);
		Task AppendFile(string path, Func<Stream, Task> write);
		Task<Stream> ReadFile(string path);
		Task DeleteFile(string path);

		Task<FileMetadata> ReadFileMetadata(string path);

		Task CopyFile(string source, string destination);
		Task MoveFile(string source, string destination);

		Task<bool> DirectoryExists(string path);
		Task CreateDirectory(string path);
		Task<IEnumerable<string>> ListFiles(string path);
		Task<IEnumerable<string>> ListDirectories(string path);
		Task DeleteDirectory(string path);
	}
}
