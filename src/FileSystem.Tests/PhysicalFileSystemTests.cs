namespace FileSystem.Tests
{
	public class PhysicalFileSystemTests : FileSystemTests
	{
		public PhysicalFileSystemTests() : base(new PhysicalFileSystem())
		{
		}
	}
}
