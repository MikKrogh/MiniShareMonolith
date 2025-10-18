
using PostsModule.Infrastructure;

namespace PostsModule.Tests.Helper;

internal class PresignedUrlGeneratorMock : IPresignedUrlGenerator
{
    public Task<string> GetPresignedThumbnailUrl(string postId)
    {
        return Task.FromResult(postId);
    }

    public async Task<IEnumerable<string>> GetPresignedUris(string dirName, int count)
    {
        var uris = Enumerable.Range(0, count).Select(i => i.ToString());

        return await Task.FromResult(uris);
    }
}
