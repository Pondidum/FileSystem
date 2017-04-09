# FileSystem
An asnyc FileSystem abstraction, with decoration and in-memory/virtual support.


# Installation

This package is [available on NuGet](https://www.nuget.org/packages/FileSystem).

```
PM> install-package FileSystem
```

# Usage


```csharp
public class Configuration
{
    private readonly IFileSystem _fileSystem;

    public Configuration(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task Load()
    {
        using (var stream = await _fileSystem.ReadFile("config.json"))
        {
            //...
        }
    }

    public async Task Save()
    {
        await _fileSystem.WriteFile("config.json", async stream => {
            await stream.Write(/* ... */);
        });
    }
}

//usage:
var config = new Configuration(new PhysicalFileSystem());
```

# Logging

As I didn't want to take a dependency on any particular logging library, there is no out of the box implementation.  However, implementing your own only takes a few lines of code, making use of the `EventingFileSystem` class.  For example, logging everything as Debug with [Serilog](https://serilog.net/):

```csharp
public class LoggingFileSystem : EventingFileSystem
{
    public LoggingFileSystem(IFileSystem inner) : base(inner)
    {
        HandleEvent = message =>
        {
            Log.Debug($"{message}: {{@event}}", message);
            return Task.CompletedTask;
        };
    }
}
```

Each event emitted by the `EventingFileSystem` has a reasonable `.ToString` implementaiton, so you can just write `Console.WriteLine(message.ToString())` if you wish.

# Decoration

For ease of implementing, FileSystem supplies a `FileSystemDecorator` class, which implements all `IFileSystem` methods as virtual calls to an inner `IFileSystem`.

For example, an encrypting filesystem could be implemented by just overriding a few methods:

```csharp
public class EncryptingFileSystem : FileSystemDecorator
{
    private readonly ICrypto _crypto;

    public EncryptingFileSystem(IFileSystem inner, ICrypto crypto) : base(inner)
    {
        _crypto = crypto;
    }

    public override  async Task<Stream> ReadFile(string path)
    {
        return await _crypto.DecryptStream(await base.ReadFile(path));
    }

    public override async Task WriteFile(string path, Func<Stream, Task> write)
    {
        await base.WriteFile(path, async stream =>
        {
            using (var encrypted = await _crypto.Encrypt(async cryptoStream => await write(cryptoStream)))
            {
                await encrypted.CopyToAsync(stream);
            }
        });
    }

    public override Task AppendFile(string path, Func<Stream, Task> write)
    {
        throw new NotSupportedException("You cannot append to an encrypted file.  Try reading, and the writing the whole file.");
    }
}
```

# To do

* Caching FileSystem
    * Read caching I guess
    * pluggable caching strategies
* Commitable FileSystem (call `.Commit()` to flush writes to disk.)
* Case(In)Sensitive FileSystem?
  * Not sure how this would work
  * ReadFile would ListFiles first, then find the right mapping perhaps?
 * S3FileSystem
    * Separate package I guess
