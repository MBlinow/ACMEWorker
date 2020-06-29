using ACMEWorker.Helpers;
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
    class PhotosController
    {
        private string ConnectionString;
        static HttpClient Client;
        private List<Photo> photoData;
        public PhotosController(HttpClient client, string connectionString)
        {
            Client = client;
            ConnectionString = connectionString;
            photoData = new List<Photo>();
        }
        public async Task GetPhotos()
        {
            string url = ConnectionHelper.GetConnectionString("photos", ConnectionString);

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                photoData =
                    await JsonSerializer.DeserializeAsync<List<Photo>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SavePhotosToDB()
        {
            foreach (Photo photo in photoData)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        using (var cmd = new SqlCommand("PhotoInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@id", photo.id));
                            cmd.Parameters.Add(new SqlParameter("@albumId", photo.albumId));
                            cmd.Parameters.Add(new SqlParameter("@title", photo.title));
                            cmd.Parameters.Add(new SqlParameter("@url", photo.url));
                            cmd.Parameters.Add(new SqlParameter("@thumbnailUrl", photo.thumbnailUrl));


                            con.Open();
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                catch (SqlException e)
                { 
                    
                }
            }
        }
    }
}
