using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class ListFilesTests : PhysicalFileSystemTests
	{
		[Fact]
		public void When_listing_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(async () => await Fs.ListFiles("non_existing"));
		}

		[Fact]
		public async Task When_listing_a_directory_without_files()
		{
			var path = Path.Combine(Root, "without_files");
			await CreateDirectory(path);

			var files = await Fs.ListFiles(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_a_directory_with_files()
		{
			var path = Path.Combine(Root, "with_files");
			await CreateDirectory(path);
			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListFiles(path);

			files.ShouldBe(new[]
			{
				path + "\\1.txt",
				path + "\\2.txt",
				path + "\\3.txt"
			});
		}

		[Fact]
		public async Task When_listing_a_directory_with_directories()
		{
			var path = Path.Combine(Root, "with_directories");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			var files = await Fs.ListFiles(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_a_directory_with_both()
		{
			var path = Path.Combine(Root, "with_both");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListFiles(path);

			files.ShouldBe(new[]
			{
				path + "\\1.txt",
				path + "\\2.txt",
				path + "\\3.txt"
			});
		}
	}
}
