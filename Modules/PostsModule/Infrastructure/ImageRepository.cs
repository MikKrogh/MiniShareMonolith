using PostsModule.Domain;

namespace PostsModule.Infrastructure;

internal sealed class ImageRepository : IImageRepository
{
    private readonly PostsContext _context;

    public ImageRepository(PostsContext context)   
    {
        _context = context;
    }

    public Task Create(Image image)
    {

        var imageEntity = new ImageEntity
        {
            Id = image.FileName,
            PostId = image.PostId
        };

        _context.Images.Add(imageEntity);
        return _context.SaveChangesAsync();
    }
}
