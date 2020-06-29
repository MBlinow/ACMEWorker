using ACMEWorker.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using ACMEWorker.Helpers;

namespace ACMEWorker.APIControllers
{
    class AlbumsController 
    {
        private static HttpClient Client;
        private string ConnectionString;
        private List<Album> albumData;

        public AlbumsController(HttpClient client, string connectionString)
        {
            Client = client;
            ConnectionString = connectionString;
            albumData = new List<Album>();
        }
        public async Task GetAlbums()
        {
            string url = ConnectionHelper.GetConnectionString("albums", ConnectionString);

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                albumData =
                    await JsonSerializer.DeserializeAsync<List<Album>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SaveAlbumsToDB()
        {

            foreach (Album album in albumData)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {


                        using (var cmd = new SqlCommand("AlbumInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@userId", album.userId));
                            cmd.Parameters.Add(new SqlParameter("@id", album.id));
                            cmd.Parameters.Add(new SqlParameter("@title", album.title));

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }


                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
        }
            
        
        
    }
}
