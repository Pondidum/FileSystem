namespace FileSystem.Tests.InMemoryFileSystemTests
{
	public class Acceptance : FileSystemAcceptanceTests
	{
		public Acceptance() : base(new InMemoryFileSystem())
		{
		}
	}
}