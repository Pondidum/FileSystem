using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.FileSystemExtensionsTests
{
	public class ReadFileLinesTests
	{
		private readonly IFileSystem _fileSystem;

		public ReadFileLinesTests()
		{
			_fileSystem = Substitute.For<IFileSystem>();
		}

		[Fact]
		public void When_the_file_is_not_found()
		{
			_fileSystem.ReadFile(Arg.Any<string>()).Throws(new FileNotFoundException());

			Should.Throw<FileNotFoundException>(() => _fileSystem.ReadFileLines("wat"));
		}

		[Fact]
		public async Task When_no_lines_are_returned()
		{
			_fileSystem.ReadFile(Arg.Any<string>()).Returns(new MemoryStream());

			var lines = await _fileSystem.ReadFileLines("wat");
			lines.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_only_one_line_with_no_newline_char_is_returned()
		{
			var oneLine = "this is a single line with no newline".ToStream();
			_fileSystem.ReadFile(Arg.Any<string>()).Returns(oneLine);

			var lines = await _fileSystem.ReadFileLines("wat");
			lines.ShouldBe(new[]
			{
				"this is a single line with no newline"
			});
		}

		[Fact]
		public async Task When_only_one_line_with_a_newline_char_is_returned()
		{
			var oneLine = "this is a single line with a newline\n".ToStream();
			_fileSystem.ReadFile(Arg.Any<string>()).Returns(oneLine);

			var lines = await _fileSystem.ReadFileLines("wat");
			lines.ShouldBe(new[]
			{
				"this is a single line with a newline"
			});
		}

		[Fact]
		public async Task When_multiple_lines_are_returned_using_LF()
		{
			var twoLines = "this is the first line\nthis is the second line".ToStream();
			_fileSystem.ReadFile(Arg.Any<string>()).Returns(twoLines);

			var lines = await _fileSystem.ReadFileLines("wat");
			lines.ShouldBe(new[]
			{
				"this is the first line",
				"this is the second line"
			});
		}

		[Fact]
		public async Task When_multiple_lines_are_returned_using_CRLF()
		{
			var twoLines = "this is the first line\r\nthis is the second line".ToStream();
			_fileSystem.ReadFile(Arg.Any<string>()).Returns(twoLines);

			var lines = await _fileSystem.ReadFileLines("wat");
			lines.ShouldBe(new[]
			{
				"this is the first line",
				"this is the second line"
			});
		}
	}
}