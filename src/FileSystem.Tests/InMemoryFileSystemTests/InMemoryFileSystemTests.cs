using FileSystem.Tests.PhysicalFileSystemTests;

namespace FileSystem.Tests.InMemoryFileSystemTests
{
	public class InMemoryFileSystemTests : FileSystemTests
	{
		public InMemoryFileSystemTests() : base(new InMemoryFileSystem())
		{
		}
	}
}
