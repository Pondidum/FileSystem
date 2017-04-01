using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class DeleteDirectoryTests : PhysicalFileSystemTests
	{
		[Fact]
		public async Task When_deleting_an_existing_directory()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_files()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);
			await WriteFile(Path.Combine(path, "1.txt"), "first");
			await WriteFile(Path.Combine(path, "2.txt"), "second");

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_subdirectories()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "1"));
			await CreateDirectory(Path.Combine(path, "2"));

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_a_non_existing_directory()
		{
			var path = "some fake";

			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.DeleteDirectory(path));

			await DirectoryDoesntExist("some fake");
		}
	}
}
