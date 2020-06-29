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
    class TodosController
    {
        private string ConnectionString;
        static HttpClient Client;
        private List<Todo> TodoData;

        public TodosController(HttpClient client, string connectionString)
        {
            Client = client;
            ConnectionString = connectionString;
            TodoData = new List<Todo>();
        }
        public async Task GetTodos()
        {
            string url = ConnectionHelper.GetConnectionString("todos", ConnectionString);

            using (HttpResponseMessage response = await Client.GetAsync(url))
            {
                TodoData =
                    await JsonSerializer.DeserializeAsync<List<Todo>>
                        (await response.Content.ReadAsStreamAsync());
            }
        }

        public void SaveTodosToDB()
        {
            foreach (Todo Todo in TodoData)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        using (var cmd = new SqlCommand("TodoInsertCommand", con)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            cmd.Parameters.Add(new SqlParameter("@id", Todo.id));
                            cmd.Parameters.Add(new SqlParameter("@userId", Todo.userId));
                            cmd.Parameters.Add(new SqlParameter("@title", Todo.title));
                            cmd.Parameters.Add(new SqlParameter("@completed", Todo.completed));

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
