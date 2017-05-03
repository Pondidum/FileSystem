using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem
{
	public class StreamCapture
	{
		private readonly List<MemoryStream> _streams;

		public StreamCapture()
		{
			_streams = new List<MemoryStream>();
		}

		public IEnumerable<MemoryStream> Streams => _streams;
		public MemoryStream Last => _streams.Last();

		public void Capture(Func<Stream, Task> callback)
		{
			using (var ms = new MemoryStream())
			{
				callback(ms).Wait();
				_streams.Add(new MemoryStream(ms.ToArray()));
			}
		}
	}
}
