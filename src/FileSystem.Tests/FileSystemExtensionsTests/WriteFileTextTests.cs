using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.FileSystemExtensionsTests
{
	public class WriteFileTextTests
	{
		private readonly IFileSystem _fileSystem;
		private readonly StreamCapture _stream;

		public WriteFileTextTests()
		{
			_stream = new StreamCapture();
			_fileSystem = Substitute.For<IFileSystem>();
			_fileSystem.WriteFile(Arg.Any<string>(), Arg.Do<Func<Stream, Task>>(_stream.Capture));
		}

		private string Content => Encoding.UTF8.GetString(_stream.Last.ToArray());

		[Fact]
		public async Task When_writing_to_a_file()
		{
			await _fileSystem.WriteFileText("wat", "this is a test");

			Content.ShouldBe("this is a test");
		}
	}
}
