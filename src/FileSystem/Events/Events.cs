using FileSystem.Internal;

namespace FileSystem.Events
{
	public class FileSystemEvent
	{
		public string Path { get; set; }

		public override string ToString() => $"{GetType().Name.ToSentence()} '{Path}'";
	}

	public class FileSystemDestinationEvent : FileSystemEvent
	{
		public string Destination { get; set; }

		public override string ToString() => $"{GetType().Name.ToSentence()} from '{Path}' to '{Destination}'";
	}

	public class FileSystemExistenceEvent : FileSystemEvent
	{
		public bool Exists { get; set; }

		public override string ToString()
		{
			var suffix = Exists
				? "It did exist"
				: "It didn't exist";

			return $"{GetType().Name.ToSentence()} '{Path}'. {suffix}";
		}
	}

	public class FileExistenceChecked : FileSystemExistenceEvent { }
	public class FileWritten : FileSystemEvent { }
	public class FileRead : FileSystemEvent { }
	public class FileDeleted : FileSystemEvent { }
	public class FileMoved : FileSystemDestinationEvent { }
	public class FileCopied : FileSystemDestinationEvent { }

	public class DirectoryExistenceChecked : FileSystemExistenceEvent { }
	public class DirectoryCreated : FileSystemExistenceEvent { }
	public class DirectoryDeleted : FileSystemEvent { }

	public class DirectoryFilesListed : FileSystemEvent { }
	public class DirectoryDirectoriesListed : FileSystemEvent { }
}