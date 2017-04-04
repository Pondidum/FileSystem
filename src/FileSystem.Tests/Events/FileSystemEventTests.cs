using FileSystem.Events;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.Events
{
	public class FileSystemEventTests
	{
		[Fact]
		public void When_calling_tostring()
		{
			var fse = new FileSystemEvent
			{
				Path = "./some/path/file.json"
			};

			fse.ToString().ShouldBe($"File System Event '{fse.Path}'");
		}
	}
}