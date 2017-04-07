using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;

namespace FileSystem.Tests
{
	public class EventingFileSystemTests : FileSystemTests
	{
		private readonly List<object> _events;

		public EventingFileSystemTests()
			: this(new EventingFileSystem(new InMemoryFileSystem()))
		{
		}

		private EventingFileSystemTests(EventingFileSystem fs)
			: base(fs)
		{
			_events = new List<object>();

			fs.HandleEvent = async @event =>
			{
				_events.Add(@event);
				await Task.CompletedTask;
			};
		}

		public override void Dispose()
		{
			base.Dispose();

			_events.ShouldNotBeEmpty();
		}
	}
}