using System.Collections.Generic;
using System.IO;

namespace FileSystem
{
  public interface IFileSystem
	{
		bool FileExists(string path);
		void WriteFile(string path, Stream contents);
		Stream ReadFile(string path);
		void DeleteFile(string path);
		void CopyFile(string source, string destination);

		bool DirectoryExists(string path);
		void CreateDirectory(string path);
		IEnumerable<string> ListFiles(string path);
		IEnumerable<string> ListDirectories(string path);
		void DeleteDirectory(string path);
	}
}
