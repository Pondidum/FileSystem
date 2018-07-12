namespace FileSystem.Tests
{
	public class FileSystemDecoratorTests : FileSystemTests<FileSystemDecorator>
	{
		public FileSystemDecoratorTests()
			: base(new FileSystemDecorator(new InMemoryFileSystem()))
		{
		}
	}
}
