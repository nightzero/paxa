using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace paxa_asp_rest.Utilities
{
    public class ConnectionManager
    {
        private string ConnectionString;

        public ConnectionManager(string connectionString)
        {
            this.ConnectionString = connectionString +
                ";Min Pool Size=0;Max Pool Size=50;Pooling=true;";
        }

        public MySqlConnection GetConnection()
        {
            MySqlConnection con = new MySqlConnection(ConnectionString);
            con.Open();
            return con;
        }

        public void CloseConnection(MySqlConnection con)
        {
            if (con != null)
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}