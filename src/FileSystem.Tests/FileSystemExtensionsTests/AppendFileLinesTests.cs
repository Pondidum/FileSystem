using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.FileSystemExtensionsTests
{
	public class AppendFileLinesTests
	{
		private readonly IFileSystem _fileSystem;
		private string _content;

		public AppendFileLinesTests()
		{
			_fileSystem = Substitute.For<IFileSystem>();

			_fileSystem.AppendFile(Arg.Any<string>(), Arg.Do<Func<Stream, Task>>(async func =>
			{
				using (var ms = new MemoryStream())
				{
					await func(ms);
				    _content = Encoding.UTF8.GetString(ms.ToArray());
				}
			}));
		}

		[Fact]
		public void Please_work()
		{
			_fileSystem.AppendFile("wat", async stream => await "text".WriteTo(stream));

			_content.ShouldBe("text");
		}

		[Fact]
		public async Task When_writing_no_lines_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json");

			_fileSystem.DidNotReceive().AppendFile(Arg.Any<string>(), Arg.Any<Func<Stream, Task>>());
		}

		[Fact]
		public async Task When_writing_one_line_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json", "a line");

			_content.ShouldBe("a line" + Environment.NewLine);
		}

		[Fact]
		public async Task When_writing_multiple_lines_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json", "One", "Two", "Three");

			_content.ShouldBe($"One{Environment.NewLine}Two{Environment.NewLine}Three{Environment.NewLine}");
		}
	}
}
