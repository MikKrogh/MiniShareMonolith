
using PostsModule.Infrastructure;

namespace PostsModule.Application.DeletePost;

public class DeletePostsProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<DeletePostsProcessor> logger;

    public DeletePostsProcessor(ILogger<DeletePostsProcessor> logger, IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
        this.logger = logger;
    }


    protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("backgroundworker for delete posts has started.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDeletePostService>();
                var postsAwaitingDeletion = await service.FetchUnfinishedJobs(10);
                foreach (var postId in postsAwaitingDeletion)
                {
                    if (stoppingToken.IsCancellationRequested) break;                    
                    try
                    {
                        await service.TryDelete(postId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "failed deletiong on post with id: {0}", postId);
                    }
                }
            }
            logger.LogInformation("backgroundworker for delete posts has completed a run.");
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }

    }
}
