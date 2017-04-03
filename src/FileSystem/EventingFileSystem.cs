using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystem
{
	public class FileSystemEvent
	{
		public string Path { get; set; }
	}

	public class FileSystemDestinationEvent : FileSystemEvent
	{
		public string Destination { get; set; }
	}

	public class FileSystemExistanceEvent : FileSystemEvent
	{
		public bool Exists { get; set; }
	}

	public class FileExistanceChecked : FileSystemExistanceEvent { }
	public class FileWritten : FileSystemEvent { }
	public class FileRead : FileSystemEvent { }
	public class FileDeleted : FileSystemEvent { }
	public class FileMoved : FileSystemDestinationEvent { }
	public class FileCopied : FileSystemDestinationEvent { }

	public class DirectoryExistanceChecked : FileSystemExistanceEvent { }
	public class DirectoryCreated : FileSystemExistanceEvent { }
	public class DirectoryDeleted : FileSystemEvent { }

	public class DirectoryFilesListed : FileSystemEvent { }
	public class DirectoryDirectoriesListed : FileSystemEvent { }

	public class EventingFileSystem : IFileSystem
	{
		private readonly IFileSystem _inner;
		private readonly Func<object, Task> _handleEvent;

		public EventingFileSystem(IFileSystem inner, Func<object, Task> handleEvent)
		{
			_inner = inner;
			_handleEvent = handleEvent;
		}

		private async Task Emit(object @event) => await _handleEvent(@event);

		public async Task<bool> FileExists(string path)
		{
			var exists = await _inner.FileExists(path);

			await Emit(new FileExistanceChecked { Path = path, Exists = exists });

			return exists;
		}

		public async Task WriteFile(string path, Func<Stream, Task> write)
		{
			await _inner.WriteFile(path, write);
			await Emit(new FileWritten { Path = path });
		}

		public async Task<Stream> ReadFile(string path)
		{
			var stream = await _inner.ReadFile(path);
			await Emit(new FileRead { Path = path });

			return stream;
		}

		public async Task DeleteFile(string path)
		{
			await _inner.DeleteFile(path);
			await Emit(new FileDeleted { Path = path });
		}

		public async Task CopyFile(string source, string destination)
		{
			await _inner.CopyFile(source, destination);
			await Emit(new FileCopied { Path = source, Destination = destination });
		}

		public async Task MoveFile(string source, string destination)
		{
			await _inner.MoveFile(source, destination);
			await Emit(new FileMoved { Path = source, Destination = destination });
		}

		public async Task<bool> DirectoryExists(string path)
		{
			var exists = await _inner.DirectoryExists(path);
			await Emit(new DirectoryExistanceChecked { Path = path, Exists = exists });

			return exists;
		}

		public async Task CreateDirectory(string path)
		{
			await _inner.CreateDirectory(path);
			await Emit(new DirectoryCreated { Path = path });
		}

		public async Task<IEnumerable<string>> ListFiles(string path)
		{
			var files = await _inner.ListFiles(path);
			await Emit(new DirectoryFilesListed { Path = path });

			return files;
		}

		public async Task<IEnumerable<string>> ListDirectories(string path)
		{
			var directories = await _inner.ListDirectories(path);
			await Emit(new DirectoryDirectoriesListed { Path = path });

			return directories;
		}

		public async Task DeleteDirectory(string path)
		{
			await _inner.DeleteDirectory(path);
			await Emit(new DirectoryDeleted { Path = path });
		}
	}
}
