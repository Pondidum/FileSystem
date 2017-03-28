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
			Directory.CreateDirectory(path);

			await Fs.DeleteDirectory(path);

			Directory.Exists(path).ShouldBe(false);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_files()
		{
			var path = Path.Combine(Root, "existing");
			Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, "1.txt"), "first");
			File.WriteAllText(Path.Combine(path, "2.txt"), "second");

			await Fs.DeleteDirectory(path);

			Directory.Exists(path).ShouldBe(false);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_subdirectories()
		{
			var path = Path.Combine(Root, "existing");
			Directory.CreateDirectory(path);
			Directory.CreateDirectory(Path.Combine(path, "1"));
			Directory.CreateDirectory(Path.Combine(path, "2"));

			await Fs.DeleteDirectory(path);

			Directory.Exists(path).ShouldBe(false);
		}

		[Fact]
		public void When_deleting_a_non_existing_directory()
		{
			var path = "some fake";

			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.DeleteDirectory(path));

			Directory.Exists("some fale").ShouldBe(false);
		}
	}
}
