using Microsoft.AspNetCore.Http;
using PostsModule.Domain;

namespace PostsModule.Tests.Helper;

public class FakeImageBlobStorage : IImageStorageService
{
    private Dictionary<string, IEnumerable<MockFile>> _blobStorage = new();
    private object _blobStorageLock = new object();
    public async Task UploadImage(Stream stream, string directoryName, string fileName, string extension)
    {
        try
        {
            lock (_blobStorageLock)
            {
                var file = ReadFully(stream);

                var mockfile = new MockFile()
                {
                    Name = fileName,
                    File = file,
                    Size = file.Length,
                };

                if (!_blobStorage.ContainsKey(directoryName))
                    _blobStorage.Add(directoryName, Enumerable.Empty<MockFile>());
                _blobStorage[directoryName] = _blobStorage[directoryName].Append(mockfile);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private static byte[] ReadFully(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public Task<Stream> GetImage(string directoryName, string fileName)
    {
        lock (_blobStorageLock)
        {
            if (_blobStorage.ContainsKey(directoryName))
            {
                var file = _blobStorage[directoryName].FirstOrDefault(x => x.Name == fileName);
                if (file != null)
                    return Task.FromResult<Stream>(new MemoryStream(file.File));
            }
        }
        return Task.FromResult<Stream>(null);
    }

    public IEnumerable<MockFile> GetDirectory(string key)
    {
        if (_blobStorage.ContainsKey(key))
            return _blobStorage[key];
        return Enumerable.Empty<MockFile>();
    }

}
public class MockFile
{
    public int Size { get; set; }
    public string Name { get; set; }
    public byte[] File { get; set; }
}
