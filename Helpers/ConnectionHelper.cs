using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ACMEWorker.Helpers
{
    static class ConnectionHelper
    {
        /*=================
         Fetches connection strings from DB
         Each object's target string corresponds to its name.
         so Posts uses "posts"

         the base url can be retrieved by querying for "base"
         ================*/
        public static string GetConnectionString(string target, string connectionString)
        {
            string result ="";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("GetUrl", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.Parameters.Add(new SqlParameter("@target", target));

                    var returnParameter = cmd.Parameters.Add(new SqlParameter("@result", result));
                    returnParameter.Direction = ParameterDirection.Output;
                    returnParameter.Size = 50;

                    con.Open();
                    cmd.ExecuteNonQuery();

                    result = cmd.Parameters["@result"].Value.ToString();
                }

                return result;
            }
        }
    }
}
