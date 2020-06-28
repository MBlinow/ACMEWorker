using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ACMEWorker.APIControllers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACMEWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static HttpClient Client;
        private int DelayInSeconds = 5;

        private PostsController postsController;
        private CommentsController commentsController;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        private void initController()
        {
            Client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Client = new HttpClient();
            initController();

            postsController = new PostsController(Client);
            commentsController = new CommentsController(Client);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await postsController.GetPosts();
                postsController.SavePostsToDB();
                _logger.LogInformation("Saving Posts");


                await commentsController.GetComments();
                commentsController.SaveCommentsToDB();
                _logger.LogInformation("Saving Comments");

                await Task.Delay(DelayInSeconds * 1000, stoppingToken);

            }
        }
    }
}
