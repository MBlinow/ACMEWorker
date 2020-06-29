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
    class UsersController
    {
        private string ConnectionString;
        static HttpClient Client;
        private List<User> UserData;

        public UsersController(HttpClient client, string connectionString)
        {
            Client = client;
            ConnectionString = connectionString;
            UserData = new List<User>();
        }
        public async Task GetUsers()
        {
            string url = ConnectionHelper.GetConnectionString("users", ConnectionString);

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                UserData =
                    await JsonSerializer.DeserializeAsync<List<User>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SaveUsersToDB()
        {
            
            foreach (User User in UserData)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {

                        using (var cmd = new SqlCommand("UserInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@id", User.id));
                            cmd.Parameters.Add(new SqlParameter("@username", User.username));
                            cmd.Parameters.Add(new SqlParameter("@email", User.email));
                            cmd.Parameters.Add(new SqlParameter("@phone", User.phone));
                            cmd.Parameters.Add(new SqlParameter("@website", User.website));
                            cmd.Parameters.Add(new SqlParameter("@street", User.address.street));
                            cmd.Parameters.Add(new SqlParameter("@suite", User.address.suite));
                            cmd.Parameters.Add(new SqlParameter("@city", User.address.city));
                            cmd.Parameters.Add(new SqlParameter("@zipcode", User.address.zipcode));
                            cmd.Parameters.Add(new SqlParameter("@lat", User.address.geo.lat));
                            cmd.Parameters.Add(new SqlParameter("@lng", User.address.geo.lat));
                            cmd.Parameters.Add(new SqlParameter("@companyName", User.company.name));
                            cmd.Parameters.Add(new SqlParameter("@catchPhrase", User.company.catchPhrase));
                            cmd.Parameters.Add(new SqlParameter("@bs", User.company.bs));

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                        
                }
                catch (SqlException e)
                { Console.WriteLine(e.Message); }
                catch (Exception e)
                {

                }
            }
            
        }
    }
}
