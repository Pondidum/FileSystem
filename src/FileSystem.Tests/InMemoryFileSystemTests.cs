using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests
{
	public class InMemoryFileSystemTests : FileSystemTests
	{
		public InMemoryFileSystemTests() : base(new InMemoryFileSystem())
		{
		}

		[Fact]
		public async Task When_writing_and_reading_a_file_to_a_relative_file_with_no_directory()
		{
			await Fs.AppendFile("history.json", async stream => await "Some Content".WriteTo(stream));

			var content = (await Fs.ReadFile("history.json")).ReadAsString();

			content.ShouldBe("Some Content");
		}

		[Fact]
		public async Task Deleting_a_directory_deletes_files()
		{
			var filepath = "test/child/history.json";

			await Fs.CreateDirectory("test/child");
			await Fs.AppendFile(filepath, async stream => await "Some Content".WriteTo(stream));

			await Fs.DeleteDirectory("test/child");

			(await Fs.FileExists(filepath)).ShouldBeFalse();
		}
	}
}
