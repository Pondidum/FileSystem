namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class PhysicalFileSystemTests : FileSystemTests
	{
		public PhysicalFileSystemTests() : base(new PhysicalFileSystem())
		{
		}
	}
}
