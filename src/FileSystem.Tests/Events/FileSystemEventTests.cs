using FileSystem.Events;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.Events
{
	public class FileSystemEventTests
	{
		[Fact]
		public void When_calling_tostring_on_an_event()
		{
			var fse = new FileSystemEvent
			{
				Path = "./some/path/file.json"
			};

			fse.ToString().ShouldBe($"File System Event '{fse.Path}'");
		}

		[Fact]
		public void When_calling_tostring_on_a_destination_event()
		{
			var fsde = new FileSystemDestinationEvent
			{
				Path = "./some/source/path.json",
				Destination = "./some/destinaton.json"
			};

			fsde.ToString().ShouldBe($"File System Destination Event from '{fsde.Path}' to '{fsde.Destination}'");
		}
	}
}