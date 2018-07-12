namespace FileSystem.Tests
{
	public class PhysicalFileSystemTests : FileSystemTests<PhysicalFileSystem>
	{
		public PhysicalFileSystemTests() : base(new PhysicalFileSystem())
		{
		}
	}
}
