using ACMEWorker.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACMEWorker.APIControllers
{
    class CommentsController
    {
        static HttpClient Client { get; set; }
        public List<Comment> commentData { get; set; }

        public CommentsController(HttpClient client)
        {
            Client = client;
            commentData = new List<Comment>();
        }
        public async Task GetComments()
        {
            string url = "/comments";

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                commentData =
                    await JsonSerializer.DeserializeAsync<List<Comment>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SaveCommentsToDB()
        {
            var cs = "Server=.;Database=ACMEWorkerDB;Trusted_Connection=True;";

            foreach (Comment comment in commentData)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        using (var cmd = new SqlCommand("CommentInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@postID", comment.postId));
                            cmd.Parameters.Add(new SqlParameter("@id", comment.id));
                            cmd.Parameters.Add(new SqlParameter("@name", comment.name));
                            cmd.Parameters.Add(new SqlParameter("@email", comment.email));
                            cmd.Parameters.Add(new SqlParameter("@body", comment.body));

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                catch (SqlException e) 
                { Console.WriteLine(e.Message); }
            }
        }
    }
}
