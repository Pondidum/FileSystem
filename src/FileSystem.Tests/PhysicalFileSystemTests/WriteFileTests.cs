using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class WriteFileTests : PhysicalFileSystemTests
	{
		public WriteFileTests()
		{
			WriteFile(Path.Combine(Root, "exists.txt"), "This is a single line test").Wait();
		}

		[Theory]
		[InlineData("exists.txt", "replacement text")]
		[InlineData("nonexisting.txt", "other text")]
		public async Task When_writing_to_a_file(string filename, string content)
		{
			var path = Path.Combine(Root, filename);

			await Fs.WriteFile(path, async stream => await content.WriteTo(stream));

			await FileHasContents(path, content);
		}

		[Fact]
		public void When_writing_to_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(async () => await Fs.WriteFile(
				"sub\\nonexisting.txt",
				async stream => await "something".WriteTo(stream)));
		}
	}
}
