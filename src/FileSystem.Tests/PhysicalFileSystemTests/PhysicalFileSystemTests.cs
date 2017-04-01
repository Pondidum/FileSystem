using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;

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

			CreateDirectory(Root).Wait();
		}

		protected async Task FileExists(string path) => (await Fs.FileExists(path)).ShouldBe(true);
		protected async Task FileDoesntExist(string path) => (await Fs.FileExists(path)).ShouldBe(false);
		protected async Task FileHasContents(string path, string contents) => (await Fs.ReadFile(path)).ReadAsString().ShouldBe(contents);

		protected async Task DirectoryExists(string path) => (await Fs.DirectoryExists(path)).ShouldBe(true);
		protected async Task DirectoryDoesntExist(string path) => (await Fs.DirectoryExists(path)).ShouldBe(false);


		protected async Task WriteFile(string path, string content) => await Fs.WriteFile(path, async stream => await content.WriteTo(stream));
		protected async Task CreateDirectory(string path) => await Fs.CreateDirectory(path);


		public virtual void Dispose()
		{
			try
			{
				Fs.DeleteDirectory(Root);
			}
			catch (Exception)
			{
				Console.WriteLine("Unable to Delete test directory");
			}
		}
	}
}
