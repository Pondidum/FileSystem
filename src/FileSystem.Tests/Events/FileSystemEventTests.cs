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

		[Fact]
		public void When_calling_tostring_on_an_existence_event_non_existing()
		{
			var fse = new FileSystemExistenceEvent()
			{
				Path = "./some/path/file.json",
				Exists = false
			};

			fse.ToString().ShouldBe($"File System Existence Event '{fse.Path}'. It didn't exist");
		}

		[Fact]
		public void When_calling_tostring_on_an_existence_event_existing()
		{
			var fse = new FileSystemExistenceEvent()
			{
				Path = "./some/path/file.json",
				Exists = true
			};

			fse.ToString().ShouldBe($"File System Existence Event '{fse.Path}'. It did exist");
		}
	}
}