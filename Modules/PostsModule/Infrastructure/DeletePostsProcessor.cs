
using PostsModule.Infrastructure;

namespace PostsModule.Application.DeletePost;

public class DeletePostsProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly int DelayTime = 30000;

    private readonly ILogger<DeletePostsProcessor> logger;

    public DeletePostsProcessor(ILogger<DeletePostsProcessor> logger, IServiceProvider serviceProvider, IConfiguration config)
    {
        this._serviceProvider = serviceProvider;
        this.logger = logger;

        var isTestEnvironment = config["Environment"];
        if (isTestEnvironment == "Test")        
            DelayTime = 50; // Reduce delay time for tests    
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
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
                                await Task.Delay(3);
                            await service.TryDelete(postId);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "failed deletiong on post with id: {0}", postId);
                        }
                    }
                }
                logger.LogInformation("backgroundworker for delete posts has completed a run.");
                await Task.Delay(TimeSpan.FromMilliseconds(DelayTime), stoppingToken);
            }

            }
        finally
        {
            Console.WriteLine( "reached");
            logger.LogInformation("reached");
        }
    }
}
