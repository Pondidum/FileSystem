using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests
{
	public class InMemoryFileSystemTests : FileSystemTests<InMemoryFileSystem>
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

		[Fact]
		public async Task Resetting_the_filesystem_removes_all_files_and_directories()
		{
			var dirpath = "test/child";
			var filepath = Path.Combine(dirpath, "history.json");

			await Fs.CreateDirectory(dirpath);
			await Fs.AppendFile(filepath, async stream => await "Some Content".WriteTo(stream));

			Fs.Reset();

			(await Fs.DirectoryExists(dirpath)).ShouldBeFalse();
			(await Fs.FileExists(filepath)).ShouldBeFalse();
		}
	}
}
