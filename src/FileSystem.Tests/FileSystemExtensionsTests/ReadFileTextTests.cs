using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.FileSystemExtensionsTests
{
	public class ReadFileTextTests
	{
		private readonly IFileSystem _fileSystem;

		public ReadFileTextTests()
		{
			_fileSystem = Substitute.For<IFileSystem>();
		}

		[Fact]
		public void When_the_file_is_not_found()
		{
			_fileSystem.ReadFile(Arg.Any<string>()).Throws(new FileNotFoundException());
			Should.Throw<FileNotFoundException>(() => _fileSystem.ReadFileText("wat"));
		}

		[Fact]
		public async Task When_the_file_has_content()
		{
			var content = "This is a test".ToStream();;
			_fileSystem.ReadFile("wat").Returns(content);

			var read = await _fileSystem.ReadFileText("wat");

			read.ShouldBe("This is a test");
		}
	}
}
