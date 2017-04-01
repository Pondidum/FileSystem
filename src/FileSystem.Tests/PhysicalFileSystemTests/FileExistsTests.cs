using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public class FileExistsTests : PhysicalFileSystemTests
	{
		public FileExistsTests()
		{
			WriteFile(Path.Combine(Root, "exists.json"), "").Wait();
		}

		[Theory]
		[InlineData("{root}\\exists.json", true)]
		[InlineData("{root}\\not-exists.json", false)]
		[InlineData("{root}\\not\\here.json", false)]
		[InlineData("{root}", false)]
		public async Task When_checking_existance(string path, bool exist)
		{
			path = path.Replace("{root}", Root);

			(await Fs.FileExists(path)).ShouldBe(exist);
		}
	}
}
