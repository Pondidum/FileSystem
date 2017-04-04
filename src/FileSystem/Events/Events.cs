﻿namespace FileSystem.Events
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
}