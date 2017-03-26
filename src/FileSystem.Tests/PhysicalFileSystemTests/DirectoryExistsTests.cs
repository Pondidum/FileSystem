using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class DirectoryExistsTests : PhysicalFileSystemTests
	{
		public DirectoryExistsTests()
		{
			Directory.CreateDirectory(Path.Combine(Root, "test-dir"));
		}

		[Theory]
		[InlineData("test-dir", true)]
		[InlineData("test-dir\\other", false)]
		[InlineData("not-here", false)]
		public async Task When_checking_existance(string path, bool exists)
		{
			var directory = Path.Combine(Root, path);

			(await Fs.DirectoryExists(directory)).ShouldBe(exists);
		}
	}
}
