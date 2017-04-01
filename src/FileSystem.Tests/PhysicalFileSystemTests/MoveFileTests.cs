using Shouldly;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class MoveFileTests : PhysicalFileSystemTests
	{
		private const string Content = "this is a test";

		[Fact]
		public void When_moveing_a_non_existing_source()
		{
			var source = Path.Combine(Root, "source-non-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			Should.Throw<FileNotFoundException>(
				async () => await Fs.MoveFile(source, destination));

			File.Exists(source).ShouldBe(false);
			File.Exists(destination).ShouldBe(false);
		}

		[Fact]
		public async Task When_moveing_an_exising_source_to_non_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			File.WriteAllText(source, Content);

			await Fs.MoveFile(source, destination);

			File.Exists(source).ShouldBe(false);
			File.ReadAllText(destination).ShouldBe(Content);
		}

		[Fact]
		public void When_moveing_an_existing_source_to_non_existing_directory()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "target\\dest-non-existing.json");

			File.WriteAllText(source, Content);

			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.MoveFile(source, destination));

			File.Exists(source).ShouldBe(true);
			File.Exists(destination).ShouldBe(false);
		}

		[Fact]
		public async Task When_moveing_an_existing_source_to_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-existing.json");

			File.WriteAllText(source, Content);

			await Fs.MoveFile(source, destination);

			File.Exists(source).ShouldBe(false);
			File.ReadAllText(destination).ShouldBe(Content);
		}
	}
}
