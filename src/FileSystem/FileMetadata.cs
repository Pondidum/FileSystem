using System;
using System.IO;

namespace FileSystem
{
	public class FileMetadata
	{
		public DateTime CreationTime { get; set; }
		public DateTime ModificationTime { get; set; }
		public DateTime AccessTime { get; set; }
		public FileAttributes Attributes { get; set; }
	}
}
