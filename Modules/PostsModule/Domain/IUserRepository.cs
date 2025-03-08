namespace PostsModule.Domain;

public interface IUserRepository
{
	Task Create(User user);

	
	Task Update(User user);
	Task Delete(string userId);
}

public interface IImageRepository
{
    Task Create(Image image);
    
}