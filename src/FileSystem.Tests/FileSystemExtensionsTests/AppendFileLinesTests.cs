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
		private readonly StreamCapture _stream;

		public AppendFileLinesTests()
		{
			_stream = new StreamCapture();

			_fileSystem = Substitute.For<IFileSystem>();
			_fileSystem.AppendFile(Arg.Any<string>(), Arg.Do<Func<Stream, Task>>(_stream.Capture));
		}

		private string Content => Encoding.UTF8.GetString(_stream.Last.ToArray());

		[Fact]
		public void Please_work()
		{
			_fileSystem.AppendFile("wat", async stream => await "text".WriteTo(stream));

			Content.ShouldBe("text");
		}

		[Fact]
		public async Task When_writing_no_lines_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json");

			await _fileSystem.DidNotReceive().AppendFile(Arg.Any<string>(), Arg.Any<Func<Stream, Task>>());
		}

		[Fact]
		public async Task When_writing_one_line_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json", "a line");

			Content.ShouldBe("a line" + Environment.NewLine);
		}

		[Fact]
		public async Task When_writing_multiple_lines_to_a_file()
		{
			await _fileSystem.AppendFileLines("wat.json", "One", "Two", "Three");

			Content.ShouldBe($"One{Environment.NewLine}Two{Environment.NewLine}Three{Environment.NewLine}");
		}
	}
}
