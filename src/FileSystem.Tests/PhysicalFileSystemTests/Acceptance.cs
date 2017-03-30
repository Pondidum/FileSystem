namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class Acceptance : FileSystemAcceptanceTests
	{
		public Acceptance() : base(new PhysicalFileSystem())
		{
		}
	}
}
