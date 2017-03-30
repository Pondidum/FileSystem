using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class ReadFileTests : PhysicalFileSystemTests
	{
		private const string Content = "This is the content";

		public ReadFileTests()
		{
			File.WriteAllText(
				Path.Combine(Root, "existing.txt"),
				Content);
		}

		[Fact]
		public async Task When_reading_an_existing_file()
		{
			var stream = await Fs.ReadFile(Path.Combine(Root, "existing.txt"));

			((string)stream.ReadAsString()).ShouldBe(Content);
		}

		[Fact]
		public void When_reading_a_non_existing_file()
		{
			Should.Throw<FileNotFoundException>(
				async () => await Fs.ReadFile(Path.Combine(Root, "non-existing.txt")));
		}
	}
}
