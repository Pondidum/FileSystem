using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.PhysicalFileSystemTests
{
	public abstract class FileSystemTests : IDisposable
	{
		private const string Json = "{ \"message\": \"Hello world!\"}";
		private const string OtherJson = "[ 'one', 'two', 'three', 'four' ]";
		private const string Content = "this is a test";

		protected string Root { get; }
		protected IFileSystem Fs { get; }

		public FileSystemTests(IFileSystem fileSystem)
		{
			Root = Guid.NewGuid().ToString();
			Fs = fileSystem;

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

		[Fact]
		public async Task Acceptance_test()
		{
			var root = Path.Combine(Root, "Acceptance");

			await Fs.CreateDirectory(root);

			var dir = Path.Combine(root, "some/sub/dir");
			var firstFile = Path.Combine(dir, "somefile.json");
			var secondFile = Path.Combine(dir, "anotherFile.json");

			await DirectoryDoesntExist(dir);

			await Fs.CreateDirectory(dir);

			await DirectoryExists(dir);
			await FileDoesntExist(firstFile);

			await Fs.WriteFile(firstFile, async stream => await Json.WriteTo(stream));

			await FileExists(firstFile);

			//can read twice
			await FileHasContents(firstFile, Json);
			await FileHasContents(firstFile, Json);

			await Fs.CopyFile(firstFile, secondFile);

			await FileExists(firstFile);
			await FileHasContents(firstFile, Json);

			await FileExists(secondFile);
			await FileHasContents(secondFile, Json);

			await Fs.WriteFile(firstFile, async stream => await OtherJson.WriteTo(stream));

			await FileExists(firstFile);
			await FileHasContents(firstFile, OtherJson);

			await FileExists(secondFile);
			await FileHasContents(secondFile, Json);

			await Fs.DeleteFile(firstFile);

			await FileDoesntExist(firstFile);
			await FileExists(secondFile);
		}

		[Fact]
		public async Task When_copying_a_non_existing_source()
		{
			var source = Path.Combine(Root, "source-non-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			Should.Throw<FileNotFoundException>(async () => await Fs.CopyFile(source, destination));

			await FileDoesntExist(source);
			await FileDoesntExist(destination);
		}

		[Fact]
		public async Task When_copying_an_exising_source_to_non_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			await Fs.CopyFile(source, destination);

			await FileExists(source);
			await FileHasContents(destination, Content);
		}

		[Fact]
		public async Task When_copying_an_existing_source_to_non_existing_directory()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "target\\dest-non-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			Should.Throw<DirectoryNotFoundException>(async () => await Fs.CopyFile(source, destination));

			await FileExists(source);
			await FileDoesntExist(destination);
		}

		[Fact]
		public async Task When_copying_an_existing_source_to_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			await Fs.CopyFile(source, destination);

			await FileExists(source);
			await FileHasContents(destination, Content);
		}

		[Fact]
		public async Task When_creating_a_non_existing_directory()
		{
			var path = Path.Combine(Root, "non-existing");
			await Fs.CreateDirectory(path);

			await DirectoryExists(path);
		}

		[Fact]
		public async Task When_creating_an_existing_directory()
		{
			var path = Path.Combine(Root, "non-existing");
			await CreateDirectory(path);

			await Fs.CreateDirectory(path);

			await DirectoryExists(path);
		}

		[Fact]
		public async Task When_creating_a_non_existing_directory_tree()
		{
			var path = Path.Combine(Root, "non-existing\\tree\\of\\directories");
			await Fs.CreateDirectory(path);

			await DirectoryExists(path);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_files()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);
			await WriteFile(Path.Combine(path, "1.txt"), "first");
			await WriteFile(Path.Combine(path, "2.txt"), "second");

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_an_existing_directory_with_subdirectories()
		{
			var path = Path.Combine(Root, "existing");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "1"));
			await CreateDirectory(Path.Combine(path, "2"));

			await Fs.DeleteDirectory(path);

			await DirectoryDoesntExist(path);
		}

		[Fact]
		public async Task When_deleting_a_non_existing_directory()
		{
			var path = "some fake";

			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.DeleteDirectory(path));

			await DirectoryDoesntExist("some fake");
		}

		[Theory]
		[InlineData("existing.txt")]
		[InlineData("non-existing.txt")]
		public async Task When_deleting_a_file(string filename)
		{
			var path = Path.Combine(Root, filename);

			await Fs.DeleteFile(path);
		}

		[Fact]
		public void When_deleting_from_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(
				async () => await Fs.DeleteFile("sub\\nonexisting.txt"));
		}

		[Theory]
		[InlineData("test-dir", true)]
		[InlineData("test-dir\\other", false)]
		[InlineData("not-here", false)]
		public async Task When_checking_a_directory_exists(string path, bool exists)
		{
			await CreateDirectory(Path.Combine(Root, "test-dir"));

			var directory = Path.Combine(Root, path);

			(await Fs.DirectoryExists(directory)).ShouldBe(exists);
		}

		[Theory]
		[InlineData("{root}\\exists.json", true)]
		[InlineData("{root}\\not-exists.json", false)]
		[InlineData("{root}\\not\\here.json", false)]
		[InlineData("{root}", false)]
		public async Task When_checking_a_file_exists(string path, bool exist)
		{
			await WriteFile(Path.Combine(Root, "exists.json"), "");
			path = path.Replace("{root}", Root);

			(await Fs.FileExists(path)).ShouldBe(exist);
		}
		
		[Fact]
		public void When_listing_directories_in_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(async () => await Fs.ListFiles("non_existing"));
		}

		[Fact]
		public async Task When_listing_directories_in_a_directory_without_files()
		{
			var path = Path.Combine(Root, "without_files");
			await CreateDirectory(path);

			var files = await Fs.ListDirectories(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_directories_in_a_directory_with_files()
		{
			var path = Path.Combine(Root, "with_files");
			await CreateDirectory(path);
			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListDirectories(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_directories_in_a_directory_with_directories()
		{
			var path = Path.Combine(Root, "with_directories");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			var files = await Fs.ListDirectories(path);

			files.ShouldBe(new[]
			{
				path + "\\a",
				path + "\\b",
				path + "\\c"
			});
		}

		[Fact]
		public async Task When_listing_directories_in_a_directory_with_both()
		{
			var path = Path.Combine(Root, "with_both");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListDirectories(path);

			files.ShouldBe(new[]
			{
				path + "\\a",
				path + "\\b",
				path + "\\c"
			});
		}

		[Fact]
		public void When_listing_files_in_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(async () => await Fs.ListFiles("non_existing"));
		}

		[Fact]
		public async Task When_listing_files_in_a_directory_without_files()
		{
			var path = Path.Combine(Root, "without_files");
			await CreateDirectory(path);

			var files = await Fs.ListFiles(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_files_in_a_directory_with_files()
		{
			var path = Path.Combine(Root, "with_files");
			await CreateDirectory(path);
			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListFiles(path);

			files.ShouldBe(new[]
			{
				path + "\\1.txt",
				path + "\\2.txt",
				path + "\\3.txt"
			});
		}

		[Fact]
		public async Task When_listing_files_in_a_directory_with_directories()
		{
			var path = Path.Combine(Root, "with_directories");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			var files = await Fs.ListFiles(path);

			files.ShouldBeEmpty();
		}

		[Fact]
		public async Task When_listing_files_in_a_directory_with_both()
		{
			var path = Path.Combine(Root, "with_both");
			await CreateDirectory(path);
			await CreateDirectory(Path.Combine(path, "a"));
			await CreateDirectory(Path.Combine(path, "b"));
			await CreateDirectory(Path.Combine(path, "c"));

			await WriteFile(Path.Combine(path, "1.txt"), "wat");
			await WriteFile(Path.Combine(path, "2.txt"), "is");
			await WriteFile(Path.Combine(path, "3.txt"), "this");

			var files = await Fs.ListFiles(path);

			files.ShouldBe(new[]
			{
				path + "\\1.txt",
				path + "\\2.txt",
				path + "\\3.txt"
			});
		}

		[Fact]
		public async Task When_moveing_a_non_existing_source()
		{
			var source = Path.Combine(Root, "source-non-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			Should.Throw<FileNotFoundException>(async () => await Fs.MoveFile(source, destination));

			await FileDoesntExist(source);
			await FileDoesntExist(destination);
		}

		[Fact]
		public async Task When_moveing_an_exising_source_to_non_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-non-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			await Fs.MoveFile(source, destination);

			await FileDoesntExist(source);
			await FileHasContents(destination, Content);
		}

		[Fact]
		public async Task When_moveing_an_existing_source_to_non_existing_directory()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "target\\dest-non-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			Should.Throw<DirectoryNotFoundException>(async () => await Fs.MoveFile(source, destination));

			await FileExists(source);
			await FileDoesntExist(destination);
		}

		[Fact]
		public async Task When_moveing_an_existing_source_to_existing_file()
		{
			var source = Path.Combine(Root, "source-existing.json");
			var destination = Path.Combine(Root, "dest-existing.json");

			await Fs.WriteFile(source, async stream => await Content.WriteTo(stream));

			await Fs.MoveFile(source, destination);

			await FileDoesntExist(source);
			await FileHasContents(destination, Content);
		}

		[Fact]
		public async Task When_reading_an_existing_file()
		{
			await WriteFile(Path.Combine(Root, "existing.txt"), Content);

			var stream = await Fs.ReadFile(Path.Combine(Root, "existing.txt"));

			((string)stream.ReadAsString()).ShouldBe(Content);
		}

		[Fact]
		public void When_reading_a_non_existing_file()
		{
			Should.Throw<FileNotFoundException>(async () => await Fs.ReadFile(Path.Combine(Root, "non-existing.txt")));
		}

		[Theory]
		[InlineData("exists.txt", "replacement text")]
		[InlineData("nonexisting.txt", "other text")]
		public async Task When_writing_to_a_file(string filename, string content)
		{
			var path = Path.Combine(Root, filename);

			await Fs.WriteFile(path, async stream => await content.WriteTo(stream));

			await FileHasContents(path, content);
		}

		[Fact]
		public void When_writing_to_a_non_existing_directory()
		{
			Should.Throw<DirectoryNotFoundException>(async () => await Fs.WriteFile(
				"sub\\nonexisting.txt",
				async stream => await "something".WriteTo(stream)));
		}
	}
}
