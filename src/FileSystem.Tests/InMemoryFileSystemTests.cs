namespace FileSystem.Tests
{
	public class InMemoryFileSystemTests : FileSystemTests
	{
		public InMemoryFileSystemTests() : base(new InMemoryFileSystem())
		{
		}
	}
}
