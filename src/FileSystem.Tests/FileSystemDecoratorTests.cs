namespace FileSystem.Tests
{
	public class FileSystemDecoratorTests : FileSystemTests
	{
		public FileSystemDecoratorTests()
			: base(new FileSystemDecorator(new InMemoryFileSystem()))
		{
		}
	}
}
