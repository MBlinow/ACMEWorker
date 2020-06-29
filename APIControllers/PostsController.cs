using ACMEWorker.Helpers;
using ACMEWorker.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACMEWorker.APIControllers
{
    class PostsController
    {
        private string ConnectionString;
        static HttpClient Client;
        private List<Post> postData;
        public PostsController(HttpClient client, string connectionString)
        {
            Client = client;
            ConnectionString = connectionString;
            postData = new List<Post>();
        }

        public async Task GetPosts()
        {
            string url = ConnectionHelper.GetConnectionString("posts", ConnectionString);

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                postData = 
                    await JsonSerializer.DeserializeAsync<List<Post>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SavePostsToDB()
        {
            foreach (Post post in postData) {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        using (var cmd = new SqlCommand("PostInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@userId", post.userId));
                            cmd.Parameters.Add(new SqlParameter("@id", post.id));
                            cmd.Parameters.Add(new SqlParameter("@title", post.title));
                            cmd.Parameters.Add(new SqlParameter("@body", post.body));

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                catch (SqlException e)
                { Console.WriteLine(e.Message);
                    break;
                }
            }
        }
    }
}
