using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ACMEWorker.APIControllers;
using ACMEWorker.Helpers;
using ACMEWorker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACMEWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration Configuration;
        private static HttpClient Client;
        private int DelayInSeconds;
        private string ConnectionString;

        private PostsController PostsController;
        private CommentsController CommentsController;
        private AlbumsController AlbumsController;
        private PhotosController PhotosController;
        private TodosController TodosController;
        private UsersController UsersController;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;

            DelayInSeconds = Configuration.GetValue<int>("Preferences:RefreshRate");
        }

        private void initConnections()
        {
            Client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");            

            ConnectionString = Configuration.GetConnectionString("DB");

            Client.BaseAddress = new Uri(ConnectionHelper.GetConnectionString("base", ConnectionString));
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Client = new HttpClient();
            initConnections();

            PostsController = new PostsController(Client, ConnectionString);
            CommentsController = new CommentsController(Client, ConnectionString);
            AlbumsController = new AlbumsController(Client, ConnectionString);
            PhotosController = new PhotosController(Client, ConnectionString);
            TodosController = new TodosController(Client, ConnectionString);
            UsersController = new UsersController(Client, ConnectionString);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await UsersController.GetUsers();
                UsersController.SaveUsersToDB();
                _logger.LogInformation("Saved Users");

                await PostsController.GetPosts();
                PostsController.SavePostsToDB();
                _logger.LogInformation("Saved Posts");

                await CommentsController.GetComments();
                CommentsController.SaveCommentsToDB();
                _logger.LogInformation("Saved Comments");

                await AlbumsController.GetAlbums();
                AlbumsController.SaveAlbumsToDB();
                _logger.LogInformation("Saved Albums");

                await PhotosController.GetPhotos();
                PhotosController.SavePhotosToDB();
                _logger.LogInformation("Saving Photos");

                await TodosController.GetTodos();
                TodosController.SaveTodosToDB();
                _logger.LogInformation("Saving Todos");

                await Task.Delay(DelayInSeconds * 1000, stoppingToken);

            }
        }
    }
}
