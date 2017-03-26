using System;
using System.IO;
using System.Text;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public abstract class PhysicalFileSystemTests : IDisposable
	{
		protected string Root { get; }
		protected PhysicalFileSystem Fs { get; }

		public PhysicalFileSystemTests()
		{
			Root = Guid.NewGuid().ToString();
			Fs = new PhysicalFileSystem();

			Directory.CreateDirectory(Root);
		}

		protected Stream StreamFrom(string text)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(text));
		}

		protected string StringFrom(Stream stream)
		{
			using (var sr = new StreamReader(stream))
				return sr.ReadToEnd();
		}

		public virtual void Dispose()
		{
			try
			{
				Directory.Delete(Root, true);
			}
			catch (Exception)
			{
				Console.WriteLine("Unable to Delete test directory");
			}
		}
	}
}
