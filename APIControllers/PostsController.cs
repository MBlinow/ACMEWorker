using ACMEWorker.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACMEWorker.APIControllers
{
    class PostsController
    {
        static HttpClient Client { get; set; }
        public List<Post> postData { get; set; }
        public PostsController(HttpClient client)
        {
            Client = client;
            postData = new List<Post>();
        }

        public async Task GetPosts()
        {
            string url = "/posts";

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                postData = 
                    await JsonSerializer.DeserializeAsync<List<Post>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SavePostsToDB()
        {
            var cs = "Server=.;Database=ACMEWorkerDB;Trusted_Connection=True;";

            foreach (Post post in postData) {
                try
                {
                    using (SqlConnection con = new SqlConnection(cs))
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
                catch { }
            }
        }
    }
}
