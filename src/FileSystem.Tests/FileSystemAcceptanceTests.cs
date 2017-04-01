using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests
{
	public abstract class FileSystemAcceptanceTests : IDisposable
	{
		private const string Json = "{ \"message\": \"Hello world!\"}";
		private const string OtherJson = "[ 'one', 'two', 'three', 'four' ]";

		private readonly IFileSystem _fileSystem;
		private string _root;

		protected FileSystemAcceptanceTests(IFileSystem fileSystem)
		{
			_root = "Acceptance-" + Guid.NewGuid().ToString();
			_fileSystem = fileSystem;
		}

		[Fact]
		public async Task When_a_file_is_written()
		{
			var dir = Path.Combine(_root, "some//sub//dir");
			var firstFile = Path.Combine(dir, "somefile.json");
			var secondFile = Path.Combine(dir, "anotherFile.json");

			await DirectoryDoesntExist(dir);

			await _fileSystem.CreateDirectory(dir);

			await DirectoryExists(dir);
			await FileDoesntExist(firstFile);

			await _fileSystem.WriteFile(firstFile, async stream => await Json.WriteTo(stream));

			await FileExists(firstFile);

			//can read twice
			await FileHasContents(firstFile, Json);
			await FileHasContents(firstFile, Json);

			await _fileSystem.CopyFile(firstFile, secondFile);

			await FileExists(firstFile);
			await FileHasContents(firstFile, Json);

			await FileExists(secondFile);
			await FileHasContents(secondFile, Json);

			await _fileSystem.WriteFile(firstFile, async stream => await OtherJson.WriteTo(stream));

			await FileExists(firstFile);
			await FileHasContents(firstFile, OtherJson);

			await FileExists(secondFile);
			await FileHasContents(secondFile, Json);

			await _fileSystem.DeleteFile(firstFile);

			await FileDoesntExist(firstFile);
			await FileExists(secondFile);
		}

		private async Task DirectoryExists(string dir)
		{
			(await _fileSystem.DirectoryExists(dir)).ShouldBe(true);
		}

		private async Task DirectoryDoesntExist(string dir)
		{
			(await _fileSystem.DirectoryExists(dir)).ShouldBe(false);
		}

		private async Task FileHasContents(string firstFile, string contents)
		{
			(await _fileSystem.ReadFile(firstFile)).ReadAsString().ShouldBe(contents);
		}

		private async Task FileExists(string firstFile)
		{
			(await _fileSystem.FileExists(firstFile)).ShouldBe(true);
		}

		private async Task FileDoesntExist(string firstFile)
		{
			(await _fileSystem.FileExists(firstFile)).ShouldBe(false);
		}

		public void Dispose()
		{
			_fileSystem.DeleteDirectory(_root);
		}
	}
}
