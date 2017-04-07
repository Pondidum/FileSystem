using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileSystem.Events;

namespace FileSystem
{
	public class EventingFileSystem : IFileSystem
	{
		private readonly IFileSystem _inner;
		public Func<object, Task> HandleEvent { get; set; }

		public EventingFileSystem(IFileSystem inner)
			: this(inner, async @event => await Task.CompletedTask)
		{
		}

		public EventingFileSystem(IFileSystem inner, Func<object, Task> handleEvent)
		{
			_inner = inner;
			HandleEvent = handleEvent;
		}

		protected virtual async Task Emit(object @event) => await HandleEvent(@event);

		public async Task<bool> FileExists(string path)
		{
			var exists = await _inner.FileExists(path);

			await Emit(new FileExistenceChecked { Path = path, Exists = exists });

			return exists;
		}

		public async Task WriteFile(string path, Func<Stream, Task> write)
		{
			await _inner.WriteFile(path, write);
			await Emit(new FileWritten { Path = path });
		}

		public async Task AppendFile(string path, Func<Stream, Task> write)
		{
			await _inner.AppendFile(path, write);
			await Emit(new FileAppended { Path = path });
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
			await Emit(new DirectoryExistenceChecked { Path = path, Exists = exists });

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
