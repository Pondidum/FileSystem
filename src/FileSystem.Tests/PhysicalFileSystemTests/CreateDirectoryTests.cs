using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class CreateDirectoryTests : PhysicalFileSystemTests
	{
		[Fact]
		public async Task When_creating_a_non_existing_directory()
		{
			var path = Path.Combine(Root, "non-existing");
			await Fs.CreateDirectory(path);

			Directory.Exists(path).ShouldBe(true);
		}

		[Fact]
		public async Task When_creating_an_existing_directory()
		{
			var path = Path.Combine(Root, "non-existing");
			Directory.CreateDirectory(path);

			await Fs.CreateDirectory(path);

			Directory.Exists(path).ShouldBe(true);
		}

		[Fact]
		public async Task When_creating_a_non_existing_directory_tree()
		{
			var path = Path.Combine(Root, "non-existing\\tree\\of\\directories");
			await Fs.CreateDirectory(path);

			Directory.Exists(path).ShouldBe(true);
		}
	}
}
