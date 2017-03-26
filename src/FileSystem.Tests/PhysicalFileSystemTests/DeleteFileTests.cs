using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class DeleteFileTests : PhysicalFileSystemTests
	{
		public DeleteFileTests()
		{
			File.WriteAllText(
				Path.Combine(Root, "existing.txt"),
				"this is some content");
		}

		[Theory]
		[InlineData("existing.txt")]
		[InlineData("non-existing.txt")]
		public async Task When_deleting_a_file(string filename)
		{
			var path = Path.Combine(Root, filename);

			await Fs.DeleteFile(path);
		}

		[Fact]
		public void When_deleting_from_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.DeleteFile("sub\\nonexisting.txt"));
		}
	}
}
